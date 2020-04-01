using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Entities;
using Coursework.Graphics;
using Coursework.Serialization;
using Coursework.Entities.Enemies;
using Coursework.Entities.Interactables;

namespace Coursework.Levels
{
    /// <summary>
    /// Class representing a whole level in the game, based on class of same name in 
    /// the platformer demo (lab2)
    /// </summary>
    public class Level: EventSubscriber
    {
        public static Level CurrentLevel;//The currently active level. NOTE not a singleton class, but only one level can be active at once

        ContentManager content;//The content specific to this level

        Tile[,] tiles;//2D array of tiles            

        readonly Vector2 tileTextureScale;
        public readonly Rectangle LevelBounds;//Level bounds in world coordinates

        public readonly string levelName;
        public readonly string nextLevelName;

        public readonly Point tileSize = GameData.Instance.levelConstants.tileSize;        

        readonly int coinValue = GameData.Instance.levelConstants.coinValue;

        //List of all entities in the level, including enemies
        public List<CollidableObject> LevelEntities { get; protected set; } = new List<CollidableObject>();
        private List<CollidableObject> killList = new List<CollidableObject>();//List of entities to be removed

        private List<Decal> corpses = new List<Decal>();


        public Level(IServiceProvider provider, string contentRoot, string levelName,Background background, bool makeCurrent = true)
        {
            //By default creating a new level makes it the current level.
            if (makeCurrent)
            {
                CurrentLevel = this;
            }

            content = new ContentManager(provider, contentRoot);

            float tileResolution = GameData.Instance.levelConstants.tileResolution;//Resolution of tile images
            tileTextureScale = tileSize.ToVector2() / tileResolution;

            this.levelName = levelName;

            var levelData = GameData.GetLevelData(levelName);

            nextLevelName = levelData.nextLevelName;

            Dictionary<Color, LevelEntitySpecification> colourBindings = new Dictionary<Color, LevelEntitySpecification>();

            //Initialise binding of colours to entity specifications for this level
            foreach (var item in levelData.bindings)
            {
                colourBindings.Add(item.color, GameData.Instance.entitySpecificationMap[item.entityName]);
            }

            //Load all the relevant content to initialise the level
            LoadContent(levelData.mapName,colourBindings);

            //Load the background texture for this level and apply it
            Texture2D bgTex = content.Load<Texture2D>(levelData.backgroundPath);
            background.SetTexture(bgTex);

            //Initialise the level bounds
            LevelBounds = new Rectangle(Point.Zero, new Point(tiles.GetLength(0)* tileSize.X, tiles.GetLength(1)* tileSize.Y));

            BindEvents();
        }

        public void Update(GameTime gameTime)
        {
            //Remove any entities that were scheduled for deletion
            foreach (var item in killList)
            {                
                LevelEntities.Remove(item);
                if (item is IDisposable d)
                {
                    d.Dispose();//Dispose of entity if necessary
                }
            }
            killList.Clear();

            //Update all entities (note that for many of them this does nothing)
            foreach (var item in LevelEntities)
            {
                item.Update(gameTime);
            }
        }

        /// <summary>
        /// Loads only the content that is actually used by this level, based on the colorbindings
        /// defined for this level in the relevant XML file
        /// </summary>
        private void LoadContent(string mapName,Dictionary<Color, LevelEntitySpecification> colourBindings)
        {
            //Create prefab dictionaries
            Dictionary<Color, Tile> tilePrefabs = new Dictionary<Color, Tile>();//Prefabs for each type of tile     
            Dictionary<Color, Interactable> interactablePrefabs = new Dictionary<Color, Interactable>();//Prefabs for all interactables
            Dictionary<Color, Enemy> enemyPrefabs = new Dictionary<Color, Enemy>();//Enemies

            //Used to create entities from their data descriptions
            Unpacker unpacker = new Unpacker(content);

            //Load resources based on colour bindings to populate the prefabs
            foreach (var item in colourBindings)
            {
                var spec = item.Value;
                switch (spec.entityData)
                {
                    case TileData t:
                        {
                            tilePrefabs[item.Key] = unpacker.Unpack(t);
                        }
                        break;
                    case EnemyData e:
                        {
                            enemyPrefabs[item.Key] = unpacker.Unpack(e);
                        }
                        break;                 
                    case InteractableData i:
                        {                            
                            interactablePrefabs[item.Key] = unpacker.Unpack(i);
                        }
                        break;
                    default:
                        break;
                }
            }

            //Now that the prefabs are loaded, populate the level from the map file
            string mapFilePath = GameData.Instance.levelConstants.mapFilePath;
            Texture2D map = content.Load<Texture2D>(mapFilePath + mapName);

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
                    var colour = mapColours[i + j * width];
                    if (colour == Color.Transparent)
                    {
                        continue;
                    }

                    LevelEntitySpecification spec;
                    if (colourBindings.TryGetValue(colour, out spec))
                    {
                        //Instantiate relevant prefab based on type of entity
                        switch (spec.entityData)
                        {
                            case TileData d:
                                {
                                    var tile = tilePrefabs[colour];
                                    tiles[i, j] = tile;
                                }
                                break;
                            case EnemyData d:
                                {
                                    var enemy = enemyPrefabs[colour];
                                    var newEnemy = enemy.Clone();//Prototype pattern

                                    //Ensure the enemy is placed such that the bottom of them is on the ground (if applicable)
                                    var offset = new Vector2(0, tileSize.Y - enemy.BoundingBox.Size.Y);
                                    newEnemy.SetPosition(GetWorldPosition(i, j) + offset);

                                    LevelEntities.Add(newEnemy);
                                }
                                break;
                            case InteractableData d:
                                {
                                    var inter = interactablePrefabs[colour].Clone();//Prototype pattern
                                    inter.SetPosition(GetWorldPosition(i, j));
                                    LevelEntities.Add(inter);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
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
                        spriteBatch.Draw(tile.texture, worldPosition, null, Color.White, 0, Vector2.Zero, tileTextureScale, SpriteEffects.None, 0.8f);
                    }
                }
            }

            //Draw all interactables
            foreach (var item in LevelEntities)
            {
                item.Draw(spriteBatch);
            }

            foreach (var item in corpses)
            {
                item.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Detect and resolve player collisions with the level interactables
        /// </summary>
        private void OnPlayerCollisionEnter(object sender, PlayerCollisionEventArgs e)
        {
            Interactable interactable = e.colllidedWith as Interactable;
            if (interactable != null)
            {
                //Run the interaction for the specific interactable that was entered
                interactable.InteractOnEnter(this, e);                
            }
        }

        public void ScheduleForDeletion(CollidableObject obj)
        {
            killList.Add(obj);//Schedule for deletion next update
        }

        
        /// <summary>
        /// When an enemy is killed, delete it and replace it with its corpse.
        /// This saves processing it for collisions etc.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEnemyKilled(object sender,EnemyKilledEventArgs e)
        {
            Enemy enemy = e.enemy;
            killList.Add(enemy);

            Decal corpse = enemy.CorpseAppearance;
            
            corpse = corpse.Clone() as Decal;//New instance of the corpse decal
            corpse.Effect = enemy.DirectionalEffect;
            var offset = enemy.Appearance.Size - corpse.Size;

            corpse.SetPosition(enemy.Position + offset);

            corpses.Add(corpse);
            
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

        public void BindEvents()
        {
            //Detect when the player starts colliding with level entities
            GameEventManager.Instance.OnPlayerCollisionEnter += OnPlayerCollisionEnter;
            //Detect death of an enemy
            GameEventManager.Instance.OnEnemyKilled += OnEnemyKilled;
        }

        public void UnbindEvents()
        {
            GameEventManager.Instance.OnPlayerCollisionEnter -= OnPlayerCollisionEnter;
            GameEventManager.Instance.OnEnemyKilled -= OnEnemyKilled;
        }

        public void Dispose()
        {
            //Dispose of any disposable entities
            foreach (var item in LevelEntities)
            {
                if (item is IDisposable d)
                {
                    d.Dispose();
                }
            }
            content.Unload();
            UnbindEvents();
        }

    }
}
