using Coursework.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Coursework.Powerups
{
    /// <summary>
    /// Used to determine what effect to apply to the player
    /// when a powerup is collected.
    /// </summary>
    public enum PowerupType {
        fireball,
        invincibility,
    }
    /// <summary>
    /// Responsible for managing the lifetime of all powerup effects
    /// on a player.
    /// </summary>
    public class PowerupManager:IDisposable {

        private Dictionary<PowerupType, PowerupEffect> powerupEffects = new Dictionary<PowerupType, PowerupEffect>();

        //Subset of powerupEffects that are active
        private Dictionary<PowerupType, PowerupEffect> activeEffects = new Dictionary<PowerupType, PowerupEffect>();

        public PowerupManager()
        {
            FireballEffect fireballEffect = new FireballEffect();
            powerupEffects.Add(PowerupType.fireball, fireballEffect);

            InvincibilityEffect invincibilityEffect = new InvincibilityEffect();
            powerupEffects.Add(PowerupType.invincibility, invincibilityEffect);
        }

        public void Update(GameTime gameTime) {
            float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;

            List<PowerupType> removeList = new List<PowerupType>();

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

        public void AddPowerupEffect(PowerupType type, Player p) {
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

        public void Dispose()
        {
            //Call Dispose on all effects
            foreach (var item in powerupEffects)
            {
                item.Value.Dispose();
            }
        }
    }
}
