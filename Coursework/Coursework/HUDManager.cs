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
        HUDElement healthText;


        private Vector2 offset;//Offset required to keep text aligned with camera

        public HUDManager(SpriteFont font) {
            this.font = font;

            scoreText = new HUDElement("Score: 0", new Vector2(0, 0));
            healthText = new HUDElement("Health: "+ GameData.Instance.playerData.startHealth.ToString(), new Vector2(0, 30));

            GameEventManager.Instance.OnScoreChanged += OnScoreChanged;
            GameEventManager.Instance.OnPlayerHealthChanged += OnHealthChanged;
        }

        public void Update(GameTime gameTime,Camera camera)
        {
            var viewingArea = camera.VisibleArea;
            var camPos = camera.Position;
            offset = new Vector2(camPos.X - viewingArea.Width/2f, camPos.Y - viewingArea.Height / 2f);
        }

        public void OnHealthChanged(object sender,PlayerHealthChangedEventArgs e)
        {
            healthText.text = "Health: " + e.player.Health.ToString();
        }

        public void OnScoreChanged(object sender, ScoreEventArgs e)
        {
            scoreText.text = "Score: " + e.newScore.ToString();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawElement(scoreText, spriteBatch);
            DrawElement(healthText, spriteBatch);
        }

        private void DrawElement(HUDElement element, SpriteBatch spriteBatch) {
            var origin = offset;
            var position = new Vector2(element.relativePosition.X + origin.X, element.relativePosition.Y + origin.Y);
            spriteBatch.DrawString(font, element.text,position,element.color);
        }

        public void Dispose()
        {
            GameEventManager.Instance.OnScoreChanged -= OnScoreChanged;
            GameEventManager.Instance.OnPlayerHealthChanged -= OnHealthChanged;
        }
    }
}
