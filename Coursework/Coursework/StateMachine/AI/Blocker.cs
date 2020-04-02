using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Entities.Enemies;
using Coursework.Entities;
using Microsoft.Xna.Framework;
using Coursework.Levels;

/// <summary>
/// States for the behaviour of the Blocker enemy.
/// </summary>
namespace Coursework.StateMachine.AI.Blocker
{
    class Idle : State
    {
        public float IdleDurationSeconds { get; set; } = 2;
        public float IdleTimerSeconds { get; set; }

        public bool DurationOver { get => IdleTimerSeconds <= 0; }

        public Idle()
        {
            Name = "Idle";
        }

        public override void OnEnter(object owner)
        {
            IdleTimerSeconds = IdleDurationSeconds;
        }

        public override void OnExit(object owner)
        {
        }

        public override void Update(object owner, GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            IdleTimerSeconds -= dt;
        }
    }

    /// <summary>
    /// Start state, figures out whether to go up or down first
    /// </summary>
    class Start : State
    {
        public bool up = false;
        public bool down = false;

        public Start()
        {
            Name = "Start";
        }

        public override void OnEnter(object owner)
        {
            up = false;
            down = false;
        }

        public override void OnExit(object owner)
        {            
        }

        public override void Update(object owner, GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (owner is Enemy enemy)
            {
                var currentLevel = Level.CurrentLevel;

                Point upTile;
                var topPoint = enemy.BoundingBox.Center;
                topPoint.Y -= enemy.BoundingBox.Height / 2;
                upTile = currentLevel.GetTileIndices(topPoint);

                Point downTile;
                var botomPoint = enemy.BoundingBox.GetBottomCenter();
                downTile = currentLevel.GetTileIndices(botomPoint);

                if (currentLevel.GetCollisionModeAt(downTile) == TileCollisionMode.solid)
                {
                    up = true;//If there is a tile beneath the blocker, start by going up
                }
                else
                {
                    down = true;//Otherwise, start by going down
                }
            }
        }
    }

    class Up : State,EventSubscriber
    {
        public float Speed = 64;
        public bool Done = false;
        private Enemy owner;

        public Up(Enemy owner)
        {
            Name = "Up";
            this.owner = owner;
        }

        public override void OnEnter(object owner)
        {
            Done = false;
            BindEvents();
        }

        public override void OnExit(object owner)
        {
            UnbindEvents();
        }

        public override void Update(object owner, GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Enemy enemy = owner as Enemy;
            if (enemy != null)
            {
                var currentLevel = Level.CurrentLevel;

                Point upTile;

                var topPoint = enemy.BoundingBox.Center;
                topPoint.Y -= enemy.BoundingBox.Height / 2;
                upTile = currentLevel.GetTileIndices(topPoint);

                if (currentLevel.GetCollisionModeAt(upTile) == TileCollisionMode.solid)
                {
                    Done = true;
                }
                else {
                    //Update position
                    enemy.SetPosition(enemy.Position - new Vector2(0, Speed * dt));
                }
            }
        }

        /// <summary>
        /// Crude simulation of crushing the player.
        /// Occurs when the player gets squashed into the ceiling.
        /// </summary>
        public void OnPlayerCollision(object sender, PlayerCollisionEventArgs e)
        {
            if (e.colllidedWith.Equals(owner))
            {
                //If player is colliding with this blocker, check the collision depth.
                var depth = e.collisionDepth;
                var absCollY = Math.Abs(depth.Y);
                var absCollX = Math.Abs(depth.X);
                //If the collision is vertical, and the collision depth is too much, kill the player
                if (absCollY < absCollX && depth.Y < -owner.BoundingBox.Height / 3)
                {
                    e.player.Kill();//Player got crushed
                }
            }
        }

        public void BindEvents()
        {
            GameEventManager.Instance.WhilePlayerColliding += OnPlayerCollision;
        }

        public void UnbindEvents()
        {
            GameEventManager.Instance.WhilePlayerColliding -= OnPlayerCollision;
        }

        public void Dispose()
        {
            UnbindEvents();
        }
    }

    class Down : State,EventSubscriber
    {
        public float Speed = 96;
        public bool Done = false;
        private Enemy owner;

        public Down(Enemy owner)
        {
            Name = "Down";
            this.owner = owner;
        }

        public override void OnEnter(object owner)
        {
            Done = false;
            BindEvents();
        }

        public override void OnExit(object owner)
        {
            UnbindEvents();
        }

        public override void Update(object owner, GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Enemy enemy = owner as Enemy;
            if (enemy != null)
            {
                var currentLevel = Level.CurrentLevel;

                Point downTile;

                var botomPoint = enemy.BoundingBox.GetBottomCenter();
                downTile = currentLevel.GetTileIndices(botomPoint);

                if (currentLevel.GetCollisionModeAt(downTile) == TileCollisionMode.solid)
                {
                    Done = true;
                }
                else
                {
                    //Update position
                    enemy.SetPosition(enemy.Position + new Vector2(0, Speed * dt));
                }
            }
        }

        /// <summary>
        /// Crude simulation of crushing the player.
        /// Occurs when the player gets squashed into the floor.
        /// </summary>
        public void OnPlayerCollision(object sender, PlayerCollisionEventArgs e)
        {
            if (e.colllidedWith.Equals(owner))
            {
                //If player is colliding with this blocker, check the collision depth.
                var depth = e.collisionDepth;
                var absCollY = Math.Abs(depth.Y);
                var absCollX = Math.Abs(depth.X);
                //If the collision is vertical, and the collision depth is too much, kill the player
                if (absCollY< absCollX && depth.Y > owner.BoundingBox.Height / 3)
                {
                    e.player.Kill();//Player got crushed
                }
            }
        }

        public void BindEvents()
        {
            GameEventManager.Instance.WhilePlayerColliding += OnPlayerCollision;
        }

        public void UnbindEvents()
        {
            GameEventManager.Instance.WhilePlayerColliding -= OnPlayerCollision;
        }

        public void Dispose()
        {
            UnbindEvents();
        }
    }
}
