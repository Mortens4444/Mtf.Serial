using System;

namespace Mtf.Serial.CustomEventArgs
{
    public class DataReceivedEventArgs : EventArgs
    {
        public string Data { get; set; }
    }
}
