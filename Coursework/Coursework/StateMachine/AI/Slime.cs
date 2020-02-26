using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Entities;
using Microsoft.Xna.Framework;

/// <summary>
/// Collection of states for Slime ai
/// </summary>
namespace Coursework.StateMachine.AI.Slime
{
    class Patrol : State
    {
        public float speed = -16;

        public Patrol()
        {
            Name = "Patrol";
        }

        public override void OnEnter(object owner)
        {

        }

        public override void OnExit(object owner)
        {

        }

        public override void Update(object owner, GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Enemy enemy = owner as Enemy;
            if (enemy != null)
            {
                var currentLevel = Level.CurrentLevel;

                Point aheadTile;
                Point aheadBelowTile;

                if (speed < 0)
                {
                    enemy.DirectionalEffect = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
                    aheadTile = currentLevel.GetTileIndices(new Point(enemy.BoundingBox.Left, enemy.BoundingBox.Top));
                }
                else
                {
                    enemy.DirectionalEffect = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
                    aheadTile = currentLevel.GetTileIndices(new Point(enemy.BoundingBox.Right, enemy.BoundingBox.Top));                    
                }

                aheadBelowTile = aheadTile;
                aheadBelowTile.Y += 1;

                if (currentLevel.GetCollisionModeAt(aheadTile) == TileCollisionMode.solid)
                {
                    speed = -1 * speed;//Reverse direction if going to hit a wall
                }
                else if (currentLevel.GetCollisionModeAt(aheadBelowTile) != TileCollisionMode.solid)
                {
                    speed = -1 * speed;//Reverse direction if going to walk off a ledge
                }

                //Update position
                enemy.SetPosition(enemy.Position + new Vector2(speed*dt,0));
            }
        }
    }

    
}
