using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Coursework
{
    /// <summary>
    /// Represents a 2D game object
    /// </summary>
    class GameObject//TODO consider making abstract?
    {
        public Vector2 Position { get; protected set; }
        public Vector2 Veclocity { get; protected set; }

        public virtual void Update(GameTime gameTime) {

        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch) {

        }

    }
}
