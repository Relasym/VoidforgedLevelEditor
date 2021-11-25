using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace LevelEditor
{
    //a collection of Level objects with some metadata and a JSON export function
    [FirestoreData]

    public class Game
    {
        [FirestoreProperty]

        public List<Level> Levels { get; set; }
        [FirestoreProperty]

        public string Name { get; set; }
        [FirestoreProperty]

        public DateTime LastModification { get; set; }

        public Game()
        {
            Levels = new List<Level>();
            LastModification = DateTime.Now;
        }
        public void AddLevel(Level level)
        {
            Levels.Add(level);
        }
        public Level RemoveLevel(Level level)
        {
            Levels.Remove(level);
            return level;
        }
        public void ClearLevels()
        {
            Levels.Clear();
        }

        public List<Level> GetLevels()
        {
            return Levels.ToList();
        }

        public string ToJson()
        {
            LastModification = DateTime.Now;
            if(Name==null)
            {
                Name = "Default Name";
            }
            return JsonSerializer.Serialize(this);
        }

        public static Game FromJson(string json)
        {
        
            return JsonSerializer.Deserialize<Game>(json);
        }

        public Boolean Verify()
        {
            //TODO implement Game Verification
            return false;
        }
    }
}
