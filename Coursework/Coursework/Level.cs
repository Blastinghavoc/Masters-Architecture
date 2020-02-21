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
    /// Class representing a whole level in the game
    /// </summary>
    class Level: IDisposable
    {
        ContentManager content;//The content specific to this level

        Tile[,] tiles;//2D array of tiles

        Dictionary<char, Texture2D> tileSkins;//Textures for each type of tile

        readonly Vector2 tileSize = new Vector2(32,32);//Tile size in world coordinates
        readonly float tileResolution = 256.0f;
        readonly Vector2 tileTextureScale;
        readonly Rectangle bounds;//Level bounds in world coordinates

        public Level(IServiceProvider provider, string contentRoot)
        {
            content = new ContentManager(provider, contentRoot);
            tileSkins = new Dictionary<char, Texture2D>();

            tileTextureScale = tileSize / tileResolution;


            LoadContent();
            InitialiseTiles();


            bounds = new Rectangle(Point.Zero, new Point(tiles.GetLength(0)*(int)tileSize.X, tiles.GetLength(1)*(int)tileSize.Y));
        }

        public void Update(Camera camera)
        {
            //Clamp the camera's position to be within level bounds
            var visibleArea = camera.VisibleArea;

            //Snap to bottom left corner, TODO remove
            //camera.Position= new Vector2(0 * tileSize.X, tiles.GetLength(1) * tileSize.Y);

            var halfWidth = visibleArea.Width / 2f;
            var halfHeight = visibleArea.Height / 2f;

            var adjustedX = MathHelper.Clamp(camera.Position.X, bounds.Left + halfWidth, bounds.Right - halfWidth);
            var adjustedY = MathHelper.Clamp(camera.Position.Y, bounds.Top + halfHeight, bounds.Bottom - halfHeight);
            
            camera.Position = new Vector2(adjustedX, adjustedY);
        }

        private void LoadContent()
        {
            tileSkins['G'] = content.Load<Texture2D>("Tiles/greyTile");
        }

        private void InitialiseTiles()
        { 
            int width = 40;
            int height = 30;

            tiles = new Tile[width, height];//TESTING
            for (int i = 0; i < width; i++)
            {
                tiles[i, height-1] = new Tile(tileSkins['G'], TileCollisionMode.solid);
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

        public void Dispose()
        {
            content.Unload();
        }
    }
}
