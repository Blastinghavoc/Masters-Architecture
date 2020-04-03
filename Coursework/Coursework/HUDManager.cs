using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Entities;

namespace Coursework
{
    /// <summary>
    /// Class managing the HUD.
    /// </summary>
    class HUDManager
    {
        private SpriteFont font;
        private const float fontScale = 0.5f;//Base scaling applied to font

        private List<HUDElement> elements = new List<HUDElement>();

        private Vector2 offset;//Offset required to keep text aligned with camera as the camera moves
        private Vector2 viewDimensions;//Current viewport dimensions
        private Matrix transformMatrix;//Current camera matrix

        public HUDManager(SpriteFont font) {
            this.font = font;
        }

        /// <summary>
        /// Update with the current camera properties
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="camera"></param>
        public void Update(GameTime gameTime,Camera camera)
        {
            var viewingArea = camera.VisibleArea;
            var camPos = camera.Position;
            viewDimensions = new Vector2(viewingArea.Width, viewingArea.Height);
            offset = new Vector2(camPos.X - viewingArea.Width/2f, camPos.Y - viewingArea.Height / 2f);
            transformMatrix = camera.Transform;
        }

        /// <summary>
        /// Add a hud element
        /// </summary>
        /// <param name="element"></param>
        public void AddElement(HUDElement element) {
            elements.Add(element);
        }        

        /// <summary>
        /// Draw all hud elements
        /// </summary>
        /// <param name="graphicsDevice"></param>
        public void Draw(GraphicsDevice graphicsDevice)
        {
            //Use a separate sprite batch with Point sampling to avoid blurry text.
            //Based on advice from http://community.monogame.net/t/how-to-change-spritefont-sizes-without-anit-aliasing/9514
            var myBatch = new SpriteBatch(graphicsDevice);
            myBatch.Begin(sortMode: SpriteSortMode.BackToFront,transformMatrix: transformMatrix, samplerState: SamplerState.PointWrap);
            
            foreach (var item in elements)
            {
                item.Draw(myBatch, offset, viewDimensions, font, fontScale);
            }

            myBatch.End();            
        }       
    }

    /// <summary>
    /// Class representing a Hud element, consisting of some text at some position
    /// relative to the screen.
    /// </summary>
    public class HUDElement {
        public string text="";

        public Vector2 relativeAnchor = Vector2.Zero;//Anchor position relative to the screen (top left is 0,0)

        public Vector2 relativePosition;//Position relative to the anchor

        public Color color = Color.White;        

        public float scale = 1.0f;
        public Vector2 alignment = Vector2.Zero;//Alignment of text relative to the position

        public HUDElement()
        {
        }

        public HUDElement(string text, Vector2 relativePosition)
        {
            this.text = text;
            this.relativePosition = relativePosition;
        }

        public HUDElement(string text, Vector2 relativePosition, Color color, Vector2 relativeAnchor, float scale) : this(text, relativePosition)
        {
            this.color = color;
            this.relativeAnchor = relativeAnchor;
            this.scale = scale;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 offset, Vector2 viewDimensions,SpriteFont font,float fontScale) {
            var elemOff = offset;

            //Where to position text on screen
            elemOff += relativeAnchor * viewDimensions;
            var position = relativePosition + elemOff;//new Vector2(relativePosition.X + elemOff.X, relativePosition.Y + elemOff.Y);

            var textDimensions = font.MeasureString(text);
            var relativeOrigin = textDimensions * alignment;//Allow text to be centered, or aligned left/right etc

            var finalScale = scale * fontScale;

            spriteBatch.DrawString(font, text, position, color, 0, relativeOrigin, finalScale, SpriteEffects.None, 0);
        }
    }
}
