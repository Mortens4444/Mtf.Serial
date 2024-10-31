using System;

namespace Mtf.Serial.CustomEventArgs
{
    public class DataReceicedEventArgs : EventArgs
    {
        public string Data { get; set; }
    }
}
