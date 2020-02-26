using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Animation;
using Coursework.Entities;
using Microsoft.Xna.Framework;

namespace Coursework.Entities
{
    class Enemy : Interactable
    {
        public int Health { get; set; }
        public int Damage { get; protected set; }
        public bool IsAlive { get => Health > 0; }

        public Enemy(Drawable appearance, Vector2 position, int health, int damage) : base(appearance, position)
        {
            Health = health;
            Damage = damage;
        }

        public override void Update(GameTime gameTime)
        {
            //TODO FSM based logic

        }
    }
}
