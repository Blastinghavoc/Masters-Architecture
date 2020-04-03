using Coursework.Graphics;
using Coursework.Projectiles;
using Coursework.StateMachine;
using Coursework.StateMachine.Projectile;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Entities.Projectiles
{
    /// <summary>
    /// Implementation of a fireball projectile
    /// </summary>
    class FireballProjectile : Projectile
    {
        public FireballProjectile(Drawable appearance, Vector2 position, float speed, int damage, bool isEnemy = false) : base(appearance, position, speed, damage, isEnemy)
        {
            var spriteSize = appearance.Size.ToPoint();
            //Collision bounds of fireball are about half the sprite size, due to empty space in sprite.
            UpdateBounds(Position, spriteSize.X / 2, spriteSize.Y / 2);
        }

        protected override void InitialiseBehaviour()
        {
            behaviour = new FSM(this);
            
            Fireball fireballState = new Fireball();//Simple movement behaviour
            Dead dead = new Dead();

            fireballState.AddTransition(dead, () => { return fireballState.LifeTime <= 0; });

            behaviour.AddState(fireballState);
            behaviour.AddState(dead);

            behaviour.Initialise("Fireball");           
        }
    }
}
