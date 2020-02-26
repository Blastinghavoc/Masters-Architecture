using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Coursework.Animation
{
    //Sprite with a built-in effect. Used for enemy corpses to retain their orientation
    class Decal : Sprite
    {
        private SpriteEffects effect;

        public Decal(Texture2D texture, Vector2 scale, Color color,SpriteEffects effects = SpriteEffects.None) : base(texture, scale, color)
        {
            this.effect = effects;
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteEffects effect = SpriteEffects.None)
        {
            effect = effect | this.effect;
            base.Draw(spriteBatch, effect);
        }
    }
}
