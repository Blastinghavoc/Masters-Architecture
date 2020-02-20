using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ChaseCameraSample
{
    public class ChaseState: State
    {
        public ChaseState()
        {
            Name = "Chase";
        }

        public override void Enter(object owner)
        {
            EnemyShip ship = owner as EnemyShip;
            if (ship != null)
            {
                ship.VelocityScalar = 5000.0f;
                SelectTarget(ship);
            }
        }

        public override void Exit(object owner)
        {
            EnemyShip ship = owner as EnemyShip;
            if (ship != null)
            {
                ship.VelocityScalar = 5000.0f;
                ship.Target = null;
            }
        }

        public override void Execute(object owner, GameTime gameTime)
        {
            EnemyShip ship = owner as EnemyShip;
            if (ship == null) return;

            if (ship.Target == null)
            {
                SelectTarget(ship);
            }

            if (ship.Target != null)
            {
                Vector3 targetPosition = ship.Target.Position;
                ship.Direction = targetPosition - ship.Position;
                ship.Direction.Normalize();
            }
        }

        private void SelectTarget(EnemyShip ship)
        {
            List<Ship> ships = ship.GameWorld.AllShips;

            //int index;// = RandomHelper.Rand() % ships.Count;
            
            //Locate nearest target
            Ship targ = null;
            float distance = float.PositiveInfinity;
            foreach (var item in ships)
            {
                if (item == ship)
                {
                    continue;
                }

                var tmpDist = (item.Position - ship.Position).Length();
                if (tmpDist < distance)
                {
                    distance = tmpDist;
                    targ = item;
                }
            }

            //if (ships[index] == ship)
            //{
            //    index++;
            //    if (index == ships.Count) index = 0;
            //}

            ship.Target = targ;
        }

    }
}
