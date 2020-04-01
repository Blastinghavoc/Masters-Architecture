using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Graphics;
using Coursework.Levels;
using Microsoft.Xna.Framework;

namespace Coursework.Entities.Interactables
{
    /// <summary>
    /// Interactable that grants an extra life
    /// </summary>
    class ExtraLife : Interactable
    {
        public ExtraLife(Drawable appearance, Vector2 position) : base(appearance, position)
        {
        }

        public override void InteractOnEnter(Level sender, PlayerCollisionEventArgs e)
        {
            sender.ScheduleForDeletion(this);//The extra life gets picked up
            e.player.Heal(1);//Player recovers one life
        }
    }
}
