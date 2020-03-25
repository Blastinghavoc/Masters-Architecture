using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Coursework.Graphics
{
    //A simple tiled background
    public class Background : Sprite
    {
        public Background(Texture2D texture, Vector2 scale, Color color) : base(texture, scale, color)
        {
            RotationOrigin = Vector2.Zero;
        }

        public void SetTexture(Texture2D texture) {
            this.texture = texture;
        }

        //Texture wrapping and scrolling based on https://www.david-gouveia.com/scrolling-textures-in-xna
        public void Draw(GraphicsDevice device, SpriteEffects effect = SpriteEffects.None)
        {
            //New spritebatch to render in tiled mode
            var myBatch = new SpriteBatch(device);
            myBatch.Begin(sortMode:SpriteSortMode.BackToFront,samplerState:SamplerState.LinearWrap);

            var screenRect = new Rectangle((int)position.X, (int)position.Y, device.Viewport.Width, device.Viewport.Height);    
            
            myBatch.Draw(texture, Vector2.Zero, screenRect, color,Rotation,rotationOrigin,scale,effect,1);

            myBatch.End();
        }
    }
}
