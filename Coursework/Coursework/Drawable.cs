using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Animation
{
    //A drawable entity that can have its position updated.
    interface Drawable
    {
        void Update(GameTime gameTime);
        void Update(GameTime gameTime, Vector2 position);
        void SetPosition(Vector2 pos);
        void Draw(SpriteBatch spriteBatch, SpriteEffects effect = SpriteEffects.None);
        
        float Rotation { get; set; }
        Vector2 RotationOrigin { get; set; }
        Vector2 PositionOffset { get;}

        Color color { get; set; }
        
        //Display size
        Vector2 Size { get; }

        Drawable Clone();
    }
}
