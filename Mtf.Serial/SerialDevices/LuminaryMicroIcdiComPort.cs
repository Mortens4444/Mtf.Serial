using System.Text;

namespace Mtf.Serial.SerialDevices
{
    /// <summary>
    /// Luminary Micro ICDI COM Port
    /// </summary>
    public class LuminaryMicroIcdiComPort : SerialDevice
    {
        public LuminaryMicroIcdiComPort(string portName)
            : base(portName, 115200, dataTerminalReady: true)
        {
            //Encoding = Encoding.Default;
        }
    }
}
