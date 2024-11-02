using Mtf.Serial.Enums;
using Mtf.Serial.Extensions;
using System;
using System.Text;

namespace Mtf.Serial.SerialDevices
{
    /// <summary>
    /// Silicon Labs CP210x USB to UART Bridge - https://www.cartft.com/catalog/il/1132
    /// </summary>
    public class CarTft : SerialDevice
    {
        /// <summary>
        /// Turn On Radio Data System
        /// </summary>
        public const string TurnOnRadioDataSystem = "FF56787856";

        /// <summary>
        /// Turn Off Radio Data System
        /// </summary>
        public const string TurnOffRadioDataSystem = "FF53787853";

        public const string Manufacturer = "FF430D0043";
        public const string DeviceInformation = "FF430F0043";
        public const string Stop = "FF739B0573";
        private static readonly string[] separator = new[] { "3F" };

        public CarTft(string portName = "")
            : base(portName, 38400, dataTerminalReady: true)
        {
            Encoding = Encoding.ASCII;
        }

        protected override void DisposeManagedResources()
        {
            Uninitialize();
        }

        public void Initialize()
        {
            RadioDataSystem(true);
            GetDeviceInformation();
            Write("FF41962641");
            Write("FF44000044");
            Write("FF50560050");
            ChangeChannel(RadioChannel.RockFM);
            Write("FF50560E50");
        }

        public void RadioDataSystem(bool turnOn)
        {
            Write(turnOn ? TurnOnRadioDataSystem : TurnOffRadioDataSystem);
        }

        public void GetDeviceInformation() => Write(DeviceInformation);

        private void Uninitialize()
        {
            Write(Stop);
            RadioDataSystem(false);
        }

        public void ChangeChannel(RadioChannel channel)
        {
            var frequency = channel.GetFrequency();
            Write(FrequencyToRdsCode(frequency));
        }

        public void StartMusic()
        {
            for (var i = 0; i < 254; i++)
            {
                for (var j = 0; j < 254; j++)
                {
                    Write($"FF41{i:X2}{j:X2}41");
                }
            }
        }

        public static double RdsCodeToFrequency(string channelId)
        {
            var hexPart = channelId?.Substring(4, 2) ?? throw new InvalidOperationException();
            var frequencyValue = Convert.ToInt32(hexPart, 16);
            var normalizedFrequency = frequencyValue * 0.1;
            return 87.5 + normalizedFrequency;
        }

        public static string FrequencyToRdsCode(double frequency)
        {
            var frequencyOffset = frequency - 87.5;
            var channelValue = (int)Math.Round(frequencyOffset / 0.1);
            return $"FF73{channelValue:X2}0573";
        }

        public static string ConvertHexToAscii(string hex)
        {
            if (String.IsNullOrEmpty(hex))
            {
                return String.Empty;
            }
            var parts = hex.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            var result = new StringBuilder();

            foreach (var part in parts)
            {
                for (var i = 0; i < part.Length; i += 2)
                {
                    var hexByte = part.Substring(i, 2);
                    var asciiChar = (char)Convert.ToByte(hexByte, 16);
                    _ = result.Append(asciiChar);
                }
            }

            return result.ToString();
        }
    }
}
