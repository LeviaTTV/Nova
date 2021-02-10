namespace Nova.GUIEngine.Units
{
    public struct UDim
    {
        public float Scale { get; set; }
        public float Offset { get; set; }

        public UDim(float scale, float offset)
        {
            Scale = scale;
            Offset = offset;
        }

        public bool IsAbsolute() => Scale == 0f;

        public static UDim operator +(UDim a, UDim b) => new UDim(a.Scale + b.Scale, a.Offset + b.Offset);
        public static UDim operator -(UDim a, UDim b) => new UDim(a.Scale - b.Scale, a.Offset - b.Offset);
        public static UDim operator *(UDim a, UDim b) => new UDim(a.Scale * b.Scale, a.Offset * b.Offset);
        public static UDim operator *(UDim a, float scale) => new UDim(a.Scale * scale, a.Offset * scale);
        public static UDim operator /(UDim a, UDim b) => new UDim(a.Scale / b.Scale, a.Offset / b.Offset);
        public static UDim operator /(UDim a, float scale) => new UDim(a.Scale / scale, a.Offset / scale);

        public static UDim Zero => new UDim();
        public static UDim Percent => new UDim(0.01f, 0f);
        public static UDim Pixel => new UDim(0, 1f);

        public static UDim Absolute(float absolute) => new UDim(0, absolute);
        public static UDim Relative(float relative) => new UDim(relative, 0);

        public float Absolute() => Offset;
        public float Relative() => Scale;

        public override string ToString()
        {
            return $"UDim Scale {Scale} Offset {Offset}";
        }
    }
}
