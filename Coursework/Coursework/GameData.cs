using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Coursework.Serialization;

namespace Coursework
{
    //Based on lab 2
    public class GameData
    {
        public static GameData Instance { get {
                return instance;
            }
        }
        private static GameData instance = null;

        [XmlIgnoreAttribute]
        public Dictionary<string, LevelEntitySpecification> entitySpecificationMap = new Dictionary<string, LevelEntitySpecification>();

        public static void Initialise() {
            try
            {
                StreamReader reader = new StreamReader("Content/XML/Config.xml");
                instance = (GameData)new XmlSerializer(typeof(GameData)).Deserialize(reader.BaseStream);
                //new Type[] { typeof(TileData),typeof(InteractableData),typeof(EnemyData) }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: XML File could not be deserialized!");
                Console.WriteLine("Exception Message: " + e.Message);
            }

            foreach (var item in instance.levelConstants.entitySpecifications)
            {
                instance.entitySpecificationMap.Add(item.entityName, item);
            }

            //XmlSerializer ser = new XmlSerializer(typeof(GameData));
            //LevelEntitySpecification spec = new LevelEntitySpecification();
            //spec.entityName = "tst";
            //spec.entityType = LevelEntityType.enemy;
            //var enemyData = new EnemyData();
            //enemyData.health = 1;
            //enemyData.damage = 1;
            //enemyData.enemyType = Entities.EnemyType.slime;
            //enemyData.animationData = new AnimationData();
            //enemyData.animationData.baseName = "testName";
            //enemyData.animationData.frameDimensions = new Point(10, 10);
            //enemyData.animationData.frameDuration = 100;
            //enemyData.animationData.numFrames = 2;
            //enemyData.corpseData = new AppearanceData();
            //enemyData.corpseData.textureName = "testTexture";
            //spec.entityData = enemyData;

            //TextWriter writer = new StreamWriter("Content/XML/TestOutput.xml");
            //instance.levelConstants.entitySpecifications = new LevelEntitySpecification[1] { spec };
            //ser.Serialize(writer, instance);
            //writer.Close();


            //StreamReader reader1 = new StreamReader("Content/XML/TestOutput.xml");
            //instance = (GameData)new XmlSerializer(typeof(GameData)).Deserialize(reader1.BaseStream);
        }

        public static LevelData GetLevelData(string levelName)
        {
            LevelData data = null;
            try
            {
                StreamReader reader = new StreamReader("Content/XML/"+levelName+".xml");
                data = (LevelData)new XmlSerializer(typeof(LevelData)).Deserialize(reader.BaseStream);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: XML File could not be deserialized!");
                Console.WriteLine("Exception Message: " + e.Message);
            }
            return data;
        }

        public PlayerData playerData;

        public ConstantLevelData levelConstants;
    }

    public class PlayerData {
        public string walkAnimationPath;
        public string idlePath;
        public string jumpPath;
        public Point frameDimensions;
        public int numWalkFrames;
        public int walkFrameTime;
    }

    //Data used by all levels
    public class ConstantLevelData {
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
    public class LevelData {
        public string mapName;
        public string nextLevelName;
        [XmlArrayItem("bind")]
        public ColorBinding[] bindings;
    }

    public class ColorBinding {
        public Color color;
        public string entityName;
    }
}
