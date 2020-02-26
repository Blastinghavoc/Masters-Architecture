using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Coursework.Entities
{
    /// <summary>
    /// Represents a 2D game object
    /// </summary>
    class GameObject//TODO consider making abstract?
    {
        public virtual Vector2 Position { get; protected set; }
        public Vector2 Velocity { get; protected set; }

        public virtual void Update(GameTime gameTime) {

        }

        public virtual void Draw(SpriteBatch spriteBatch, SpriteEffects effect = SpriteEffects.None)
        { 

        }

    }
}
