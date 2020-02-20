using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ChaseCameraSample
{
    public class IdleState : State
    {
        private const double directionChangeTime = 0.5;
        private double curTime = 0.0;

        public IdleState()
        {
            Name = "Idle";
        }

        public override void Enter(object owner)
        {
            EnemyShip ship = owner as EnemyShip;
            if (ship != null) ship.VelocityScalar = 5000.0f;
            curTime = 0.0;
        }

        public override void Exit(object owner)
        {
            EnemyShip ship = owner as EnemyShip;
            if (ship != null) ship.VelocityScalar = 0.0f;
            curTime = 0.0;
        }

        public override void Execute(object owner, GameTime gameTime)
        {
            EnemyShip ship = owner as EnemyShip;
            if (ship == null) return;
            if (curTime >= directionChangeTime)
            {
                curTime = 0.0;
                ship.SetRandomDirection();
            }
            else
            {
                curTime += gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
    }
}
