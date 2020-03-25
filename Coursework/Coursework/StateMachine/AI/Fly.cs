using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Graphics;
using Coursework.Entities;
using Microsoft.Xna.Framework;
using Coursework.Entities.Enemies;

namespace Coursework.StateMachine.AI.Fly
{
    class Patrol : State
    {
        public float speed = -32;

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

        //Similar to slime update, but without checks for ground beneath it
        public override void Update(object owner, GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Enemy enemy = owner as Enemy;
            if (enemy != null)
            {
                var currentLevel = Level.CurrentLevel;

                Point aheadTile;

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


                if (currentLevel.GetCollisionModeAt(aheadTile) == TileCollisionMode.solid)
                {
                    speed = -1 * speed;//Reverse direction if going to hit a wall
                }

                //Update position
                enemy.SetPosition(enemy.Position + new Vector2(speed * dt, 0));
            }
        }
    }

    class Dying : State
    {
        public Decal DyingAppearance { get;protected set; }
        public bool HitGround = false;
        public float FallSpeed = 48;

        public Dying(Decal dyingAppearance)
        {
            Name = "Dying";
            this.DyingAppearance = dyingAppearance;
        }

        public override void OnEnter(object owner)
        {
            var enemy = owner as Enemy;
            enemy.Damage = 0;//Can't deal damage while dying
            //Replace appearance with dying appearance
            DyingAppearance.Effect = enemy.DirectionalEffect;
            enemy.SetAppearance(DyingAppearance);
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

                Point belowTile;

                belowTile = currentLevel.GetTileIndices(enemy.BoundingBox.GetBottomCenter());
                
                if (currentLevel.GetCollisionModeAt(belowTile) != TileCollisionMode.solid)
                {
                    //Update position
                    enemy.SetPosition(enemy.Position + new Vector2(0, FallSpeed * dt));
                }
                else {
                    HitGround = true;
                }

            }
        }
    }
    
}
