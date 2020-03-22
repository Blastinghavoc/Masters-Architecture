﻿using Microsoft.Xna.Framework;
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
                //Sub-integer correction for very small penetration depth
                var xAdjustment = penetrationDepth.X;
                if (xMag ==1)
                {
                    var floatRemainder = Position.X % 1;
                    if (xAdjustment > 0)
                    {
                        xAdjustment = 1- floatRemainder;
                        xAdjustment += epsilon;
                    }
                    else
                    {
                        xAdjustment = (floatRemainder > 0) ? -floatRemainder : -1;
                        xAdjustment -= epsilon;
                    }                    
                    //xAdjustment *= Math.Sign(penetrationDepth.X);
                }

                adjustment = new Vector2(xAdjustment, 0);
                Velocity = new Vector2(0, Velocity.Y);//Arrest velocity in colliding direction
            }
            else
            {
                //Sub-integer correction for very small penetration depth
                var yAdjustment = penetrationDepth.Y;
                if (yMag == 1)
                {
                    var floatRemainder = Position.Y % 1;
                    if (yAdjustment > 0)
                    {
                        yAdjustment = 1 - floatRemainder;
                        yAdjustment += epsilon;
                    }
                    else
                    {                        
                        yAdjustment = (floatRemainder > 0) ? -floatRemainder : -1;
                        yAdjustment -= epsilon;
                    }
                    //yAdjustment *= Math.Sign(penetrationDepth.Y);
                }

                adjustment = new Vector2(0, yAdjustment);
                Velocity = new Vector2(Velocity.X,0);//Arrest velocity in colliding direction
            }

            this.Position += adjustment;
        }
    }

}
