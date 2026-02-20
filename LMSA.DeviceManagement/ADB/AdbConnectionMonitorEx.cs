using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading;
using SharpAdbClient;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.devicemgt.DeviceOperator;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt;

/// <summary>
/// Monitors ADB device connections and disconnections.
/// Uses both SharpAdbClient device enumeration and WMI-based USB detection
/// for Lenovo/Motorola devices.
/// </summary>
public class AdbConnectionMonitorEx : IDisposable
{
    private class AdbScaner
    {
        private readonly AdbConnectionMonitorEx _outer;

        private readonly Dictionary<string, DeviceEx> _cache = new Dictionary<string, DeviceEx>();

        private volatile bool _isRunning;

        protected AdbOperator Operator = new AdbOperator();

        private static readonly string[] Manufacturers = new[] { "LENOVO", "MOTOROLA" };

        public AdbScaner(AdbConnectionMonitorEx outer)
        {
            _outer = outer;
        }

        public void Start()
        {
            _isRunning = true;
            _cache.Clear();
            List<DeviceDataEx> foundDevices = new List<DeviceDataEx>();
            while (_isRunning)
            {
                foundDevices.Clear();
                List<SharpAdbClient.DeviceData> list = Operator.FindAdbDevices();
                if (list.Count == 0)
                {
                    DeviceDataEx? wmiDevice = FindNewDeviceByWmi();
                    if (wmiDevice != null)
                    {
                        foundDevices.Add(wmiDevice);
                    }
                }
                else
                {
                    list.ForEach(n => foundDevices.Add(new DeviceDataEx
                    {
                        Data = n,
                        PhyState = (DevicePhysicalStateEx)(int)n.State
                    }));
                }
                foreach (DeviceDataEx item in foundDevices)
                {
                    if (!item.Data.Serial.StartsWith("emulator-"))
                    {
                        if (!_cache.ContainsKey(item.Data.Serial))
                        {
                            AdbDeviceEx adbDeviceEx = new AdbDeviceEx
                            {
                                ConnectType = ConnectType.Adb,
                                Identifer = item.Data.Serial,
                                ValidateCodeFunc = (_outer._listener as DeviceConnectionManagerEx)?.ValidateCodeFunc
                            };
                            _cache.Add(item.Data.Serial, adbDeviceEx);
                            _outer._listener.OnConnect(adbDeviceEx, item.PhyState);
                        }
                        else if (_cache[item.Data.Serial].IsRemove)
                        {
                            _cache[item.Data.Serial].IsRemove = false;
                            _outer._listener.OnConnect(_cache[item.Data.Serial], item.PhyState);
                        }
                        else
                        {
                            _cache[item.Data.Serial].PhysicalStatus = item.PhyState;
                        }
                    }
                }
                foreach (string id in _cache
                    .Where(n => !n.Value.IsRemove)
                    .Select(n => n.Key)
                    .Except(foundDevices.Select(n => n.Data.Serial))
                    .ToList())
                {
                    DeviceEx deviceEx = _cache[id];
                    if (deviceEx.WorkType == DeviceWorkType.None)
                    {
                        _cache.Remove(id);
                    }
                    else
                    {
                        deviceEx.IsRemove = true;
                    }
                    _outer._listener.OnDisconnect(deviceEx);
                }
                Thread.Sleep(1000);
            }
        }

        public void Stop()
        {
            _isRunning = false;
        }

        private DeviceDataEx? FindNewDeviceByWmi()
        {
#if WINDOWS
            try
            {
                string query = "SELECT * FROM Win32_PnPEntity WHERE ClassGuid='{eec5ad98-8080-425f-922a-dabf3de3f69a}'";
                using System.Management.ManagementObjectSearcher searcher = new System.Management.ManagementObjectSearcher(query);
                foreach (System.Management.ManagementObject obj in searcher.Get())
                {
                    object? manufacturer = obj["Manufacturer"];
                    object? pnpId = obj["PNPDeviceID"];
                    if (manufacturer != null && pnpId != null &&
                        Manufacturers.Contains(manufacturer.ToString()!.ToUpper()))
                    {
                        string id = pnpId.ToString()!;
                        if (id.StartsWith("USB"))
                        {
                            return new DeviceDataEx
                            {
                                Data = new SharpAdbClient.DeviceData { Serial = id },
                                PhyState = DevicePhysicalStateEx.UsbDebugSwitchClosed
                            };
                        }
                    }
                }
            }
            catch (Exception) { }
#endif
            return null;
        }
    }

    private class DeviceDataEx
    {
        public SharpAdbClient.DeviceData Data { get; set; } = new SharpAdbClient.DeviceData();
        public DevicePhysicalStateEx PhyState { get; set; }
    }

    public static IAdbClient m_AdbClient = new AdbClient();

    private readonly ICompositListener _listener;

    private readonly string _adbExeFileFullName;

    private readonly AdbScaner _adbScaner;

    public AdbConnectionMonitorEx(ICompositListener listener, string adbFileName)
    {
        _listener = listener;
        _adbExeFileFullName = adbFileName;
        _adbScaner = new AdbScaner(this);
    }

    public void StartMonitoring()
    {
        int retries = 3;
        do
        {
            try
            {
                AdbServer.Instance.StartServer(_adbExeFileFullName, restartServerIfNewer: true);
            }
            catch
            {
                continue;
            }
            break;
        } while (retries-- > 0);
        _adbScaner.Start();
    }

    public void StopMonitoring()
    {
        _adbScaner.Stop();
        TryKillAdb();
    }

    private void TryKillAdb()
    {
        try
        {
            m_AdbClient?.KillAdb();
        }
        catch (Exception) { }
    }

    public void Dispose()
    {
        StopMonitoring();
    }
}
