using System;
using System.Collections.Generic;
using System.IO;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt.DeviceOperator;

public class FastbootOperator : IDeviceOperator
{
    public static string fastbootExe = Path.Combine(".", "fastboot.exe");

    public static string fastbootMonitorExe = Path.Combine(".", "fastbootmonitor.exe");

    public string Command(string command, int timeout = -1, string deviceID = "")
    {
        string fullCommand = command;
        if (!string.IsNullOrEmpty(deviceID) && string.Compare(deviceID, "UNKNOWN", ignoreCase: true) != 0)
        {
            fullCommand = "-s " + deviceID + " " + command;
        }
        return ProcessRunner.ProcessString(Configurations.FastbootPath, fullCommand, timeout);
    }

    public List<string> FindDevices()
    {
        return FindDevices(fastbootMonitorExe);
    }

    public List<string> FindDevices(string exe)
    {
        string text = ProcessRunner.ProcessString(exe, "devices", 2000);
        List<string> list = new List<string>();
        string[] lines = text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(new char[] { '\t' });
            if (parts.Length >= 2)
            {
                string serial = parts[0].Trim();
                if (!serial.Contains("??????"))
                {
                    list.Add(serial);
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
