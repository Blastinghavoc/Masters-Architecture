using System.Collections.Generic;
using Coursework.Input;
using Coursework.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Coursework.StateMachine.GameStates
{
    /// <summary>
    /// Subclass of TextScreen used for both the win and lose screens.
    /// Displays the high score table.
    /// </summary>
    class EndScreen : TextScreen
    {
        public EndScreen(Game1 owner, string title, string subtitle, Keys transitionKey) : base(owner, title, subtitle, transitionKey)
        {
        }

        protected List<HUDElement> highScoreTexts = new List<HUDElement>();
        protected HUDElement highScoreTitle;
        protected HUDElement currentScoreText;
        protected HUDElement resetText;

        public override void InitHUD()
        {
            base.InitHUD();
            //Re-orient title and subtitle to be towards the top of the screen
            titleText.relativePosition += new Vector2(0, 40);
            titleText.relativeAnchor = new Vector2(0.5f, 0.1f);
            subtitleText.relativePosition += new Vector2(0, 40);
            subtitleText.relativeAnchor = new Vector2(0.5f, 0.1f);

            highScoreTitle = new HUDElement("High   Scores:", Vector2.Zero);
            highScoreTitle.relativeAnchor = new Vector2(0.5f, 0.5f);
            highScoreTitle.alignment = new Vector2(0.5f);
            hudManager.AddElement(highScoreTitle);

            var currentScore = GameEventManager.Instance.score;

            currentScoreText = new HUDElement("Score:   "+currentScore.ToString(), new Vector2(0,-40));
            currentScoreText.relativeAnchor = new Vector2(0.5f, 0.5f);
            currentScoreText.alignment = new Vector2(0.5f);
            hudManager.AddElement(currentScoreText);

            resetText = new HUDElement("Press   R    to    reset    scoreboard", new Vector2(0, -40));
            resetText.relativeAnchor = new Vector2(0.5f, 1.0f);
            resetText.alignment = new Vector2(0.5f);
            hudManager.AddElement(resetText);

            //Set up the list of high scores
            var highScoreData = GameData.GetHighScoreData();
            if (highScoreData == null)
            {
                highScoreData = new HighScoreData();
            }
            
            highScoreData.Update(currentScore);

            float spacing = 40.0f;
            for (int i = 0; i < highScoreData.scores.Length; i++)
            {
                var amount = highScoreData.scores[i];

                HUDElement scoreText = new HUDElement(amount.ToString(), new Vector2(0, spacing * i));
                scoreText.relativeAnchor = new Vector2(0.5f, 0.6f);
                scoreText.alignment = new Vector2(0.5f);

                highScoreTexts.Add(scoreText);
                hudManager.AddElement(scoreText);
            }

            highScoreData.Save();

            //Score has been recorded if relevant, now reset
            GameEventManager.Instance.ResetScore();
        }

        public override void InitKeybindings()
        {
            base.InitKeybindings();
            keybindingManager.BindKeyEvent(Keys.R, InputState.down, ResetScoreboard);
        }

        private void ResetScoreboard() {
            var highScoreData = GameData.GetHighScoreData();
            if (highScoreData == null)
            {
                highScoreData = new HighScoreData();
            }

            highScoreData.Reset();

            foreach (var item in highScoreTexts)
            {
                item.text = "0";
            }

            //Save the reset scores.
            highScoreData.Save();
        }
    }
}
