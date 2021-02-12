using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Nova.Common.Sprite
{
    public interface IBinaryAssetSerializer
    {
        long Serialize(object obj, Stream stream);
        object Deserialize(GraphicsDevice device, Stream stream, string fileName);

        AssetType AssetType { get; }
    }
}
