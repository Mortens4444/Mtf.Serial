using System;
using System.Text;
using System.Threading;

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
            AppendLineFeed = true;
            Encoding = Encoding.ASCII;
        }

        private const int DelayBetweenSends = 4;
        private const int InitDelay = 1023;

        public void Initialize()
        {
            SendCommand(new byte[] { 1 }, maxTries: 4, checkRead: true);
            Thread.Sleep(InitDelay);
            SendCommand(new byte[] { 1 }, maxTries: 4);
            Thread.Sleep(InitDelay);
        }

        private void SendCommand(byte[] data, int maxTries = 1, bool checkRead = false)
        {
            for (var i = 0; i < maxTries; i++)
            {
                Write(data, 0, data.Length);
                Thread.Sleep(DelayBetweenSends);
                if (checkRead && BytesToRead > 3)
                {
                    break;
                }
            }
        }

        public static byte GetCRC(byte[] data, int startIdx = 0, int length = -1)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            length = length == -1 ? data.Length : length;
            byte crc = 0;
            for (var i = startIdx; i < startIdx + length; i++)
            {
                crc ^= data[i];
            }

            return crc;
        }

        public void Function1() => ExecuteFunction(3);

        public void Function2() => ExecuteFunction(7);

        public void Function3() => ExecuteFunction(8);

        private void ExecuteFunction(byte command)
        {
            var sendBuffer = new byte[] { 0, command, 1, 255, 0, Constants.LineFeed };
            sendBuffer[4] = GetCRC(sendBuffer, 0, 4);
            Write(sendBuffer, 0, sendBuffer.Length);
        }
    }
}
