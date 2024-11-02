# Mtf.Serial Library Documentation

## Overview

The `Mtf.Serial` library offers a `SerialDevice` class designed for robust serial communication with configuration options for port parameters, enhanced data reception handling, and logging integration using `ILogger`. The library provides both synchronous and asynchronous methods for data handling, connection management, and error tracking, making it well-suited for .NET applications requiring reliable serial communications.

## Installation

1. **Install the Package**:
   Run the following command in your project directory:

   ```bash
   dotnet add package Mtf.Serial
   ```

2. **Add the Namespace**:
   Include the `Mtf.Serial` namespace in your code file:

   ```csharp
   using Mtf.Serial;
   ```

## Class: SerialDevice

The `SerialDevice` class simplifies serial port communication and configuration, with customizable settings for port name, baud rate, encoding, and connection settings. Logging actions are included to assist with error and debug tracking.

### Constructor

**`SerialDevice(string portName = "", int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One, Handshake handshake = Handshake.None, bool dataTerminalReady = false, bool requestToSend = false, bool discardNull = false)`**

- **Parameters**:
  - `portName`: The serial port name (e.g., "COM1"). Defaults to the first available port if unspecified.
  - `baudRate`: Communication speed. Default is 9600.
  - Additional parameters configure parity, data bits, stop bits, handshake, DTR/RTS, and null discarding options.

### Properties

| Property             | Type        | Description                                                                          |
|----------------------|-------------|--------------------------------------------------------------------------------------|
| `AppendCarriageReturn` | `bool`  | Appends a carriage return (`\r`) to outgoing messages if set to `true`.              |
| `AppendLineFeed`    | `bool`      | Appends a line feed (`\n`) to outgoing messages if set to `true`.                    |
| `Encoding`          | `Encoding`  | Defines the character encoding for messages (default is `UTF-8`).                    |
| `Logger`            | `ILogger`   | An `ILogger` instance for logging serial events and errors.                          |
| `PortName`          | `string`    | The name of the connected serial port.                                               |
| `BytesToRead`       | `int`       | Number of bytes available to read from the buffer.                                   |

### Methods

#### Connection Management

- **`void Connect(bool subscribeToDefaultEvents = true)`**  
  Connects to the serial port. If `subscribeToDefaultEvents` is `true`, it subscribes to default data and error event handlers.

- **`void Disconnect()`**  
  Disconnects from the serial port and unsubscribes from event handlers.

- **`Dispose()`**  
  Releases all resources used by `SerialDevice` and disconnects if connected. The destructor also invokes cleanup to ensure proper disposal.

#### Data Transmission

- **`string Read()`**  
  Reads available data from the port as a string, using the set encoding.

- **`string Read(Encoding encoding)`**  
  Reads data with the specified encoding.

- **`int Read(byte[] buffer)`**  
  Reads data into a byte array and returns the number of bytes read.

- **`void Write(byte[] buffer)`**  
  Writes a byte array to the serial port without any modifications.

- **`void WriteAsync(byte[] buffer)`**  
  Asynchronously writes a byte array to the serial port without any modifications.

- **`void Write(byte[] buffer, int offset, int count)`**  
  Writes a specified number of bytes from the byte array, starting at the given offset, to the serial port without any modifications.

- **`void WriteAsync(byte[] buffer, int offset, int count)`**  
  Asynchronously writes a specified number of bytes from the byte array, starting at the given offset, to the serial port without any modifications.

- **`void Write(string message)`**  
  Writes a string message to the serial port using the specified encoding. If `AppendCarriageReturn` or `AppendLineFeed` is set, it will modify the sent bytes accordingly."

- **`Task WriteAsync(string message)`**  
  Writes a message asynchronously to the serial port. If `AppendCarriageReturn` or `AppendLineFeed` is set, it will modify the sent bytes accordingly."

- **`void Write(string message, Encoding encoding)`**  
  Writes a string message with specified encoding. If `AppendCarriageReturn` or `AppendLineFeed` is set, it will modify the sent bytes accordingly."

- **`Task WriteAsync(string message, Encoding encoding)`**  
  Writes a message asynchronously with specified encoding. If `AppendCarriageReturn` or `AppendLineFeed` is set, it will modify the sent bytes accordingly."

#### Event Handling

| Event             | Description                                               |
|-------------------|-----------------------------------------------------------|
| `RawDataReceived` | Triggered when raw binary data is received.               |
| `DataReceived`    | Triggered when processed data is available.               |
| `ErrorReceived`   | Triggered when an error occurs during serial communication.|

### Logging

The `SerialDevice` class supports logging through an `ILogger` instance:
- **Error Logging**: Logs errors during communication with `logErrorAction`.
- **Debug Logging**: Logs debug messages for connection and data events using `logDebugAction`.

### Example Usage

```csharp
using Mtf.Serial;
using Microsoft.Extensions.Logging;
using System.Text;

public class SerialCommunicationExample
{
    public void Example()
    {
        // Initialize SerialDevice with default settings
        var serialDevice = new SerialDevice("COM3", 9600)
        {
            Logger = new ConsoleLogger<SerialDevice>()
        };

        // Subscribe to events
        serialDevice.RawDataReceived += (sender, args) =>
        {
            Console.WriteLine($"Raw data received: {BitConverter.ToString(args.Data)}");
        };
        serialDevice.DataReceived += (sender, args) =>
        {
            Console.WriteLine($"Data received: {args.Data}");
        };
        serialDevice.ErrorReceived += (sender, args) =>
        {
            Console.WriteLine($"Error received: {args}");
        };

        // Connect and write data
        serialDevice.Connect(true);
        serialDevice.Write("Hello, Serial Port!", Encoding.UTF8);

        // Read response
        var response = serialDevice.Read();
        Console.WriteLine($"Response: {response}");

        // Disconnect from the device
        serialDevice.Disconnect();
    }
}
```

### Notes

- **Logging**: Use the `Logger` property to capture and log serial events for monitoring.
- **Thread Safety**: The `SerialDevice` class ensures thread safety with a connection lock.
- **Error Handling**: Ensure exception handling for port access errors and communication issues.
