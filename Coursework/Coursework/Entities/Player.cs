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
using Coursework.Serialization;

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
        private readonly Vector2 inputScale = GameData.Instance.playerData.inputScale;//Amount by which to scale input forces in each axis
        private readonly Vector2 maxSpeed = GameData.Instance.playerData.maxSpeed;
        private readonly Vector2 dragFactor = GameData.Instance.playerData.dragFactor;//No vertical drag
        private readonly float gravity = GameData.Instance.playerData.gravity;

        private bool isJumping = false;//Used for animation purposes
        private readonly int maxJumps = GameData.Instance.playerData.maxJumps;
        private int jumpsRemaining = GameData.Instance.playerData.maxJumps;
        private bool isGrounded = false;
        //Based on variable of same name in platformer example, this is used to detect when the player is grounded
        private int bottomAtLastUpdate = 0;
        private bool CanJump { get { return jumpsRemaining > 0; } }

        private Vector2 previousPosition;//Position of player before they tried to move each frame

        private FSM animator;

        private PowerupManager powerupManager = new PowerupManager();

        public int Health { get; private set; } = GameData.Instance.playerData.startHealth;
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

        //Completely resets the player, health, position, velocity etc
        public void HardReset() 
        {
            Position = new Vector2(0, 0);
            InitialiseAnimator();
            Health = GameData.Instance.playerData.startHealth;
            GameEventManager.Instance.PlayerHealthChanged(this);
            isJumping = false;
            jumpsRemaining = maxJumps; 
            isGrounded = false;
            Velocity = Vector2.Zero;
            directionalEffect = SpriteEffects.None;
            powerupManager.Reset();
        }

        public override void Update(GameTime gameTime)
        {            
            animator.Update(gameTime);
            UpdatePhysics(gameTime);
            currentAnimation.Update(gameTime,Position);
            powerupManager.Update(gameTime);

            if (!IsAlive)
            {
                //Notify the event manager of the player's death
                GameEventManager.Instance.PlayerDied();
            }

            bottomAtLastUpdate = BoundingBox.Bottom;
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
            var defaultColor = currentAnimation.color;
            //Apply special effect color
            if (powerupManager.IsPowerupActive(PowerupManager.powerUpType.fireball))
            {
                currentAnimation.color = Color.Orange;
            }

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
                    isGrounded = true;
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
                return;
            }

            Interactable interact = e.colllidedWith as Interactable;
            if (interact != null)
            {
                if (interact.interactableType == InteractableType.powerup_fireball)
                {
                    powerupManager.AddPowerup(PowerupManager.powerUpType.fireball, 10);
                }
                return;
            }
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
                --jumpsRemaining;
            }
        }

        public void Crouch()
        {
            //Currently does nothing, TODO implement platforms that you crouch to go down through?
        }

        //When the mouse button is clicked, shoot a fireball if the player has the appropriate powerup
        public void OnMouseButtonDown(Point location)
        {
            if (powerupManager.IsPowerupActive(PowerupManager.powerUpType.fireball))
            {
                var worldPoint = Camera.mainCamera.ScreenToWorldPoint(location);
                GameEventManager.Instance.LaunchProjectile(Projectiles.ProjectileType.fireball,Position, worldPoint, false);
            }
        }


        private class PowerupManager {
            public enum powerUpType {
                fireball
            }

            //Active powerups, and their remaining time
            private Dictionary<powerUpType, float> activePowerups = new Dictionary<powerUpType, float>();

            public void Update(GameTime gameTime) {
                float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;

                List<powerUpType> keys = new List<powerUpType>(activePowerups.Keys);
                //Update time remaining for each powerup
                foreach (var key in keys)
                {
                    var timeLeft = activePowerups[key];
                    timeLeft -= dt;
                    if (timeLeft <= 0)
                    {
                        activePowerups.Remove(key);
                    }
                    else {
                        activePowerups[key] = timeLeft;
                    }
                }
            }

            public void AddPowerup(powerUpType type, float duration = 5) {
                activePowerups[type] = duration;
            }

            public bool IsPowerupActive(powerUpType type) {
                return activePowerups.ContainsKey(type);
            }

            public void Reset() {
                activePowerups.Clear();
            }
        }
    }
}
