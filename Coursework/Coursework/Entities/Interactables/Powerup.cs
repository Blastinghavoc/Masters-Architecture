using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Entities.Interactables;
using Coursework.Graphics;
using Coursework.Levels;
using Coursework.Powerups;
using Microsoft.Xna.Framework;

namespace Coursework.Entities.Interactables
{
    //A simple subclass of interactable that represents a powerup item
    class Powerup : Interactable
    {
        public PowerupType powerupType = PowerupType.fireball;

        public Powerup(Drawable appearance, Vector2 position) : base(appearance, position)
        {
        }

        public override void InteractOnEnter(Level currentLevel, PlayerCollisionEventArgs p)
        {
            p.player.AddPowerupEffect(powerupType);//Add the correct powerup effect to the player
            currentLevel.ScheduleForDeletion(this);//Remove powerup after it's been collected.
        }
    }
}
