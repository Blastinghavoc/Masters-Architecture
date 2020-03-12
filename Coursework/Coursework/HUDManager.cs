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
    class HUDManager
    {
        private SpriteFont font;

        private List<HUDElement> elements = new List<HUDElement>();

        private Vector2 offset;//Offset required to keep text aligned with camera
        private Vector2 viewDimensions;//Current viewport dimensions

        public HUDManager(SpriteFont font) {
            this.font = font;
        }

        public void Update(GameTime gameTime,Camera camera)
        {
            var viewingArea = camera.VisibleArea;
            var camPos = camera.Position;
            viewDimensions = new Vector2(viewingArea.Width, viewingArea.Height);
            offset = new Vector2(camPos.X - viewingArea.Width/2f, camPos.Y - viewingArea.Height / 2f);
        }

        public void AddElement(HUDElement element) {
            elements.Add(element);
        }        

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var item in elements)
            {
                DrawElement(item, spriteBatch);
            }
        }

        private void DrawElement(HUDElement element, SpriteBatch spriteBatch) {
            var elemOff = offset;

            elemOff += element.relativeAnchor * viewDimensions;
            //TODO see https://stackoverflow.com/questions/10263734/how-to-align-text-drawn-by-spritebatch-drawstring
            var position = new Vector2(element.relativePosition.X + elemOff.X, element.relativePosition.Y + elemOff.Y);
            spriteBatch.DrawString(font, element.text,position,element.color,0,Vector2.Zero,element.scale,SpriteEffects.None,0);
        }
    }

    public class HUDElement {
        public string text="";
        public Vector2 relativePosition;
        public Color color = Color.White;
        public Vector2 relativeAnchor;
        public float scale;

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
    }
}
