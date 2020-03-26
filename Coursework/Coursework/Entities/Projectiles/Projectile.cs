using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Graphics;
using Coursework.Entities;
using Coursework.StateMachine;
using Microsoft.Xna.Framework;
using Coursework.StateMachine.Projectile;

namespace Coursework.Projectiles
{
    /// <summary>
    /// Represents a collidable projectile object.
    /// May collide with players or enemies, depending on affiliation.
    /// </summary>
    public class Projectile : CollidableObject
    {
        private bool isEnemyAffiliated = false;
        public bool IsEnemy { get { return isEnemyAffiliated; } }
        public bool IsAlly { get { return !isEnemyAffiliated; } }

        //The damage dealt by the projectile
        public int Damage { get; protected set; }

        public readonly ProjectileType projectileType;

        public float Speed { get; protected set; }

        //Support for potentially complex projectile behaviours
        private FSM behaviour;

        public Projectile(Drawable appearance, Vector2 position, ProjectileType projectileType,float speed,int damage,bool isEnemy = false) : base(appearance, position)
        {
            this.projectileType = projectileType;
            isEnemyAffiliated = isEnemy;
            this.Speed = speed;
            this.Damage = damage;
            InitialiseBehaviour();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            behaviour.Update(gameTime);
        }

        /// <summary>
        /// Speed is fixed, direction can be changed.
        /// This changes the velocity of the projectile
        /// to have the relevant speed in the given direction
        /// </summary>
        /// <param name="direction">Expected to be normalize</param>
        public void SetDirection(Vector2 direction) {
            Velocity = direction * Speed;
        }

        private void InitialiseBehaviour()
        {
            behaviour = new FSM(this);
            switch (projectileType)
            {
                case ProjectileType.fireball:
                    {
                        //Fireball has a simple single state behaviour
                        Fireball fireballState = new Fireball();
                        Dead dead = new Dead();

                        fireballState.AddTransition(dead, () => { return fireballState.LifeTime <= 0; });

                        behaviour.AddState(fireballState);
                        behaviour.AddState(dead);

                        behaviour.Initialise("Fireball");
                    }
                    break;
                default:
                    break;
            }
        }

        public void SetAffiliation(bool enemy)
        {
            isEnemyAffiliated = enemy;
        }

        public virtual new Projectile Clone()
        {
            var tmp = base.Clone() as Projectile;
            tmp.InitialiseBehaviour();
            return tmp;
        }
    }

    public enum ProjectileType
    {
        fireball
    }
}
