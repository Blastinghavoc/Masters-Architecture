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
        public Drawable Appearance { get; protected set; }
        public override Vector2 Position { get => base.Position; protected set { base.Position = value; Appearance.SetPosition(value); } }

        public Interactable(Drawable appearance,Vector2 position)
        {
            this.Appearance = appearance;
            Position = position;
            UpdateBounds(Position, (int)appearance.Size.X, (int)appearance.Size.Y);
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteEffects effect = SpriteEffects.None)
        {
            Appearance.Draw(spriteBatch, effect);
        }
    }
}
