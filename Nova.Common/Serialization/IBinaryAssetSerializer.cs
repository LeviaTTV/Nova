using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Nova.Common.Sprite
{
    public interface IBinaryAssetSerializer
    {
        long Serialize(object obj, Stream stream);
        object Deserialize(GraphicsDevice device, Stream stream);

        AssetType AssetType { get; }
    }
}
