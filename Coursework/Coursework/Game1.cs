using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Coursework.Input;
using Coursework.Entities;
using Coursework.Projectiles;

namespace Coursework
{

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        Camera camera;

        Player player;
        Level currentLevel;
        CollisionManager collisionManager;
        KeybindingManager keybindingManager;
        HUDManager hudManager;
        GameEventManager eventManager;
        ProjectileManager projectileManager;


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
            eventManager.OnNextLevel += OnNextLevel;

            player = new Player(Services, Content.RootDirectory);
            eventManager.OnPlayerDied += OnPlayerDied;
            currentLevel = new Level(Services, Content.RootDirectory, GameData.Instance.levelConstants.startLevelName);

            projectileManager = new ProjectileManager(Services, Content.RootDirectory);

            camera = new Camera(graphics.GraphicsDevice.Viewport);
            camera.Position = new Vector2(0, 0);
            Camera.mainCamera = camera;
            collisionManager = new CollisionManager();
            keybindingManager = new KeybindingManager();


            InitKeybindings();
            base.Initialize();
        }

        /// <summary>
        /// Set up keybindings for game controls
        /// </summary>
        private void InitKeybindings() {
            keybindingManager.BindKeyEvent(Keys.A, InputState.held, player.LeftHeld);
            keybindingManager.BindKeyEvent(Keys.D, InputState.held, player.RightHeld);
            keybindingManager.BindKeyEvent(Keys.Space, InputState.down, player.Jump);
            keybindingManager.BindKeyEvent(Keys.S, InputState.held, player.Crouch);

            keybindingManager.BindPointerEvent(MouseButton.left, InputState.down, player.OnMouseButtonDown);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            font = Content.Load<SpriteFont>("Fonts/gamefont");//Same font as used in the labs

            //Initialise hud after font has been loaded
            hudManager = new HUDManager(font);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            player.Dispose();
            currentLevel.Dispose();
            GameEventManager.Instance.OnNextLevel -= OnNextLevel;
            GameEventManager.Instance.OnPlayerDied -= OnPlayerDied;
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

            // TODO: Add your update logic here
            keybindingManager.Update();//Update input events

            player.Update(gameTime);
            currentLevel.Update(gameTime);
            projectileManager.Update(gameTime);

            //Detect collisions
            collisionManager.Update(currentLevel, player,projectileManager);


            //Update camera
            camera.Position = player.Position;
            currentLevel.ConstrainCamera(camera);
            camera.Update(graphics.GraphicsDevice.Viewport);

            //Update HUD
            hudManager.Update(gameTime, camera);

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

            //Draw level and all entities managed by it
            currentLevel.Draw(gameTime, spriteBatch);

            //Draw projectiles
            projectileManager.Draw(spriteBatch);

            //Draw player
            player.Draw(spriteBatch);

            //Draw hud
            hudManager.Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void OnPlayerDied(object sender, System.EventArgs e)
        {
            //TODO game over screen
            player.HardReset();
            SwitchToLevel(GameData.Instance.levelConstants.startLevelName);
        }

        private void OnNextLevel(object sender, System.EventArgs e)
        {
            var nextLevelName = currentLevel.nextLevelName;
            if (nextLevelName != "")
            {
                SwitchToLevel(nextLevelName);
            }
            else {
                //TODO game over
            }
        }

        private void SwitchToLevel(string levelName) 
        {
            currentLevel.Dispose();
            currentLevel = new Level(Services, Content.RootDirectory, levelName);
            player.SetPosition(Vector2.Zero);
        }
    }
}
