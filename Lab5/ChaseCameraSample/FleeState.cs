using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ChaseCameraSample
{
    public class FleeState : State
    {
        public FleeState()
        {
            Name = "Flee";
        }

        public override void Enter(object owner)
        {
            EnemyShip ship = owner as EnemyShip;
            if (ship != null) ship.VelocityScalar = 8000.0f;
        }

        public override void Exit(object owner)
        {
            EnemyShip ship = owner as EnemyShip;
            if (ship != null) ship.VelocityScalar = 0.0f;
        }

        public override void Execute(object owner, GameTime gameTime)
        {
            EnemyShip ship = owner as EnemyShip;
            if (ship == null) return;
            if (ship.GameWorld.CurrentTagger == null) return;
            Vector3 taggerPosition = ship.GameWorld.CurrentTagger.Position;
            ship.Direction = ship.Position - taggerPosition;
            ship.Direction.Normalize();
        }
    }
}
