using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Animation;

namespace Coursework.Entities
{
    class Player : CollidableObject, IDisposable
    {
        private Drawable animation;
        private readonly int width = GameData.tileSize.Y;//Width in world coords

        private ContentManager content;//Player currently manages own content, as this persists between levels

        private Vector2 inputForce;//"Force" currently being applied to the player by user input
        private readonly Vector2 inputScale = new Vector2(800,19600);//Amount by which to scale input forces in each axis
        private readonly Vector2 maxSpeed = new Vector2(500,500);
        private readonly Vector2 dragFactor = new Vector2(0.9f,1);//No vertical drag
        private const float gravity = 981f;

        private Vector2 previousPosition;//Position of player before they tried to move each frame

        public int Health { get; private set; }
        public bool IsAlive { get => Health > 0; }

        public Player(IServiceProvider provider,string contentRoot) {
            Position = new Vector2(0, 0);
            content = new ContentManager(provider, contentRoot);
            LoadContent();
            //Update collision bounds based on visible size
            UpdateBounds(Position, width, (int)(animation.Size.Y));

            GameEventManager.Instance.OnPlayerColliding += WhileColliding;
            GameEventManager.Instance.OnPlayerCollisionEnter += OnCollisionEnter;
        }

        public override void Update(GameTime gameTime)
        {
            UpdatePhysics(gameTime);
            animation.Update(gameTime,Position);
        }

        private void UpdatePhysics(GameTime gameTime)
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

        public override void Draw(SpriteBatch spriteBatch, SpriteEffects effect = SpriteEffects.None)
        {
            //TODO animation controller (probably FSM)            
            if (Velocity.X < 0)
            {
                effect = effect | SpriteEffects.FlipHorizontally;
            }

            animation.Draw(spriteBatch,effect);

        }

        //What to do while the player is colliding with something
        private void WhileColliding(object sender, PlayerCollisionEventArgs e)
        {
            Level level = e.colllidedWith as Level;

            if (level != null)//Resolve collisions with level
            {
                StaticCollisionResponse(e.collisionDepth);
                return;
            }                    
        }

        //What to do only when the player starts colliding with something
        private void OnCollisionEnter(object sender, PlayerCollisionEventArgs e)
        {
            Enemy enemy = e.colllidedWith as Enemy;
            if (enemy != null)
            {
                var bottom = BoundingBox.GetBottomCenter();
                var enemyBottom = enemy.BoundingBox.GetBottomCenter();
                var enemyHeight = enemy.Appearance.Size.Y;
                //Must be above the enemy by at least half its height to squash it
                if (bottom.Y <= enemyBottom.Y - enemyHeight / 2)
                {
                    enemy.Health = 0;
                }
                else
                {
                    TakeDamage(enemy.Damage);
                }
            }
        }

        public void TakeDamage(int amount)
        {
            Health -= amount;
            GameEventManager.Instance.PlayerHealthChanged(this,amount);
        }

        public void SetPosition(Vector2 pos)
        {
            Position = pos;
        }

        public void LoadContent()
        {
            var frameWidth = 72;
            var texScale = width / (float)frameWidth;

            string filePath = GameData.GraphicsDirectory + "Player/p1_walk/PNG/p1_walk";
            int numFrames = 11;
            Texture2D[] frames = new Texture2D[11];
            for (int i = 1; i <= numFrames; i++)
            {
                string fileName = filePath;
                if (i < 10)
                {
                    fileName += "0";
                }
                fileName += i.ToString();
                frames[i - 1] = content.Load<Texture2D>(fileName);
            }

            animation = new MultiImageAnimation(frames, Position, 72, 97, 11, 32, Color.White, texScale, true);
            //var spriteSheet = content.Load<Texture2D>(GameData.GraphicsDirectory+"Player/p1_walk/p1_walkRegular");
            //animation = new SpriteSheetAnimation(spriteSheet, Position, 72, 97, 11, 32, Color.White, texScale, true);
        }

        public void Dispose()
        {
            content.Unload();
            //Unsubscribe from events
            GameEventManager.Instance.OnPlayerColliding -= WhileColliding;
            GameEventManager.Instance.OnPlayerCollisionEnter -= OnCollisionEnter;
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
