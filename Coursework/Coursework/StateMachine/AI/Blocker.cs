using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Entities;
using Microsoft.Xna.Framework;

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

    class Up : State
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
            GameEventManager.Instance.WhilePlayerColliding += OnPlayerCollision;
        }

        public override void OnExit(object owner)
        {
            GameEventManager.Instance.WhilePlayerColliding -= OnPlayerCollision;
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
                if (absCollY < absCollX && depth.Y < -owner.BoundingBox.Height / 4)
                {
                    e.player.Kill();//Player got crushed
                }
            }
        }
    }

    class Down : State
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
            GameEventManager.Instance.WhilePlayerColliding += OnPlayerCollision;
        }

        public override void OnExit(object owner)
        {
            GameEventManager.Instance.WhilePlayerColliding -= OnPlayerCollision;
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
                if (absCollY< absCollX && depth.Y > owner.BoundingBox.Height / 4)
                {
                    e.player.Kill();//Player got crushed
                }
            }
        }
    }
}
