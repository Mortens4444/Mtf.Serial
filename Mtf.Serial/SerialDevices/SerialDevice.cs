using Microsoft.Extensions.Logging;
using Mtf.Serial.CustomEventArgs;
using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mtf.Serial.SerialDevices
{
    public class SerialDevice : IDisposable
    {
        private readonly SerialPort comPort;
        private int threadSafeDisposing;
        private readonly object connectionLock = new object();
        private readonly Action<ILogger, SerialDevice, SerialErrorReceivedEventArgs, Exception> logErrorAction;
        private readonly Action<ILogger, SerialDevice, string, Exception> logDebugAction;

        public event EventHandler<RawDataReceicedEventArgs> RawDataReceived;
        public event EventHandler<DataReceicedEventArgs> DataReceived;
        public event EventHandler<SerialErrorReceivedEventArgs> ErrorReceived;

        public SerialDevice(string portName = "", int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One, Handshake handshake = Handshake.None, bool dataTerminalReady = false, bool requestToSend = false, bool discardNull = false)
        {
            if (String.IsNullOrEmpty(portName))
            {
                var ports = SerialPort.GetPortNames();
                portName = ports.Length > 0 ? ports[0] : throw new InvalidOperationException("No serial port found.");
            }

            PortName = portName;
            comPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits)
            {
                Handshake = handshake,
                DtrEnable = dataTerminalReady,
                RtsEnable = requestToSend,
                DiscardNull = discardNull
            };
            SetTimeout();

            logErrorAction = LoggerMessage.Define<SerialDevice, SerialErrorReceivedEventArgs>(LogLevel.Error, new EventId(LogEventConstants.SerialErrorReceivedEventId, nameof(SerialDevice)), LogEventConstants.SerialErrorReceivedFormatMessage);
            logDebugAction = LoggerMessage.Define<SerialDevice, string>(LogLevel.Debug, new EventId(LogEventConstants.SerialDebugReceivedEventId, nameof(SerialDevice)), LogEventConstants.SerialDebugReceivedFormatMessage);
        }

        public void SetTimeout(int timeout = Constants.ReadWriteTimeout)
        {
            comPort.ReadTimeout = timeout;
            comPort.WriteTimeout = timeout;
        }

        public bool AppendCarriageReturn { get; set; }

        public bool AppendLineFeed { get; set; }

        public int BytesToRead => comPort.IsOpen ? comPort.BytesToRead : 0;

        public Encoding Encoding { get; set; } = Encoding.UTF8;

        public ILogger Logger { get; set; }

        public string PortName { get; private set; }

        ~SerialDevice()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Interlocked.Exchange(ref threadSafeDisposing, 1) != 0)
            {
                return;
            }

            if (comPort.IsOpen)
            {
                Disconnect();
            }
            if (disposing)
            {
                DisposeManagedResources();
                comPort.Dispose();
            }
            DisposeUnmanagedResources();
        }

        protected virtual void DisposeManagedResources()
        {

        }

        protected virtual void DisposeUnmanagedResources()
        {

        }

        public void Connect(bool subscribeToDefaultEvents = true)
        {
            lock (connectionLock)
            {
                LogDebug("Connecting...");
                if (!comPort.IsOpen)
                {
                    LogDebug($"{PortName} is not open, trying to open it...");
                    if (subscribeToDefaultEvents)
                    {
                        LogDebug("Subscribing to default events...");
                        comPort.DataReceived += ComPortDataReceived;
                        comPort.ErrorReceived += ComPortErrorReceived;
                    }
                    LogDebug($"{PortName} opening...");
                    comPort.Open();
                    LogDebug($"{PortName} opened...");
                }
                else
                {
                    LogDebug($"{PortName} is already opened");
                }
            }
        }

        public void Disconnect()
        {
            lock (connectionLock)
            {
                LogDebug("Unsubscribing from default events...");
                comPort.DataReceived -= ComPortDataReceived;
                comPort.ErrorReceived -= ComPortErrorReceived;
                if (comPort.IsOpen)
                {
                    LogDebug("BaseStream flush...");
                    comPort.BaseStream.Flush();
                    LogDebug("Discarding buffers...");
                    comPort.DiscardInBuffer();
                    comPort.DiscardOutBuffer();
                    LogDebug($"{PortName} closing...");
                    comPort.Close();
                    comPort.Close();
                    LogDebug($"{PortName} closed...");
                }
                else
                {
                    logDebugAction(Logger, this, $"{PortName} is already closed", null);
                }
            }
        }

        public void SetNewLine(string newLine)
        {
            comPort.NewLine = newLine;
        }

        public string Read()
        {
            return Read(Encoding);
        }

        public string Read(Encoding encoding)
        {
            ValidateEncoding(encoding);

            var resultBuilder = new StringBuilder();
            var buffer = new byte[Constants.BufferSize];

            while (comPort.IsOpen && comPort.BytesToRead > 0)
            {
                var bytesRead = Read(buffer);
                _ = resultBuilder.Append(encoding.GetString(buffer, Constants.DefaultBufferStartIndex, bytesRead));
            }
            var result = resultBuilder.ToString();
            OnDataReceived(new DataReceicedEventArgs { Data = result });
            return result;
        }

        public int Read(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            var readBytes = comPort.Read(buffer, Constants.DefaultBufferStartIndex, buffer.Length);
            if (RawDataReceived != null)
            {
                var result = new byte[readBytes];
                Buffer.BlockCopy(buffer, Constants.DefaultBufferStartIndex, result, Constants.DefaultBufferStartIndex, readBytes);
                OnRawDataReceived(new RawDataReceicedEventArgs { Data = result });
            }
            return readBytes;
        }

        public void WriteLine(string message)
        {
            comPort.WriteLine(message);
        }

        public string ReadLine()
        {
            return comPort.ReadLine();
        }

        public string WriteAndRead(string message)
        {
            Write(message, Encoding);
            return Read();
        }

        public void Write(string message)
        {
            Write(message, Encoding);
        }

        public Task WriteAsync(string message)
        {
            return WriteAsync(message, Encoding);
        }

        public byte[] GetBytes(string message, Encoding encoding)
        {
            ValidateEncoding(encoding);

            var bytes = encoding.GetBytes(message);

            var extraBytes = (AppendCarriageReturn ? 1 : 0) + (AppendLineFeed ? 1 : 0);
            if (extraBytes == 0)
            {
                return bytes;
            }

            var extendedBytes = new byte[bytes.Length + extraBytes];
            Buffer.BlockCopy(bytes, Constants.DefaultBufferStartIndex, extendedBytes, Constants.DefaultBufferStartIndex, bytes.Length);

            var index = bytes.Length;
            if (AppendCarriageReturn)
            {
                extendedBytes[index++] = Constants.CarriageReturn;
            }
            if (AppendLineFeed)
            {
                extendedBytes[index] = Constants.LineFeed;
            }
            return extendedBytes;
        }

        public void Write(string message, Encoding encoding)
        {
            var bytes = GetBytes(message, encoding);
            Write(bytes, Constants.DefaultBufferStartIndex, bytes.Length);
        }

        public Task WriteAsync(string message, Encoding encoding)
        {
            var bytes = GetBytes(message, encoding);
            return WriteAsync(bytes, Constants.DefaultBufferStartIndex, bytes.Length);
        }

        public void Write(byte[] buffer)
        {
            ValidateBuffer(buffer);
            comPort.Write(buffer, Constants.DefaultBufferStartIndex, buffer.Length);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            comPort.Write(buffer, offset, count);
        }

        public Task WriteAsync(byte[] buffer)
        {
            ValidateBuffer(buffer);
            return comPort.BaseStream.WriteAsync(buffer, Constants.DefaultBufferStartIndex, buffer.Length);
        }

        public Task WriteAsync(byte[] buffer, int offset, int count)
        {
            return comPort.BaseStream.WriteAsync(buffer, offset, count);
        }

        public override bool Equals(object obj)
        {
            return obj is SerialDevice serialDevice && PortName == serialDevice.PortName;
        }

        public override int GetHashCode() => PortName.GetHashCode();

        public override string ToString()
        {
            return $"{GetType()} {PortName}";
        }

        protected virtual void ComPortErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            OnErrorReceived(e);
        }

        protected virtual async void ComPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            _ = await Task.Run(() => _ = Read()).ConfigureAwait(false);
        }

        protected virtual void OnRawDataReceived(RawDataReceicedEventArgs e) => RawDataReceived?.Invoke(this, e);

        protected virtual void OnDataReceived(DataReceicedEventArgs e) => DataReceived?.Invoke(this, e);

        protected virtual void OnErrorReceived(SerialErrorReceivedEventArgs e)
        {
            ErrorReceived?.Invoke(this, e);
            LogError(e);
        }

        protected void LogError(SerialErrorReceivedEventArgs e)
        {
            if (Logger != null)
            {
                logErrorAction(Logger, this, e, null);
            }
        }

        protected void LogDebug(string message)
        {
            if (Logger != null)
            {
                logDebugAction(Logger, this, message, null);
            }
        }

        private static void ValidateBuffer(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
        }

        private static void ValidateEncoding(Encoding encoding)
        {
            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }
        }
    }
}
