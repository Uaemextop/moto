using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt;

/// <summary>
/// Manages all connected devices (ADB, Fastboot, WiFi).
/// Coordinates connection monitoring and device state tracking.
/// </summary>
public class DeviceConnectionManagerEx : ICompositListener
{
    private AdbConnectionMonitorEx? _adbListener;

    private FBConnectionMonitorEx? _fastbootListener;

    private readonly object _sync = new object();

    private readonly IList<DeviceEx> _connectedDevices = new List<DeviceEx>();

    private DeviceEx? _masterDevice;

    public Func<Task, string, bool>? ValidateCodeFunc { get; set; }

    public IList<DeviceEx> ConnectedDevices
    {
        get
        {
            lock (_sync)
            {
                return _connectedDevices.ToList();
            }
        }
    }

    public DeviceEx? MasterDevice
    {
        get
        {
            lock (_sync)
            {
                return _masterDevice;
            }
        }
    }

    public event EventHandler<DeviceEx>? DeviceConnected;

    public event EventHandler<DeviceEx>? DeviceDisconnected;

    public void StartMonitoring(string adbExePath)
    {
        _adbListener = new AdbConnectionMonitorEx(this, adbExePath);
        _fastbootListener = new FBConnectionMonitorEx(this);

        Task.Run(() => _adbListener.StartMonitoring());
        Task.Run(() => _fastbootListener.StartMonitoring());
    }

    public void StopMonitoring()
    {
        _adbListener?.StopMonitoring();
        _fastbootListener?.StopMonitoring();
    }

    public void OnConnect(DeviceEx device, DevicePhysicalStateEx phyState)
    {
        lock (_sync)
        {
            if (!_connectedDevices.Contains(device))
            {
                _connectedDevices.Add(device);
                if (_masterDevice == null)
                {
                    _masterDevice = device;
                }
            }
        }
        device.PhysicalStatus = phyState;
        LogHelper.LogInstance.Info($"Device connected: {device.Identifer}, state: {phyState}");
        DeviceConnected?.Invoke(this, device);
    }

    public void OnDisconnect(DeviceEx device)
    {
        lock (_sync)
        {
            _connectedDevices.Remove(device);
            if (_masterDevice == device)
            {
                _masterDevice = _connectedDevices.FirstOrDefault();
            }
        }
        device.PhysicalStatus = DevicePhysicalStateEx.Offline;
        LogHelper.LogInstance.Info($"Device disconnected: {device.Identifer}");
        DeviceDisconnected?.Invoke(this, device);
    }

    public void OnWifiMonitoringEndPointChanged(List<Tuple<string, string>> endpoints)
    {
        LogHelper.LogInstance.Debug($"WiFi monitoring endpoints changed: {endpoints?.Count} endpoints");
    }
}
