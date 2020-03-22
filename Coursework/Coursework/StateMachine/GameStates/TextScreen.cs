using Coursework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Coursework.StateMachine.GameStates
{
    /// <summary>
    /// A text-only screen with a single transition out of it.
    /// Has a title and subtitle, and listens to a key event to transition.
    /// </summary>
    class TextScreen : GameState {
        public TextScreen(Game1 owner,string title, string subtitle, Keys transitionKey) : base(owner)
        {
            this.title = title;
            this.subtitle = subtitle;
            this.transitionKey = transitionKey;
        }

        //Used in the transition out of this state
        public bool done { get; protected set; } = false;

        string title;
        string subtitle;
        Keys transitionKey;

        protected HUDElement titleText;
        protected HUDElement subtitleText;

        public override void InitHUD()
        {
            base.InitHUD();
            titleText = new HUDElement(title, new Vector2(0, -30));
            titleText.relativeAnchor = new Vector2(0.5f, 0.5f);//Center of screen
            titleText.alignment = new Vector2(0.5f, 0.5f);//Center text
            titleText.scale = 2.0f;

            subtitleText = new HUDElement(subtitle, new Vector2(0, 40));
            subtitleText.relativeAnchor = new Vector2(0.5f, 0.5f);//Center of screen
            subtitleText.alignment = new Vector2(0.5f, 0.5f);//Center text
            subtitleText.scale = 1.0f;

            hudManager.AddElement(titleText);
            hudManager.AddElement(subtitleText);
        }

        public override void OnEnter(object owner)
        {
            base.OnEnter(owner);
            done = false;
        }

        public override void InitKeybindings()
        {
            base.InitKeybindings();
            keybindingManager.BindKeyEvent(transitionKey, InputState.down, Done);
        }

        private void Done()
        {
            done = true;
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}
