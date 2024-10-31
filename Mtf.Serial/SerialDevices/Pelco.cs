using System.IO.Ports;
using System.Text;

namespace Mtf.Serial.SerialDevices
{
    public class Pelco : SerialDevice
    {
        public Pelco(string portName)
            : base(portName, parity: Parity.Odd)
        {
            Encoding = Encoding.ASCII;
        }
    }
}
