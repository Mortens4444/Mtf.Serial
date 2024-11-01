using Mtf.Serial.Attributes;
using Mtf.Serial.Enums;

namespace Mtf.Serial.Extensions
{
    public static class RadioChannelExtensions
    {
        public static double GetFrequency(this RadioChannel channel)
        {
            var fieldInfo = channel.GetType().GetField(channel.ToString());
            var attributes = (FrequencyAttribute[])fieldInfo.GetCustomAttributes(typeof(FrequencyAttribute), false);
            return attributes.Length > 0 ? attributes[0].Value : 0.0;
        }
    }
}
