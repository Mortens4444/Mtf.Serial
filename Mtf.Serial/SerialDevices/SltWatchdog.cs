using Mtf.Serial.Enums;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace Mtf.Serial.SerialDevices
{
    public class SltWatchdog : SerialDevice
    {
        private const string OkMsg = "OK";
        private const string ErrorMsg = "ERROR";
        private const string VersionMsg = "VER";
        private const string Version1_02Msg = "VER1.02";
        private const string VersionError = "VERERR";
        private const string Version2Msg = "VER2.0";
        private const string StartWatchdogMsg = "WDT1";
        private const string StopWatchdogMsg = "WDT0";
        private const string ClearWatchdogTimerMsg = "CLRWDT";
        private const string AlarmOffMsg = "ALARM0";
        private const string AlarmOnMsg = "ALARM1";
        private const string EEWriteMsg = "EEWRITE";
        private const string ReadByteMsg = "EERB";
        private const string ReadWordMsg = "EERW";

        private const int NotFound = -1;
        private const int WatchdogPetDelayMs = 5000;
        private const int WatchdogResetDelayMs = 1000;
        private const string NotExpectedAnswerArrivedFromWatchdog = "Not expected answer arrived from watchdog";

        private Task pettingTask;
        private CancellationTokenSource cancellationTokenSource;

        public SltWatchdog(string portName = "", WatcdogType watcdogType = WatcdogType.Unknown)
            : base(portName)
        {
            WatcdogType = watcdogType == WatcdogType.Unknown ? GetWatchDogType() : watcdogType;
            //SetNewLine(WatcdogType == WatcdogType.USB ? "\r\n" : "\r");

            //AppendCarriageReturn = true;
            //AppendLineFeed = true;
            //Encoding = Encoding.ASCII;
        }

        public WatcdogType WatcdogType { get; private set; }

        public WatcdogType GetWatchDogType()
        {
            var type = WatcdogType.Unknown;

            string result;
            do
            {
                SetNewLine("\r\n");
                WriteLine(VersionMsg);

                try
                {
                    result = $"{ReadLine()}{ReadLine()}";
                }
                catch (TimeoutException)
                {
                    SetNewLine("\r");
                    WriteLine(VersionMsg);
                    result = $"{ReadLine()}{ReadLine()}";
                }
            }
            while (result.IndexOf(VersionError, StringComparison.InvariantCulture) > NotFound);

            if (result.IndexOf(Version1_02Msg, StringComparison.InvariantCulture) > NotFound)
            {
                type = WatcdogType.COM;
            }
            else if (result.IndexOf(Version2Msg, StringComparison.InvariantCulture) > NotFound)
            {
                type = WatcdogType.USB;
            }

            if (type == WatcdogType.USB)
            {
                _ = ReadLine(); // ERROR\r\n
            }
            return type;
        }

        public static string GetPortName(WatcdogType expectedType)
        {
            if (expectedType == WatcdogType.Unknown)
            {
                throw new ArgumentException($"{expectedType} should not be set to Unknown.");
            }

            var portNames = SerialPort.GetPortNames();
            if (portNames.Length == 0)
            {
                throw new InvalidOperationException("No COM ports found.");
            }

            foreach (var portName in portNames)
            {
                try
                {
                    using (var watchdog = new SltWatchdog(portName))
                    {
                        var detectedType = watchdog.GetWatchDogType();

                        if (detectedType == expectedType)
                        {
                            var response = watchdog.WriteAndRead(String.Empty);
                            if (response == SltWatchdog.ErrorMsg)
                            {
                                response = watchdog.WriteAndRead(String.Empty);
                            }
                            if (response == SltWatchdog.OkMsg)
                            {
                                return portName;
                            }
                        }
                    }
                }
                catch
                {
                    // Ignore any errors and try the next port
                }
            }

            throw new InvalidOperationException("No matching watchdog device found.");
        }


        protected override void DisposeManagedResources()
        {
            StopPetting();
            _ = Rest();
        }

        public new string WriteAndRead(string message)
        {
            WriteLine(message);
            var result = ReadLine();
            if (WatcdogType == WatcdogType.USB)
            {
                _ = ReadLine();
            }
            return result;
        }

        public string Keep()
        {
            return WriteAndRead(StartWatchdogMsg);
        }

        public string Rest()
        {
            return WriteAndRead(StopWatchdogMsg);
        }

        public string AlarmOff()
        {
            return WriteAndRead(AlarmOffMsg);
        }

        public string AlarmOn()
        {
            return WriteAndRead(AlarmOnMsg);
        }

        public string WriteRegister(WatchDogRegister register, byte value = 0)
        {
            var address = register.ToString().Substring(1);
            return WriteAndRead($"{EEWriteMsg} {address} {value:X2}");
        }

        public byte ReadRegisterByte(WatchDogRegister register)
        {
            var address = register.ToString().Substring(1);
            var result = WriteAndRead($"{ReadByteMsg} {address}");
            return Convert.ToByte(result, 16);
        }

        public ushort ReadRegister(WatchDogRegister register)
        {
            var address = register.ToString().Substring(1);
            var result = WriteAndRead($"{ReadWordMsg} {address}");
            return Convert.ToUInt16(result, 16);
        }

        public string ResetCounters()
        {
            var registers = new List<WatchDogRegister> { WatchDogRegister._02, WatchDogRegister._03, WatchDogRegister._04, WatchDogRegister._05, WatchDogRegister._08, WatchDogRegister._09 };
            foreach (var reg in registers)
            {
                EnsureSuccess(reg.ToString(), WriteRegister(reg, 0));
            }
            return OkMsg;
        }

        public void StartPetting()
        {
            StopPetting();

            cancellationTokenSource = new CancellationTokenSource();
            pettingTask = Task.Run(() => Petting(cancellationTokenSource.Token));
        }

        public void StopPetting()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }

            pettingTask?.Wait();
            pettingTask = null;
        }

        public void Pet()
        {
            EnsureSuccess(ClearWatchdogTimerMsg, WriteAndRead(ClearWatchdogTimerMsg));
            Thread.Sleep(WatchdogResetDelayMs);
            EnsureSuccess(AlarmOffMsg, AlarmOff());
        }

        public override string ToString()
        {
            return $"{PortName} - {WatcdogType}";
        }

        private async Task Petting(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(WatchdogPetDelayMs, cancellationToken).ConfigureAwait(false);
                    Pet();
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        private static void EnsureSuccess(string operation, string result)
        {
            if (result != OkMsg)
            {
                throw new InvalidOperationException($"{NotExpectedAnswerArrivedFromWatchdog} - {operation}: {result}");
            }
        }
    }
}
