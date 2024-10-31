using System;

namespace Mtf.Serial.CustomEventArgs
{
    public class RawDataReceicedEventArgs : EventArgs
    {
        public byte[] Data { get; set; }
    }
}
