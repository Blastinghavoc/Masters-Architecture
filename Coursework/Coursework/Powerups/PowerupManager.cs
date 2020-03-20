using Coursework.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Coursework.Powerups
{
    public enum powerUpType {
        fireball,
        invincibility,
    }
    /// <summary>
    /// Responsible for managing the lifetime of all powerup effects
    /// on a player.
    /// </summary>
    public class PowerupManager:EventSubscriber {

        private Dictionary<powerUpType, PowerupEffect> powerupEffects = new Dictionary<powerUpType, PowerupEffect>();

        //Subset of powerupEffects that are active
        private Dictionary<powerUpType, PowerupEffect> activeEffects = new Dictionary<powerUpType, PowerupEffect>();

        public PowerupManager()
        {
            FireballEffect fireballEffect = new FireballEffect();
            powerupEffects.Add(powerUpType.fireball, fireballEffect);

            InvincibilityEffect invincibilityEffect = new InvincibilityEffect();
            powerupEffects.Add(powerUpType.invincibility, invincibilityEffect);

            BindEvents();
        }

        public void Update(GameTime gameTime) {
            float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;

            List<powerUpType> removeList = new List<powerUpType>();

            //Update each active powerup
            foreach (var pair in activeEffects)
            {
                var effect = pair.Value;

                effect.Update(gameTime);

                if (!effect.Active)
                {
                    effect.OnExpired();
                    removeList.Add(pair.Key);
                }
            }

            //Remove inactive powerups
            foreach (var item in removeList)
            {
                activeEffects.Remove(item);
            }

        }

        public void AddPowerup(powerUpType type, Player p) {
            PowerupEffect effect;
            if (activeEffects.TryGetValue(type,out effect))
            {
                //If effect already active, refresh it
                effect.RefreshDuration();
            }
            else
            {
                effect = powerupEffects[type];

                effect.OnAquiredBy(p);//Effect has been acquired

                activeEffects.Add(type, effect);
            }
        }

        //Deactivate all active effects
        public void Reset() {
            foreach (var item in activeEffects)
            {
                item.Value.OnExpired();
            }
            activeEffects.Clear();
        }

        //Compute an effect colour based on the colours of all active effects
        public Color GetCumulativeEffectColour() {            
            if (activeEffects.Count == 0)
            {
                return Color.White;
            }

            Vector3 colourAccumulator = Vector3.Zero;
            foreach (var item in activeEffects.Values)
            {
                colourAccumulator += item.EffectColor.ToVector3();
            }

            var max = MathHelper.Max(MathHelper.Max(colourAccumulator.X, colourAccumulator.Y),colourAccumulator.Z);
            Vector3 result = colourAccumulator;
            if (max > 1)
            {
                result /= max;
            }
            return new Color(result);
        }

        //Check if the player has collided with a powerup
        private void OnPlayerCollisionEnter(object sender, PlayerCollisionEventArgs e)
        {
            Powerup powerup = e.colllidedWith as Powerup;
            if (powerup != null)
            {                
                AddPowerup(powerup.powerupType, e.player);//Add the powerup                
            }
        }

        public void BindEvents()
        {
            GameEventManager.Instance.OnPlayerCollisionEnter += OnPlayerCollisionEnter;
        }

        public void UnbindEvents()
        {
            GameEventManager.Instance.OnPlayerCollisionEnter -= OnPlayerCollisionEnter;
        }

        public void Dispose()
        {
            UnbindEvents();

            //Call Dispose on all effects
            foreach (var item in powerupEffects)
            {
                item.Value.Dispose();
            }
        }
    }
}
