using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Nova.Common.Sprite;
using SharpDX.Text;

namespace Nova.Content.Pipeline.AnimatedSpriteSheet
{
    [ContentTypeWriter]
    public class AnimatedSpriteSheetWriter : ContentTypeWriter<Common.Sprite.AnimatedSpriteSheet>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform) => "Nova.Content.Pipeline.AnimatedSpriteSheet.AnimatedSpriteSheetReader, Nova.Content.Pipeline";

        protected override void Write(ContentWriter output, Common.Sprite.AnimatedSpriteSheet obj)
        {
            var headerBytes = Encoding.ASCII.GetBytes("NOVA");

            output.Write(headerBytes);


            output.Write((byte)AssetType.SpriteSheet);
            output.Write((byte)1);

            output.Write(obj.Name);
            output.Write(obj.AssetName);

            output.Write(obj.StartDelay);
            output.Write(obj.TimeBetweenSprites);
            output.Write(obj.Repeat);
            output.Write(obj.ReverseAtEnd);

            output.Write((ushort)obj.Sprites.Count);

            ushort count = 0;

            foreach (var sprite in obj.Sprites)
            {
                output.Write((ushort)count);
                output.Write(sprite.Name);
                output.Write((ushort)sprite.Width);
                output.Write((ushort)sprite.Height);
                output.Write((ushort)sprite.X);
                output.Write((ushort)sprite.Y);
            }
        }
    }
}