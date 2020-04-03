using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Entities;
using Coursework.Levels;
using Coursework.Projectiles;
using Microsoft.Xna.Framework;

namespace Coursework
{
    /// <summary>
    /// Class to manage all collisions occuring in the game.
    /// Keeps track of collisions that happend in the previous frame too, to detect
    /// whether a collision this frame is new or continuing.
    /// </summary>
    class CollisionManager
    {
        private Dictionary<CollidableObject, HashSet<CollidableObject>> collisionsLastFrame = new Dictionary<CollidableObject, HashSet<CollidableObject>>();
        private Dictionary<CollidableObject, HashSet<CollidableObject>> collisionsThisFrame = new Dictionary<CollidableObject, HashSet<CollidableObject>>();


        public void Update(Level currentLevel,Player player,ProjectileManager projectileManager)
        {
            //Save last frame's collisions
            collisionsLastFrame = new Dictionary<CollidableObject, HashSet<CollidableObject>>(collisionsThisFrame);
            collisionsThisFrame.Clear();

            //Update player-level collisions
            UpdateObjectLevelCollisions(currentLevel, player);

            List<CollidableObject> enemyProjectiles = new List<CollidableObject>();
            List<CollidableObject> allyProjectiles = new List<CollidableObject>();

            //Update projectile-level collisions
            foreach (var item in projectileManager.ActiveProjectiles)
            {
                UpdateObjectLevelCollisions(currentLevel, item);
                if (item.IsEnemy)
                {
                    enemyProjectiles.Add(item);
                }
                else
                {
                    allyProjectiles.Add(item);
                }
            }

            //Get a combined list of all entities that can collide with the player, including projectiles
            List<CollidableObject> combinedEntities = new List<CollidableObject>(currentLevel.LevelEntities);
            combinedEntities.AddRange(enemyProjectiles);

            //Update player collisions with all entities (level managed ones, and projectiles)
            UpdateObjectEntityCollisions(combinedEntities, player);

            //Get list of just enemies
            List<CollidableObject> enemies = new List<CollidableObject>();
            foreach (var item in currentLevel.LevelEntities)
            {
                if (item is Entities.Enemies.Enemy)
                {
                    enemies.Add(item);
                }
            }

            //Update allied projectile collisions with all enemies (projectiles do not collide with non-enemy interactables)
            foreach (var proj in allyProjectiles)
            {
                UpdateObjectEntityCollisions(enemies, proj);
            }
        }

        /// <summary>
        /// Updates the collisions between a list of entities and a particular object
        /// by brute force iteration over all the entities.
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="obj"></param>
        private void UpdateObjectEntityCollisions(List<CollidableObject> entities, CollidableObject obj)
        {
            foreach (var item in entities)
            {
                Vector2 penDepth;
                if (obj.CheckCollision(item, out penDepth))
                {
                    RecordCollision(obj,item);

                    CollisionType type = CollisionType.stay;

                    if (!wasCollidingWith(obj,item))//Weren't colliding before-> collision enter
                    {
                        type = CollisionType.enter;
                    }

                    Player player = obj as Player;
                    if (player != null)
                    {
                        GameEventManager.Instance.PlayerCollision(player, item,penDepth,type);
                    }
                    else
                    {
                        GameEventManager.Instance.NonPlayerCollision(obj, item, penDepth);
                    }
                }
            }
        }

        /// <summary>
        /// Deal with collisions between an object and a level (based on Lab 2)
        /// </summary>
        /// <param name="currentLevel"></param>
        /// <param name="obj"></param>
        private void UpdateObjectLevelCollisions(Level currentLevel, CollidableObject obj)
        {
            Vector2 tileDimensions = new Vector2(currentLevel.tileSize.X,currentLevel.tileSize.Y);
            Rectangle playerBounds = obj.BoundingBox;

            //Determination of neighbouring tile indices from platformer demo (lab 2)
            int left = (int)Math.Floor(playerBounds.Left / tileDimensions.X);            
            int right = (int)Math.Ceiling((playerBounds.Right / tileDimensions.X));
            int top = (int)Math.Floor(playerBounds.Top / tileDimensions.Y);
            int bottom = (int)Math.Ceiling((playerBounds.Bottom / tileDimensions.Y));

            //For each neighbouring tile, check whether it collides with the object
            for (int i = left; i < right; i++)
            {
                for (int j = top; j < bottom; j++)
                {
                    var collisionMode = currentLevel.GetCollisionModeAt(i,j);
                    //Skip collisions with non-colliding tiles
                    if (TileCollisionMode.empty.Equals(collisionMode))
                    {
                        continue;
                    }

                    var tileBounds = currentLevel.GetBoundsAt(i, j);

                    Vector2 penDepth;
                    if (obj.CheckCollision(tileBounds,out penDepth))
                    {                        
                        //If a collision occured, fire relevant event

                        TileDescriptor tileDescriptor = new TileDescriptor(collisionMode, currentLevel.GetWorldPosition(i, j), new Point(i, j),tileBounds);

                        Player player = obj as Player;
                        if (player != null)
                        {
                            GameEventManager.Instance.PlayerCollision(player, tileDescriptor, penDepth);
                        }
                        else {
                            GameEventManager.Instance.NonPlayerCollision(obj, tileDescriptor, penDepth);
                        }
                        
                    }
                }
            }
        }

        //Record a collision between object 1 and object 2 (one way only)
        private void RecordCollision(CollidableObject o1, CollidableObject o2)
        {           
            if (!collisionsThisFrame.ContainsKey(o1))
            {
                collisionsThisFrame.Add(o1, new HashSet<CollidableObject>());
            }
            collisionsThisFrame[o1].Add(o2);
        }

        //Check whether object 1 and 2 were colliding last frame
        private bool wasCollidingWith(CollidableObject o1, CollidableObject o2)
        {
            HashSet<CollidableObject> set;
            if (collisionsLastFrame.TryGetValue(o1,out set))
            {
                return set.Contains(o2);
            }
            return false;
        }

    }

    /// <summary>
    /// Possible types of collision event.
    /// </summary>
    public enum CollisionType
    {
        enter,
        exit,
        stay
    }
}
