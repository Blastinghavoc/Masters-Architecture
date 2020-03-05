using Coursework.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework
{
    class GameEventManager
    {
        public static GameEventManager Instance;

        public event EventHandler<ScoreEventArgs> OnScoreChanged = delegate { };
        public event EventHandler<PlayerCollisionEventArgs> OnPlayerColliding = delegate { };
        public event EventHandler<PlayerCollisionEventArgs> OnPlayerCollisionEnter = delegate { };
        public event EventHandler<PlayerHealthChangedEventArgs> OnPlayerHealthChanged = delegate { };
        public event EventHandler<EnemyKilledEventArgs> OnEnemyKilled = delegate { };
        public event EventHandler<EventArgs> OnNextLevel = delegate { };
        public event EventHandler<EventArgs> OnPlayerDied = delegate { };
        


        private int prevScore;
        private int score;

        public void Update(GameTime gameTime)
        {
          
        }

        public void AddScore(int amount)
        {
            prevScore = score;
            score += amount;
            OnScoreChanged?.Invoke(this, new ScoreEventArgs(score, score - prevScore));
        }

        public void PlayerDied()
        {
            OnPlayerDied?.Invoke(this, new EventArgs());
        }

        public void NextLevel()
        {
            OnNextLevel?.Invoke(this, new EventArgs());
        }

        public void PlayerHealthChanged(Player player)
        {
            OnPlayerHealthChanged?.Invoke(this,new PlayerHealthChangedEventArgs(player));
        }

        public void EnemyKilled(Enemy enemy)
        {
            OnEnemyKilled?.Invoke(this, new EnemyKilledEventArgs(enemy));
        }

        //Fire player collision events
        public void OnPlayerCollision(Player player, Object collidedWith, Vector2 collisionDepth,CollisionType collisionType= CollisionType.stay)
        {
            //Always fires if there is a collision happening
            if (collisionType != CollisionType.exit)
            {
                OnPlayerColliding?.Invoke(this,new PlayerCollisionEventArgs(player,collidedWith,collisionDepth,collisionType));
            }

            //Only fires when a collision first starts happening
            if (collisionType == CollisionType.enter)
            {
                OnPlayerCollisionEnter?.Invoke(this, new PlayerCollisionEventArgs(player, collidedWith, collisionDepth,collisionType));
            }
        }
    }

    class EnemyKilledEventArgs
    {
        public Enemy enemy;

        public EnemyKilledEventArgs(Enemy enemy)
        {
            this.enemy = enemy;
        }
    }

    class PlayerHealthChangedEventArgs
    {
        public Player player;

        public PlayerHealthChangedEventArgs(Player player)
        {
            this.player = player;
        }
    }

    class PlayerCollisionEventArgs
    {
        public Player player;
        public Object colllidedWith;
        public Vector2 collisionDepth;
        public CollisionType collisionType;

        public PlayerCollisionEventArgs(Player player, object colllidedWith, Vector2 collisionDepth, CollisionType collisionType)
        {
            this.player = player;
            this.colllidedWith = colllidedWith;
            this.collisionDepth = collisionDepth;
            this.collisionType = collisionType;
        }
    }

    class ScoreEventArgs {
        public int newScore;
        public int amountAdded;

        public ScoreEventArgs(int newScore, int amountAdded)
        {
            this.newScore = newScore;
            this.amountAdded = amountAdded;
        }
    }
}
