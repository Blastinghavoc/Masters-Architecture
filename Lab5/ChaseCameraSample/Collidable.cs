using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace ChaseCameraSample
{
    public class Collidable
    {
        #region Fields
        protected BoundingSphere boundingSphere = new BoundingSphere();
        public BoundingSphere BoundingSphere
        {
            get { return boundingSphere; }
        }
        #endregion

        #region Member Functions
        public virtual bool CollisionTest(Collidable obj)
        {
            return false;
        }

        public virtual void OnCollision(Collidable obj)
        {
        }
        #endregion


    }
}
