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
        string command2 = command;
        if (!string.IsNullOrEmpty(deviceID) && string.Compare(deviceID, "UNKNOWN", ignoreCase: true) != 0)
        {
            command2 = "-s " + deviceID + " " + command;
        }
        return ProcessRunner.ProcessString(Configurations.FastbootPath, command2, timeout);
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
            string[] array2 = array[i].Split(new char[] { '\t' });
            if (array2.Length >= 2)
            {
                string text2 = array2[0].Trim();
                if (!text2.Contains("??????"))
                {
                    list.Add(text2);
                }
            }
        }
        return list;
    }

    public void ForwardPort(string deviceID, int devicePort, int localPort)
    {
        throw new NotImplementedException("Fastboot does not support port forwarding");
    }

    public void Install(string deviceID, string apkPath)
    {
        throw new NotImplementedException("Fastboot does not support app installation");
    }

    public void PushFile(string deviceID, string localFilePath, string deviceFilePath)
    {
        throw new NotImplementedException("Fastboot does not support file push");
    }

    public void Reboot(string deviceID, string mode)
    {
        if (string.IsNullOrEmpty(mode))
        {
            Command("reboot", 20000, deviceID);
        }
        else if (mode.Equals("bootloader", StringComparison.OrdinalIgnoreCase))
        {
            Command("reboot-bootloader", 10000, deviceID);
        }
        else
        {
            Command("reboot", 20000, deviceID);
        }
    }

    public void RemoveForward(string deviceID, int localPort)
    {
        throw new NotImplementedException("Fastboot does not support port forwarding");
    }

    public string Shell(string deviceID, string command)
    {
        throw new NotImplementedException("Fastboot does not support shell commands");
    }

    public void Uninstall(string deviceID, string apkName)
    {
        throw new NotImplementedException("Fastboot does not support app uninstallation");
    }

    public void RemoveAllForward(string deviceID)
    {
        throw new NotImplementedException("Fastboot does not support port forwarding");
    }

    public string Flash(string partition, string imagePath, int timeout = 300000, string deviceID = "")
    {
        return Command($"flash {partition} \"{imagePath}\"", timeout, deviceID);
    }

    public string Erase(string partition, int timeout = 60000, string deviceID = "")
    {
        return Command($"erase {partition}", timeout, deviceID);
    }

    public string Format(string partition, int timeout = 60000, string deviceID = "")
    {
        return Command($"format {partition}", timeout, deviceID);
    }

    public string GetVar(string variable, int timeout = 20000, string deviceID = "")
    {
        return Command($"getvar {variable}", timeout, deviceID);
    }

    public string OemCommand(string oemCommand, int timeout = 60000, string deviceID = "")
    {
        return Command($"oem {oemCommand}", timeout, deviceID);
    }

    public string Continue(int timeout = 10000, string deviceID = "")
    {
        return Command("continue", timeout, deviceID);
    }
}
