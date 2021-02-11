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
        public Dictionary<TileCoordinate, FoliageGameObject> FoliageGameObjects { get; private set; } = new Dictionary<TileCoordinate, FoliageGameObject>();

        public List<GameObject> GameObjects = new List<GameObject>();

        public void AddFoliageGameObject(FoliageGameObject gameObject)
        {
            FoliageGameObjects[new TileCoordinate(gameObject.Tile.X, gameObject.Tile.Y)] = gameObject;
        }

        public void AddGameObject(GameObject gameObject)
        {
            GameObjects.Add(gameObject);
        }

        public void SortFoliageGameObjectsByYCoordinate()
        {
            FoliageGameObjects = FoliageGameObjects.OrderBy(x => x.Key.Y).ToDictionary(x => x.Key, z => z.Value);
        }

        public void LoadAll(ContentManager content)
        {
            foreach (var obj in FoliageGameObjects.Values)
                obj.LoadContent(content);

            foreach (var obj in GameObjects)
                obj.LoadContent(content);
        }

        public IEnumerable<GameObject> GetCollidingFoliageGameObjects(GameObject self, Rectangle rect)
        {
            var foliageList = FoliageGameObjects.Where(z => z.Value.CollisionEnabled && z.Value.CollisionBounds.Intersects(rect)).Select(z => z.Value);

            var gameObjectList = GameObjects.Where(z => z.CollisionEnabled && z.CollisionBounds.Intersects(rect) && z != self);

            return foliageList.Concat(gameObjectList);
        }
    }
}
