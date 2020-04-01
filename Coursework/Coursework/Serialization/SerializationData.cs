using Coursework.Entities;
using Coursework.Levels;
using Coursework.Powerups;
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
        public string idlePath;
        public string jumpPath;
        public string walkAnimationPath;
        public AnimationData walkAnimation;

        public Vector2 inputScale;
        public float maxSpeed;
        public Vector2 dragFactor;
        public float gravity;
        public int maxJumps;

        public int startHealth;
        public float damageImmunityDuration;
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
        public int coinValue;
        [XmlArrayItem("spec")]
        public LevelEntitySpecification[] entitySpecifications;
    }

    //Data specific to a single level
    public class LevelData
    {
        public string mapName;
        public string nextLevelName;
        public string backgroundPath;
        [XmlArrayItem("bind")]
        public ColorBinding[] bindings;
    }

    public class ColorBinding
    {
        public Color color;
        public string entityName;
    }    

    public class LevelEntitySpecification
    {
        public string entityName;
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

    /// <summary>
    /// Used to simplify serialization of 
    /// simple interactables (those with no extra data)
    /// </summary>
    public enum InteractableType
    {
        coin,
        nextLevel,
        powerup,
        extraLife,
    }

    [XmlInclude(typeof(PowerupData))]
    public class InteractableData : LevelEntityData
    {
        public string textureName;
        public InteractableType interactableType;
    }

    public class PowerupData : InteractableData
    {
        public PowerupType powerupType;
    }

    /// <summary>
    /// Used to simplify serialization of enemies, since they
    /// currently all have the same data.
    /// </summary>
    public enum EnemyType
    {
        slime,
        fly,
        blocker
    }

    public class EnemyData : LevelEntityData
    {
        public EnemyType enemyType;
        public int health;
        public int damage;
        public bool invincible;
        public bool solid;
        public AnimationData animationData;
        public string corpseTextureName;
    }

    public class AnimationData
    {
        public string baseName;
        public Point frameDimensions;
        public int numFrames;
        public int frameDuration;
    }
}
