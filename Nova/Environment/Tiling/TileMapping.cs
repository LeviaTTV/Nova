namespace Nova.Environment
{
    public class TileMapping
    {
        public string Sheet { get; set; }
        public TileType TileType { get; set; }
        public string Sprite { get; set; }
        public float UpperLimit { get; set; }
        public float LowerLimit { get; set; }
        public bool Traversable { get; set; } = true;
    }
}