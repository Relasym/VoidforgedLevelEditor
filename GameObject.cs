using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace LevelEditor
{

    [FirestoreData]
    public class GameObject
    {
        [FirestoreProperty]
        public GameObjectTypes Type { get; set; }
        [FirestoreProperty]
        public double XPosition { get; set; }
        [FirestoreProperty]
        public double YPosition { get; set; }
        [FirestoreProperty]
        public int Width { get; set; }
        [FirestoreProperty]
        public int Height { get; set; }

        public GameObject(GameObjectTypes type, double xPosition, double yPosition, int width, int height)
        {
            Type = type;
            XPosition = xPosition;
            YPosition = yPosition;
            Width = width;
            Height = height;
        }

        public string ToJSON()
        {
            return JsonSerializer.Serialize(this);
        }

    }
}
