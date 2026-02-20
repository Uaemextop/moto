using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using SharpAdbClient;
using SharpAdbClient.DeviceCommands;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt.DeviceOperator;

public class AdbOperator : IDeviceOperator
{
    private static AdbOperator? _instance;

    protected IAdbClient adb = new AdbClient();

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
        DeviceData? deviceData = FindDeviceData(deviceID);
        if (deviceData != null)
        {
            string local = $"tcp:{localPort}";
            string remote = $"tcp:{devicePort}";
            adb.CreateForward(deviceData, local, remote, allowRebind: false);
        }
    }

    public void Install(string deviceID, string apkPath)
    {
        DeviceData? deviceData = FindDeviceData(deviceID);
        if (deviceData != null)
        {
            using Stream apkStream = File.OpenRead(apkPath);
            adb.Install(deviceData, apkStream);
        }
    }

    public void PushFile(string deviceID, string localFilePath, string deviceFilePath)
    {
        DeviceData? deviceData = FindDeviceData(deviceID);
        if (deviceData == null)
        {
            return;
        }
        using SyncService syncService = new SyncService(deviceData);
        using Stream stream = File.OpenRead(localFilePath);
        syncService.Push(stream, deviceFilePath, 777, DateTime.Now, null, CancellationToken.None);
    }

    public void Reboot(string deviceID, string mode)
    {
        DeviceData? deviceData = FindDeviceData(deviceID);
        if (deviceData != null)
        {
            adb.Reboot(mode, deviceData);
        }
    }

    public void RemoveForward(string deviceID, int localPort)
    {
        DeviceData? deviceData = FindDeviceData(deviceID);
        if (deviceData != null)
        {
            adb.RemoveForward(deviceData, localPort);
        }
    }

    public void RemoveAllForward(string deviceID)
    {
        DeviceData? deviceData = FindDeviceData(deviceID);
        if (deviceData != null)
        {
            adb.RemoveAllForwards(deviceData);
        }
    }

    public string Shell(string deviceID, string command)
    {
        ConsoleOutputReceiver consoleOutputReceiver = new ConsoleOutputReceiver();
        DeviceData? deviceData = FindDeviceData(deviceID);
        if (deviceData == null)
        {
            return "failed";
        }
        try
        {
            adb.ExecuteRemoteCommandAsync(command, deviceData, consoleOutputReceiver, default(CancellationToken), -1).Wait();
            string text = consoleOutputReceiver.ToString();
            if (text.EndsWith("\r\n"))
            {
                text = text.Remove(text.LastIndexOf("\r\n"));
            }
            return text;
        }
        catch
        {
            return "failed";
        }
    }

    public void Uninstall(string deviceID, string apkName)
    {
        DeviceData? deviceData = FindDeviceData(deviceID);
        if (deviceData != null)
        {
            Shell(deviceID, $"pm uninstall {apkName}");
        }
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

    public DeviceData? FindDeviceData(string deviceID)
    {
        return FindAdbDevices().FirstOrDefault(n => n.Serial == deviceID);
    }

    public List<string> FindDevices()
    {
        return FindAdbDevices().Select(n => n.Serial).ToList();
    }

    public List<DeviceData> FindAdbDevices()
    {
        try
        {
            if (!AdbServer.Instance.GetStatus().IsRunning)
            {
                AdbServer.Instance.StartServer("adb.exe", restartServerIfNewer: true);
            }
            List<DeviceData>? list = adb.GetDevices();
            if (list == null)
            {
                list = new List<DeviceData>();
            }
            return list.Where(n => !string.IsNullOrEmpty(n.Serial)).ToList();
        }
        catch
        {
            return new List<DeviceData>();
        }
    }
}
