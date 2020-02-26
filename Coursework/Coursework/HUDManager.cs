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
    class HUDManager:IDisposable
    {
        private class HUDElement {
            public string text="";
            public Vector2 relativePosition;
            public Color color = Color.White;

            public HUDElement()
            {
            }

            public HUDElement(string text, Vector2 relativePosition)
            {
                this.text = text;
                this.relativePosition = relativePosition;
            }
        }

        SpriteFont font;

        HUDElement scoreText;
        private Vector2 offset;//Offset required to keep text aligned with camera

        public HUDManager(SpriteFont font) {
            scoreText = new HUDElement("Score: 0", new Vector2(0, 0));
            this.font = font;
            GameEventManager.Instance.OnScoreChanged += OnScoreChanged;
        }

        public void Update(GameTime gameTime,Camera camera)
        {
            var viewingArea = camera.VisibleArea;
            var camPos = camera.Position;
            offset = new Vector2(camPos.X - viewingArea.Width/2f, camPos.Y - viewingArea.Height / 2f);
        }

        public void OnScoreChanged(object sender, ScoreEventArgs e)
        {
            scoreText.text = "Score: " + e.newScore.ToString();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawElement(scoreText, spriteBatch);
        }

        private void DrawElement(HUDElement element, SpriteBatch spriteBatch) {
            var origin = offset;
            var position = new Vector2(element.relativePosition.X + origin.X, element.relativePosition.Y + origin.Y);
            spriteBatch.DrawString(font, element.text,position,element.color);
        }

        public void Dispose()
        {
            GameEventManager.Instance.OnScoreChanged -= OnScoreChanged;
        }
    }
}
