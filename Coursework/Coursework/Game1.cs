using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Coursework.Input;
using Coursework.Entities;
using Coursework.Projectiles;
using Coursework.StateMachine;
using Coursework.StateMachine.GameStates;
using Coursework.Serialization;

namespace Coursework
{

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public GraphicsDeviceManager graphics;
        public SpriteFont font;
        public Camera camera;

        public Graphics.Background background;
        public Texture2D defaultBackgroundTex;

        GameEventManager eventManager;

        FSM GameStateMachine;

        //Keybindings that persist accross all game states (fullscreen control)
        private KeybindingManager globalBindings;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            //Set window size
            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 600;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Initialises necessary objects for game start.
        /// Most notably initialises the game state machine
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = true;

            globalBindings = new KeybindingManager();
            //Bind the fullscreen toggle to the global bindings
            globalBindings.BindKeyEvent(Keys.F11, InputState.down, ToggleFullScreen);

            GameData.Initialise();

            eventManager = new GameEventManager();
            GameEventManager.Instance = eventManager;                    

            camera = new Camera(graphics.GraphicsDevice.Viewport);
            camera.Position = new Vector2(0, 0);
            Camera.mainCamera = camera;

            //Default background
            defaultBackgroundTex = Content.Load<Texture2D>("PlatformerGraphicsDeluxe/Backgrounds/bg");
            background = new Graphics.Background(defaultBackgroundTex, Vector2.One, Color.White);

            GameStateMachine = new FSM();
            //NOTE spacing doesn't seem to scale correctly, so tabs are used.
            TextScreen startScreenState = new StartScreen(this,"Explorer","Press    enter    to    continue",Keys.Enter);
            startScreenState.Name = "Start";

            PlayGame playGameState = new PlayGame(this);

            TextScreen loseScreenState = new EndScreen(this, "YOU    LOSE", "Press    enter    to    continue", Keys.Enter);
            loseScreenState.Name = "Lose";

            TextScreen winScreenState = new EndScreen(this, "YOU    WIN", "Press    enter    to    continue", Keys.Enter);
            winScreenState.Name = "Win";

            startScreenState.AddTransition(playGameState, () => { return startScreenState.done; });
            loseScreenState.AddTransition(startScreenState, () => { return loseScreenState.done; });
            winScreenState.AddTransition(startScreenState, () => { return winScreenState.done; });
            playGameState.AddTransition(loseScreenState, () => { return playGameState.GameLost; });
            playGameState.AddTransition(winScreenState, () => { return playGameState.GameWon; });

            GameStateMachine.AddStates(startScreenState,playGameState,loseScreenState,winScreenState);            

            base.Initialize();
        }

        /// <summary>
        /// Load the font and initialise the game state machine
        /// </summary>
        protected override void LoadContent()
        {
            font = Content.Load<SpriteFont>("Fonts/gamefont");//Same font as used in the labs     

            //This call is here, rather than in initialise, because the state machine requires the font to be already loaded
            GameStateMachine.Initialise("Start");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            GameStateMachine.Dispose();
            Content.Unload();
        }

        /// <summary>
        /// Update the game state machine and global keybindings
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            globalBindings.Update();

            GameStateMachine.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);          

            var gameState = GameStateMachine.CurrentState as GameState;
            if (gameState != null)
            {
                gameState.Draw();                
            }

            base.Draw(gameTime);
        }

        /// <summary>
        /// Allow toggling fullscreen on and off at any point in the game.
        /// Uses a "borderless fullscreen", 
        /// REF https://community.monogame.net/t/how-to-implement-borderless-fullscreen-on-desktopgl-project/8359
        /// </summary>
        private void ToggleFullScreen()
        {
            graphics.IsFullScreen = !graphics.IsFullScreen;
            graphics.HardwareModeSwitch = !graphics.HardwareModeSwitch;
            graphics.ApplyChanges();
        }
    }
}
