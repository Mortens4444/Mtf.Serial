using System;

namespace Mtf.Serial.Services
{
    public static class HexaConverter
    {
        public static byte[] ConvertFromHexadecimalData(string data)
        {
            if (data == null || data.Length % 2 != 0)
            {
                throw new ArgumentException("Hexadecimal string must have an even length.");
            }

            var chars = new byte[data.Length / 2];

            for (var i = 0; i < chars.Length; i++)
            {
                chars[i] = Convert.ToByte(data.Substring(i * 2, 2), 16);
            }

            return chars;
        }
    }
}
