using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Graphics;
using Microsoft.Xna.Framework;
using Coursework.StateMachine.AI.Blocker;

namespace Coursework.Entities.Enemies
{
    /// <summary>
    /// Implementation of the Blocker enemy.
    /// </summary>
    class Blocker : Enemy
    {
        public Blocker(Drawable appearance, Decal corpseAppearance, Vector2 position, int health, int damage, bool invincible = false, bool solid = false) : 
            base(appearance, corpseAppearance, position, health, damage, invincible, solid)
        {
        }

        protected override void InitialiseBrain()
        {
            brain = new StateMachine.FSM(this);
            var start = new Start();
            var idle1 = new Idle();
            var idle2 = new Idle();
            var up = new Up(this);
            var down = new Down(this);

            //Transitions out of start state
            start.AddTransition(down, () => { return start.down; });
            start.AddTransition(up, () => { return start.up; });

            //Behaviour loop: down -> idle -> up -> idle -> down
            idle1.AddTransition(down, () => { return idle1.DurationOver; });
            down.AddTransition(idle2, () => { return down.Done; });
            idle2.AddTransition(up, () => { return idle2.DurationOver; });
            up.AddTransition(idle1, () => { return up.Done; });

            brain.AddStates(start,idle1, idle2, up, down);
            brain.Initialise("Start");
        }
    }
}
