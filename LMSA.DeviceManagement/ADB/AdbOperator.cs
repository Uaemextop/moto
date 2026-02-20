using System;
using System.Collections.Generic;
using System.Linq;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt.DeviceOperator;

public class AdbOperator : IDeviceOperator
{
    private static AdbOperator? _instance;

    public static AdbOperator Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }
            return _instance = new AdbOperator();
        }
    }

    public string Command(string command, int timeout = -1, string deviceID = "")
    {
        string adbPath = Configurations.AdbPath;
        string command2 = command;
        if (!string.IsNullOrEmpty(deviceID) && string.Compare(deviceID, "UNKNOWN", ignoreCase: true) != 0)
        {
            command2 = "-s " + deviceID + " " + command;
        }
        return ProcessRunner.ProcessString(adbPath, command2, timeout);
    }

    public string Shell(string deviceID, string command)
    {
        string shellCommand = "shell " + command;
        return Command(shellCommand, -1, deviceID);
    }

    public void Install(string deviceID, string apkPath)
    {
        Command($"install -r \"{apkPath}\"", 20000, deviceID);
    }

    public void Uninstall(string deviceID, string apkName)
    {
        Command("uninstall " + apkName, 20000, deviceID);
    }

    public void ForwardPort(string deviceID, int devicePort, int localPort)
    {
        Command($"forward tcp:{localPort} tcp:{devicePort}", -1, deviceID);
    }

    public void RemoveForward(string deviceID, int localPort)
    {
        Command($"forward --remove tcp:{localPort}", -1, deviceID);
    }

    public void RemoveAllForward(string deviceID)
    {
        Command("forward --remove-all", -1, deviceID);
    }

    public void PushFile(string deviceID, string localFilePath, string deviceFilePath)
    {
        Command($"push \"{localFilePath}\" \"{deviceFilePath}\"", -1, deviceID);
    }

    public void Reboot(string deviceID, string mode)
    {
        if (string.IsNullOrEmpty(mode))
        {
            Command("reboot", -1, deviceID);
        }
        else
        {
            Command("reboot " + mode, -1, deviceID);
        }
    }

    public List<string> FindDevices()
    {
        string text = Command("devices", 5000);
        List<string> list = new List<string>();
        if (string.IsNullOrEmpty(text))
        {
            return list;
        }
        string[] array = text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string line in array)
        {
            if (line.StartsWith("List of"))
            {
                continue;
            }
            string[] parts = line.Split(new char[] { '\t' });
            if (parts.Length >= 2)
            {
                string serial = parts[0].Trim();
                if (!string.IsNullOrEmpty(serial) && !serial.StartsWith("emulator-"))
                {
                    list.Add(serial);
                }
            }
        }
        return list;
    }
}
