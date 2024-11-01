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

        public event EventHandler<RawDataReceicedEventArgs> RawDataReceived;
        public event EventHandler<DataReceicedEventArgs> DataReceived;
        public event EventHandler<SerialErrorReceivedEventArgs> ErrorReceived;

        public SerialDevice(string portName = "", int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One, Handshake handshake = Handshake.None, bool dataTerminalReady = false, bool requestToSend = false)
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
                ReadTimeout = Constants.ReadWriteTimeout,
                WriteTimeout = Constants.ReadWriteTimeout,
                DtrEnable = dataTerminalReady,
                RtsEnable = requestToSend
            };

            logErrorAction = LoggerMessage.Define<SerialDevice, SerialErrorReceivedEventArgs>(LogLevel.Error, new EventId(LogEventConstants.SerailErrorReceivedEventId, nameof(SerialDevice)), LogEventConstants.SerailErrorReceivedFormatMessage);
        }

        public bool AppendCarriageReturn { get; set; }

        public bool AppendLineFeed { get; set; }

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

            Disconnect();
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

        public void Connect(bool subscribeToDefaultEvents)
        {
            lock (connectionLock)
            {
                if (!comPort.IsOpen)
                {
                    if (subscribeToDefaultEvents)
                    {
                        comPort.DataReceived += ComPortDataReceived;
                        comPort.ErrorReceived += ComPortErrorReceived;
                    }
                    comPort.Open();
                }
            }
        }

        public void Disconnect()
        {
            lock (connectionLock)
            {
                comPort.DataReceived -= ComPortDataReceived;
                comPort.ErrorReceived -= ComPortErrorReceived;
                if (comPort.IsOpen)
                {
                    comPort.DiscardInBuffer();
                    comPort.DiscardOutBuffer();
                    comPort.Close();
                }
            }
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

        public void Write(string message)
        {
            Write(message, Encoding);
        }

        public Task WriteAsync(string message)
        {
            return Task.Run(() => Write(message, Encoding));
        }

        public void Write(string message, Encoding encoding)
        {
            ValidateEncoding(encoding);

            var bytes = encoding.GetBytes(message);

            var extraBytes = (AppendCarriageReturn ? 1 : 0) + (AppendLineFeed ? 1 : 0);
            if (extraBytes > 0)
            {
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
                Write(extendedBytes, Constants.DefaultBufferStartIndex, extendedBytes.Length);
            }
            else
            {
                Write(bytes, Constants.DefaultBufferStartIndex, bytes.Length);
            }
        }

        public Task WriteAsync(string message, Encoding encoding)
        {
            return Task.Run(() => Write(message, encoding));
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            comPort.Write(buffer, offset, count);
        }

        public Task WriteAsync(byte[] buffer, int offset, int count)
        {
            return Task.Run(() => Write(buffer, offset, count));
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
            logErrorAction(Logger, this, e, null);
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
