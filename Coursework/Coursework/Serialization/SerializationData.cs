using Coursework.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

/// <summary>
/// A collection of serialization models
/// </summary>
namespace Coursework.Serialization
{
    //Data used by the player
    public class PlayerData
    {
        public string walkAnimationPath;
        public string idlePath;
        public string jumpPath;
        public Point frameDimensions;
        public int numWalkFrames;
        public int walkFrameTime;

        public Vector2 inputScale;
        public Vector2 maxSpeed;
        public Vector2 dragFactor;
        public float gravity;
        public int maxJumps;

        public int startHealth;
    }

    //Data used by all levels
    public class ConstantLevelData
    {
        public Point tileSize;
        public float tileResolution;
        public string tileFilePath;
        public string itemFilePath;
        public string enemyFilePath;
        public string mapFilePath;
        public string startLevelName;
        [XmlArrayItem("spec")]
        public LevelEntitySpecification[] entitySpecifications;
    }

    //Data specific to a level
    public class LevelData
    {
        public string mapName;
        public string nextLevelName;
        [XmlArrayItem("bind")]
        public ColorBinding[] bindings;
    }

    public class ColorBinding
    {
        public Color color;
        public string entityName;
    }

    public enum LevelEntityType
    {
        tile,
        interactable,
        enemy
    }

    public class LevelEntitySpecification
    {
        public string entityName;
        public LevelEntityType entityType;
        public LevelEntityData entityData;
    }

    [XmlInclude(typeof(TileData))]
    [XmlInclude(typeof(InteractableData))]
    [XmlInclude(typeof(EnemyData))]
    public class LevelEntityData
    {
    }

    public class TileData : LevelEntityData
    {
        public string textureName;
        public TileCollisionMode collisionMode;
    }

    public class InteractableData : LevelEntityData
    {
        public string textureName;
        public InteractableType interactableType;
    }

    public class EnemyData : LevelEntityData
    {
        public EnemyType enemyType;
        public int health;
        public int damage;
        public AnimationData animationData;
        public AppearanceData corpseData;
    }

    public class AppearanceData
    {        
        public string textureName;
    }

    public class AnimationData
    {
        public string baseName;
        public Point frameDimensions;
        public int numFrames;
        public int frameDuration;
    }
}
