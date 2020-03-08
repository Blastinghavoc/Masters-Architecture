using Coursework.Entities;
using Coursework.Projectiles;
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
        public event EventHandler<ProjectileLaunchEventArgs> OnLaunchProjectile = delegate { };
        public event EventHandler<ProjectileKilledEventArgs> OnProjectileKilled = delegate { };

        //Anything other than the player collides with something (i.e, neither entity involved in the collision was the player)
        public event EventHandler<NonPlayerCollisionEventArgs> OnNonPlayerCollision = delegate { };
        //As above, but specifically when a projectile collides with something
        public event EventHandler<NonPlayerCollisionEventArgs> OnProjectileNonPlayerCollision = delegate { };

        private int prevScore = 0;
        private int score = 0;

        public void ResetScore() {
            prevScore = 0;
            score = 0;
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

        public void LaunchProjectile(ProjectileType type, Vector2 launchPosition, Point target,bool isEnemy)
        {
            OnLaunchProjectile?.Invoke(this, new ProjectileLaunchEventArgs(type, launchPosition, target,isEnemy));
        }

        public void KilledProjectile(Projectile p) {
            OnProjectileKilled?.Invoke(this, new ProjectileKilledEventArgs(p));
        }

        //Fire player collision events
        public void PlayerCollision(Player player, Object collidedWith, Vector2 collisionDepth,CollisionType collisionType= CollisionType.stay)
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

            //TODO collision exit if necessary
        }

        public void NonPlayerCollision(CollidableObject obj, Object collidedWith, Vector2 collisionDepth, CollisionType collisionType = CollisionType.stay)
        {
            //Always fires if there is a collision happening
            if (collisionType != CollisionType.exit)
            {
                Projectile proj = obj as Projectile;
                if (proj != null)
                {
                    OnProjectileNonPlayerCollision?.Invoke(this, new NonPlayerCollisionEventArgs(obj, collidedWith, collisionDepth, collisionType));
                }
                else {
                    //NOTE currently unused
                    OnNonPlayerCollision?.Invoke(this, new NonPlayerCollisionEventArgs(obj, collidedWith, collisionDepth, collisionType));
                }
            }

            //TODO collision enter/exit if necessary
        }
    }

    class NonPlayerCollisionEventArgs
    {
        public CollidableObject collider;
        public Object colllidedWith;
        public Vector2 collisionDepth;
        public CollisionType collisionType;

        public NonPlayerCollisionEventArgs(CollidableObject collider, object colllidedWith, Vector2 collisionDepth, CollisionType collisionType)
        {
            this.collider = collider;
            this.colllidedWith = colllidedWith;
            this.collisionDepth = collisionDepth;
            this.collisionType = collisionType;
        }
    }

    class ProjectileKilledEventArgs {
        public Projectile projectile;

        public ProjectileKilledEventArgs(Projectile projectile)
        {
            this.projectile = projectile;
        }
    }

    class ProjectileLaunchEventArgs {
        public ProjectileType projectileType;
        public Vector2 launchPosition;
        public Point worldPointTarget;
        public bool isEnemy;

        public ProjectileLaunchEventArgs(ProjectileType projectileType, Vector2 launchPosition, Point worldPointTarget, bool isEnemy)
        {
            this.projectileType = projectileType;
            this.launchPosition = launchPosition;
            this.worldPointTarget = worldPointTarget;
            this.isEnemy = isEnemy;
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
