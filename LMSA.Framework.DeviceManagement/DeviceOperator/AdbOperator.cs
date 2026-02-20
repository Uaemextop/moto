using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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
        if (string.IsNullOrEmpty(deviceID))
        {
            return;
        }

        string command = $"-s {deviceID} forward tcp:{localPort} tcp:{devicePort}";
        Command(command, 5000, deviceID);
    }

    public void Install(string deviceID, string apkPath)
    {
        if (string.IsNullOrEmpty(deviceID) || !File.Exists(apkPath))
        {
            return;
        }

        string command = $"-s {deviceID} install -r \"{apkPath}\"";
        Command(command, 30000, deviceID);
    }

    public void PushFile(string deviceID, string localFilePath, string deviceFilePath)
    {
        if (string.IsNullOrEmpty(deviceID) || !File.Exists(localFilePath))
        {
            return;
        }

        string command = $"-s {deviceID} push \"{localFilePath}\" \"{deviceFilePath}\"";
        Command(command, 60000, deviceID);
    }

    public void Reboot(string deviceID, string mode)
    {
        if (string.IsNullOrEmpty(deviceID))
        {
            return;
        }

        string command = $"-s {deviceID} reboot";
        if (!string.IsNullOrEmpty(mode))
        {
            command += $" {mode}";
        }
        Command(command, 5000, deviceID);
    }

    public void RemoveForward(string deviceID, int localPort)
    {
        if (string.IsNullOrEmpty(deviceID))
        {
            return;
        }

        string command = $"-s {deviceID} forward --remove tcp:{localPort}";
        Command(command, 5000, deviceID);
    }

    public void RemoveAllForward(string deviceID)
    {
        if (string.IsNullOrEmpty(deviceID))
        {
            return;
        }

        string command = $"-s {deviceID} forward --remove-all";
        Command(command, 5000, deviceID);
    }

    public string Shell(string deviceID, string command)
    {
        if (string.IsNullOrEmpty(deviceID))
        {
            return "failed";
        }

        try
        {
            string cmd = $"-s {deviceID} shell {command}";
            string result = Command(cmd, 10000, deviceID);
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
        if (string.IsNullOrEmpty(deviceID) || string.IsNullOrEmpty(apkName))
        {
            return;
        }

        string command = $"-s {deviceID} uninstall {apkName}";
        Command(command, 20000, deviceID);
    }

    public string Command(string command, int timeout = -1, string deviceID = "")
    {
        string adbPath = Configurations.AdbPath;
        string command2 = command;
        if (!string.IsNullOrEmpty(deviceID) && string.Compare(deviceID, "UNKNOWN", ignoreCase: true) != 0)
        {
            if (!command.StartsWith("-s"))
            {
                command2 = "-s " + deviceID + " " + command;
            }
        }
        return ProcessRunner.ProcessString(adbPath, command2, timeout);
    }

    public List<string> FindDevices()
    {
        List<string> devices = new List<string>();
        try
        {
            string result = Command("devices", 5000);
            if (string.IsNullOrEmpty(result))
            {
                return devices;
            }

            string[] lines = result.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                if (line.Contains("\t"))
                {
                    string[] parts = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2 && !string.IsNullOrEmpty(parts[0]))
                    {
                        string serial = parts[0].Trim();
                        if (!serial.Contains("List") && !serial.Contains("*") && serial.Length > 0)
                        {
                            devices.Add(serial);
                        }
                    }
                }
            }
        }
        catch
        {
            return devices;
        }

        return devices;
    }
}
