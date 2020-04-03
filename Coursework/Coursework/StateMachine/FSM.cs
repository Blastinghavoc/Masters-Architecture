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
    public class FSM:IDisposable
    {
        private object owner = null;
        private List<State> states = new List<State>();

        public State CurrentState { get; private set; } = null;

        public FSM(object owner = null)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Initialise the FSM with the state with the given name
        /// </summary>
        /// <param name="startStateName"></param>
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

        /// <summary>
        /// Facilitates shorthand for adding multiple states at once
        /// </summary>
        /// <param name="list"></param>
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

        /// <summary>
        /// Disposes of the current state, if it is disposable.
        /// Note that states are expected to release any
        /// required resources on exit, so only the current
        /// state should ever need disposing.
        /// </summary>
        public void Dispose()
        {
            //Dispose of the current state.
            if (CurrentState != null)
            {
                var disp = CurrentState as IDisposable;
                if (disp != null)
                {
                    disp.Dispose();
                }
            }            
        }
    }
}
