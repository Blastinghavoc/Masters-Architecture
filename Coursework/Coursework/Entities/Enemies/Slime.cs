using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Graphics;
using Microsoft.Xna.Framework;
using Coursework.StateMachine.AI.Slime;
using Coursework.StateMachine.AI;

namespace Coursework.Entities.Enemies
{
    /// <summary>
    /// Implementation of the slime enemy
    /// </summary>
    class Slime : Enemy
    {
        public Slime(Drawable appearance, Decal corpseAppearance, Vector2 position, int health, int damage, bool invincible = false, bool solid = false) : 
            base(appearance, corpseAppearance, position, health, damage, invincible, solid)
        {
        }

        protected override void InitialiseBrain()
        {
            brain = new StateMachine.FSM(this);
            var patrol = new Patrol();
            var dead = new Dead();
            patrol.AddTransition(dead, () => { return !IsAlive; });

            brain.AddState(patrol);
            brain.AddState(dead);
            brain.Initialise("Patrol");
        }
    }
}
