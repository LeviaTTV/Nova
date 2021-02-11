using System;
using System.Collections.Generic;
using System.Text;
using Nova.Common.AI.BT.Base;

namespace Nova.Common.AI.BT.Models
{
    public class JsonNode
    {
        public List<JsonNode> Children { get; set; } = new List<JsonNode>();
        public string Name { get; set; }
        public string Type { get; set; }
        public string AppliesTo { get; set; }
    }
}
