using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Graphics
{
    /// <summary>
    /// Sprite-sheet based animation, based on Lab1.
    /// Currently unused, as the format of assets used did not lend itself
    /// to easy spritesheet animation.
    /// </summary>
    class SpriteSheetAnimation: AbstractAnimation
    {
        protected override Texture2D Image { get => spriteSheet; set => spriteSheet = value; }

        Texture2D spriteSheet;

        public SpriteSheetAnimation(Texture2D spriteSheet,Vector2 position, int frameWidth, int frameHeight, int
            frameCount, int frametime, Color color, float scale, bool looping): 
            base(position, frameWidth, frameHeight, frameCount, frametime, color, scale, looping)
        {       
            this.spriteSheet = spriteSheet;
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Compute correct frame in source image
            var texWidth = spriteSheet.Width;
            var texWidthInFrames = texWidth / FrameWidth;
            var xPos = (currentFrameIndex % texWidthInFrames) * FrameWidth;
            var yPos = (currentFrameIndex / texWidthInFrames) * FrameHeight;

            sourceRect = new Rectangle(xPos, yPos, FrameWidth, FrameHeight);            
        }
    }
}
