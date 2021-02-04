using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nova.Environment
{
    public class TileMappings
    {
        public IList<TileMapping> Mappings = new List<TileMapping>();
        public IList<TransitionMask> TransitionMasks = new List<TransitionMask>();

    }
}
