using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Coursework.Animation
{
    /// <summary>
    /// Based on lab 1 animation tutorial
    /// </summary>
    abstract class AbstractAnimation
    {
        protected abstract Texture2D Image { get; set; }

        protected float scale;

        protected Rectangle sourceRect = new Rectangle();

        protected Rectangle destinationRect = new Rectangle();

        protected int elapsedTime;
        protected int frameTime;

        protected int frameCount;
        protected int currentFrameIndex;

        protected Color color;

        public bool Active { get; protected set; }
        public bool Looping { get; protected set; }
        public int FrameWidth { get; protected set; }
        public int FrameHeight { get; protected set; }
        public Vector2 Position { get; set; }

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
            Position = position;
            

            // Set the time to zero
            elapsedTime = 0;
            currentFrameIndex = 0;
            // Set the Animation to active by default
            Active = true;
        }        public void Pause()
        {
            Active = false;
        }        public void Play()
        {
            Active = true;
        }        //Reset to initial frame        public void Reset()
        {
            elapsedTime = 0;
            currentFrameIndex = 0;
        }

        public virtual void Update(GameTime gameTime)
        {
            // Do not update the game if we are not active
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

            var scaledWidth = FrameWidth * scale;            var scaledHeight = FrameHeight * scale;            destinationRect = new Rectangle((int)(Position.X), (int)(Position.Y), (int)(scaledWidth), (int)(scaledHeight));
        }

        public void Draw(SpriteBatch spriteBatch,SpriteEffects effect = SpriteEffects.None)
        {
            // Only draw the animation when we are active
            if (Active)
            {
                spriteBatch.Draw(Image,destinationRectangle: destinationRect,sourceRectangle: sourceRect,color: color,effects: effect);
            }
        }
    }
}
