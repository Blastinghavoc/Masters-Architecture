﻿using Coursework.Entities;
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

            switch (enemy.enemyType)
            {
                case EnemyType.slime:
                    {
                        State patrol = new Slime.Patrol();
                        State dead = new Dead();
                        patrol.AddTransition(dead, () => { return !enemy.IsAlive; });

                        brain.AddState(patrol);
                        brain.AddState(dead);
                        brain.Initialise("Patrol");
                    }
                    break;
                case EnemyType.fly:
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
                default:
                    break;
            }

            return brain;
        }
    }
}
