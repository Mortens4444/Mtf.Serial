using System.Text;

namespace Mtf.Serial.SerialDevices
{
    /// <summary>
    /// Conrad Relay Card
    /// </summary>
    public class ConradRelayCard : SerialDevice
    {
        public ConradRelayCard(string portName)
            : base(portName, 19200)
        {
            // AppendCrc
            AppendLineFeed = true;
            Encoding = Encoding.ASCII;
        }
    }
}
