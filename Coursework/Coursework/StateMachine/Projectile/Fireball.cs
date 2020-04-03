using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using P = Coursework.Projectiles.Projectile;

namespace Coursework.StateMachine.Projectile
{
    /// <summary>
    /// Fireball projectile state. Simply moves forward at a constant speed,
    /// and spins as it goes.
    /// </summary>
    class Fireball : State
    {
        //Lifetime in seconds
        public float LifeTime { get; private set; } = 20;

        public Fireball()
        {
            Name = "Fireball";
        }

        public override void OnEnter(object owner)
        {
            
        }

        public override void OnExit(object owner)
        {

        }

        public override void Update(object owner, GameTime gameTime)
        {
            P projectile = owner as P;
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            LifeTime -= dt;

            projectile.Appearance.Rotation += 10*dt;//Spin

            projectile.SetPosition(projectile.Position + projectile.Velocity * dt);            
        }
    }
}
