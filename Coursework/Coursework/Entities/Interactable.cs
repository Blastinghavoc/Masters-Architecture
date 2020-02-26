using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Entities;
using Coursework.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Coursework.Entities
{
    //A collidable object that can interact with the player
    class Interactable: CollidableObject
    {
        private Drawable appearance;
        private Action<Player,Interactable> onCollected;
        public override Vector2 Position { get => base.Position; protected set { base.Position = value; appearance.SetPosition(value); } }

        public Interactable(Drawable appearance,Vector2 position, Action<Player, Interactable> onCollected)
        {
            this.appearance = appearance;
            this.onCollected = onCollected;
            Position = position;
        }

        //If an NPC collides with the player, run its action
        public override void OnCollision(CollidableObject obj, Vector2 penetrationDepth)
        {
            Player player = obj as Player;
            if (player != null)
            {
                onCollected(player,this);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteEffects effect = SpriteEffects.None)
        {
            appearance.Draw(spriteBatch, effect);
        }
    }
}
