using Coursework.Entities;
using Coursework.Entities.Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.StateMachine.AI
{
    /// <summary>
    /// Factory class responsible for creating FSM "brains" for
    /// enemies of different types.
    /// Keeps AI code separate from the rest of the Enemy class
    /// </summary>
    public class BrainFactory
    {
        public static FSM GetBrainFor(Enemy enemy) {
            var brain = new FSM(enemy);

            switch (enemy)
            {
                case Entities.Enemies.Slime s:
                    {
                        State patrol = new Slime.Patrol();
                        State dead = new Dead();
                        patrol.AddTransition(dead, () => { return !enemy.IsAlive; });

                        brain.AddState(patrol);
                        brain.AddState(dead);
                        brain.Initialise("Patrol");
                    }
                    break;
                case Entities.Enemies.Fly f:
                    {
                        var patrol = new Fly.Patrol();
                        var dying = new Fly.Dying(enemy.CorpseAppearance);
                        var dead = new Dead();

                        patrol.AddTransition(dying, () => { return !enemy.IsAlive; });
                        dying.AddTransition(dead, () => { return dying.HitGround; });

                        brain.AddState(patrol);
                        brain.AddState(dying);
                        brain.AddState(dead);
                        brain.Initialise("Patrol");
                    }
                    break;
                case Entities.Enemies.Blocker b:
                    {
                        var idle1 = new Blocker.Idle();
                        var idle2 = new Blocker.Idle();
                        var up = new Blocker.Up(enemy);
                        var down = new Blocker.Down(enemy);

                        //Behaviour loop: down -> idle -> up -> idle -> down
                        idle1.AddTransition(down, () => { return idle1.DurationOver; });
                        down.AddTransition(idle2, () => { return down.Done; });
                        idle2.AddTransition(up, () => { return idle2.DurationOver; });
                        up.AddTransition(idle1, () => { return up.Done; });

                        brain.AddStates(idle1, idle2, up, down);
                        brain.Initialise("Idle");
                    }
                    break;
                default:
                    break;
            }

            return brain;
        }
    }
}
