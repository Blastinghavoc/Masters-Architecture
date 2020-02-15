#region File Description
//-----------------------------------------------------------------------------
// Ship.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;
#endregion

namespace ChaseCameraSample
{
    public class Ship : Collidable
    {
        #region Fields

        private const float MinimumAltitude = 350.0f;

        /// <summary>
        /// A reference to the graphics device used to access the viewport for touch input.
        /// </summary>
        private GraphicsDevice graphicsDevice;

        /// <summary>
        /// Location of ship in world space.
        /// </summary>
        private Vector3 position;
        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                boundingSphere.Center = position;
            }
        }

        /// <summary>
        /// Direction ship is facing.
        /// </summary>
        public Vector3 Direction;

        /// <summary>
        /// Ship's up vector.
        /// </summary>
        public Vector3 Up;

        private Vector3 right;
        /// <summary>
        /// Ship's right vector.
        /// </summary>
        public Vector3 Right
        {
            get { return right; }
        }

        /// <summary>
        /// Full speed at which ship can rotate; measured in radians per second.
        /// </summary>
        private const float RotationRate = 1.5f;

        /// <summary>
        /// Amount of rotation to apply to the ship from input
        /// </summary>
        private Vector2 RotationAmount = new Vector2();

        /// <summary>
        /// Mass of ship.
        /// </summary>
        private const float Mass = 1.0f;

        /// <summary>
        /// Maximum force that can be applied along the ship's direction.
        /// </summary>
        private const float ThrustForce = 24000.0f;

        /// <summary>
        /// Amount of thrust to apply from input
        /// </summary>
        private float ThrustAmount = 0.0f;

        /// <summary>
        /// Velocity scalar to approximate drag.
        /// </summary>
        private const float DragFactor = 0.97f;

        /// <summary>
        /// Current ship velocity.
        /// </summary>
        public Vector3 Velocity;

        /// <summary>
        /// Ship world transform matrix.
        /// </summary>
        public Matrix World
        {
            get { return world; }
        }
        private Matrix world;

        #endregion

        #region Initialization

        public Ship(GraphicsDevice device)
        {
            graphicsDevice = device;
            Reset();
        }

        /// <summary>
        /// Restore the ship to its original starting state
        /// </summary>
        public void Reset()
        {
            Position = new Vector3(0, MinimumAltitude, 0);
            Direction = Vector3.Forward;
            Up = Vector3.Up;
            right = Vector3.Right;
            Velocity = Vector3.Zero;

            boundingSphere = new BoundingSphere(Position, 1000.0f);
        }

        #endregion

        /// <summary>
        /// Applies a simple rotation to the ship and animates position based
        /// on simple linear motion physics.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Scale rotation amount to radians per second
            RotationAmount = RotationAmount * RotationRate * elapsed;

            // Correct the X axis steering when the ship is upside down
            if (Up.Y < 0)
                RotationAmount.X = -RotationAmount.X;


            // Create rotation matrix from rotation amount
            Matrix rotationMatrix =
                Matrix.CreateFromAxisAngle(Right, RotationAmount.Y) *
                Matrix.CreateRotationY(RotationAmount.X);

            // Rotate orientation vectors
            Direction = Vector3.TransformNormal(Direction, rotationMatrix);
            Up = Vector3.TransformNormal(Up, rotationMatrix);

            // Re-normalize orientation vectors
            // Without this, the matrix transformations may introduce small rounding
            // errors which add up over time and could destabilize the ship.
            Direction.Normalize();
            Up.Normalize();

            // Re-calculate Right
            right = Vector3.Cross(Direction, Up);

            // The same instability may cause the 3 orientation vectors may
            // also diverge. Either the Up or Direction vector needs to be
            // re-computed with a cross product to ensure orthagonality
            Up = Vector3.Cross(Right, Direction);

            // Calculate force from thrust amount
            Vector3 force = Direction * ThrustAmount * ThrustForce;


            // Apply acceleration
            Vector3 acceleration = force / Mass;
            Velocity += acceleration * elapsed;

            // Apply psuedo drag
            Velocity *= DragFactor;

            // Apply velocity
            Position += Velocity * elapsed;


            // Prevent ship from flying under the ground
            position.Y = Math.Max(Position.Y, MinimumAltitude);

            // Update the bounding sphere
            boundingSphere.Center = Position;

            // Reconstruct the ship's world matrix
            world = Matrix.Identity;
            world.Forward = Direction;
            world.Up = Up;
            world.Right = right;
            world.Translation = Position;
        }

        public void AddPosition(Vector3 p)
        {
            position += p;
            boundingSphere.Center = position;
        }

        public void Thrust(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
            {
                ThrustAmount = 1.0f;
            }
            else
            {
                ThrustAmount = 0.0f;
            }
        }

        public void TurnLeft(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
            {
                RotationAmount.X = 1.0f;
            }
        }

        public void TurnRight(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
            {
                RotationAmount.X = -1.0f;
            }
        }

        public void MouseTurn(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
            {
                int width = graphicsDevice.Viewport.Width;
                //We could use this code to find a value from -1.0 to 1.0
                //RotationAmount.X = -(float)(amount.X-width/2)/(float)width;
                //Or we can use this to check if it's left or right
                if (amount.X - width/2 > 0)
                    RotationAmount.X = -1.0f;
                else
                    RotationAmount.X = 1.0f;
            }
        }

        public override bool CollisionTest(Collidable obj) {
            if (obj != null) {
                return BoundingSphere.Intersects(obj.BoundingSphere);
            }
            return false;
        }

        public override void OnCollision(Collidable obj)
        {
            Ship otherShip = obj as Ship;
            if (otherShip != null)
            {
                // The collision normal is the direction in which the collision occurred                  
                // We want the ships to react in this direction                  
                Vector3 collisionNormal = Vector3.Normalize(otherShip.BoundingSphere.Center - BoundingSphere.Center);
                
                // The distance between the two ships can be calculated using the centers of the two bounding spheres                  
                float distance = Vector3.Distance(otherShip.BoundingSphere.Center, BoundingSphere.Center);
                
                // The penetration depth determines how much the two spheres have intersected                  
                float penetrationDepth = (otherShip.BoundingSphere.Radius + BoundingSphere.Radius) - distance;
                
                // Negate the collision normal as we want to act in the opposite direction                  
                // of the collision and multiply by how much the spheres have intersected                  
                AddPosition(-collisionNormal * penetrationDepth); 
            }
        }


    }

    public class EnemyShip : Ship
    {
        public Color CollisionColor = Color.White;

        public EnemyShip(GraphicsDevice device)
            : base(device)
        {
        }

        public override void OnCollision(Collidable obj)
        {
            Ship playerShip = obj as Ship;
            if (playerShip != null)
            {
                Vector3 collisionNormal = Vector3.Normalize(playerShip.BoundingSphere.Center - BoundingSphere.Center);
                AddPosition(-collisionNormal * 50.0f);
            }
        }
    }
}
