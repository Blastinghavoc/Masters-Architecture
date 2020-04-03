using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Entities;
using Microsoft.Xna.Framework;

namespace Coursework.Powerups
{
    /// <summary>
    /// Effect that grants the ability to launch fireballs
    /// </summary>
    class FireballEffect : PowerupEffect
    {
        protected float cooldownSeconds = 0.5f;
        private float cooldownTimer = 0;

        public FireballEffect():base()
        {
            DurationSeconds = 10;
            EffectColor = Color.Orange;
        }

        public override void OnAquiredBy(Player p)
        {
            base.OnAquiredBy(p);
            cooldownTimer = 0;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Active && cooldownTimer > 0)//Update cooldown
            {
                cooldownTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public override void BindEvents()
        {
            base.BindEvents();
            GameEventManager.Instance.OnPlayerAttemptToUseWeapon += LaunchProjectile;
        }

        public override void UnbindEvents()
        {
            base.UnbindEvents();
            GameEventManager.Instance.OnPlayerAttemptToUseWeapon -= LaunchProjectile;
        }

        //Launch a projectile, with a cooldown
        public void LaunchProjectile(object sender, PlayerUseWeaponEventArgs e)
        {
            /*Don't do anything if the player that tried to use the weapon isn't the one who has this effect
            Note that this is pointless right now, as there's only ever one player, 
            but it's extensible in case multiplayer was implemented.
            */
            if (e.player != this.player)
            {
                return;
            }

            var launchPosition = player.Position;

            if (Active && cooldownTimer <= 0)
            {
                GameEventManager.Instance.LaunchProjectile(Projectiles.ProjectileType.fireball, launchPosition, e.targetWorldPosition, false);
                cooldownTimer = cooldownSeconds;
            }
        }
    }
}
