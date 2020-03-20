using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Input;
using Coursework.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Coursework.StateMachine.GameState
{
    //Represents states of the whole game (title screen, gameplay, end screen)
    abstract class GameState : State
    {
        protected Game1 owner;
        protected GameEventManager eventManager;
        protected Graphics.Background background;
        public GameState(Game1 owner) {
            this.owner = owner;
            eventManager = GameEventManager.Instance;
            background = owner.background;
        }

        protected KeybindingManager keybindingManager;
        protected HUDManager hudManager;

        public override void OnEnter(object owner)
        {
            LoadContent();
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            background.Draw(spriteBatch);
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
            //Update camera and background
            owner.camera.Update(owner.graphics.GraphicsDevice.Viewport);
            background.SetPosition(owner.camera.Position);

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
        public TextScreen(Game1 owner,string title, string subtitle, Keys transitionKey) : base(owner)
        {
            this.title = title;
            this.subtitle = subtitle;
            this.transitionKey = transitionKey;
        }

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
