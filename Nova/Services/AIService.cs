using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Nova.AI.BT.Base;
using Nova.AI.BT.Models;
using Nova.Common.Extensions;
using Nova.Objects.Character;

namespace Nova.Services
{
    public class AIService
    {
        private readonly GameServiceContainer _services;
        private readonly ContentManager _contentManager;

        private readonly Dictionary<string, Type> _detectedTypes = new Dictionary<string, Type>();
        private readonly Dictionary<string, Node> _behaviorTrees = new Dictionary<string, Node>();
        private readonly List<LivingGameObject> _aiGameObjects = new List<LivingGameObject>();

        public AIService(GameServiceContainer services, ContentManager contentManager)
        {
            _services = services;
            _contentManager = contentManager;
            
            var commonTypes = Assembly.GetAssembly(typeof(Node)).GetTypes();
            var novaTypes = Assembly.GetExecutingAssembly().GetTypes();

            foreach (Type type in commonTypes.Concat(novaTypes).Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Node))))
                _detectedTypes[type.Name] = type;

            Preload();
        }

        private void Preload()
        {
            var allBehaviorTreeFiles = Directory.GetFiles(Path.Combine(_contentManager.RootDirectory, "AI"), "*", SearchOption.AllDirectories);

            foreach (var file in allBehaviorTreeFiles)
                LoadBehaviorTree(Path.Combine("AI/", Path.GetFileNameWithoutExtension(file)));
        }

        public Node LoadBehaviorTree(string assetName)
        {
            var jsonNode = _contentManager.LoadObject<JsonNode>(assetName);
            var node = ProcessJsonNode(jsonNode, null);

            _behaviorTrees[node.Name] = node;

            return node;
        }

        private Node ProcessJsonNode(JsonNode jsonNode, Node parent = null)
        {
            var type = _detectedTypes[jsonNode.Type];

            var node = (Node)Activator.CreateInstance(type);
            node.Name = jsonNode.Name;
            node.Parent = parent;
            node.AppliesTo = jsonNode.AppliesTo;

            if (parent != null)
                parent.Children.Add(node);

            foreach (var child in jsonNode.Children)
                ProcessJsonNode(child, node);
            

            return node;
        }

        public Node GetBehaviorTree(string name)
        {
            return _behaviorTrees[name];
        }

        public Node GetBehaviorTreeThatAppliesTo(string className)
        {
            return _behaviorTrees.Values.FirstOrDefault(x => x.AppliesTo == className);
        }

        public void Register(LivingGameObject gameObject)
        {
            _aiGameObjects.Add(gameObject);
        }

        public void Unregister(LivingGameObject gameObject)
        {
            _aiGameObjects.Remove(gameObject);
        }

        public void Update()
        {
            foreach (var node in _aiGameObjects)
                node.UpdateAI();
        }
    }
}
