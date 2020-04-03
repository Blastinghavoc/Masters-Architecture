using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Coursework.Graphics
{
    /// <summary>
    /// Adaptation of AbstractAnimation to use an array of individual textures as
    /// the animation frames.
    /// </summary>
    public class MultiImageAnimation : AbstractAnimation
    {
        protected override Texture2D Image { get => frames[currentFrameIndex]; set => frames[currentFrameIndex] = value; }

        Texture2D[] frames;

        public MultiImageAnimation(Texture2D[] frames, Vector2 position, int frameWidth, int frameHeight, int
            frameCount, int frametime, Color color, float scale, bool looping) :
            base(position, frameWidth, frameHeight, frameCount, frametime, color, scale, looping)
        {
            this.frames = frames;
            sourceRect = new Rectangle(0, 0, FrameWidth, FrameHeight);
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
