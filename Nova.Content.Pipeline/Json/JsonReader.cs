using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using Nova.Common.Sprite;
using SharpDX.Text;

namespace Nova.Content.Pipeline.Json
{
    public class JsonReader : ContentTypeReader<JObject>
    {
        protected override JObject Read(ContentReader input, JObject existingInstance)
        {
            var dataLength = input.ReadInt32();
            var data = input.ReadBytes(dataLength);

            return JObject.Parse(Encoding.UTF8.GetString(data));
        }
    }
}