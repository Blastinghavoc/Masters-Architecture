using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Coursework.Input;
using Coursework.Entities;
using Coursework.Projectiles;
using Coursework.StateMachine;
using Coursework.StateMachine.GameState;

namespace Coursework
{

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public SpriteFont font;
        public Camera camera;

        //Player player;
        //Level currentLevel;
        //CollisionManager collisionManager;
        //KeybindingManager keybindingManager;
        //HUDManager hudManager;
        GameEventManager eventManager;
        //ProjectileManager projectileManager;

        FSM GameStateMachine;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = true;

            GameData.Initialise();

            eventManager = new GameEventManager();
            GameEventManager.Instance = eventManager;                    

            camera = new Camera(graphics.GraphicsDevice.Viewport);
            camera.Position = new Vector2(0, 0);
            Camera.mainCamera = camera;          

            GameStateMachine = new FSM();
            //NOTE spacing doesn't seem to scale correctly, so tabs are used.
            TextScreen startScreenState = new TextScreen(this,"Explorer","press    enter    to    continue",Keys.Enter);
            startScreenState.Name = "Start";

            PlayGame playGameState = new PlayGame(this);

            TextScreen loseScreenState = new TextScreen(this, "YOU    LOSE", "press    enter    to    continue", Keys.Enter);
            loseScreenState.Name = "Lose";

            TextScreen winScreenState = new TextScreen(this, "YOU    WIN", "press    enter    to    continue", Keys.Enter);
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
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("Fonts/gamefont");//Same font as used in the labs

            GameStateMachine.Initialise("Start");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

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

            spriteBatch.Begin(transformMatrix: camera.Transform);

            var gameState = GameStateMachine.CurrentState as GameState;
            if (gameState != null)
            {
                gameState.Draw(gameTime, spriteBatch);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
