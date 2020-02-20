using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChaseCameraSample
{
    public class EnemyShip : Ship
    {
        public float VelocityScalar = 0.0f;

        FSM fsm;

        public EnemyShip(GraphicsDevice device)
            : base(device)
        {
            Initialise();
        }

        private void Initialise()
        {
            fsm = new FSM(this);

            // Create the states
            IdleState idle = new IdleState();
            FleeState flee = new FleeState();
            ChaseState chase = new ChaseState();
            
            // Create the transitions between the states
            idle.AddTransition(new Transition(flee, () => TaggerSeen));
            flee.AddTransition(new Transition(idle, () => !TaggerSeen));
            idle.AddTransition(new Transition(chase, () => isTagged));
            flee.AddTransition(new Transition(chase, () => isTagged));
            chase.AddTransition(new Transition(idle, () => !isTagged));

            // Add the created states to the FSM
            fsm.AddState(idle);
            fsm.AddState(flee);
            fsm.AddState(chase);

            // Set the starting state of the FSM
            fsm.Initialise("Idle");
        }

        public void SetRandomDirection()
        {
            float randX = RandomHelper.RandFloat();
            float randZ = RandomHelper.RandFloat();

            Direction.X += randX * 500.0f;
            Direction.Z += randZ * 500.0f;
            Direction.Y = 0.0f;
            Direction.Normalize();
        }

        public override void Update(GameTime gameTime)
        {
            Sense();
            Think(gameTime);

            Velocity = Direction * VelocityScalar;

            base.Update(gameTime);
        }

        private void Sense()
        {
            List<Ship> enemies = GameWorld.AllShips;
            Ship player = GameWorld.Player;

            taggerSeen = false;//reset
            foreach (Ship enemy in enemies)
            {
                if (enemy != this)
                {
                    if (enemy.IsTagged && Sensor.Intersects(enemy.BoundingSphere))
                    {
                        taggerSeen = true;

                    }

                    //Lost our target
                    //if (target == enemy && !Sensor.Intersects(enemy.BoundingSphere))
                    //{

                    //}

                }
            }
        }

        private void Think(GameTime gameTime)
        {
            fsm.Update(gameTime);
        }
    }
}
