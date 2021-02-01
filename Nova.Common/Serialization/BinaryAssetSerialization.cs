using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Nova.Common.Sprite
{
    public class BinaryAssetSerialization
    {
        private Dictionary<AssetType, IBinaryAssetSerializer> _detectedSerializers = new Dictionary<AssetType, IBinaryAssetSerializer>();
        
        public BinaryAssetSerialization()
        {
            var interfaceType = typeof(IBinaryAssetSerializer);
            var types = typeof(BinaryAssetSerialization).Assembly.GetTypes()
                .Where(p => interfaceType.IsAssignableFrom(p) && p.IsClass);

            foreach (var type in types)
            {
                var obj = (IBinaryAssetSerializer)Activator.CreateInstance(type);
                _detectedSerializers[obj.AssetType] = obj;
            }
        }

        public long Serialize(object obj, Stream stream)
        {
            var assetType = AssetTypeMapping.TypeMapping[obj.GetType()];

            var headerBytes = Encoding.ASCII.GetBytes("NOVA");
            stream.Write(headerBytes, 0, headerBytes.Length);

            stream.WriteByte((byte)assetType);

            return _detectedSerializers[assetType].Serialize(obj, stream) + 5;
        }

        public object Deserialize(GraphicsDevice device, Stream stream)
        {
            byte[] buf = new byte[4];
            stream.Read(buf, 0, 4);

            var assetType = (AssetType)stream.ReadByte();

            var deserializer = _detectedSerializers[assetType];
            return deserializer.Deserialize(device, stream);
        }

        public T Deserialize<T>(GraphicsDevice device, Stream stream)
        {
            byte[] buf = new byte[4];
            stream.Read(buf, 0, 4);

            var assetType = (AssetType)stream.ReadByte();

            var deserializer = _detectedSerializers[assetType];
            var obj = deserializer.Deserialize(device, stream);

            return (T) obj;
        }
    }
}
