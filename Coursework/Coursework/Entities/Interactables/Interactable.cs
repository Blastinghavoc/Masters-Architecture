using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Entities;
using Coursework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Coursework.Entities.Interactables
{
    /// <summary>
    /// A collidable object in the level (other than a tile)
    /// that has some interaction with the player on collision.
    /// </summary>
    public abstract class Interactable: CollidableObject
    {

        /// <summary>
        /// All interactables by defininition must have some interaction with
        /// the player on collision enter. They may also have others, but
        /// are responsible for defining those themselves.
        /// The current Level will call this method for any interactable. 
        /// This greatly reduces event subscription/unsubscription overhead
        /// by preventing each object having to manage its own subscriptions
        /// </summary>
        public abstract void InteractOnEnter(Level sender, PlayerCollisionEventArgs e);

        #region Inherited methods

        public Interactable(Drawable appearance, Vector2 position) : base(appearance, position)
        {
        }

        public virtual new Interactable Clone()
        {
            return base.Clone() as Interactable;
        }
        #endregion

    }
}
