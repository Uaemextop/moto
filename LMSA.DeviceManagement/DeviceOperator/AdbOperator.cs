using System;
using System.Collections.Generic;
using System.Linq;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt.DeviceOperator;

public class AdbOperator : IDeviceOperator
{
    private static AdbOperator _instance;

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
        Command($"forward tcp:{localPort} tcp:{devicePort}", 5000, deviceID);
    }

    public void Install(string deviceID, string apkPath)
    {
        Command($"install -r \"{apkPath}\"", 60000, deviceID);
    }

    public void PushFile(string deviceID, string localFilePath, string deviceFilePath)
    {
        Command($"push \"{localFilePath}\" \"{deviceFilePath}\"", 60000, deviceID);
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
        Command($"forward --remove tcp:{localPort}", 5000, deviceID);
    }

    public void RemoveAllForward(string deviceID)
    {
        Command("forward --remove-all", 5000, deviceID);
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
        Command($"uninstall {apkName}", 60000, deviceID);
    }

    public string Command(string command, int timeout = -1, string deviceID = "")
    {
        string adbPath = Configurations.AdbPath;
        string command2 = command;
        if (!string.IsNullOrEmpty(deviceID) && string.Compare(deviceID, "UNKNOWN", ignoreCase: true) != 0)
        {
            command2 = "-s " + deviceID + " " + command;
        }
        LogHelper.LogInstance.Debug($"ADB command: {command2}");
        string response = ProcessRunner.ProcessString(adbPath, command2, timeout);
        LogHelper.LogInstance.Debug($"ADB response: {response}");
        return response;
    }

    public List<string> FindDevices()
    {
        string text = Command("devices", 5000);
        List<string> list = new List<string>();
        if (string.IsNullOrEmpty(text))
        {
            return list;
        }
        string[] lines = text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string line in lines)
        {
            if (line.StartsWith("List of devices") || line.StartsWith("*"))
            {
                continue;
            }
            string[] parts = line.Split(new char[] { '\t' });
            if (parts.Length >= 2)
            {
                string serial = parts[0].Trim();
                if (!string.IsNullOrEmpty(serial))
                {
                    list.Add(serial);
                }
            }
        }
        return list;
    }
}
