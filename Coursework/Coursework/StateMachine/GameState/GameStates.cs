using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Coursework.StateMachine.GameState
{
    //Represents states of the whole game (title screen, gameplay, end screen)
    abstract class GameState : State
    {
        protected Game1 owner;
        protected GameEventManager eventManager;
        public GameState(Game1 owner) {
            this.owner = owner;
            eventManager = GameEventManager.Instance;
        }

        protected KeybindingManager keybindingManager;
        protected HUDManager hudManager;

        public override void OnEnter(object owner)
        {
            LoadContent();
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Draw hud
            hudManager.Draw(spriteBatch);
        }

        public override void OnExit(object owner)
        {
            UnloadContent();
        }

        public virtual void LoadContent() {
            InitHUD();
            InitKeybindings();
            BindEvents();
        }

        public abstract void BindEvents();
        public abstract void UnbindEvents();

        public virtual void InitHUD()
        {
            hudManager = new HUDManager(owner.font);
        }

        public virtual void InitKeybindings()
        {
            keybindingManager = new KeybindingManager();
        }

        public virtual void UnloadContent() {
            UnbindEvents();
        }

        private void PreUpdate(GameTime gameTime) {
            keybindingManager.Update();//Update input events
        }

        private void PostUpdate(GameTime gameTime) {
            owner.camera.Update(owner.graphics.GraphicsDevice.Viewport);

            //Update HUD
            hudManager.Update(gameTime, owner.camera);
        }

        public override void Update(object owner, GameTime gameTime)
        {
            PreUpdate(gameTime);
            Update(gameTime);
            PostUpdate(gameTime);
        }

        //Equivalent to Game.Update
        public abstract void Update(GameTime gameTime);

    }

    //A text-only screen with a single transition out of it
    class TextScreen : GameState {
        public TextScreen(Game1 owner,string title, string subtitle, Microsoft.Xna.Framework.Input.Keys transitionKey) : base(owner)
        {
            this.title = title;
            this.subtitle = subtitle;
            this.transitionKey = transitionKey;
        }

        public bool done { get; protected set; } = false;

        string title;
        string subtitle;
        Microsoft.Xna.Framework.Input.Keys transitionKey;

        HUDElement titleText;
        HUDElement subtitleText;

        public override void InitHUD()
        {
            base.InitHUD();
            titleText = new HUDElement(title, new Vector2(0, -30));
            titleText.relativeAnchor = new Vector2(0.5f, 0.5f);//Center of screen
            titleText.alignment = new Vector2(0.5f, 0.5f);//Center text
            titleText.scale = 2.0f;

            subtitleText = new HUDElement(subtitle, new Vector2(0, 45));
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

        public override void BindEvents()
        {
        }

        public override void UnbindEvents()
        {
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}
