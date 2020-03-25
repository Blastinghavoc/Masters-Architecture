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

namespace Coursework.Entities
{
    public enum EnemyType
    {
        slime,
        fly,
        blocker
    }

    public class Enemy : Interactable
    {
        public int Health { get; protected set; }
        public int Damage { get; set; }
        public bool IsAlive { get => Health > 0; }

        public readonly bool IsInvincible = false;

        public readonly bool IsSolid = false;//Determines how the player collides with the enemy

        public EnemyType enemyType { get; private set; }

        private FSM brain;

        public SpriteEffects DirectionalEffect { get; set; } = SpriteEffects.None;

        public Decal CorpseAppearance { get; protected set; }

        public Enemy(Drawable appearance,Decal corpseAppearance, Vector2 position, int health, int damage,EnemyType type = EnemyType.slime,bool invincible=false, bool solid = false) : base(appearance, position)
        {
            Health = health;
            Damage = damage;
            enemyType = type;
            interactableType = InteractableType.enemy;
            CorpseAppearance = corpseAppearance;
            IsInvincible = invincible;
            IsSolid = solid;

            InitialiseBrain();
        }

        public override void Update(GameTime gameTime)
        {
            brain.Update(gameTime);
            Appearance.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteEffects effect = SpriteEffects.None)
        {
            effect = effect | DirectionalEffect;

            base.Draw(spriteBatch, effect);
        }

        public new Enemy Clone()
        {
            var tmp = this.MemberwiseClone() as Enemy;
            tmp.Appearance = Appearance.Clone();
            tmp.InitialiseBrain();
            return tmp;
        }

        public void SetAppearance(Drawable app)
        {
            this.Appearance = app;
        }

        private void InitialiseBrain()
        {
            brain = BrainFactory.GetBrainFor(this);
        }

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
    }
}
