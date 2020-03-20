using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Graphics;
using Coursework.Powerups;
using Microsoft.Xna.Framework;

namespace Coursework.Entities
{
    //A simple subclass of interactable that represents a powerup item
    class Powerup : Interactable
    {
        public powerUpType powerupType = powerUpType.fireball;

        public Powerup(Drawable appearance, Vector2 position) : base(appearance, position)
        {
        }        
    }
}
