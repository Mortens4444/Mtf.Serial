using Mtf.Serial.Models;
using Mtf.WmiHelper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mtf.Serial.Services
{
    public static class SerialDeviceInfoProvider
    {
        public static IEnumerable<SerialDeviceProperties> GetSerialDevices()
        {
            var comPorts = Wmi.WmiGetObjects("SELECT Availability, Caption, ClassGuid, CompatibleID, ConfigManagerErrorCode, ConfigManagerUserConfig, CreationClassName, Description, DeviceID, ErrorCleared, ErrorDescription, HardwareID, InstallDate, LastErrorCode, Manufacturer, Name, PNPClass, PNPDeviceID, PowerManagementCapabilities, PowerManagementSupported, Present, Service, Status, StatusInfo, SystemCreationClassName, SystemName FROM Win32_PnPEntity WHERE Name LIKE '% (COM%' AND Status = 'OK'", "cimv2");
            return comPorts.Select(comPort => comPort.ToList()).Select(port =>
                new SerialDeviceProperties
                {
                    Availability = port[0] as int?,
                    Caption = port[1]?.ToString(),
                    ClassGuid = port[2]?.ToString(),
                    CompatibleID = port[3] as string[],
                    ConfigManagerErrorCode = port[4] as int?,
                    ConfigManagerUserConfig = port[5] as bool?,
                    CreationClassName = port[6]?.ToString(),
                    Description = port[7]?.ToString(),
                    DeviceID = port[8]?.ToString(),
                    ErrorCleared = port[9] as bool?,
                    ErrorDescription = port[10]?.ToString(),
                    HardwareID = port[11] as string[],
                    InstallDate = port[12] as DateTime?,
                    LastErrorCode = port[13] as int?,
                    Manufacturer = port[14]?.ToString(),
                    Name = port[15]?.ToString(),
                    PNPClass = port[16]?.ToString(),
                    PNPDeviceID = port[17]?.ToString(),
                    PowerManagementCapabilities = port[18] as int[],
                    PowerManagementSupported = port[19] as bool?,
                    Present = port[20] as bool?,
                    Service = port[21]?.ToString(),
                    Status = port[22]?.ToString(),
                    StatusInfo = port[23] as int?,
                    SystemCreationClassName = port[24]?.ToString(),
                    SystemName = port[25]?.ToString(),
                });
        }
    }
}
