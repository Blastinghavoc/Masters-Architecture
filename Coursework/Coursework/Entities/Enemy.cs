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
using Fly = Coursework.StateMachine.AI.Fly;
using Coursework.StateMachine.AI;

namespace Coursework.Entities
{
    public enum EnemyType
    {
        slime,
        fly
    }

    class Enemy : Interactable
    {
        public int Health { get; set; }
        public int Damage { get; set; }
        public bool IsAlive { get => Health > 0; }

        public EnemyType enemyType { get; private set; }

        private FSM brain;

        public SpriteEffects DirectionalEffect { get; set; } = SpriteEffects.None;

        public Enemy(Drawable appearance, Vector2 position, int health, int damage,EnemyType type = EnemyType.slime) : base(appearance, position)
        {
            Health = health;
            Damage = damage;
            this.enemyType = type;
            interactableType = InteractableType.enemy;

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

        public void SetPosition(Vector2 pos)
        {
            Position = pos;
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
            brain = new FSM(this);

            switch (enemyType)
            {
                case EnemyType.slime:
                    {
                        State patrol = new Slime.Patrol();
                        State dead = new Dead();
                        patrol.AddTransition(dead, () => { return !this.IsAlive; });

                        brain.AddState(patrol);
                        brain.AddState(dead);
                        brain.Initialise("Patrol");
                    }
                    break;
                case EnemyType.fly:
                    {
                        var patrol = new Fly.Patrol();
                        var dying = new Fly.Dying();
                        dying.DyingAppearance = Level.CurrentLevel.CorpseSkins[enemyType].Clone() as Decal;
                        var dead = new Dead();

                        patrol.AddTransition(dying, () => { return !this.IsAlive; });
                        dying.AddTransition(dead,() => { return dying.HitGround; });

                        brain.AddState(patrol);
                        brain.AddState(dying);
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
