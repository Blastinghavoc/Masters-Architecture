﻿using Microsoft.Xna.Framework;
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
using Coursework.Entities.Enemies;
using Coursework.Levels;

namespace Coursework.Entities
{
    public class Player : PhysicsObject, EventSubscriber
    {
        //Animation and drawing data
        //private Drawable currentAnimation;
        private Drawable[] animations;
        public SpriteEffects directionalEffect= SpriteEffects.None;

        private ContentManager content;//Player manages own content, as this persists between levels

        private Vector2 inputForce = Vector2.Zero;//Force currently aplied by user input
        private readonly Vector2 inputScale = GameData.Instance.playerData.inputScale;//Amount by which to scale input forces in each axis

        //Jumping data
        private bool isJumping = false;//Used for animation purposes
        private readonly int maxJumps = GameData.Instance.playerData.maxJumps;
        private int jumpsRemaining = GameData.Instance.playerData.maxJumps;        
        private int bottomAtLastUpdate = 0;//Based on variable of same name in platformer example, this is used to detect when the player is grounded
        private bool CanJump { get { return jumpsRemaining > 0; } }

        //FSM based animation control
        private FSM animator;

        //Manages the lifetime and effect of powerups on the player
        private PowerupManager powerupManager = new PowerupManager();

        public int Health { get; private set; } = GameData.Instance.playerData.startHealth;
        private readonly int maxHealth = GameData.Instance.playerData.startHealth;
        public bool IsAlive { get => Health > 0; }
        public bool IsInvincible = false;
        private readonly float damageImmunityDuration = GameData.Instance.playerData.damageImmunityDuration;
        private float damageImmunityTimerSeconds = 0;

        public Player(IServiceProvider provider,string contentRoot):base() {
            content = new ContentManager(provider, contentRoot);

            LoadContent();
            InitialiseAnimator();

            //Initialise physics state
            MaxSpeed = GameData.Instance.playerData.maxSpeed;
            DragFactor = GameData.Instance.playerData.dragFactor;
            Gravity = GameData.Instance.playerData.gravity;

            //Update collision bounds based on visible size
            UpdateBounds(Position, (int)Appearance.Size.X, (int)Appearance.Size.Y);

            BindEvents();
        }

        //Resets the player ready for a new level.
        public void Reset(Vector2 position)
        {
            Position = position;
            isJumping = false;
            jumpsRemaining = 0;
            Velocity = Vector2.Zero;
            directionalEffect = SpriteEffects.None;
            powerupManager.Reset();
        }

        /// <summary>
        /// Player update loop. Updates animation, physics and powerups.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            animator.Update(gameTime);     
            
            Force += inputForce * inputScale;//Add physics force based on input force    
            base.Update(gameTime);//Update physics

            if (Velocity.Y > 0)//If falling, then not jumping!
            {
                isJumping = false;
            }

            powerupManager.Update(gameTime);

            //Update immunity timer
            if (damageImmunityTimerSeconds> 0)
            {
                damageImmunityTimerSeconds -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (!IsAlive)
            {
                //Notify the event manager of the player's death
                GameEventManager.Instance.PlayerDied();
            }

            bottomAtLastUpdate = BoundingBox.Bottom;
            inputForce = Vector2.Zero;//Reset input forces for next update
        }

        /// <summary>
        /// Draw the player's current appearance, with a colour based on 
        /// the powerups active on them, and how recently they have taken damage.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="effect"></param>
        public override void Draw(SpriteBatch spriteBatch, SpriteEffects effect = SpriteEffects.None)
        {
            var damageColour = Color.Red;

            //Get colour from all active powerups
            var powerupColour = powerupManager.GetCumulativeEffectColour();

            //Interpolate with damage colour
            var resultColour = Color.Lerp(powerupColour, damageColour, Math.Max(damageImmunityTimerSeconds / damageImmunityDuration, 0));

            //Apply special effect color
            Appearance.color = resultColour;

            base.Draw(spriteBatch,effect | directionalEffect);
        }

        /// <summary>
        /// What to do while the player is colliding with something
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WhileColliding(object sender, PlayerCollisionEventArgs e)
        {
            switch (e.colllidedWith)
            {
                case TileDescriptor tile:
                    {
                        if (tile.collisionMode == TileCollisionMode.solid)
                        {
                            //Solid tiles -> solid collision response
                            SolidCollisionResponse(e.collisionDepth, tile.bounds);
                        }
                        else if (tile.collisionMode == TileCollisionMode.lava)
                        {
                            //Take damage on collision with lava
                            TakeDamage(1);
                        }
                    }
                    break;
                case Enemy enemy:
                    {
                        if (enemy.IsAlive)
                        {
                            //Provided the enemy is alive, they may damage the player
                            //Note that the player may be immune and ignore it.
                            TakeDamage(enemy.Damage);

                            if (enemy.IsSolid)
                            {//If the enemy is solid, perform collision response
                                SolidCollisionResponse(e.collisionDepth, enemy.BoundingBox);
                            }
                        }                        
                    }
                    break;
                default:
                    break;
            }               
        }

        /// <summary>
        /// Collision response when hitting a solid object.
        /// </summary>
        private void SolidCollisionResponse(Vector2 collisionDepth, Rectangle collidedWithBounds)
        {
            var absCollY = Math.Abs(collisionDepth.Y);
            var absCollX = Math.Abs(collisionDepth.X);

            //Collided from above something, therefore we are now grounded
            if (absCollY < absCollX && collisionDepth.Y < 0 && bottomAtLastUpdate >= collidedWithBounds.Top)
            {
                jumpsRemaining = maxJumps;//Reset jump counter
            }

            //Use base class collision response that moves the player such that they are no longer colliding
            StaticCollisionResponse(collisionDepth);
        }

        /// <summary>
        /// What to do when the player starts colliding with something.
        /// Handles squashing enemies to death.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCollisionEnter(object sender, PlayerCollisionEventArgs e)
        {
            Enemy enemy = e.colllidedWith as Enemy;
            if (enemy != null && enemy.IsAlive)
            {
                //If the player enters an enemies hitbox from above, they squash them

                var bottom = BoundingBox.GetBottomCenter();
                var enemyBottom = enemy.BoundingBox.GetBottomCenter();
                var enemyHeight = enemy.BoundingBox.Size.Y;

                //Must be above the enemy by at least half its height to squash it
                if (bottom.Y <= enemyBottom.Y - enemyHeight / 2)
                {
                    enemy.TakeDamage(1);
                }
                //Otherwise the squash fails. The player may take damage; see the WhileColliding function
            }
        }

        /// <summary>
        /// Attempt to use weapons. Actual weapon code is decoupled from the player
        /// </summary>
        /// <param name="location"></param>
        public void AttemptToUseWeapon(Point location) {
            var worldPoint = Camera.mainCamera.ScreenToWorldPoint(location);
            GameEventManager.Instance.PlayerAttemptToFireWeapon(this, worldPoint.ToVector2());
        }

        /// <summary>
        /// Attempt to take damage. The player gets a short
        /// period of immunity after taking damage.
        /// </summary>
        /// <param name="amount"></param>
        public void TakeDamage(int amount)
        {
            if (IsInvincible || damageImmunityTimerSeconds > 0)
            {
                return;//Can't take damage if immune
            }

            if (amount == 0)
            {
                return;
            }

            Health -= amount;
            damageImmunityTimerSeconds = damageImmunityDuration;
            GameEventManager.Instance.PlayerHealthChanged(this);
            //Fire the player died event if necessary
            if (!IsAlive)
            {
                GameEventManager.Instance.PlayerDied();
            }
        }

        /// <summary>
        /// Similar to TakeDamage, but with no "immunity".
        /// Cannot heal the player above their max health.
        /// </summary>
        /// <param name="amount"></param>
        public void Heal(int amount)
        {
            if (amount == 0)
            {
                return;
            }

            var tmpHealth = Health + amount;
            if (tmpHealth <= maxHealth)
            {
                Health = tmpHealth;
                GameEventManager.Instance.PlayerHealthChanged(this);
            }
        }

        /// <summary>
        /// Attempt to add a powerup effect to the player.
        /// Delegated on to the powerupManager.
        /// </summary>
        /// <param name="powerUpType"></param>
        public void AddPowerupEffect(PowerupType powerUpType)
        {
            powerupManager.AddPowerupEffect(powerUpType, this);
        }

        /// <summary>
        /// Forcibly kill the player.
        /// </summary>
        public void Kill()
        {
            Health = 0;
            GameEventManager.Instance.PlayerHealthChanged(this);
            GameEventManager.Instance.PlayerDied();            
        }

        /// <summary>
        /// Load all the animations/sprites used by the player
        /// </summary>
        private void LoadContent()
        {
            animations = new Drawable[3];//idle, walk, jump

            var animationData = GameData.Instance.playerData.walkAnimation;

            var frameDimensions = animationData.frameDimensions;            
            var texScale = Utils.scaleForTexture(frameDimensions.X);

            string filePath = GameData.Instance.playerData.walkAnimationPath;

            Unpacker unpacker = new Unpacker(content);
            var walkAnim = unpacker.Unpack(animationData,filePath);
            walkAnim.LayerDepth = 0.1f;

            animations[1] = walkAnim;

            Texture2D idleTex = content.Load<Texture2D>(GameData.Instance.playerData.idlePath);
            Sprite idleSprite = new Sprite(idleTex, texScale, Color.White);
            idleSprite.LayerDepth = 0.1f;
            animations[0] = idleSprite;

            Texture2D jumpTex = content.Load<Texture2D>(GameData.Instance.playerData.jumpPath);
            Sprite jumpSprite = new Sprite(jumpTex, texScale, Color.White);
            jumpSprite.LayerDepth = 0.1f;
            animations[2] = jumpSprite;

        }

        /// <summary>
        /// Create the animator FSM for the player.
        /// </summary>
        private void InitialiseAnimator()
        {
            animator = new FSM(this);
            var idle = new Idle(SetCurrentAnimation);
            var walking = new Walking(SetCurrentAnimation);
            var jumping = new Jumping(SetCurrentAnimation);
            var dead = new Dead();

            //Utility functions for use in transitions
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

        /// <summary>
        /// Function to set the current animation. Passed to relevant
        /// states in the animator FSM so that they can call it.
        /// </summary>
        /// <param name="index"></param>
        private void SetCurrentAnimation(int index)
        {            
            Appearance = animations[index];
            Appearance.SetPosition(Position);
            if (Appearance is AbstractAnimation anim)
            {
                anim.Reset();
            }
        }

        public void BindEvents()
        {
            GameEventManager.Instance.WhilePlayerColliding += WhileColliding;
            GameEventManager.Instance.OnPlayerCollisionEnter += OnCollisionEnter;
        }

        public void UnbindEvents()
        {            
            GameEventManager.Instance.WhilePlayerColliding -= WhileColliding;
            GameEventManager.Instance.OnPlayerCollisionEnter -= OnCollisionEnter;
        }

        public void Dispose()
        {
            powerupManager.Dispose();
            content.Unload();
            UnbindEvents();
        }

        /// <summary>
        /// Input function for going left
        /// </summary>
        public void LeftHeld()
        {
            inputForce -= Vector2.UnitX;
        }

        /// <summary>
        /// Input function for going right
        /// </summary>
        public void RightHeld()
        {
            inputForce += Vector2.UnitX;
        }

        /// <summary>
        /// Input function for jumping
        /// </summary>
        public void Jump()
        {
            if (CanJump)
            {
                inputForce -= Vector2.UnitY;//Subtraction because Y axis points down
                isJumping = true;
                --jumpsRemaining;
            }
        }           

    }
}
