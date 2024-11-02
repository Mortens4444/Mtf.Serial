using Mtf.Serial.Models.Lego;
using System;
using System.Threading;

namespace Mtf.Serial.SerialDevices
{
    public class LegoMindstormsEv3 : SerialDevice
    {
        private const int syncWaitAfterConnectInMs = 5000;
        private const int syncWaitAfterDisconnectInMs = 30000;
        private static short messageConter = 1;

        public DaisyChainLayer DaisyChainLayer { get; set; } = DaisyChainLayer.EV3;

        public OutputPort LeftMotor { get; set; } = OutputPort.B;

        public OutputPort RightMotor { get; set; } = OutputPort.C;

        public OutputPort BothMotor => LeftMotor | RightMotor;

        public LegoMindstormsEv3(string portName)
            : base(portName, 115200)
        {
        }

        public void Move(int speed, MotorType motorType = null)
        {
            StartMotor(BothMotor, speed, motorType);
        }

        public void TurnRight(int speed, MotorType motorType = null)
        {
            StartMotor(LeftMotor, speed, motorType);
            StartMotor(RightMotor, -speed, motorType);
        }

        public void TurnLeft(int speed, MotorType motorType = null)
        {
            StartMotor(RightMotor, speed, motorType);
            StartMotor(LeftMotor, -speed, motorType);
        }

        public new void Connect(bool subscribeToDefaultEvents = true)
        {
            base.Connect(subscribeToDefaultEvents);
            LogDebug($"{PortName} sync wait {syncWaitAfterConnectInMs} ms after connect...");
            Thread.Sleep(syncWaitAfterConnectInMs);
            LogDebug($"{PortName} sync wait over...");
        }

        public new void Disconnect()
        {
            base.Disconnect();
            LogDebug($"{PortName} sync wait {syncWaitAfterDisconnectInMs} ms after disconnect...");
            Thread.Sleep(syncWaitAfterDisconnectInMs);
            LogDebug($"{PortName} sync wait over...");
        }

        public void Stop()
        {
            var messageConterBytes = BitConverter.GetBytes(messageConter++);
            var bytes = new byte[] { 0x09, 0x00, messageConterBytes[0], messageConterBytes[1], CommandType.DirectCommand | Response.NotExpected, 0, 0, OpCode.OutputStop, DaisyChainLayer, BothMotor, 0x01 };
            _ = WriteAsync(bytes);
        }

        private void StartMotor(OutputPort outputPort, int speed, MotorType motorType = null)
        {
            var speedBytes = BitConverter.GetBytes((short)Math.Max(Math.Min(speed, 100), -100));
            var messageConterBytes = BitConverter.GetBytes(messageConter++);
            var bytes = new byte[] { 0x12, 0x00, messageConterBytes[0], messageConterBytes[1], CommandType.DirectCommand | Response.NotExpected, 0, 0, OpCode.OutputSetType, DaisyChainLayer, outputPort, motorType ?? MotorType.Medium, OpCode.OutputSpeed, DaisyChainLayer, outputPort, ParameterFormat.Long | FollowType.TwoBytes, speedBytes[0], speedBytes[1], OpCode.OutputStart, DaisyChainLayer, outputPort };
            _ = WriteAsync(bytes);
        }
    }
}
