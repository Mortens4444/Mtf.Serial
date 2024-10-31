using Mtf.Serial.Services;
using System.IO.Ports;

Console.WriteLine($"SerialDeviceInfoProvider.GetSerialDevices()");
Console.WriteLine($"-------------------------------------------");
var devices = SerialDeviceInfoProvider.GetSerialDevices();
foreach (var device in devices)
{
    Console.WriteLine($"{device}");
}

Console.WriteLine();
Console.WriteLine($"SerialPort.GetPortNames()");
Console.WriteLine($"-------------------------");
foreach (var device in SerialPort.GetPortNames())
{
    Console.WriteLine(device);
}