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
    class Player : GameObject, IContentOwner
    {
        private Texture2D texture;
        private readonly Vector2 textureScale = new Vector2(1.0f / 8);
        private ContentManager content;//Player currently manages own content, as this persists between levels


        public Player(IServiceProvider provider,string contentRoot) {
            content = new ContentManager(provider, contentRoot);
            LoadContent();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {            
            spriteBatch.Draw(texture, Position, null, Color.White, 0, Vector2.Zero, textureScale, SpriteEffects.None, 0);
        }

        public void LoadContent()
        {
            texture = content.Load<Texture2D>("Sprites/player");
        }

        public void ReleaseContent()
        {
            content.Unload();
        }
    }
}
