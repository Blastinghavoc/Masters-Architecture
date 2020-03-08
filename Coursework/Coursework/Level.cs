using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Entities;
using Coursework.Animation;
using Coursework.Serialization;

namespace Coursework
{
    /// <summary>
    /// Class representing a whole level in the game, based on class of same name in 
    /// the platformer demo (lab2)
    /// </summary>
    class Level: IDisposable
    {
        public static Level CurrentLevel;//The currently active level. NOTE not a singleton class, but only one level can be active at once

        ContentManager content;//The content specific to this level

        Tile[,] tiles;//2D array of tiles

        Dictionary<Color, Texture2D> tileSkins = new Dictionary<Color, Texture2D>();//Textures for each type of tile        
        Dictionary<Color, Interactable> interactablePrefabs = new Dictionary<Color, Interactable>();//Prefabs for all non-enemy interactables
        Dictionary<Color, Enemy> enemyPrefabs = new Dictionary<Color, Enemy>();//Enemies

        Dictionary<Color, LevelEntitySpecification> colourBindings = new Dictionary<Color, LevelEntitySpecification>();

        readonly float tileResolution = GameData.Instance.levelConstants.tileResolution;//Resolution of tile images
        readonly Vector2 tileTextureScale;
        readonly Rectangle bounds;//Level bounds in world coordinates

        public readonly string levelName;
        public readonly string nextLevelName;

        public readonly Point tileSize = GameData.Instance.levelConstants.tileSize;

        readonly string tileFilePath = GameData.Instance.levelConstants.tileFilePath;
        readonly string itemFilePath = GameData.Instance.levelConstants.itemFilePath;
        readonly string enemyFilePath = GameData.Instance.levelConstants.enemyFilePath;
        readonly string mapFilePath = GameData.Instance.levelConstants.mapFilePath;

        public List<Interactable> Interactables { get; protected set; }
        private List<Interactable> killList = new List<Interactable>();//List of interactables to be removed

        public Dictionary<EnemyType, Decal> CorpseSkins { get; private set; } = new Dictionary<EnemyType, Decal>();
        private List<Decal> corpses = new List<Decal>();


        public Level(IServiceProvider provider, string contentRoot, string levelName, bool makeCurrent = true)
        {
            if (makeCurrent)
            {
                Level.CurrentLevel = this;
            }

            content = new ContentManager(provider, contentRoot);
            Interactables = new List<Interactable>();

            tileTextureScale = new Vector2(tileSize.X/tileResolution,tileSize.Y/tileResolution);
            this.levelName = levelName;

            var levelData = GameData.GetLevelData(levelName);
            nextLevelName = levelData.nextLevelName;
            
            //Initialise binding of colours to entity specifications for this level
            foreach (var item in levelData.bindings)
            {
                colourBindings.Add(item.color, GameData.Instance.entitySpecificationMap[item.entityName]);
            }

            LoadContent();

            InitialiseFromMap(levelData.mapName);

            bounds = new Rectangle(Point.Zero, new Point(tiles.GetLength(0)* tileSize.X, tiles.GetLength(1)* tileSize.Y));

            //Detect when the player starts colliding with level entities
            GameEventManager.Instance.OnPlayerCollisionEnter += OnPlayerCollisionEnter;
            //Detect death of an enemy
            GameEventManager.Instance.OnEnemyKilled += OnEnemyKilled;
        }

        public void Update(GameTime gameTime)
        {
            //Remove any interactables that were scheduled for deletion
            foreach (var item in killList)
            {
                Interactables.Remove(item);
            }
            killList.Clear();

            //Update all interactables (note that many of them have nothing to update)
            foreach (var item in Interactables)
            {
                item.Update(gameTime);
            }
        }

        //Clamp the camera's position to be within level bounds
        public void ConstrainCamera(Camera camera)
        {
            var visibleArea = camera.VisibleArea;

            var halfWidth = visibleArea.Width / 2f;
            var halfHeight = visibleArea.Height / 2f;

            var adjustedX = MathHelper.Clamp(camera.Position.X, bounds.Left + halfWidth, bounds.Right - halfWidth);
            var adjustedY = MathHelper.Clamp(camera.Position.Y, bounds.Top + halfHeight, bounds.Bottom - halfHeight);

            camera.Position = new Vector2(adjustedX, adjustedY);
        }

        /// <summary>
        /// Loads only the content that is actually used by this level, based on the colorbindings
        /// defined for this level in the relevant XML file
        /// </summary>
        private void LoadContent()
        {
            //Load resources based on colour bindings
            foreach (var item in colourBindings)
            {
                var spec = item.Value;
                switch (spec.entityType)
                {
                    case LevelEntityType.tile:
                        {
                            var tileData = spec.entityData as TileData;
                            tileSkins[item.Key] = content.Load<Texture2D>(tileFilePath + tileData.textureName);
                        }
                        break;
                    case LevelEntityType.interactable:
                        {
                            var interData = spec.entityData as InteractableData;
                            var tex = content.Load<Texture2D>(itemFilePath + interData.textureName);
                            Sprite sprite = new Sprite(tex, scaleForTexture(tex), Color.White);
                            Interactable newInteractable = new Interactable(sprite, Vector2.Zero);
                            newInteractable.interactableType = interData.interactableType;
                            interactablePrefabs[item.Key] = newInteractable;
                        }
                        break;
                    case LevelEntityType.enemy:
                        {
                            var enemyData = spec.entityData as EnemyData;
                            var deadTex = content.Load<Texture2D>(enemyFilePath + enemyData.corpseData.textureName);
                            CorpseSkins[enemyData.enemyType] = new Decal(deadTex, scaleForTexture(deadTex), Color.White);

                            var animData = enemyData.animationData;
                            Texture2D[] frames = new Texture2D[animData.numFrames];
                            //Load all animation frame textures
                            for (int i = 0; i < animData.numFrames; i++)
                            {
                                frames[i] = content.Load<Texture2D>(enemyFilePath + animData.baseName + (i+1).ToString());
                            }

                            var newAnimation = new MultiImageAnimation(frames, Vector2.Zero, animData.frameDimensions.X, 
                                animData.frameDimensions.Y, animData.numFrames, animData.frameDuration, 
                                Color.White, scaleForTexture(animData.frameDimensions.X).X, true);

                            enemyPrefabs[item.Key] = new Enemy(newAnimation, Vector2.Zero, 1, 1, enemyData.enemyType);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void InitialiseFromMap(string mapName)
        {
            Texture2D map = content.Load<Texture2D>(mapFilePath+mapName);     
            

            int width = map.Width;
            int height = map.Height;

            tiles = new Tile[width, height];
            Color[] mapColours = new Color[width * height];
            map.GetData(mapColours);//Get colours into 1D array

            //Getting data from texture, based on answers from https://stackoverflow.com/questions/9532919/how-to-get-color-and-coordinatesx-y-from-texture2d-xna-c
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var colour= mapColours[i + j* width];
                    if (colour == Color.Transparent)
                    {
                        continue;
                    }

                    LevelEntitySpecification spec;
                    if (colourBindings.TryGetValue(colour,out spec))
                    {
                        switch (spec.entityType)
                        {
                            case LevelEntityType.tile:
                                {
                                    var tex = tileSkins[colour];
                                    var tileData = spec.entityData as TileData;
                                    tiles[i, j] = new Tile(tex, tileData.collisionMode);
                                }
                                break;
                            case LevelEntityType.interactable:
                                {
                                    var inter = interactablePrefabs[colour].Clone();
                                    inter.SetPosition(GetWorldPosition(i, j));
                                    Interactables.Add(inter);
                                }
                                break;
                            case LevelEntityType.enemy:
                                {
                                    var enemy = enemyPrefabs[colour];
                                    var offset = new Vector2(0, tileSize.Y - enemy.Appearance.Size.Y);

                                    var newEnemy = enemy.Clone();
                                    newEnemy.SetPosition(GetWorldPosition(i, j) + offset);

                                    Interactables.Add(newEnemy);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Draw all tiles
            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    Vector2 worldPosition = new Vector2(i * tileSize.X, j * tileSize.Y);
                    var tile = tiles[i, j];
                    if (tile.texture != null)
                    {
                        //Draw tile with appropriate scale
                        spriteBatch.Draw(tile.texture, worldPosition, null, Color.White, 0, Vector2.Zero, tileTextureScale, SpriteEffects.None, 0);
                    }
                }
            }

            //Draw all interactables
            foreach (var item in Interactables)
            {
                item.Draw(spriteBatch);
            }

            foreach (var item in corpses)
            {
                item.Draw(spriteBatch);
            }
        }

        private void OnPlayerCollisionEnter(object sender, PlayerCollisionEventArgs e)
        {
            Interactable interactable = e.colllidedWith as Interactable;
            if (interactable != null)
            {
                switch (interactable.interactableType)
                {
                    case InteractableType.coin:
                        {
                            GameEventManager.Instance.AddScore(1);
                            killList.Add(interactable);//Schedule for deletion
                        }
                        break;
                    case InteractableType.enemy:
                        {
                            //Do nothing, these collisions are handled by the player
                        }
                        break;
                    case InteractableType.nextLevel:
                        {
                            GameEventManager.Instance.NextLevel();
                        }
                        break;
                    case InteractableType.powerup_fireball:
                        {
                            killList.Add(interactable);//Schedule for deletion
                        }
                        break;
                    default:
                        break;
                }
                
            }
        }

        private void OnEnemyKilled(object sender,EnemyKilledEventArgs e)
        {
            Enemy enemy = e.enemy;
            killList.Add(enemy);
            Decal corpse;
            if (CorpseSkins.TryGetValue(enemy.enemyType,out corpse))
            {
                corpse = corpse.Clone() as Decal;//New instance
                corpse.Effect = enemy.DirectionalEffect;
                var offset = enemy.Appearance.Size - corpse.Size;

                corpse.SetPosition(enemy.Position + offset);

                corpses.Add(corpse);
            }
        }

        public Vector2 scaleForTexture(Texture2D t)
        {
            return new Vector2(tileSize.X / (float)t.Width);
        }

        public Vector2 scaleForTexture(float textureWidth)
        {
            return new Vector2(tileSize.X / textureWidth);
        }

        public TileCollisionMode GetCollisionModeAt(Point p)
        {
            return GetCollisionModeAt(p.X, p.Y);
        }

        public TileCollisionMode GetCollisionModeAt(int i, int j)
        {
            if (i<0 || i >= tiles.GetLength(0))
            {
                return TileCollisionMode.solid;//Outside bounds left and right are considered solid.
            }

            if (j < 0 || j >= tiles.GetLength(1))
            {
                return TileCollisionMode.empty;//Outside bounds top and bottom are considered empty
            }

            return tiles[i, j].collisionMode;
        }

        /// <summary>
        /// Get the bounding rectangle of the tile at the given tile coordinates
        /// </summary>
        public Rectangle GetBoundsAt(int i, int j)
        {
            return new Rectangle(i * tileSize.X, j * tileSize.Y, tileSize.X, tileSize.Y);          
        }

        public Vector2 GetWorldPosition(int i, int j)
        {
            return new Vector2(i * tileSize.X, j* tileSize.Y);
        }

        public Point GetTileIndices(Vector2 position)
        {
            int i = (int)(position.X / tileSize.X);
            int j = (int)(position.Y / tileSize.Y);
            return new Point(i, j);
        }

        public Point GetTileIndices(Point position)
        {
            int i = (position.X / tileSize.X);
            int j = (position.Y / tileSize.Y);
            return new Point(i, j);
        }

        public void Dispose()
        {
            content.Unload();
            GameEventManager.Instance.OnPlayerCollisionEnter -= OnPlayerCollisionEnter;
            GameEventManager.Instance.OnEnemyKilled -= OnEnemyKilled;
        }
    }
}
