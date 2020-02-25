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
        //TODO collision detection between non-static entities
        public void Update(Level currentLevel,Player player)
        {

            UpdatePlayerLevelCollisions(currentLevel, player);
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
                            player.OnStaticCollision(penDepth);
                        }
                    }
                }
            }
        }

    }
}
