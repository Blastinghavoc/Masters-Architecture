#region File Description
//-----------------------------------------------------------------------------
// Game.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
#endregion

namespace ChaseCameraSample
{
    /// <summary>
    /// Sample showing how to implement a simple chase camera.
    /// </summary>
    public class ChaseCameraGame : Microsoft.Xna.Framework.Game
    {
        #region Fields

        GraphicsDeviceManager graphics;
        DebugDraw debug;

        SpriteBatch spriteBatch;
        SpriteFont spriteFont;

        Ship ship;
        List<EnemyShip> enemies;
        
        ChaseCamera camera;

        Model shipModel;
        Model groundModel;

        bool cameraSpringEnabled = true;

        CommandManager commandManager;

        CollisionManager collisionManager;

        #endregion

        #region Initialization


        public ChaseCameraGame()
        {
            commandManager = new CommandManager();
            graphics = new GraphicsDeviceManager(this);
            graphics.SupportedOrientations = DisplayOrientation.Portrait;

            collisionManager = new CollisionManager();
            
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

#if WINDOWS_PHONE
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;
            
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            graphics.IsFullScreen = true;
#else
            graphics.PreferredBackBufferWidth = 853;
            graphics.PreferredBackBufferHeight = 480;
#endif

            // Create the chase camera
            camera = new ChaseCamera();

            // Set the camera offsets
            camera.DesiredPositionOffset = new Vector3(0.0f, 2000.0f, 3500.0f);
            camera.LookAtOffset = new Vector3(0.0f, 150.0f, 0.0f);

            // Set camera perspective
            camera.NearPlaneDistance = 10.0f;
            camera.FarPlaneDistance = 100000.0f;
        }


        /// <summary>
        /// Initalize the game
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            debug = new DebugDraw(GraphicsDevice);

            // Load ship and add to collision manager
            ship = new Ship(GraphicsDevice);

            // Load enemy ships and add to collision manager
            const int numEnemies = 5;
            enemies = new List<EnemyShip>();
            Random rand = new Random();
            for (int i = 0; i < numEnemies; i++)
            {
                EnemyShip enemy = new EnemyShip(GraphicsDevice);
                
                // Generate two floats between -1.0f and 1.0f
                float randX = (float)(rand.NextDouble() - rand.NextDouble());
                float randZ = (float)(rand.NextDouble() - rand.NextDouble());

                enemy.Position = new Vector3(randX * 50000.0f, 350.0f, randZ * 50000.0f);
                enemies.Add(enemy);
            }

            // Initialise the collidable objects
            InitializeCollidableObjects();

            // Set the camera aspect ratio
            // This must be done after the class to base.Initalize() which will
            // initialize the graphics device.
            camera.AspectRatio = (float)graphics.GraphicsDevice.Viewport.Width /
                graphics.GraphicsDevice.Viewport.Height;


            // Perform an inital reset on the camera so that it starts at the resting
            // position. If we don't do this, the camera will start at the origin and
            // race across the world to get behind the chased object.
            // This is performed here because the aspect ratio is needed by Reset.
            UpdateCameraChaseTarget();
            camera.Reset();

            InitializeBindings();
        }

        private void InitializeCollidableObjects()
        {
            collisionManager.AddCollidable(ship);
            foreach (var item in enemies)
            {
                collisionManager.AddCollidable(item);
            }
        }

        private void InitializeBindings()
        {
            commandManager.AddKeyboardBinding(Keys.Escape, StopGame);
            commandManager.AddKeyboardBinding(Keys.C, EnableCameraSpring);
            commandManager.AddKeyboardBinding(Keys.W, ship.Thrust);
            commandManager.AddKeyboardBinding(Keys.A, ship.TurnLeft);
            commandManager.AddKeyboardBinding(Keys.D, ship.TurnRight);
            commandManager.AddMouseBinding(MouseButton.LEFT, ship.MouseTurn);

        }


        /// <summary>
        /// Load graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>("gameFont");

            shipModel = Content.Load<Model>(@"FBX\Ship");
            groundModel = Content.Load<Model>("Ground");
        }


        #endregion

        #region Game Actions
        public void StopGame(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
            {
                Exit();
            }
        }

        public void EnableCameraSpring(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.PRESSED)
            {
                cameraSpringEnabled = !cameraSpringEnabled;
            }
        }
        #endregion

        #region Update and Draw


        /// <summary>
        /// Allows the game to run logic.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
			// Update the command manager (updates polling input and fires input events)
            commandManager.Update();

            collisionManager.Update();

            // Update the ship
            ship.Update(gameTime);

            // Update enemies
            foreach (EnemyShip enemy in enemies)
            {
                enemy.Update(gameTime);
            }

            // Update the camera to chase the new target
            UpdateCameraChaseTarget();

            // The chase camera's update behavior is the springs, but we can
            // use the Reset method to have a locked, spring-less camera
            if (cameraSpringEnabled)
                camera.Update(gameTime);
            else
                camera.Reset();


            base.Update(gameTime);
        }

        /// <summary>
        /// Update the values to be chased by the camera
        /// </summary>
        private void UpdateCameraChaseTarget()
        {
            camera.ChasePosition = ship.Position;
            camera.ChaseDirection = ship.Direction;
            camera.Up = ship.Up;
        }


        /// <summary>
        /// Draws the ship and ground.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice device = graphics.GraphicsDevice;

            device.Clear(Color.CornflowerBlue);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            DrawModel(shipModel, ship.World);

            foreach (Ship enemy in enemies)
            {
                DrawModel(shipModel, enemy.World);
            }

            DrawModel(groundModel, Matrix.Identity);

            debug.Begin(camera.View, camera.Projection);

            debug.DrawWireSphere(ship.BoundingSphere, Color.Green);

            foreach (EnemyShip enemy in enemies)
            {
                debug.DrawWireSphere(enemy.BoundingSphere, Color.White);
            }

            debug.End();

            DrawOverlayText();

            base.Draw(gameTime);
        }


        /// <summary>
        /// Simple model drawing method. The interesting part here is that
        /// the view and projection matrices are taken from the camera object.
        /// </summary>        
        private void DrawModel(Model model, Matrix world)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * world;

                    // Use the matrices provided by the chase camera
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                }
                mesh.Draw();
            }
        }


        /// <summary>
        /// Displays an overlay showing what the controls are,
        /// and which settings are currently selected.
        /// </summary>
        private void DrawOverlayText()
        {
            spriteBatch.Begin();

            string text = "Camera spring (" + (cameraSpringEnabled ?
                              "on" : "off") + ")";

            // Draw the string twice to create a drop shadow, first colored black
            // and offset one pixel to the bottom right, then again in white at the
            // intended position. This makes text easier to read over the background.
            spriteBatch.DrawString(spriteFont, text, new Vector2(65, 65), Color.Black);
            spriteBatch.DrawString(spriteFont, text, new Vector2(64, 64), Color.White);

            spriteBatch.End();
        }


        #endregion
    }


    #region Entry Point

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static class Program
    {
        static void Main()
        {
            using (ChaseCameraGame game = new ChaseCameraGame())
            {
                game.Run();
            }
        }
    }

    #endregion
}
