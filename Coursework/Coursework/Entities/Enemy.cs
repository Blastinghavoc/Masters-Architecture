using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Animation;
using Coursework.Entities;
using Coursework.StateMachine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Slime = Coursework.StateMachine.AI.Slime;

namespace Coursework.Entities
{
    enum EnemyType
    {
        slime
    }

    class Enemy : Interactable
    {
        public int Health { get; set; }
        public int Damage { get; protected set; }
        public bool IsAlive { get => Health > 0; }

        public EnemyType Type { get; private set; }

        private FSM brain;

        public SpriteEffects DirectionalEffect { get; set; } = SpriteEffects.None;

        public Enemy(Drawable appearance, Vector2 position, int health, int damage,EnemyType type = EnemyType.slime) : base(appearance, position)
        {
            Health = health;
            Damage = damage;
            this.Type = type;

            InitialiseBrain();
        }

        public override void Update(GameTime gameTime)
        {
            brain.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteEffects effect = SpriteEffects.None)
        {
            effect = effect | DirectionalEffect;

            base.Draw(spriteBatch, effect);
        }

        public void SetPosition(Vector2 pos)
        {
            Position = pos;
        }

        private void InitialiseBrain()
        {
            brain = new FSM(this);

            switch (Type)
            {
                case EnemyType.slime:
                    {
                        State patrol = new Slime.Patrol();
                        State dead = new Slime.Dead();
                        patrol.AddTransition(new Transition(dead, () => { return !this.IsAlive; }));

                        brain.AddState(patrol);
                        brain.AddState(dead);
                        brain.Initialise("Patrol");
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
