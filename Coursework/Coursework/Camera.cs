﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework
{
    /// <summary>
    /// Camera class based on example found here: http://community.monogame.net/t/simple-2d-camera/9135   
    /// </summary>
    public class Camera
    {
        //Not singleton (there could in principle be multiple cameras), but this is the main one.
        public static Camera mainCamera;
        //Currently, there is no need for more than one camera anyway.

        public float Zoom { get; set; }
        public Vector2 Position { get; set; }
        public Rectangle Bounds { get; protected set; }
        public Rectangle VisibleArea { get; protected set; }
        public Matrix Transform { get; protected set; }

        public Camera(Viewport viewport)
        {
            Bounds = viewport.Bounds;
            Zoom = 1f;
            Position = Vector2.Zero;
        }


        private void UpdateVisibleArea()
        {
            var inverseViewMatrix = Matrix.Invert(Transform);

            var tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
            var tr = Vector2.Transform(new Vector2(Bounds.X, 0), inverseViewMatrix);
            var bl = Vector2.Transform(new Vector2(0, Bounds.Y), inverseViewMatrix);
            var br = Vector2.Transform(new Vector2(Bounds.Width, Bounds.Height), inverseViewMatrix);

            var min = new Vector2(
                MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
                MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
            var max = new Vector2(
                MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
                MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));
            VisibleArea = new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        private void UpdateMatrix()
        {
            Transform = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                    Matrix.CreateScale(Zoom) *
                    Matrix.CreateTranslation(new Vector3(Bounds.Width * 0.5f, Bounds.Height * 0.5f, 0));
            UpdateVisibleArea();
        }

        public void AdjustZoom(float zoomAmount)
        {
            Zoom += zoomAmount;
            if (Zoom < .35f)
            {
                Zoom = .35f;
            }
            if (Zoom > 2f)
            {
                Zoom = 2f;
            }
        }

        public void Update(Viewport bounds)
        {
            Bounds = bounds.Bounds;
            UpdateMatrix();          
        }

        public Point ScreenToWorldPoint(Point screenPoint) {
            return screenPoint + new Point(VisibleArea.Left, VisibleArea.Top);
        }

        /// <summary>
        /// Clamps the camera's visible area to be within the given bounds,
        /// or at least be as close as possible.
        /// Used to clamp the camera view to the level bounds.
        /// </summary>
        /// <param name="bounds"></param>
        public void ConstrainToArea(Rectangle bounds) {
            var halfWidth = VisibleArea.Width / 2f;
            var halfHeight = VisibleArea.Height / 2f;

            var leftBound = bounds.Left + halfWidth;
            var rightBound = bounds.Right - halfWidth;

            var adjustedX = MathHelper.Clamp(Position.X, leftBound, rightBound);

            var topBound = bounds.Top + halfHeight;
            var bottomBound = bounds.Bottom - halfHeight;

            /*
             If the camera view can't fit properly in the bounds, prefer to "spill" the view
             upwards rather than downwards. This prevents ever seeing beneath a level, but you
             may see above it.
             */
            if (bottomBound < topBound)
            {
                topBound = bottomBound;
            }

            var adjustedY = MathHelper.Clamp(Position.Y, topBound, bottomBound);

            Position = new Vector2(adjustedX, adjustedY);
        }
    }
}
