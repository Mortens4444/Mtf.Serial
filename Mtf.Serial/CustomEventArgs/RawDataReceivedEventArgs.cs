using System;

namespace Mtf.Serial.CustomEventArgs
{
    public class RawDataReceivedEventArgs : EventArgs
    {
        public byte[] Data { get; set; }
    }
}
