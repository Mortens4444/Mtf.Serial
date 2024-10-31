# Mtf.Serial Library Documentation

## Overview

The `Mtf.Serial` library provides a `SerialDevice` class designed for serial communication, including features for connecting to and configuring serial ports, handling data reception, and managing errors. This document covers the installation, properties, methods, events, and usage examples for effective serial communication in .NET applications.

## Installation

To install the `Mtf.Serial` package, use the following steps:

1. **Add Package**:
   Open the terminal in your project directory and run:

   ```bash
   dotnet add package Mtf.Serial
   ```

2. **Include the Namespace**:
   Add the `Mtf.Serial` namespace at the beginning of your code file:

   ```csharp
   using Mtf.Serial;
   ```

## Class: SerialDevice

The `SerialDevice` class facilitates serial communication with configurations for port name, baud rate, and data handling. This class also includes events to monitor data and errors and provides both synchronous and asynchronous methods for data reading and writing.

### Constructor

**`SerialDevice(string portName = "", int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One, Handshake handshake = Handshake.None, bool dataTerminalReady = false, bool requestToSend = false)`**

- **Parameters**:
  - `portName`: The serial port name (e.g., "COM1"). Defaults to the first available port if unspecified.
  - `baudRate`: Communication speed. Default is 9600.
  - `parity`, `dataBits`, `stopBits`, `handshake`, `dataTerminalReady`, and `requestToSend`: Optional settings to configure communication behavior.

### Properties

| Property             | Type        | Description                                                                          |
|----------------------|-------------|--------------------------------------------------------------------------------------|
| `AppendCarriageReturn` | `bool`  | Appends a carriage return (`\r`) to outgoing messages if set to `true`.              |
| `AppendLineFeed`    | `bool`      | Appends a line feed (`\n`) to outgoing messages if set to `true`.                    |
| `Encoding`          | `Encoding`  | Defines the character encoding for messages (default is `UTF-8`).                    |
| `Logger`            | `ILogger`   | An `ILogger` instance for logging serial events and errors.                          |
| `PortName`          | `string`    | The name of the connected serial port.                                               |

### Methods

#### Connection Management

- **`void Connect(bool subscribeToDefaultEvents)`**  
  Connects to the serial port. If `subscribeToDefaultEvents` is `true`, it subscribes to default data and error event handlers.

- **`void Disconnect()`**  
  Disconnects from the serial port and unsubscribes from event handlers.

#### Data Transmission

- **`string Read()`**  
  Reads available data from the port as a string.

- **`string Read(Encoding encoding)`**  
  Reads data using the specified encoding.

- **`int Read(byte[] buffer)`**  
  Reads data into a byte array and returns the number of bytes read.

- **`void Write(string message)`**  
  Writes a string message to the serial port.

- **`Task WriteAsync(string message)`**  
  Writes a message asynchronously to the serial port.

- **`void Write(string message, Encoding encoding)`**  
  Writes a string message with specified encoding.

- **`Task WriteAsync(string message, Encoding encoding)`**  
  Writes a message asynchronously with specified encoding.

#### Event Handling

The following events enable you to monitor serial communication:

| Event             | Description                                               |
|-------------------|-----------------------------------------------------------|
| `RawDataReceived` | Triggered when raw binary data is received.               |
| `DataReceived`    | Triggered when processed data is available.               |
| `ErrorReceived`   | Triggered when an error occurs during serial communication.|

### Example Usage

```csharp
using Mtf.Serial;
using System;
using System.Text;

public class SerialCommunicationExample
{
    public void Example()
    {
        // Initialize SerialDevice with default settings
        var serialDevice = new SerialDevice("COM3", 9600);

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

- Ensure exception handling for port access and communication errors.
- Use the `Logger` property to capture and log serial events for debugging and monitoring purposes.
