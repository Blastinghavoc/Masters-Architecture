using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Coursework.Graphics;

namespace Coursework.Graphics
{
    /// <summary>
    /// Based on lab 1 animation tutorial
    /// </summary>
    public abstract class AbstractAnimation : Drawable
    {
        protected abstract Texture2D Image { get; set; }

        protected float scale;

        protected Rectangle sourceRect = new Rectangle();

        protected Rectangle destinationRect = new Rectangle();

        protected int elapsedTime;
        protected int frameTime;

        protected int frameCount;
        protected int currentFrameIndex;

        public Color color { get; set; }

        public bool Active { get; protected set; }
        public bool Looping { get; protected set; }
        public int FrameWidth { get; protected set; }
        public int FrameHeight { get; protected set; }

        public Vector2 Size => new Vector2(FrameWidth*scale,FrameHeight*scale);

        public float Rotation { get; set; } = 0;

        public float LayerDepth { get; set; } = 0.5f;

        private Vector2 rotationOrigin;
        public Vector2 RotationOrigin { get => rotationOrigin; set { rotationOrigin = value; PositionOffset = RotationOrigin * scale; } }
        public Vector2 PositionOffset { get; private set; }

        protected Vector2 position;

        public AbstractAnimation(Vector2 position, int frameWidth, int frameHeight, int
            frameCount, int frametime, Color color, float scale, bool looping)
        {
            this.color = color;
            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;
            this.frameCount = frameCount;
            this.frameTime = frametime;
            this.scale = scale;
            Looping = looping;
            this.position = position;

            RotationOrigin = Size/2;

            // Set the time to zero
            elapsedTime = 0;
            currentFrameIndex = 0;
            // Set the Animation to active by default
            Active = true;
        }

        public void Pause()
        {
            Active = false;
        }

        public void Play()
        {
            Active = true;
        }

        //Reset to initial frame
        public void Reset()
        {
            elapsedTime = 0;
            currentFrameIndex = 0;
        }

        public void SetPosition(Vector2 pos)
        {
            position = pos;
        }

        public void Update(GameTime gameTime, Vector2 position)
        {
            this.position = position;
            Update(gameTime);
        }

        public virtual void Update(GameTime gameTime)
        {
            // Do not update if we are not active
            if (Active == false) return;

            // Update the elapsed time
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            // If the elapsed time is larger than the frame time
            // we need to switch frames
            if (elapsedTime > frameTime)
            {
                // Move to the next frame
                currentFrameIndex++;
                // If the currentFrame is equal to frameCount reset currentFrame to zero
                if (currentFrameIndex == frameCount)
                {
                    currentFrameIndex = 0;
                    // If we are not looping deactivate the animation
                    if (Looping == false)
                        Active = false;
                }
                // Reset the elapsed time to zero
                elapsedTime = 0;
            }

            var scaledWidth = FrameWidth * scale;
            var scaledHeight = FrameHeight * scale;

            //Account for potentially alterred rotation origin
            var drawPosition = position + PositionOffset;

            destinationRect = new Rectangle((int)(drawPosition.X), (int)(drawPosition.Y), (int)(scaledWidth), (int)(scaledHeight));
        }

        public void Draw(SpriteBatch spriteBatch,SpriteEffects effect = SpriteEffects.None)
        {
            // Only draw the animation when we are active
            if (Active)
            {                
                spriteBatch.Draw(Image, destinationRectangle: destinationRect, sourceRectangle: sourceRect, color: color, effects: effect,origin: RotationOrigin,rotation: Rotation,layerDepth: LayerDepth);
            }
        }

        public virtual Drawable Clone()
        {
            return this.MemberwiseClone() as Drawable;
        }

    }
}
