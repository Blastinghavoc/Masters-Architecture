using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChaseCameraSample
{
    public class World
    {
        private Ship player;
        private List<EnemyShip> enemies;
        private Ship currentTagger;

        private List<Ship> allShips;

        public Ship Player
        {
            get { return player; }
        }

        public List<EnemyShip> Enemies
        {
            get { return enemies; }
        }

        public List<Ship> AllShips
        {
            get { return allShips; }
        }

        public Ship CurrentTagger
        {
            get { return currentTagger; }
        }

        public World()
        {
            player = null;
            enemies = new List<EnemyShip>();
            allShips = new List<Ship>();
        }

        public void UpdateWorld(GameTime gameTime)
        {
            // Update the ship
            player.Update(gameTime);

            // Update enemies
            foreach (EnemyShip enemy in enemies)
            {
                enemy.Update(gameTime);
            }
        }

        public void SetPlayer(Ship p)
        {
            player = p;
            player.GameWorld = this;

            if (!allShips.Contains(player))
            {
                allShips.Add(player);
            }
        }

        public void AddEnemy(EnemyShip e)
        {
            e.GameWorld = this;
            enemies.Add(e);
            allShips.Add(e);
        }

        public void SetCurrentTagger(Ship t)
        {
            currentTagger = t;
        }
    }
}
