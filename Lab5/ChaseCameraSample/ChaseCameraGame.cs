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

        Model shipModel;
        Model groundModel;

        CommandManager commandManager;
        CollisionManager collisionManager;

        Matrix view;
        Matrix projection;

        World world;

        public const float WorldBoundX = 22000.0f;
        public const float WorldBoundZ = 16000.0f;

        #endregion

        #region Initialization


        public ChaseCameraGame()
        {
            commandManager = new CommandManager();
            graphics = new GraphicsDeviceManager(this);
            graphics.SupportedOrientations = DisplayOrientation.Portrait;
            
            
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

#if WINDOWS_PHONE
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;
            
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            graphics.IsFullScreen = true;
#else
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
#endif

            // Initialise the collision manager
            collisionManager = new CollisionManager();

            world = new World();
        }


        /// <summary>
        /// Initalize the game
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            debug = new DebugDraw(GraphicsDevice);

            // Load ship and add to collision manager
            Ship ship = new Ship(GraphicsDevice);
            ship.WorldBounds = new Vector3(WorldBoundX, 0.0f, WorldBoundZ);

            world.SetPlayer(ship);
            world.Player.Tag();

            // Load enemy ships and add to collision manager
            const int numEnemies = 3;
            for (int i = 0; i < numEnemies; i++)
            {
                EnemyShip enemy = new EnemyShip(GraphicsDevice);
                
                // Generate two floats between -1.0f and 1.0f
                float randX = RandomHelper.RandFloat();
                float randZ = RandomHelper.RandFloat();

                enemy.Position = new Vector3(randX * WorldBoundX, 350.0f, randZ * WorldBoundZ);
                enemy.WorldBounds = new Vector3(WorldBoundX, 0.0f, WorldBoundZ);
                world.AddEnemy(enemy);
            }

            // Initialise the collidable objects
            InitializeCollidableObjects();

            // Set camera perspective
            float nearPlaneDistance = 100.0f;
            float farPlaneDistance = 1000000.0f;
			
			// Create the view and projection matrices
            view = Matrix.CreateLookAt(new Vector3(0.0f, 40000.0f, 50.0f),
                                        new Vector3(0.0f, 0.0f, 0.0f),
                                        Vector3.UnitY);

            float aspectRatio = (float)graphics.GraphicsDevice.Viewport.Width / graphics.GraphicsDevice.Viewport.Height;
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, nearPlaneDistance, farPlaneDistance);

            InitializeBindings();
        }

        private void InitializeCollidableObjects()
        {
            collisionManager.AddCollidable(world.Player);

            foreach(EnemyShip enemy in world.Enemies)
            {
                collisionManager.AddCollidable(enemy);
            }
        }

        private void InitializeBindings()
        {
            commandManager.AddKeyboardBinding(Keys.Escape, StopGame);
            commandManager.AddKeyboardBinding(Keys.W, world.Player.Thrust);
            commandManager.AddKeyboardBinding(Keys.A, world.Player.TurnLeft);
            commandManager.AddKeyboardBinding(Keys.D, world.Player.TurnRight);

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

            world.UpdateWorld(gameTime);

            base.Update(gameTime);
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

            Ship ship = world.Player;
            DrawModel(shipModel, ship.World);

            foreach (EnemyShip enemy in world.Enemies)
            {
                DrawModel(shipModel, enemy.World);
            }

            DrawModel(groundModel, Matrix.CreateScale(0.5f));

            debug.Begin(view, projection);

            debug.DrawWireSphere(ship.BoundingSphere, Color.Green);
            debug.DrawWireSphere(ship.Sensor, Color.Yellow);

            foreach (EnemyShip enemy in world.Enemies)
            {
                debug.DrawWireSphere(enemy.BoundingSphere, Color.White);
                debug.DrawWireSphere(enemy.Sensor, Color.Yellow);
            }

            if (world.CurrentTagger != null)
            {
                debug.DrawWireSphere(world.CurrentTagger.BoundingSphere, Color.Red);
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
                    effect.View = view;
                    effect.Projection = projection;
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

            string text = "Player Position: " + world.Player.World.Translation.ToString();

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
