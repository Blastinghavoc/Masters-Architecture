using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework
{
    public enum TileCollisionMode {
        empty,
        solid,
        lava,//Empty, but deals damage on collision
    }

    /// <summary>
    /// Basic world tile, based on the one from Lab 2
    /// </summary>
    public struct Tile
    {
        public Texture2D texture;
        public TileCollisionMode collisionMode;

        public Tile(Texture2D texture, TileCollisionMode collisionMode) {
            this.texture = texture;
            this.collisionMode = collisionMode;
        }

    }

    /// <summary>
    /// A complete description of a Tile, including where it is positioned in a level.
    /// </summary>
    class TileDescriptor {
        public TileCollisionMode collisionMode;
        public Vector2 worldPosition;
        public Point levelIndices;
        public Rectangle bounds;

        public TileDescriptor(TileCollisionMode collisionMode, Vector2 worldPosition, Point levelIndices, Rectangle bounds)
        {
            this.collisionMode = collisionMode;
            this.worldPosition = worldPosition;
            this.levelIndices = levelIndices;
            this.bounds = bounds;
        }
    }
}
