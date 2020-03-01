using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Animation;
using Coursework.StateMachine;
using Coursework.StateMachine.Player;

namespace Coursework.Entities
{
    class Player : CollidableObject, IDisposable
    {
        private Drawable currentAnimation;
        private Drawable[] animations;
        public SpriteEffects directionalEffect= SpriteEffects.None;

        private readonly int width = GameData.Instance.levelConstants.tileSize.Y;//Width in world coords

        private ContentManager content;//Player currently manages own content, as this persists between levels

        private Vector2 inputForce;//"Force" currently being applied to the player by user input
        private readonly Vector2 inputScale = new Vector2(800,19600);//Amount by which to scale input forces in each axis
        private readonly Vector2 maxSpeed = new Vector2(500,500);
        private readonly Vector2 dragFactor = new Vector2(0.9f,1);//No vertical drag
        private const float gravity = 981f;

        private bool isJumping = false;
        private bool isGrounded = false;
        private bool CanJump { get { return !isJumping && isGrounded; } }

        private Vector2 previousPosition;//Position of player before they tried to move each frame

        private FSM animator;

        public int Health { get; private set; } = 3;
        public bool IsAlive { get => Health > 0; }

        public Player(IServiceProvider provider,string contentRoot) {
            Position = new Vector2(0, 0);
            content = new ContentManager(provider, contentRoot);

            LoadContent();
            InitialiseAnimator();

            //Update collision bounds based on visible size
            UpdateBounds(Position, width, (int)(currentAnimation.Size.Y));

            GameEventManager.Instance.OnPlayerColliding += WhileColliding;
            GameEventManager.Instance.OnPlayerCollisionEnter += OnCollisionEnter;
        }

        public override void Update(GameTime gameTime)
        {
            animator.Update(gameTime);
            UpdatePhysics(gameTime);
            currentAnimation.Update(gameTime,Position);
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

            if (Velocity.Y > 0)
            {
                isJumping = false;//Falling
            }

            //update position
            Position += Velocity * dt;

            //Reset input force for next update
            inputForce = Vector2.Zero;
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteEffects effect = SpriteEffects.None)
        {
            currentAnimation.Draw(spriteBatch,effect | directionalEffect);
        }

        //What to do while the player is colliding with something
        private void WhileColliding(object sender, PlayerCollisionEventArgs e)
        {
            Level level = e.colllidedWith as Level;

            if (level != null)//Resolve collisions with level
            {                
                //Collided from above something
                if (e.collisionDepth.Y < 0)
                {
                    isGrounded = true;
                }                

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
                return;
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

        private void LoadContent()
        {
            animations = new Drawable[3];//idle, walk, jump

            var frameDimensions = GameData.Instance.playerData.frameDimensions;
            var texScale = width / (float)frameDimensions.X;

            string filePath = GameData.Instance.playerData.walkAnimationPath;
            int numFrames = GameData.Instance.playerData.numWalkFrames;
            Texture2D[] frames = new Texture2D[numFrames];
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

            var frameTime = GameData.Instance.playerData.walkFrameTime;

            var walkAnim = new MultiImageAnimation(frames, Position, frameDimensions.X, frameDimensions.Y,
                numFrames, frameTime, Color.White, texScale, true);

            animations[1] = walkAnim;

            Texture2D idleTex = content.Load<Texture2D>(GameData.Instance.playerData.idlePath);
            Sprite idleSprite = new Sprite(idleTex, new Vector2(texScale), Color.White);
            animations[0] = idleSprite;

            Texture2D jumpTex = content.Load<Texture2D>(GameData.Instance.playerData.jumpPath);
            Sprite jumpSprite = new Sprite(jumpTex, new Vector2(texScale), Color.White);
            animations[2] = jumpSprite;

        }

        private void InitialiseAnimator()
        {
            animator = new FSM(this);
            var idle = new Idle(SetCurrentAnimation);
            var walking = new Walking(SetCurrentAnimation);
            var jumping = new Jumping(SetCurrentAnimation);
            var dead = new Dead();

            Func<bool> diedFunc = () => { return !IsAlive; };
            Func<bool> jumpingFunc = () => { return isJumping; };
            Func<bool> movingFunc = () => { return inputForce.X != 0; };

            idle.AddTransition(walking, movingFunc);
            idle.AddTransition(dead, diedFunc );
            idle.AddTransition(jumping, jumpingFunc);

            walking.AddTransition(idle, () => { return !movingFunc(); });
            walking.AddTransition(dead, diedFunc);
            walking.AddTransition(jumping, jumpingFunc);

            jumping.AddTransition(walking, () => { return movingFunc() && !isJumping; });
            jumping.AddTransition(idle, () => { return !movingFunc() && !isJumping; });
            jumping.AddTransition(dead, diedFunc);

            animator.AddStates(idle, walking, jumping, dead);
            animator.Initialise("Idle");
        }

        private void SetCurrentAnimation(int index)
        {
            currentAnimation = animations[index];
            var anim = currentAnimation as AbstractAnimation;
            if (anim !=null)
            {
                anim.Reset();
            }
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
            if (CanJump)
            {
                inputForce -= Vector2.UnitY;//Subtraction because Y axis points down
                isJumping = true;
                isGrounded = false;
            }
        }

        public void Descend()
        {
            inputForce += Vector2.UnitY;
        }
    }
}
