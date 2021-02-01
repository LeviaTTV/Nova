using System;
using System.Diagnostics.Contracts;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Input;
using Nova.Common.Extensions;
using Nova.Common.Sprite;

namespace Nova.Content.Pipeline.AnimatedSpriteSheet
{

    [ContentImporter(".nvas", DefaultProcessor = "AnimatedSpriteSheetProcessor", DisplayName = "Animated Sprite Sheet Importer - Nova")]
    public class AnimatedSpriteSheetImporter : ContentImporter<Common.Sprite.AnimatedSpriteSheet>
    {
        public override Common.Sprite.AnimatedSpriteSheet Import(string filename, ContentImporterContext context)
        {
            var spriteSheet = new Common.Sprite.AnimatedSpriteSheet();

            using (var stream = new FileStream(filename, FileMode.Open))
            {
                byte[] novaBuf = new byte[4];
                stream.Read(novaBuf, 0, 4);

                AssetType assetType = (AssetType)stream.ReadByte(); // Asset Type

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

                    spriteSheet.Sprites.Add(sprite);
                }
            }

            return spriteSheet;
        }
    }
}
