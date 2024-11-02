using Microsoft.Extensions.Logging;
using Mtf.Serial.Models.Lego;
using Mtf.Serial.SerialDevices;
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

//var lego = new LegoMindstormsEv3("COM8")
//{
//    LeftMotor = OutputPort.B,
//    RightMotor = OutputPort.C
//};
//lego.Connect();
//lego.TurnLeft(50);
//Thread.Sleep(3000);
//lego.Stop();
//lego.Disconnect();

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Debug);
});

ILogger logger = loggerFactory.CreateLogger<SerialDevice>();


using (var lego = new LegoMindstormsEv3("COM8"))
{
    for (int i = 0; i < 100; i++)
    {
        lego.LeftMotor = OutputPort.B;
        lego.RightMotor = OutputPort.C;
        lego.Logger = logger;
        Console.WriteLine(i + 1);
        lego.Connect();
        lego.Disconnect();
    }
}