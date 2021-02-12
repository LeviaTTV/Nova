using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nova.Common.Sprite;

namespace Nova.Content.Pipeline.SpriteSheet
{
    public class SpriteSheetReader : ContentTypeReader<Common.Sprite.SpriteSheet>
    {
        protected override Common.Sprite.SpriteSheet Read(ContentReader input, Common.Sprite.SpriteSheet existingInstance)
        {
            var spriteSheet = new Common.Sprite.SpriteSheet();

            byte[] novaBuf = new byte[4];
            input.Read(novaBuf, 0, 4);

            AssetType assetType = (AssetType)input.ReadByte(); // Asset Type

            input.ReadByte();

            spriteSheet.Name = input.ReadString();
            spriteSheet.AssetName = input.ReadString();
            var spriteCount = input.ReadUInt16();

            for (int i = 0; i < spriteCount; i++)
            {
                var sprite = new Sprite();

                input.ReadUInt16();

                sprite.Name = input.ReadString();
                sprite.Width = input.ReadUInt16();
                sprite.Height = input.ReadUInt16();
                sprite.X = input.ReadUInt16();
                sprite.Y = input.ReadUInt16();

                spriteSheet.Sprites[sprite.Name] = sprite;
            }

            var path = "";
            if (input.AssetName.IndexOf('/') != 0)
                path = input.AssetName.Substring(0, input.AssetName.LastIndexOf('/') + 1);

            spriteSheet.Texture = input.ContentManager.Load<Texture2D>(path + spriteSheet.AssetName);

            foreach (var sprite in spriteSheet.Sprites)
                sprite.Value.Texture = spriteSheet.Texture;

            return spriteSheet;
        }
    }
}