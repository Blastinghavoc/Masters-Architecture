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

        private State currentState = null;

        public FSM(object owner = null)
        {
            this.owner = owner;
        }

        public void Initialise(string startStateName)
        {
            currentState = states.Find(state => state.Name.Equals(startStateName));
            if (currentState != null)
            {
                currentState.OnEnter(owner);
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

        public void Update(GameTime gameTime)
        {
            if (currentState == null)
            {
                return;
            }

            //Check transitions
            foreach (var transition in currentState.Transitions)
            {
                if (transition.Condition())
                {
                    currentState.OnExit(owner);
                    currentState = transition.NextState;
                    currentState.OnEnter(owner);
                    break;
                }
            }

            //Update current state
            currentState.Update(owner, gameTime);
        }
    }
}
