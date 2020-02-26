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
        public event EventHandler<PlayerCollisionEventArgs> OnPlayerCollided = delegate { };

        private int prevScore;
        private int score;

        public void Update(GameTime gameTime)
        {
            //Only fire score changed event once per frame, as the score could potentially be modified multiple times in one frame
            if (prevScore != score)
            {
                OnScoreChanged?.Invoke(this, new ScoreEventArgs(score, score-prevScore));
            }
        }

        public void AddScore(int amount)
        {
            prevScore = score;
            score += amount;            
        }

        public void OnPlayerCollision(Player player, Object colllidedWith, Vector2 collisionDepth)
        {
            OnPlayerCollided?.Invoke(this,new PlayerCollisionEventArgs(player,colllidedWith,collisionDepth));
        }
    }

    class PlayerCollisionEventArgs
    {
        public Player player;
        public Object colllidedWith;
        public Vector2 collisionDepth;

        public PlayerCollisionEventArgs(Player player, object colllidedWith, Vector2 collisionDepth)
        {
            this.player = player;
            this.colllidedWith = colllidedWith;
            this.collisionDepth = collisionDepth;
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
