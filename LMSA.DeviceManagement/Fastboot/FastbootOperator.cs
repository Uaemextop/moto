using System;
using System.Collections.Generic;
using System.IO;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt.DeviceOperator;

/// <summary>
/// Provides fastboot device operations: command execution and device enumeration.
/// </summary>
public class FastbootOperator : IDeviceOperator
{
    public static string fastbootExe = Path.Combine(".", "fastboot.exe");

    public static string fastbootMonitorExe = Path.Combine(".", "fastbootmonitor.exe");

    public string Command(string command, int timeout = -1, string deviceID = "")
    {
        if (!string.IsNullOrEmpty(deviceID) && string.Compare(deviceID, "UNKNOWN", ignoreCase: true) != 0)
        {
            command = "-s " + deviceID + " " + command;
        }
        return ProcessRunner.ProcessString(Configurations.FastbootPath, command, timeout);
    }

    public List<string> FindDevices()
    {
        return FindDevices(fastbootMonitorExe);
    }

    public List<string> FindDevices(string exe)
    {
        string text = ProcessRunner.ProcessString(exe, "devices", 2000);
        List<string> list = new List<string>();
        string[] array = text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < array.Length; i++)
        {
            string[] parts = array[i].Split('\t');
            if (parts.Length >= 2)
            {
                string deviceId = parts[0].Trim();
                if (!deviceId.Contains("??????"))
                {
                    list.Add(deviceId);
                }
            }
        }
        return list;
    }

    public void ForwardPort(string deviceID, int devicePort, int localPort)
    {
        throw new NotImplementedException();
    }

    public void Install(string deviceID, string apkPath)
    {
        throw new NotImplementedException();
    }

    public void PushFile(string deviceID, string localFilePath, string deviceFilePath)
    {
        throw new NotImplementedException();
    }

    public void Reboot(string deviceID, string mode)
    {
        throw new NotImplementedException();
    }

    public void RemoveForward(string deviceID, int localPort)
    {
        throw new NotImplementedException();
    }

    public string Shell(string deviceID, string command)
    {
        throw new NotImplementedException();
    }

    public void Uninstall(string deviceID, string apkName)
    {
        throw new NotImplementedException();
    }

    public void RemoveAllForward(string deviceID)
    {
        throw new NotImplementedException();
    }
}
