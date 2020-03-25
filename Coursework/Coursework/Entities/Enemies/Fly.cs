using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Graphics;
using Microsoft.Xna.Framework;
using Coursework.StateMachine.AI.Fly;
using Coursework.StateMachine.AI;

namespace Coursework.Entities.Enemies
{
    class Fly : Enemy
    {
        public Fly(Drawable appearance, Decal corpseAppearance, Vector2 position, int health, int damage, bool invincible = false, bool solid = false) : 
            base(appearance, corpseAppearance, position, health, damage, invincible, solid)
        {
        }

        protected override void InitialiseBrain()
        {
            brain = new StateMachine.FSM(this);
            var patrol = new Patrol();
            var dying = new Dying(CorpseAppearance);
            var dead = new Dead();

            patrol.AddTransition(dying, () => { return !IsAlive; });
            dying.AddTransition(dead, () => { return dying.HitGround; });

            brain.AddState(patrol);
            brain.AddState(dying);
            brain.AddState(dead);
            brain.Initialise("Patrol");
        }
    }
}
