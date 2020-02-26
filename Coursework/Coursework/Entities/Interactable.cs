using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Entities;
using Coursework.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Coursework.Entities
{
    //A collidable object that can interact with the player
    class Interactable: CollidableObject
    {
        private Drawable appearance;
        public override Vector2 Position { get => base.Position; protected set { base.Position = value; appearance.SetPosition(value); } }

        public Interactable(Drawable appearance,Vector2 position)
        {
            this.appearance = appearance;
            Position = position;
            UpdateBounds(Position, (int)appearance.Size.X, (int)appearance.Size.Y);
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteEffects effect = SpriteEffects.None)
        {
            appearance.Draw(spriteBatch, effect);
        }
    }
}
