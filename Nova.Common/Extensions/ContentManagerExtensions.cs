using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nova.Common.Extensions
{
    public static class ContentManagerExtensions
    {
        public static T LoadObject<T>(this ContentManager content, string name)
        {
            var obj = content.Load<JObject>(name);

            return obj.ToObject<T>();
        }
    }
}
