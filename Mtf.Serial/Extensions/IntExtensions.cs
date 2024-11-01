using System;

namespace Mtf.Serial.Extensions
{
    public static class IntExtensions
    {
        public static int LimitMe(this int value, int min, int max)
        {
            return Math.Max(min, Math.Min(value, max));
        }
    }
}
