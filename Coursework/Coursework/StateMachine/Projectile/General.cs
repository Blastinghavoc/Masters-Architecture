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
    /// Dead state for any projectiles.
    /// Signals to the GameEventManager that the
    /// projectile has died.
    /// </summary>
    class Dead : State
    {
        public override void OnEnter(object owner)
        {
            GameEventManager.Instance.KilledProjectile(owner as P);
        }

        public override void OnExit(object owner)
        {
        }

        public override void Update(object owner, GameTime gameTime)
        {
        }
    }
}
