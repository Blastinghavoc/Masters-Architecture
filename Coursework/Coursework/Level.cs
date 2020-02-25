using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework
{
    /// <summary>
    /// Class representing a whole level in the game, based on class of same name in 
    /// the platformer demo (lab2)
    /// </summary>
    class Level: IDisposable
    {
        ContentManager content;//The content specific to this level

        Tile[,] tiles;//2D array of tiles

        Dictionary<Color, Texture2D> tileSkins;//Textures for each type of tile        

        readonly float tileResolution = 70.0f;//Resolution of tile images, TODO configure from file
        readonly Vector2 tileTextureScale;
        readonly Rectangle bounds;//Level bounds in world coordinates
        readonly int levelNumber;

        public readonly Point tileSize;

        readonly string tileFilePath = GameData.GraphicsDirectory+"Tiles/";

        public Level(IServiceProvider provider, string contentRoot,int levelNum = 0)
        {
            content = new ContentManager(provider, contentRoot);
            tileSkins = new Dictionary<Color, Texture2D>();

            tileSize = GameData.tileSize;

            tileTextureScale = new Vector2(tileSize.X/tileResolution,tileSize.Y/tileResolution);
            levelNumber = levelNum;


            LoadContent();
            InitialiseTiles();


            bounds = new Rectangle(Point.Zero, new Point(tiles.GetLength(0)* tileSize.X, tiles.GetLength(1)* tileSize.Y));
        }

        public void Update(Camera camera)
        {
            //Clamp the camera's position to be within level bounds
            var visibleArea = camera.VisibleArea;

            var halfWidth = visibleArea.Width / 2f;
            var halfHeight = visibleArea.Height / 2f;

            var adjustedX = MathHelper.Clamp(camera.Position.X, bounds.Left + halfWidth, bounds.Right - halfWidth);
            var adjustedY = MathHelper.Clamp(camera.Position.Y, bounds.Top + halfHeight, bounds.Bottom - halfHeight);
            
            camera.Position = new Vector2(adjustedX, adjustedY);
        }

        private void LoadContent()
        {
            tileSkins[Color.Black] = content.Load<Texture2D>(tileFilePath + "grassMid");
        }

        private void InitialiseTiles()
        {
            Texture2D map = content.Load<Texture2D>("Maps/map"+levelNumber);     
            

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
                    Texture2D tileTexture;
                    if (tileSkins.TryGetValue(colour,out tileTexture))
                    {
                        tiles[i, j] = new Tile(tileTexture,TileCollisionMode.solid);
                    }
                }
            }

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
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


        public void Dispose()
        {
            content.Unload();
        }
    }
}
