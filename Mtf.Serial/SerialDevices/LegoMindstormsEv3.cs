using Mtf.Serial.Models.Lego;
using System;

namespace Mtf.Serial.SerialDevices
{
    public class LegoMindstormsEv3 : SerialDevice
    {
        public DaisyChainLayer DaisyChainLayer { get; set; } = DaisyChainLayer.EV3;

        public OutputPort OutputPort { get; set; } = OutputPort.BC;

        public LegoMindstormsEv3(string portName)
            : base(portName, 115200)
        {
        }

        public void SetMotorSpeed(int speed, MotorType motorType = null)
        {
            var speedBytes = BitConverter.GetBytes((short)Math.Max(Math.Min(speed, 100), -100));
            var bytes = new byte[] { CommandType.DirectCommand | Response.NotExpected, 0, 0, OpCode.OutputSetType, DaisyChainLayer, OutputPort, motorType ?? MotorType.Medium, OpCode.OutputSpeed, DaisyChainLayer, OutputPort, ParameterFormat.Long | FollowType.TwoBytes, speedBytes[0], speedBytes[1], OpCode.OutputStart, DaisyChainLayer, OutputPort };
            Write(bytes);
        }

        public void StopMotor()
        {
            var bytes = new byte[] { CommandType.DirectCommand | Response.NotExpected, 0, 0, OpCode.OutputStop, DaisyChainLayer, OutputPort, 0x01 };
            Write(bytes);
        }
    }
}
