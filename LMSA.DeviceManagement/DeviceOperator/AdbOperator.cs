using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt.DeviceOperator;

/// <summary>
/// ADB device operator for Android device communication.
/// Implements IDeviceOperator using external adb.exe process execution.
/// </summary>
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

    public void ForwardPort(string deviceID, int devicePort, int localPort)
    {
        string local = $"tcp:{localPort}";
        string remote = $"tcp:{devicePort}";
        Command($"forward {local} {remote}", -1, deviceID);
    }

    public void Install(string deviceID, string apkPath)
    {
        Command($"install -r \"{apkPath}\"", -1, deviceID);
    }

    public void PushFile(string deviceID, string localFilePath, string deviceFilePath)
    {
        Command($"push \"{localFilePath}\" \"{deviceFilePath}\"", -1, deviceID);
    }

    public void Reboot(string deviceID, string mode)
    {
        if (string.IsNullOrEmpty(mode))
        {
            Command("reboot", 20000, deviceID);
        }
        else
        {
            Command($"reboot {mode}", 20000, deviceID);
        }
    }

    public void RemoveForward(string deviceID, int localPort)
    {
        Command($"forward --remove tcp:{localPort}", -1, deviceID);
    }

    public void RemoveAllForward(string deviceID)
    {
        Command("forward --remove-all", -1, deviceID);
    }

    public string Shell(string deviceID, string command)
    {
        try
        {
            string result = Command($"shell {command}", -1, deviceID);
            if (result.EndsWith("\r\n"))
            {
                result = result.Remove(result.LastIndexOf("\r\n"));
            }
            return result;
        }
        catch
        {
            return "failed";
        }
    }

    public void Uninstall(string deviceID, string apkName)
    {
        Command($"uninstall {apkName}", -1, deviceID);
    }

    public string Command(string command, int timeout = -1, string deviceID = "")
    {
        string adbPath = Configurations.AdbPath;
        string fullCommand = command;
        if (!string.IsNullOrEmpty(deviceID) && string.Compare(deviceID, "UNKNOWN", ignoreCase: true) != 0)
        {
            fullCommand = "-s " + deviceID + " " + command;
        }
        return ProcessRunner.ProcessString(adbPath, fullCommand, timeout);
    }

    public List<string> FindDevices()
    {
        string result = Command("devices", 5000);
        List<string> devices = new List<string>();
        string[] lines = result.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string line in lines)
        {
            if (line.StartsWith("List of") || string.IsNullOrWhiteSpace(line))
            {
                continue;
            }
            string[] parts = line.Split(new char[] { '\t' });
            if (parts.Length >= 2)
            {
                string serial = parts[0].Trim();
                if (!string.IsNullOrEmpty(serial))
                {
                    devices.Add(serial);
                }
            }
        }
        return devices;
    }
}
