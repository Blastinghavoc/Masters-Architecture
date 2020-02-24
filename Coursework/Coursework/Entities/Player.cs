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
        private readonly Vector2 inputScale = new Vector2(800,19600);//Amount by which to scale input forces in each axis
        private readonly Vector2 maxSpeed = new Vector2(500,500);
        private readonly Vector2 dragFactor = new Vector2(0.9f,1);//No vertical drag
        private const float gravity = 981f;

        private Vector2 previousPosition;//Position of player before they tried to move each frame

        public Player(IServiceProvider provider,string contentRoot) {
            Position = new Vector2(0, 0);
            content = new ContentManager(provider, contentRoot);
            LoadContent();
            //Update collision bounds based on visible size
            UpdateBounds(Position, (int)(texture.Width * textureScale.X), (int)(texture.Height * textureScale.Y));
        }

        public override void Update(GameTime gameTime)
        {
            previousPosition = Position;

            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Basic physics based movement. TODO extract to physics manager?
            Vector2 acceleration = inputScale * inputForce;
            acceleration.Y += gravity;//gravity

            Velocity += acceleration * dt;

            //Apply drag
            Velocity *= dragFactor;

            //Enforce speed limit
            var clampedX = MathHelper.Clamp(Velocity.X, -maxSpeed.X, maxSpeed.X);
            var clampedY = MathHelper.Clamp(Velocity.Y, -maxSpeed.Y, maxSpeed.Y);
            Velocity = new Vector2(clampedX, clampedY);
            
            //update position
            Position += Velocity * dt;

            //Reset input force for next update
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
            inputForce -= Vector2.UnitY;//Subtraction because Y axis points down
        }

        public void Descend()
        {
            inputForce += Vector2.UnitY;
        }
    }
}
