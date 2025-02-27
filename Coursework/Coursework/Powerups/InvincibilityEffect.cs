﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Entities;
using Microsoft.Xna.Framework;

namespace Coursework.Powerups
{
    /// <summary>
    /// Simple powerup effect granting the player temporary invincibility
    /// </summary>
    class InvincibilityEffect:PowerupEffect
    {
        public InvincibilityEffect():base()
        {
            DurationSeconds = 5;
            EffectColor = Color.Cyan;
        }

        public override void OnAquiredBy(Player p)
        {
            base.OnAquiredBy(p);
            p.IsInvincible = true;
        }

        public override void OnExpired()
        {
            base.OnExpired();
            player.IsInvincible = false;
        }
    }
}
