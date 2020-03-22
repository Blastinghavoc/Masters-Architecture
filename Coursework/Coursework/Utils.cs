using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework
{
    /// <summary>
    /// Useful utilities that may be needed by multiple classes.
    /// Similar in use to GameData, but while GameData provides
    /// useful data, this class provides useful functions
    /// </summary>
    public class Utils
    {
        public static Vector2 scaleForTexture(Texture2D t)
        {            
            return new Vector2(Serialization.GameData.Instance.levelConstants.tileSize.X / (float)t.Width);
        }

        public static Vector2 scaleForTexture(float textureWidth)
        {
            return new Vector2(Serialization.GameData.Instance.levelConstants.tileSize.X / textureWidth);
        }
    }
}
