using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Entities
{

    class CollidableObject: GameObject
    {
        public Rectangle BoundingBox { get; protected set; }

        public void UpdateBounds()
        {
            //TODO update bounding box as entity moves
        }

        public bool CheckCollision(Rectangle box, out Vector2 penetrationDepth)
        {
            var penDepth = RectangleExtensions.GetIntersectionDepth(BoundingBox, box);
            penetrationDepth = penDepth;
            if (penDepth != Vector2.Zero)
            {
                return true;
            }
            return false;
        }

        public bool CheckCollision(CollidableObject obj,out Vector2 penetrationDepth)
        {
            return CheckCollision(obj.BoundingBox, out penetrationDepth);
        }

        //Overriden by derived classes to implement their own collision response to hiting another object
        public virtual void OnCollision(CollidableObject obj, Vector2 penetrationDepth)
        {

        }

        //Collision response to hitting a static object
        public void OnStaticCollision(Vector2 penetrationDepth)
        {
            var xMag = Math.Abs(penetrationDepth.X);
            var yMag = Math.Abs(penetrationDepth.Y);
            Vector2 adjustment;

            //Adjust in direction of minimum overlap
            if (xMag > yMag)
            {
                adjustment = new Vector2(0, penetrationDepth.Y);
            }
            else
            {
                adjustment = new Vector2(penetrationDepth.X, 0);
            }

            this.Position += adjustment;
        }
    }

}
