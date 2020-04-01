using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Coursework.Serialization
{
    
    /// <summary>
    /// Class handling the serialization and deserialization of data,
    /// and providing a common interface to it.
    /// Based on lab 2
    /// </summary>
    public class GameData
    {
        public static GameData Instance { get; private set; } = null;

        [XmlIgnoreAttribute]
        public Dictionary<string, LevelEntitySpecification> entitySpecificationMap = new Dictionary<string, LevelEntitySpecification>();

        public static void Initialise() {
            if (Instance != null)
            {
                throw new InvalidOperationException("Cannot initialise GameData, it is already initialised");
            }

            StreamReader reader = new StreamReader("Content/XML/Config.xml");
            try
            {
                Instance = (GameData)new XmlSerializer(typeof(GameData)).Deserialize(reader.BaseStream);
            }
            catch (Exception e)
            {
                //For debugging purposes only
                Console.WriteLine("ERROR: XML File could not be deserialized!");
                Console.WriteLine("Exception Message: " + e.Message);
            }
            finally
            {
                reader.Close();
            }

            foreach (var item in Instance.levelConstants.entitySpecifications)
            {
                Instance.entitySpecificationMap.Add(item.entityName, item);
            }
        }

        public static LevelData GetLevelData(string levelName)
        {
            LevelData data = null;
            StreamReader reader = new StreamReader("Content/XML/"+levelName+".xml");
            try
            {
                data = (LevelData)new XmlSerializer(typeof(LevelData)).Deserialize(reader.BaseStream);
                reader.Close();
            }
            catch (Exception e)
            {
                //For debugging purposes only
                Console.WriteLine("ERROR: XML File could not be deserialized!");
                Console.WriteLine("Exception Message: " + e.Message);
            }
            finally
            {
                reader.Close();
            }

            return data;
        }

        public static HighScoreData GetHighScoreData() {
            HighScoreData data = null;
            StreamReader reader = new StreamReader("Content/XML/HighScores.xml");
            try
            {                
                data = (HighScoreData)new XmlSerializer(typeof(HighScoreData)).Deserialize(reader.BaseStream);                
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: XML File could not be deserialized!");
                Console.WriteLine("Exception Message: " + e.Message);
            }
            finally {
                reader.Close();
            }

            return data;
        }

        public static void SaveHighScoreData(HighScoreData data) {
            XmlSerializer ser = new XmlSerializer(typeof(HighScoreData));
            TextWriter writer = new StreamWriter("Content/XML/HighScores.xml");
            ser.Serialize(writer, data);
            writer.Close();
        }

        public PlayerData playerData;

        public ConstantLevelData levelConstants;

        public ProjectileData projectileData;

    }

    
}
