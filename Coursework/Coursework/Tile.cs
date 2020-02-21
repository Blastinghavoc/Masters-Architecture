using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework
{
    enum TileCollisionMode {
        empty,
        solid
    }

    /// <summary>
    /// Basic world tile, based on the one from Lab 2
    /// </summary>
    struct Tile
    {
        public Texture2D texture;
        public TileCollisionMode collisionMode;

        public Tile(Texture2D texture, TileCollisionMode collisionMode) {
            this.texture = texture;
            this.collisionMode = collisionMode;
        }

    }
}
