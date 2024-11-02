using Mtf.Serial.CustomEventArgs;
using Mtf.Serial.Extensions;
using System;
using System.IO.Ports;
using System.Text;

namespace Mtf.Serial.SerialDevices
{
    public class PelcoKbd300A : SerialDevice
    {
        public PelcoKbd300A(string portName = "")
            : base(portName, parity: Parity.Odd)
        {
            Encoding = Encoding.ASCII;
            DataReceived += DataReceiced;
        }

        public event EventHandler<CommandArrivedEventArgs> CommandArrived;

        private void OnCommandArrived(string command) => CommandArrived?.Invoke(this, new CommandArrivedEventArgs { Command = command });

        private void DataReceiced(object sender, DataReceicedEventArgs e)
        {
            var messageBuffer = e.Data;
            if (messageBuffer.Contains("Exception"))
            {
                OnCommandArrived(messageBuffer);
            }
            else
            {
                ParseMessage(ref messageBuffer);
            }
        }

        private void ParseMessage(ref string message)
        {
            int index;
            while ((index = FindCommandIndex(message)) != Constants.NotFound)
            {
                var command = message.Substring(0, index + 1);
                OnCommandArrived(command);
                message = message.Substring(index + 1);
            }
        }

        private static int FindCommandIndex(string message)
        {
            var aIndex = message.IndexOf('a');
            var mIndex = message.IndexOf('m');
            return Math.Min(aIndex != -1 ? aIndex : int.MaxValue, mIndex != -1 ? mIndex : int.MaxValue);
        }

        private void SendCommand(string command, int value = 0, int limit = 64)
            => Write(String.Format(command, value.LimitMe(1, limit)));

        public void PanLeft(byte speed) => SendCommand("{0}La", speed);
        public void StopPanLeft() => Write("~La");
        public void PanRight(byte speed) => SendCommand("{0}Ra", speed);
        public void StopPanRight() => Write("~Ra");
        public void TiltUp(byte speed) => SendCommand("{0}Ua", speed);
        public void StopTiltUp() => Write("~Ua");
        public void TiltDown(byte speed) => SendCommand("{0}Da", speed);
        public void StopTiltDown() => Write("~Da");
        public void StopAllPTZMotion() => Write("sa");
        public void FocusNear() => Write("Na");
        public void StopFocusNear() => Write("~Na");
        public void FocusFar() => Write("Fa");
        public void StopFocusFar() => Write("~Fa");
        public void IrisOpen() => Write("Oa");
        public void StopIrisOpen() => Write("~Oa");
        public void IrisClose() => Write("Ca");
        public void StopIrisClose() => Write("~Ca");
        public void ZoomTelephoto() => Write("Ta");
        public void StopZoomTelephoto() => Write("~Ta");
        public void ZoomWide() => Write("Wa");
        public void StopZoomWide() => Write("~Wa");

        public void RecordPattern(byte value) => SendCommand("{0}/a", value, 99);
        public void ExecutePattern(byte value) => SendCommand("{0}pa", value, 99);
        public void EndPattern(byte value) => SendCommand("{0}na", value, 99);
        public void StartASequence(byte value) => SendCommand("{0}qa", value, 99);
        public void EndASequence(byte value) => SendCommand("{0}ea", value, 99);

        public void SendCameraTitles(string[] cameraNames)
        {
            var message = $"Ya{String.Join("/", cameraNames)}!a";
            Write(message);
        }

        public void SetPresetWithLabel(int presetNumber, string presetName)
        {
            presetNumber = presetNumber.LimitMe(1, 9999);
            Write($"la{presetName}!a{presetNumber}^a");
        }
    }
}
