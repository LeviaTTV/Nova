using System;
using System.Diagnostics.Contracts;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Input;
using Nova.Common.Extensions;
using Nova.Common.Sprite;

namespace Nova.Content.Pipeline.SpriteSheet
{

    [ContentImporter(".nvs", DefaultProcessor = "SpriteSheetProcessor", DisplayName = "Sprite Sheet Importer - Nova")]
    public class SpriteSheetImporter : ContentImporter<Common.Sprite.SpriteSheet>
    {
        public override Common.Sprite.SpriteSheet Import(string filename, ContentImporterContext context)
        {
            var spriteSheet = new Common.Sprite.SpriteSheet();

            using (var stream = new FileStream(filename, FileMode.Open))
            {
                byte[] novaBuf = new byte[4];
                stream.Read(novaBuf, 0, 4);

                AssetType assetType = (AssetType)stream.ReadByte(); // Asset Type

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
            }

            return spriteSheet;
        }
    }
}
