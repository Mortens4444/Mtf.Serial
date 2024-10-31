using System.Text;

namespace Mtf.Serial.SerialDevices
{
    public class StretchUart : SerialDevice
    {
        public StretchUart(string portName)
            : base(portName, 38400, dataTerminalReady: true)
        {
            // Data in hexadecimal format
            Encoding = Encoding.ASCII;
        }
    }
}
