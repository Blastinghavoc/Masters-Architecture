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
    class Coin : Interactable
    {
        public Coin(Drawable appearance, Vector2 position) : base(appearance, position)
        {
        }

        public override void InteractOnEnter(Level currentLevel, PlayerCollisionEventArgs p)
        {
            GameEventManager.Instance.AddScore(1);//Coins are worth 1 point
            currentLevel.ScheduleForDeletion(this);//The coin should be removed once it's collected
        }
    }
}
