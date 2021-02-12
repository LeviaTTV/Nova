using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Newtonsoft.Json.Linq;

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
