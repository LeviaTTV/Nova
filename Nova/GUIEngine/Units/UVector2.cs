using System;
using Microsoft.Xna.Framework;

namespace Nova.GUIEngine.Units
{
    public struct UVector2
    {
        public UDim X { get; set; }
        public UDim Y { get; set; }

        public UVector2(UDim x, UDim y)
        {
            X = x;
            Y = y;
        }

        public UVector2(UDim point)
        {
            X = point;
            Y = point;
        }

        public UVector2(float x, float y)
        {
            X = UDim.Absolute(x);
            Y = UDim.Absolute(y);
        }

        public bool IsAbsolute() => X.IsAbsolute() && Y.IsAbsolute();

        public static UVector2 operator +(UVector2 a, UVector2 b) => new UVector2(a.X + b.X, a.Y + b.Y);
        public static UVector2 operator -(UVector2 a, UVector2 b) => new UVector2(a.X - b.X, a.Y - b.Y);
        public static UVector2 operator *(UVector2 a, UVector2 b) => new UVector2(a.X * b.X, a.Y * b.Y);
        public static UVector2 operator *(UVector2 a, float scale) => new UVector2(a.X * scale, a.Y * scale);
        public static UVector2 operator /(UVector2 a, UVector2 b) => new UVector2(a.X / b.X, a.Y / b.Y);
        public static UVector2 operator /(UVector2 a, float scale) => new UVector2(a.X / scale, a.Y / scale);

        public static UVector2 Zero => new UVector2();

        public Vector2 ToVector2()
        {
            if (!IsAbsolute())
                throw new Exception("Can't convert relative coordinates to Vector2.");

            return new Vector2(X.Absolute(), Y.Absolute());
        }

        public override string ToString()
        {
            return $"UVector2 X {X} Y {Y}";
        }
    }
}