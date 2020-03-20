using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Graphics;
using Coursework.StateMachine;
using Coursework.StateMachine.Player;
using Coursework.Serialization;
using Coursework.Powerups;

namespace Coursework.Entities
{
    public class Player : PhysicsObject, EventSubscriber
    {
        private Drawable currentAnimation;
        private Drawable[] animations;
        public SpriteEffects directionalEffect= SpriteEffects.None;

        private readonly int width = GameData.Instance.levelConstants.tileSize.Y;//Width in world coords

        private ContentManager content;//Player currently manages own content, as this persists between levels

        private Vector2 inputForce = Vector2.Zero;//Force currently aplied by user input
        private readonly Vector2 inputScale = GameData.Instance.playerData.inputScale;//Amount by which to scale input forces in each axis

        private bool isJumping = false;//Used for animation purposes
        private readonly int maxJumps = GameData.Instance.playerData.maxJumps;
        private int jumpsRemaining = GameData.Instance.playerData.maxJumps;
        //Based on variable of same name in platformer example, this is used to detect when the player is grounded
        private int bottomAtLastUpdate = 0;
        private bool CanJump { get { return jumpsRemaining > 0; } }

        //FSM based animation control
        private FSM animator;

        //Manages the lifetime and effect of powerups on the player
        private PowerupManager powerupManager = new PowerupManager();

        public int Health { get; private set; } = GameData.Instance.playerData.startHealth;
        public bool IsAlive { get => Health > 0; }

        public Player(IServiceProvider provider,string contentRoot) {
            Position = new Vector2(0, 0);
            content = new ContentManager(provider, contentRoot);

            LoadContent();
            InitialiseAnimator();

            //Initialise physics state
            MaxSpeed = GameData.Instance.playerData.maxSpeed;
            DragFactor = GameData.Instance.playerData.dragFactor;
            Gravity = GameData.Instance.playerData.gravity;

            //Update collision bounds based on visible size
            UpdateBounds(Position, width, (int)(currentAnimation.Size.Y));

            BindEvents();
        }

        //Completely resets the player, health, position, velocity etc
        public void HardReset() 
        {
            Position = new Vector2(0, 0);
            InitialiseAnimator();
            Health = GameData.Instance.playerData.startHealth;
            GameEventManager.Instance.PlayerHealthChanged(this);
            isJumping = false;
            jumpsRemaining = maxJumps; 
            Velocity = Vector2.Zero;
            directionalEffect = SpriteEffects.None;
            powerupManager.Reset();
        }

        public override void Update(GameTime gameTime)
        {
            Force += inputForce * inputScale;//Add physics force based on input force    
            base.Update(gameTime);//Update physics

            if (Velocity.Y > 0)//If falling, then not jumping!
            {
                isJumping = false;
            }

            animator.Update(gameTime);
            currentAnimation.Update(gameTime,Position);
            powerupManager.Update(gameTime);

            if (!IsAlive)
            {
                //Notify the event manager of the player's death
                GameEventManager.Instance.PlayerDied();
            }

            bottomAtLastUpdate = BoundingBox.Bottom;
            inputForce = Vector2.Zero;//Reset input forces for next update
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteEffects effect = SpriteEffects.None)
        {
            var defaultColor = currentAnimation.color;

            //Apply special effect color
            currentAnimation.color = powerupManager.GetCumulativeEffectColour();

            currentAnimation.Draw(spriteBatch,effect | directionalEffect);

            currentAnimation.color = defaultColor;
        }

        //What to do while the player is colliding with something
        private void WhileColliding(object sender, PlayerCollisionEventArgs e)
        {
            TileDescriptor tile = e.colllidedWith as TileDescriptor;

            if (tile != null)//Resolve collisions with level tile
            {

                var absCollY = Math.Abs(e.collisionDepth.Y);
                var absCollX = Math.Abs(e.collisionDepth.X);

                //Collided from above something
                if (absCollY < absCollX && e.collisionDepth.Y < 0 && bottomAtLastUpdate >= tile.bounds.Top)
                {
                    jumpsRemaining = maxJumps;
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
            }
        }

        //Attempt to use weapon(s). Actual weapon code is decoupled from player
        public void AttemptToUseWeapon(Point location) {
            var worldPoint = Camera.mainCamera.ScreenToWorldPoint(location);
            GameEventManager.Instance.PlayerAttemptToFireWeapon(this, worldPoint.ToVector2());
        }

        public void TakeDamage(int amount)
        {
            Health -= amount;
            GameEventManager.Instance.PlayerHealthChanged(this);
        }

        public void SetPosition(Vector2 pos)
        {
            Position = pos;
        }

        private void LoadContent()
        {
            animations = new Drawable[3];//idle, walk, jump

            var animationData = GameData.Instance.playerData.walkAnimation;

            var frameDimensions = animationData.frameDimensions;
            var texScale = width / (float)frameDimensions.X;

            string filePath = GameData.Instance.playerData.walkAnimationPath;
            int numFrames = animationData.numFrames;
            Texture2D[] frames = new Texture2D[numFrames];
            string basePath = filePath + animationData.baseName;
            for (int i = 1; i <= numFrames; i++)
            {
                string fileName = basePath;
                if (i < 10)
                {
                    fileName += "0";
                }
                fileName += i.ToString();
                frames[i - 1] = content.Load<Texture2D>(fileName);
            }

            var frameTime = animationData.frameDuration;

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

        public void BindEvents()
        {
            GameEventManager.Instance.OnPlayerColliding += WhileColliding;
            GameEventManager.Instance.OnPlayerCollisionEnter += OnCollisionEnter;
        }

        public void UnbindEvents()
        {            
            GameEventManager.Instance.OnPlayerColliding -= WhileColliding;
            GameEventManager.Instance.OnPlayerCollisionEnter -= OnCollisionEnter;
        }

        public void Dispose()
        {
            powerupManager.Dispose();
            content.Unload();
            UnbindEvents();
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
                --jumpsRemaining;
            }
        }

        public void Crouch()
        {
            //Currently does nothing, TODO implement platforms that you crouch to go down through?
        }                

    }
}
