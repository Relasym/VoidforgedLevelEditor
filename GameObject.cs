using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LevelEditor
{
   
    public class GameObject
    {
        public GameObjectTypes Type { get; set; }
        public int XPosition { get; set; }
        public int YPosition { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public GameObject(GameObjectTypes type, int xPosition, int yPosition, int width, int height)
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
