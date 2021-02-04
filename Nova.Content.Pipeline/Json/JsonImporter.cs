using System;
using System.Diagnostics.Contracts;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nova.Common.Extensions;
using Nova.Common.Sprite;

namespace Nova.Content.Pipeline.Json
{

    [ContentImporter(".json", DisplayName = "Json importer - Nova", DefaultProcessor = "PassThroughProcessor")]
    public class JsonImporter : ContentImporter<JObject>
    {
        public override JObject Import(string filename, ContentImporterContext context)
        {
            return JObject.Parse(File.ReadAllText(filename));
        }
    }
}
