﻿using Microsoft.Xna.Framework;
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
        public Color color { get; set; }
        private Vector2 position;

        private Vector2 rotationOrigin;
        public Vector2 RotationOrigin { get => rotationOrigin; set {rotationOrigin = value; PositionOffset = RotationOrigin * scale; } }
        public Vector2 PositionOffset { get; private set; }

        public Vector2 Size => new Vector2(texture.Width * scale.X, texture.Height * scale.Y);

        public float Rotation { get; set; } = 0;

        public Sprite(Texture2D texture, Vector2 scale,Color color)
        {
            this.texture = texture;
            this.scale = scale;
            this.color = color;
            RotationOrigin = texture.Bounds.Center.ToVector2();
        }


        public virtual void Draw(SpriteBatch spriteBatch, SpriteEffects effect = SpriteEffects.None)
        {
            var drawPosition = position + PositionOffset;
            spriteBatch.Draw(texture,position: drawPosition, color: color,effects: effect,scale: scale,origin: RotationOrigin, rotation: Rotation);
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
