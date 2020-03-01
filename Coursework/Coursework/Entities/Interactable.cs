﻿using System;
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
    //A collidable object that can interact with the player and has a visual representation
    class Interactable: CollidableObject
    {
        public Drawable Appearance { get; protected set; }
        public override Vector2 Position { get => base.Position; protected set { base.Position = value; Appearance.SetPosition(value); } }

        public InteractableType interactableType = InteractableType.coin;

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

        public void SetPosition(Vector2 pos)
        {
            Position = pos;
        }

        public virtual Interactable Clone()
        {
            var tmp = this.MemberwiseClone() as Interactable;
            tmp.Appearance = Appearance.Clone();
            return tmp;
        }
    }

    public enum InteractableType
    {
       coin,
       enemy,
       nextLevel
    }
}
