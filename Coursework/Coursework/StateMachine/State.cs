using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.StateMachine
{
    abstract class State
    {
        public abstract void OnEnter(object owner);
        public abstract void OnExit(object owner);
        public abstract void Update(object owner,GameTime gameTime);

        public string Name { get; set; } = "";

        public List<Transition> Transitions { get; } = new List<Transition>();

        public void AddTransition(Transition trans)
        {
            Transitions.Add(trans);
        }

        public void AddTransition(State nextState, Func<bool> condition)
        {
            Transitions.Add(new Transition(nextState, condition));
        }
    }
}
