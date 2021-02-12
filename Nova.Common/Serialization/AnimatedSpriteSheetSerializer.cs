using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Nova.Common.Extensions;

namespace Nova.Common.Sprite
{
    public class AnimatedSpriteSheetSerializer : IBinaryAssetSerializer
    {
        public AssetType AssetType => AssetType.AnimatedSpriteSheet;

        public long Serialize(object genericObject, Stream stream)
        {
            var obj = (AnimatedSpriteSheet) genericObject;

            long offset = 0;

            stream.WriteByte(1); // Version
            ++offset;

            offset += stream.WriteStringWithLength(obj.Name);
            offset += stream.WriteStringWithLength(obj.Name + "Texture"); // Texture name
            offset += stream.WriteInt64(obj.StartDelay);
            offset += stream.WriteInt64(obj.TimeBetweenSprites);
            offset += stream.WriteBool(obj.Repeat);
            offset += stream.WriteBool(obj.ReverseAtEnd);
            offset += stream.WriteUInt16((ushort)obj.SpriteCount);

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
            var spriteSheet = new AnimatedSpriteSheet();

            var version = stream.ReadByte();
            spriteSheet.Name = stream.ReadStringWithLength();
            spriteSheet.AssetName = stream.ReadStringWithLength();

            spriteSheet.StartDelay = stream.ReadInt64();
            spriteSheet.TimeBetweenSprites = stream.ReadInt64();
            spriteSheet.Repeat = stream.ReadBool();
            spriteSheet.ReverseAtEnd = stream.ReadBool();
            
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
