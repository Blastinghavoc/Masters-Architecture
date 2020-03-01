using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Animation
{
    /// <summary>
    /// represents a static (non animated) sprite
    /// </summary>
    class Sprite: Drawable
    {
        private Texture2D texture;
        private Vector2 scale;
        private Color color;
        private Vector2 position;

        public Vector2 Size => new Vector2(texture.Width * scale.X, texture.Height * scale.Y);

        public Sprite(Texture2D texture, Vector2 scale,Color color)
        {
            this.texture = texture;
            this.scale = scale;
            this.color = color;
        }


        public virtual void Draw(SpriteBatch spriteBatch, SpriteEffects effect = SpriteEffects.None)
        {
            spriteBatch.Draw(texture,position: position,color: color,effects: effect,scale: scale);
        }


        public virtual void Update(GameTime gameTime, Vector2 position)
        {
            SetPosition(position);
        }

        public void SetPosition(Vector2 pos)
        {
            this.position = pos;
        }

        public void Update(GameTime gameTime)
        {
            //Nothing to update
        }

        public virtual Drawable Clone()
        {
            return this.MemberwiseClone() as Drawable;
        }
    }
}
