using System;

namespace Mtf.Serial.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FrequencyAttribute : Attribute
    {
        public double Value { get; }

        public FrequencyAttribute(double value)
        {
            Value = value;
        }
    }
}
