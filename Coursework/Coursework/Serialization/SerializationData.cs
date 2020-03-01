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
