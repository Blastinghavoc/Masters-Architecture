using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.StateMachine
{
    public class Transition
    {
        public readonly State NextState;
        public readonly Func<bool> Condition;

        public Transition(State nextState, Func<bool> condition)
        {
            NextState = nextState;
            Condition = condition;
        }
    }
}
