using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Graphics;
using Microsoft.Xna.Framework;

namespace Coursework.Entities.Interactables
{
    class LevelTransition : Interactable
    {
        public LevelTransition(Drawable appearance, Vector2 position) : base(appearance, position)
        {
        }

        public override void InteractOnEnter(Level currentLevel, PlayerCollisionEventArgs p)
        {
            //Signal to go to the next level.
            GameEventManager.Instance.NextLevel();
        }
    }
}
