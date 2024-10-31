namespace Mtf.Serial.SerialDevices
{
    public class LegoMindstormsEv3 : SerialDevice
    {
        public LegoMindstormsEv3(string portName)
            : base(portName, 115200)
        {
        }
    }
}
