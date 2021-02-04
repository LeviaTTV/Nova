using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nova.Common.Sprite;
using SharpDX.Text;

namespace Nova.Content.Pipeline.Json
{
    [ContentTypeWriter]
    public class  JsonWriter : ContentTypeWriter<JObject>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform) => "Nova.Content.Pipeline.Json.JsonReader, Nova.Content.Pipeline";

        protected override void Write(ContentWriter output, JObject obj)
        {
            var bytes = Encoding.UTF8.GetBytes(obj.ToString(Formatting.None));

            output.Write((int)bytes.Length);
            output.Write(bytes);
        }
    }
}