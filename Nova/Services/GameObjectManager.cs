using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Nova.Environment;
using Nova.Objects;

namespace Nova.Services
{
    public class GameObjectManager
    {
        public List<GameObject> GameObjects = new List<GameObject>();

        public void AddGameObject(GameObject gameObject)
        {
            GameObjects.Add(gameObject);
        }

        public void LoadAll(ContentManager content)
        {
            foreach (var obj in GameObjects)
                obj.LoadContent(content);
        }

        public IEnumerable<GameObject> GetCollidingFoliageGameObjects(GameObject self, Rectangle rect)
        {
            return GameObjects.Where(z => z.CollisionEnabled && z.CollisionBounds.Intersects(rect) && z != self);
        }
    }
}
