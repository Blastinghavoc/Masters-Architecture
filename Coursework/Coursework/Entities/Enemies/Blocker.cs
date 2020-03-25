using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Graphics;
using Microsoft.Xna.Framework;

namespace Coursework.Entities.Enemies
{
    class Blocker : Enemy
    {
        public Blocker(Drawable appearance, Decal corpseAppearance, Vector2 position, int health, int damage, bool invincible = false, bool solid = false) : 
            base(appearance, corpseAppearance, position, health, damage, invincible, solid)
        {
        }
    }
}
