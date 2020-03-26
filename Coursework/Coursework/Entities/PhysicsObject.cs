using Coursework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Entities
{
    /// <summary>
    /// A physics object updates its position based on physics equations
    /// and forces acting on it.
    /// </summary>
    public class PhysicsObject:CollidableObject
    {
        protected Vector2 Force { get; set; }//Force currently being applied to the object

        protected float MaxSpeed { get { return maxSpeed; } set { maxSpeed = value; maxSpeedSqr = maxSpeed * maxSpeed; } }
        private float maxSpeed;
        private float maxSpeedSqr;

        protected Vector2 DragFactor { get; set; } = new Vector2(0.9f, 1);//Drag in each direction (multiplicative)
        protected float Gravity { get; set; } = 981;

        //Inherited constructor
        public PhysicsObject():base() { }

        //Inherited constructor
        public PhysicsObject(Drawable appearance, Vector2 position) : base(appearance, position)
        {
        }

        /// <summary>
        /// Implements basic physics based movement, but with no concept of mass,
        /// meaning that it is implicitly 1kg for all objects.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //Update based on physics equations
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 acceleration = Force;
            acceleration.Y += Gravity;//gravity

            Velocity += acceleration * dt;

            //Apply drag
            Velocity *= DragFactor;

            //Enforce speed limit
            var unclampedSpeedSqr = Velocity.LengthSquared();
            if (unclampedSpeedSqr > maxSpeedSqr) {
                Velocity = Vector2.Normalize(Velocity) * maxSpeed;
            }

            //update position
            Position += Velocity * dt;

            //Reset force for next update
            Force = Vector2.Zero;
        }
    }
}
