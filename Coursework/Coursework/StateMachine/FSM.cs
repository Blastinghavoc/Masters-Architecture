using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.StateMachine
{
    /// <summary>
    /// This class and the State and Transition classes are based on the lab 5 tutorial
    /// </summary>
    class FSM
    {
        private object owner = null;
        private List<State> states = new List<State>();

        public State CurrentState { get; private set; } = null;

        public FSM(object owner = null)
        {
            this.owner = owner;
        }

        public void Initialise(string startStateName)
        {
            CurrentState = states.Find(state => state.Name.Equals(startStateName));
            if (CurrentState != null)
            {
                CurrentState.OnEnter(owner);
            }
            else
            {
                throw new ArgumentException("The start state name provided does not match any state in the FSM");
            }
        }

        public void AddState(State state)
        {
            states.Add(state);
        }

        public void AddStates(params State[] list)
        {
            foreach (var state in list)
            {
                states.Add(state);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (CurrentState == null)
            {
                return;
            }

            //Check transitions
            foreach (var transition in CurrentState.Transitions)
            {
                if (transition.Condition())
                {
                    CurrentState.OnExit(owner);
                    CurrentState = transition.NextState;
                    CurrentState.OnEnter(owner);
                    break;
                }
            }

            //Update current state
            CurrentState.Update(owner, gameTime);
        }

    }
}
