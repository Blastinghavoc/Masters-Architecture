using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Coursework.Serialization
{
    /// <summary>
    /// Serialization model for high score data
    /// </summary>
    public class HighScoreData
    {
        //Update the high scores
        public void Update(int newScore) {
            List<int> scoreList = new List<int>(scores);
            scoreList.Add(newScore);
            scores = scoreList.OrderByDescending(i => i).Take(3).ToArray();//Get the top 3 scores
        }

        public void Save() {
            GameData.SaveHighScoreData(this);
        }

        public void Reset() {
            scores = new int[3];
        }

        //The actual data
        [XmlArrayItem("score")]
        public int[] scores = new int[3];
    }
}
