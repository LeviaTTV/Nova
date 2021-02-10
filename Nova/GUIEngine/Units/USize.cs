namespace Nova.GUIEngine.Units
{
    public struct USize
    {
        public UDim Width { get; set; }
        public UDim Height { get; set; }

        public USize(UDim width, UDim height)
        {
            Width = width;
            Height = height;
        }

        public USize(UDim square)
        {
            Width = square;
            Height = square;
        }

        public USize(float width, float height)
        {
            Width = UDim.Absolute(width);
            Height = UDim.Absolute(height);
        }

        public bool IsAbsolute() => Width.IsAbsolute() && Height.IsAbsolute();

        public static USize operator +(USize a, USize b) => new USize(a.Width + b.Width, a.Height + b.Height);
        public static USize operator -(USize a, USize b) => new USize(a.Width - b.Width, a.Height - b.Height);
        public static USize operator *(USize a, USize b) => new USize(a.Width * b.Width, a.Height * b.Height);
        public static USize operator *(USize a, float scale) => new USize(a.Width * scale, a.Height * scale);
        public static USize operator /(USize a, float scale) => new USize(a.Width / scale, a.Height / scale);
        public static USize operator /(USize a, USize b) => new USize(a.Width / b.Width, a.Height / b.Height);

        public static USize Zero => new USize();

        public override string ToString()
        {
            return $"USize Width {Width} Height {Height}";
        }
    }
}