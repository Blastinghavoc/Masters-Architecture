using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Coursework.StateMachine.GameState
{
    /// <summary>
    /// Subclass of TextScreen used at the start of the game.
    /// Displays the controls as well as the title.
    /// </summary>
    class StartScreen : TextScreen
    {
        public StartScreen(Game1 owner, string title, string subtitle, Keys transitionKey) : base(owner, title, subtitle, transitionKey)
        {
        }

        protected HUDElement controlsText;

        public override void InitHUD()
        {
            base.InitHUD();

            //Re-orient title and subtitle to be towards the top of the screen
            titleText.relativePosition += new Vector2(0, 40);
            titleText.relativeAnchor = new Vector2(0.5f, 0.1f);
            subtitleText.relativePosition += new Vector2(0, 40);
            subtitleText.relativeAnchor = new Vector2(0.5f, 0.1f);


            string controlsString = "Controls:\n" +
                "A->move    left\n" +
                "D->move    right\n" +
                "SPACE->jump\n" +
                "LMB->shoot    (if    possible)";

            controlsText = new HUDElement(controlsString, Vector2.Zero, Color.White, new Vector2(0.5f,0.4f), 1);
            controlsText.alignment = new Vector2(0.5f,0);
            hudManager.AddElement(controlsText);

        }
    }
}
