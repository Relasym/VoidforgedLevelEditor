using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor
{
    public class Level
    {
        public List<GameObject> Objects { get; set; }
        public string Name { get; set; }
        public DateTime LastModification { get; set; }
        


        public Level(string name)
        {
            Name = name;
            Objects = new List<GameObject>();
            LastModification = DateTime.Now;

        }
        public void AddObject(GameObject gameobject)
        {
            Objects.Add(gameobject);
            LastModification = DateTime.Now;
        }
        public GameObject RemoveObject(GameObject gameobject)
        {
            Objects.Remove(gameobject);
            LastModification = DateTime.Now;
            return gameobject;
        }
        public void ClearGameObjects()
        {
            Objects.Clear();
            LastModification = DateTime.Now;
        }
        public List<GameObject> GetGameObjects()
        {
            return Objects.ToList();
        }
    }
}
