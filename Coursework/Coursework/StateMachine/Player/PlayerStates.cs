using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P = Coursework.Entities.Player;

namespace Coursework.StateMachine.Player
{
    //Encapsulate player update functionality common to all states (directional change of appearance)
    abstract class PlayerState : State
    {
        public override void Update(object owner, GameTime gameTime)
        {
            P player = owner as P;
            if (player.Velocity.X < 0)
            {
                player.directionalEffect = SpriteEffects.FlipHorizontally;
            }
            else if (player.Velocity.X > 0)
            {
                player.directionalEffect = SpriteEffects.None;
            }
            //No change if velocity exactly equals 0
        }
    }

    class Idle : PlayerState
    {
        public Action<int> setAnimation;

        public Idle(Action<int> setAnimation)
        {
            this.setAnimation = setAnimation;
            Name = "Idle";
        }

        public override void OnEnter(object owner)
        {
            setAnimation(0);
        }

        public override void OnExit(object owner)
        {            
        }
    }

    class Walking : PlayerState
    {
        public Action<int> setAnimation;

        public Walking(Action<int> setAnimation)
        {
            this.setAnimation = setAnimation;
            Name = "Walking";
        }

        public override void OnEnter(object owner)
        {
            setAnimation(1);
        }

        public override void OnExit(object owner)
        {
        }

    }

    class Jumping : PlayerState
    {
        public Action<int> setAnimation;

        public Jumping(Action<int> setAnimation)
        {
            this.setAnimation = setAnimation;
            Name = "Jumping";
        }

        public override void OnEnter(object owner)
        {
            setAnimation(2);
        }

        public override void OnExit(object owner)
        {
        }

    }

    /// <summary>
    /// State that does nothing.
    /// In future it could display a corpse before ending the game.
    /// </summary>
    class Dead : State
    {
        public Dead()
        {
            Name = "Dead";
        }

        public override void OnEnter(object owner)
        {            
        }

        public override void OnExit(object owner)
        {            
        }

        public override void Update(object owner, GameTime gameTime)
        {            
        }
    }
}
