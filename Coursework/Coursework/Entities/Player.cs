using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Entities
{
    class Player : CollidableObject, IDisposable
    {
        private Texture2D texture;
        private readonly Vector2 textureScale = new Vector2(1.0f / 8);
        private ContentManager content;//Player currently manages own content, as this persists between levels

        private Vector2 inputForce;//"Force" currently being applied to the player by user input
        private const float inputScale = 50f;//Amount by which to scale input forces


        public Player(IServiceProvider provider,string contentRoot) {
            content = new ContentManager(provider, contentRoot);
            LoadContent();
        }

        public override void Update(GameTime gameTime)
        {

            //Basic movement, TODO replace with physics
            Position += inputScale * inputForce * (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Reset for next update
            inputForce = Vector2.Zero;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {            
            spriteBatch.Draw(texture, Position, null, Color.White, 0, Vector2.Zero, textureScale, SpriteEffects.None, 0);
        }

        public void SetPosition(Vector2 pos)
        {
            Position = pos;
        }

        public void LoadContent()
        {
            texture = content.Load<Texture2D>("Sprites/player");
        }

        public void Dispose()
        {
            content.Unload();
        }

        public void LeftHeld()
        {
            inputForce -= Vector2.UnitX;
        }

        public void RightHeld()
        {
            inputForce += Vector2.UnitX;
        }

        public void Jump()
        {
            inputForce -= 3*Vector2.UnitY;//Subtraction because Y axis points down
        }

        public void Descend()
        {
            inputForce += Vector2.UnitY;
        }
    }
}
