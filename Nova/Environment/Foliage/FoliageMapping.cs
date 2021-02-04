using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nova.Environment.Foliage
{
    public class FoliageMappings
    {
        public IList<FoliageMapping> Foliage = new List<FoliageMapping>();
    }

    public class FoliageMapping
    {
        public FoliageType Type { get; set; }
        public float NoiseThreshold { get; set; }
        public float PoissonThreshold { get; set; }
        public int SameTypeRadius { get; set; }
        public bool PoissonPass { get; set; }
        public bool NoisePass { get; set; }
        public string GameObject { get; set; }
        public bool AllowOnTransition { get; set; }

        public IList<FoliageEligibleTileType> EligibleTileTypes = new List<FoliageEligibleTileType>();

        public FoliageSpriteDefinition Sprite { get; set; }
    }

    public class FoliageEligibleTileType
    {
        public TileType TileType { get; set; }
        public float Reduce { get; set; }
    }

    public class FoliageSpriteDefinition
    {
        public string Sheet { get; set; }
        public string Sprite { get; set; }

        public string[] RandomSprite { get; set; }
    }
}
