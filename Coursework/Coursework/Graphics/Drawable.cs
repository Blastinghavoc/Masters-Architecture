using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Graphics
{
    /// <summary>
    /// Interface for anything that can be drawn at a particular position,
    /// and may have some properties that change over time.
    /// Used so that game objects don't need to care how their appearance is implemented.
    /// </summary>
    public interface Drawable
    {
        void Update(GameTime gameTime);
        void Update(GameTime gameTime, Vector2 position);
        void SetPosition(Vector2 pos);
        void Draw(SpriteBatch spriteBatch, SpriteEffects effect = SpriteEffects.None);
        
        float Rotation { get; set; }//Rotation in degrees
        Vector2 RotationOrigin { get; set; }
        Vector2 PositionOffset { get;}

        Color color { get; set; }
        
        //Display size in world units
        Vector2 Size { get; }

        //Determines rendering order
        float LayerDepth { get; set; }

        Drawable Clone();
    }
}
