using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Coursework.StateMachine.GameStates
{
    /// <summary>
    /// Represents states of the whole game (title screen, gameplay, end screen).
    /// Mimics the usual structure of the Game1 class.
    /// </summary>
    abstract class GameState : State, EventSubscriber
    {
        protected Game1 owner;//Provides acces to certain common data.
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

        /// <summary>
        /// Handles drawing sequence.
        /// Note that anything in PreDraw and 
        /// PostDraw creates its own spritebatch.
        /// </summary>
        public void Draw()
        {
            PreDraw();

            SpriteBatch spriteBatch = new SpriteBatch(owner.GraphicsDevice);
            spriteBatch.Begin(sortMode: SpriteSortMode.BackToFront, transformMatrix: owner.camera.Transform);

            Draw(spriteBatch);

            spriteBatch.End();

            PostDraw();
        }

        private void PreDraw()
        {
            //Draw background
            background.Draw(owner.GraphicsDevice);
        }

        private void PostDraw()
        {
            //Draw HUD last, over everything else
            hudManager.Draw(owner.GraphicsDevice);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {            
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

        public virtual void BindEvents() { }
        public virtual void UnbindEvents() { }

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

        public void Dispose()
        {
            UnloadContent();
        }
    }
}
