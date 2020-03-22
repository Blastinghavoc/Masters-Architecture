using Coursework.Input;
using Coursework.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Coursework.Serialization;

namespace Coursework.StateMachine.GameStates
{
    //The actual gameplay state
    class PlayGame : GameState
    {
        public PlayGame(Game1 owner):base(owner)
        {
            Name = "PlayGame";
        }

        Entities.Player player;
        Level currentLevel;
        ProjectileManager projectileManager;
        CollisionManager collisionManager;

        //HUD
        HUDElement scoreText;
        HUDElement healthText;

        //Variables used in transitions out of this state
        public bool GameWon { get; private set; } = false;
        public bool GameLost { get; private set; } = false;

        public override void OnEnter(object owner)
        {
            base.OnEnter(owner);
            GameWon = false;
            GameLost = false;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Draw level and all entities managed by it
            currentLevel.Draw(gameTime, spriteBatch);

            //Draw projectiles
            projectileManager.Draw(spriteBatch);

            //Draw player
            player.Draw(spriteBatch);
            base.Draw(gameTime, spriteBatch);
        }

        public override void InitHUD()
        {
            base.InitHUD();

            scoreText = new HUDElement("Score:    0", new Vector2(0, 0));
            healthText = new HUDElement("Health:    " + GameData.Instance.playerData.startHealth.ToString(), new Vector2(0, 30));

            hudManager.AddElement(scoreText);
            hudManager.AddElement(healthText);
        }

        public override void InitKeybindings()
        {
            base.InitKeybindings();

            keybindingManager.BindKeyEvent(Keys.A, InputState.held, player.LeftHeld);
            keybindingManager.BindKeyEvent(Keys.D, InputState.held, player.RightHeld);
            keybindingManager.BindKeyEvent(Keys.Space, InputState.down, player.Jump);
            keybindingManager.BindKeyEvent(Keys.S, InputState.held, player.Crouch);

            keybindingManager.BindPointerEvent(MouseButton.left, InputState.down, player.AttemptToUseWeapon);
        }

        public override void LoadContent()
        {
            var Services = owner.Services;
            var Content = owner.Content;

            player = new Entities.Player(Services, Content.RootDirectory);

            currentLevel = new Level(Services, Content.RootDirectory, GameData.Instance.levelConstants.startLevelName,background);

            projectileManager = new ProjectileManager(Services, Content.RootDirectory);

            collisionManager = new CollisionManager();
            base.LoadContent();
        }

        public override void UnloadContent()
        {
            //Reset background texture
            background.SetTexture(owner.defaultBackgroundTex);
            //Dispose of all content
            player.Dispose();
            currentLevel.Dispose();
            projectileManager.Dispose();
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            player.Update(gameTime);
            currentLevel.Update(gameTime);
            projectileManager.Update(gameTime);

            //Detect collisions
            collisionManager.Update(currentLevel, player, projectileManager);


            //Update camera
            owner.camera.Position = player.Position;
            owner.camera.ConstrainToArea(currentLevel.LevelBounds);
        }

        private void SwitchToLevel(string levelName)
        {
            currentLevel.Dispose();
            currentLevel = new Level(owner.Services, owner.Content.RootDirectory, levelName,background);
            player.Reset(Vector2.Zero);
            projectileManager.Reset();
        }

        public override void BindEvents()
        {
            eventManager.OnNextLevel += OnNextLevel;
            eventManager.OnPlayerDied += OnPlayerDied;
            eventManager.OnScoreChanged += OnScoreChanged;
            eventManager.OnPlayerHealthChanged += OnHealthChanged;
        }

        public override void UnbindEvents()
        {
            eventManager.OnNextLevel -= OnNextLevel;
            eventManager.OnPlayerDied -= OnPlayerDied;
            eventManager.OnScoreChanged -= OnScoreChanged;
            eventManager.OnPlayerHealthChanged -= OnHealthChanged;
        }

        private void OnPlayerDied(object sender, System.EventArgs e)
        {
            GameLost = true;
        }

        private void OnNextLevel(object sender, System.EventArgs e)
        {
            var nextLevelName = currentLevel.nextLevelName;
            if (nextLevelName != "")
            {
                SwitchToLevel(nextLevelName);
            }
            else
            {
                GameWon = true;
            }
        }

        public void OnHealthChanged(object sender, PlayerHealthChangedEventArgs e)
        {
            healthText.text = "Health:    " + e.player.Health.ToString();
        }

        public void OnScoreChanged(object sender, ScoreEventArgs e)
        {
            scoreText.text = "Score:    " + e.newScore.ToString();
        }
    }
}
