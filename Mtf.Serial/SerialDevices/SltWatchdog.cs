using System.Text;

namespace Mtf.Serial.SerialDevices
{
    public class SltWatchdog : SerialDevice
    {
        public SltWatchdog(string portName)
            : base(portName)
        {
            AppendCarriageReturn = true;
            //Encoding = Encoding.Default;
        }
    }
}
