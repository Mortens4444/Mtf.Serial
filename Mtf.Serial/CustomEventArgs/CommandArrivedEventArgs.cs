using System;

namespace Mtf.Serial.CustomEventArgs
{
    public class CommandArrivedEventArgs : EventArgs
    {
        public string Command { get; set; }
    }
}
