using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Entities;
using Microsoft.Xna.Framework;

namespace Coursework
{
    class CollisionManager
    {
        private Dictionary<CollidableObject, HashSet<CollidableObject>> collisionsLastFrame = new Dictionary<CollidableObject, HashSet<CollidableObject>>();
        private Dictionary<CollidableObject, HashSet<CollidableObject>> collisionsThisFrame = new Dictionary<CollidableObject, HashSet<CollidableObject>>();


        public void Update(Level currentLevel,Player player)
        {
            //Save last frame's collisions
            collisionsLastFrame = new Dictionary<CollidableObject, HashSet<CollidableObject>>(collisionsThisFrame);
            collisionsThisFrame.Clear();

            UpdatePlayerLevelCollisions(currentLevel, player);
            UpdatePlayerInteractableCollisions(currentLevel, player);
        }

        private void UpdatePlayerInteractableCollisions(Level currentLevel, Player player)
        {
            foreach (var item in currentLevel.Interactables)
            {
                Vector2 penDepth;
                if (player.CheckCollision(item, out penDepth))
                {
                    RecordCollision(player,item);

                    CollisionType type = CollisionType.stay;

                    if (!wasCollidingWith(player,item))//Weren't colliding before-> collision enter
                    {
                        type = CollisionType.enter;
                    }

                    GameEventManager.Instance.OnPlayerCollision(player,item,penDepth,type);
                }
            }
        }

        //Deal with collisions between player and level
        private void UpdatePlayerLevelCollisions(Level currentLevel, Player player)
        {
            Vector2 tileDimensions = new Vector2(currentLevel.tileSize.X,currentLevel.tileSize.Y);
            Rectangle playerBounds = player.BoundingBox;

            //Determination of neighbouring tile indices from platformer demo (lab 2)
            int left = (int)Math.Floor(playerBounds.Left / tileDimensions.X);            
            int right = (int)Math.Ceiling((playerBounds.Right / tileDimensions.X));
            int top = (int)Math.Floor(playerBounds.Top / tileDimensions.Y);
            int bottom = (int)Math.Ceiling((playerBounds.Bottom / tileDimensions.Y));

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
                    if (player.CheckCollision(tileBounds,out penDepth))
                    {
                        if (TileCollisionMode.solid.Equals(collisionMode))
                        {
                            GameEventManager.Instance.OnPlayerCollision(player, currentLevel,penDepth);                            
                        }
                    }
                }
            }
        }

        //Record a collision between object 1 and object 2 (one way only at the moment)
        private void RecordCollision(CollidableObject o1, CollidableObject o2)
        {           
            if (!collisionsThisFrame.ContainsKey(o1))
            {
                collisionsThisFrame.Add(o1, new HashSet<CollidableObject>());
            }
            collisionsThisFrame[o1].Add(o2);
        }

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

    enum CollisionType
    {
        enter,
        exit,
        stay
    }
}
