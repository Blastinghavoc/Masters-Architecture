using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Graphics;
using Coursework.Entities;
using Coursework.StateMachine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Slime = Coursework.StateMachine.AI.Slime;
using Fly = Coursework.StateMachine.AI.Fly;
using Coursework.StateMachine.AI;

namespace Coursework.Entities.Enemies
{
    /// <summary>
    /// Base class for all enemies in the game
    /// </summary>
    public abstract class Enemy : CollidableObject, IDisposable
    {
        public int Health { get; protected set; }
        public int Damage { get; set; }
        public bool IsAlive { get => Health > 0; }

        public bool IsInvincible = false;//Determines whether the enemy can take damage

        public bool IsSolid = false;//Determines how the player collides with the enemy

        protected FSM brain;

        public SpriteEffects DirectionalEffect { get; set; } = SpriteEffects.None;

        public Decal CorpseAppearance { get; protected set; }

        public Enemy(Drawable appearance,Decal corpseAppearance, Vector2 position, int health, int damage,bool invincible=false, bool solid = false) : base(appearance, position)
        {
            Health = health;
            Damage = damage;
            CorpseAppearance = corpseAppearance;
            IsInvincible = invincible;
            IsSolid = solid;

            InitialiseBrain();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            brain.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteEffects effect = SpriteEffects.None)
        {
            effect = effect | DirectionalEffect;

            base.Draw(spriteBatch, effect);
        }

        public virtual new Enemy Clone()
        {
            var tmp = base.Clone() as Enemy;
            tmp.InitialiseBrain();
            return tmp;
        }

        public void SetAppearance(Drawable app)
        {
            Appearance = app;
        }

        /// <summary>
        /// Subclasses must define their own behaviour
        /// </summary>
        protected abstract void InitialiseBrain();

        public void SetHealth(int amount)
        {
            Health = amount;
        }

        public void TakeDamage(int amount)
        {
            if (IsInvincible)
            {
                return;
            }
            Health -= amount;
        }

        /// <summary>
        /// The brain must be disposed of to ensure any events
        /// subscribed to by brain states are unsubscribed.
        /// </summary>
        public void Dispose()
        {
            brain.Dispose();
        }
    }
}
