using Coursework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Entities
{

    public class CollidableObject: GameObject
    {
        //Override property so that setting the position automatically updates the bounds
        public override Vector2 Position { get => base.Position; protected set { base.Position = value; UpdateBounds(); } }

        public Rectangle BoundingBox { get; protected set; }

        //Inherited constructor
        public CollidableObject():base() { }
        //Inherited constructor
        public CollidableObject(Drawable appearance, Vector2 position):base(appearance,position)
        {
            //By default, collision bounds are calculated from appearance size
            if (Appearance!= null)
            {
                UpdateBounds(Position, (int)Appearance.Size.X,(int)Appearance.Size.Y);
            }
        }

        //Move the bounding box to the current position of the object
        protected void UpdateBounds()
        {
            UpdateBounds(Position,BoundingBox.Width, BoundingBox.Height);
        }

        //Update the position and size of the bounding box
        protected void UpdateBounds(Vector2 pos, int width,int height)
        {
            Point topLeftCorner = new Point((int)pos.X, (int)pos.Y);
            BoundingBox = new Rectangle(topLeftCorner, new Point(width,height));
        }

        /// <summary>
        /// Determine whether this object is colliding with some other bounding box.
        /// Sets the penetration depth parameter if a collision is detected
        /// </summary>
        /// <param name="box"></param>
        /// <param name="penetrationDepth"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Collsion response to hitting a static object.
        /// Moves this object such that they are no longer colliding.
        /// Based on lab 2 collision code
        /// </summary>
        /// <param name="penetrationDepth"></param>
        protected virtual void StaticCollisionResponse(Vector2 penetrationDepth)
        {
            var xMag = Math.Abs(penetrationDepth.X);
            var yMag = Math.Abs(penetrationDepth.Y);
            Vector2 adjustment;

            //Tiny extra correction to apply.
            const float epsilon = 0.0001f;

            //Adjust in direction of minimum overlap
            if (yMag > xMag)
            {
                bool left = penetrationDepth.X > 0;//Colliding as if moving left into the object

                //Sub-integer correction for very small penetration depth
                var xAdjustment = penetrationDepth.X;
                if (xMag ==1)
                {
                    var floatRemainder = Position.X % 1;
                    if (left)
                    {
                        xAdjustment = 1- floatRemainder;
                        xAdjustment += epsilon;
                    }
                    else
                    {
                        xAdjustment = (floatRemainder > 0) ? -floatRemainder : -1;
                        xAdjustment -= epsilon;
                    }                    
                }

                adjustment = new Vector2(xAdjustment, 0);

                float arrestedVel = Velocity.X;
                if (left)
                {
                    arrestedVel = Math.Max(arrestedVel, 0);
                }
                else
                {
                    arrestedVel = Math.Min(arrestedVel, 0);
                }

                Velocity = new Vector2(arrestedVel, Velocity.Y);//Arrest velocity in colliding direction
            }
            else
            {
                bool up = penetrationDepth.Y > 0;//Colliding as if moving up into the object

                //Sub-integer correction for very small penetration depth
                var yAdjustment = penetrationDepth.Y;
                if (yMag == 1)
                {
                    var floatRemainder = Position.Y % 1;
                    if (up)
                    {
                        yAdjustment = 1 - floatRemainder;
                        yAdjustment += epsilon;
                    }
                    else
                    {                        
                        yAdjustment = (floatRemainder > 0) ? -floatRemainder : -1;
                        yAdjustment -= epsilon;
                    }
                }

                adjustment = new Vector2(0, yAdjustment);

                float arrestedVel = Velocity.Y;
                if (up)
                {
                    arrestedVel = Math.Max(arrestedVel, 0);
                }
                else
                {
                    arrestedVel = Math.Min(arrestedVel, 0);
                }

                Velocity = new Vector2(Velocity.X,arrestedVel);//Arrest velocity in colliding direction
            }

            this.Position += adjustment;
        }
    }

}
