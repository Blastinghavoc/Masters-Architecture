using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Powerups
{
    /// <summary>
    /// Base class handling the effect of some powerup on the player
    /// </summary>
    class PowerupEffect:EventSubscriber
    {
        protected Entities.Player player;

        public float TimeRemaining { get; protected set; } = 0;
        public float DurationSeconds { get; protected set; } = 0;
        public bool Active { get {return TimeRemaining > 0; } }

        //A colour to apply to the player if they have the effect
        public Color EffectColor { get; protected set; }

        public PowerupEffect()
        {            
        }

        //What to do when the effect is aquired by the player
        public virtual void OnAquiredBy(Entities.Player p)
        {
            player = p;
            BindEvents();
            RefreshDuration();
        }

        //What to do when the effect runs out
        public virtual void OnExpired() {
            UnbindEvents();//Unbind any events the effect was listening to
        }

        //Update time remaining, and anything else necessary
        public virtual void Update(GameTime gameTime) {
            if (Active)
            {
                float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
                TimeRemaining -= dt;
            }
        }

        public void RefreshDuration() {
            TimeRemaining = DurationSeconds;
        }

        //Can be overriden by derived classes
        public virtual void BindEvents()
        { 
        }

        public virtual void UnbindEvents()
        {            
        }

        public void Dispose()
        {
            if (Active)
            {
                OnExpired();
            }
        }

    }
}
