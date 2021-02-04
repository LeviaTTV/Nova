using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nova.Common.Extensions;

namespace Nova.Common.Sprite
{
    public class SpriteSheetSerializer : IBinaryAssetSerializer
    {
        public AssetType AssetType => AssetType.SpriteSheet;

        public long Serialize(object genericObject, Stream stream)
        {
            var obj = (SpriteSheet)genericObject;

            long offset = 0;

            stream.WriteByte(1); // Version
            ++offset;
            

            offset += stream.WriteStringWithLength(obj.Name);
            offset += stream.WriteStringWithLength(obj.Name + "Texture"); // Texture asset name
            offset += stream.WriteUInt16((ushort)obj.Sprites.Count);

            ushort count = 0;
            foreach (var spriteEntry in obj.Sprites)
            {
                var sprite = spriteEntry.Value;
                ++count;
                offset += stream.WriteUInt16(count);
                offset += stream.WriteStringWithLength(sprite.Name);
                offset += stream.WriteUInt16((ushort)sprite.Width);
                offset += stream.WriteUInt16((ushort)sprite.Height);
                offset += stream.WriteUInt16((ushort)sprite.X);
                offset += stream.WriteUInt16((ushort)sprite.Y);
            }

            return offset;
        }

        public object Deserialize(GraphicsDevice device, Stream stream, string fileName)
        {
            var spriteSheet = new SpriteSheet();

            var version = stream.ReadByte();
            spriteSheet.Name = stream.ReadStringWithLength();
            spriteSheet.AssetName = stream.ReadStringWithLength();
            ushort frameCount = stream.ReadUInt16();

            for (int i = 0; i < frameCount; i++)
            {
                Sprite sprite = new Sprite();

                stream.ReadUInt16(); // Frame index, unused

                sprite.Name = stream.ReadStringWithLength();
                sprite.Width = stream.ReadUInt16();
                sprite.Height = stream.ReadUInt16();
                sprite.X = stream.ReadUInt16();
                sprite.Y = stream.ReadUInt16();

                spriteSheet.Sprites[sprite.Name] = sprite;
            }

            using (var fs = new FileStream(Path.Combine(Path.GetDirectoryName(fileName), spriteSheet.AssetName + ".png"), FileMode.Open))
            {
                spriteSheet.Texture = Texture2D.FromStream(device, fs);
            }

            foreach (var sprite in spriteSheet.Sprites)
                sprite.Value.Texture = spriteSheet.Texture;

            return spriteSheet;
        }
    }
}
