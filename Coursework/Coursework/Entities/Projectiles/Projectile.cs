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
    /// Currently only porjectiles that collide with enemies are implemented.
    /// </summary>
    public abstract class Projectile : CollidableObject
    {
        private bool isEnemyAffiliated = false;
        public bool IsEnemy { get { return isEnemyAffiliated; } }
        public bool IsAlly { get { return !isEnemyAffiliated; } }

        //The damage dealt by the projectile
        public int Damage { get; protected set; }

        //Speed of projectile movement
        public float Speed { get; protected set; }

        //Support for potentially complex projectile behaviours
        protected FSM behaviour;

        public Projectile(Drawable appearance, Vector2 position,float speed,int damage,bool isEnemy = false) : base(appearance, position)
        {
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

        /// <summary>
        /// Overriden by derived classes to set up the behaviour
        /// </summary>
        protected abstract void InitialiseBehaviour();

        public void SetAffiliation(bool enemy)
        {
            isEnemyAffiliated = enemy;
        }

        /// <summary>
        /// Override of Clone to ensure that behaviour is correctly set up.
        /// </summary>
        public virtual new Projectile Clone()
        {
            var tmp = base.Clone() as Projectile;
            tmp.InitialiseBehaviour();
            return tmp;
        }
    }    
}
