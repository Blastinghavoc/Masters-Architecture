using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Coursework.Entities
{
    /// <summary>
    /// Represents a 2D game object
    /// </summary>
    public abstract class GameObject
    {
        public Drawable Appearance { get; protected set; }

        //Changes to position automatically update the appearance position too
        public virtual Vector2 Position { get { return position; } protected set { position = value; Appearance?.SetPosition(value); } }
        private Vector2 position;

        public Vector2 Velocity { get; protected set; }

        protected GameObject()
        {
            Appearance = null;
            Position = Vector2.Zero;
        }

        public GameObject(Drawable appearance, Vector2 position)
        {
            Appearance = appearance;
            Position = position;
        }

        public virtual void Update(GameTime gameTime) {
            Appearance?.Update(gameTime);
        }

        public virtual void Draw(SpriteBatch spriteBatch, SpriteEffects effect = SpriteEffects.None)
        {
            Appearance?.Draw(spriteBatch, effect);
        }

        public virtual GameObject Clone()
        {
            var tmp = this.MemberwiseClone() as GameObject;
            tmp.Appearance = Appearance.Clone();
            return tmp;
        }

        //Force users to be explicit about setting the position.
        public void SetPosition(Vector2 pos)
        {
            Position = pos;
        }

    }
}
