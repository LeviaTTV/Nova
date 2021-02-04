using System.Collections.Generic;

namespace Nova.Environment.Foliage
{
    public class FoliageMappings
    {
        public IList<FoliageMapping> Foliage = new List<FoliageMapping>();
    }

    public class FoliageMapping
    {
        public string Type { get; set; }
        public float NoiseThreshold { get; set; }
        public float PoissonThreshold { get; set; }
        public int SameTypeRadius { get; set; }
        public bool PoissonPass { get; set; }
        public bool NoisePass { get; set; }
        public bool AllowOnTransition { get; set; }

        public IList<FoliageEligibleTileType> EligibleTileTypes = new List<FoliageEligibleTileType>();
    }

    public class FoliageEligibleTileType
    {
        public TileType TileType { get; set; }
        public float Reduce { get; set; }
    }
}
