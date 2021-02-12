using System;
using Microsoft.Xna.Framework;

namespace Nova.Common.Extensions
{
    public static class VectorExtensions
    {
        public static double AngleTo(this Vector2 first, Vector2 second)
        {
            return Math.Atan2(second.Y - first.Y, second.X - first.X);
        }
    }
}
