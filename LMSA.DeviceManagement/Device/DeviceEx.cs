using System;
using System.Threading.Tasks;
using lenovo.mbg.service.common.log;

namespace lenovo.mbg.service.framework.services.Device;

/// <summary>
/// Abstract base class representing a connected Android device.
/// Tracks both physical (USB/network) and soft (app/service) connection states.
/// </summary>
public abstract class DeviceEx
{
    private DeviceWorkType _workType = DeviceWorkType.None;

    private volatile DevicePhysicalStateEx _physicalStatus = DevicePhysicalStateEx.None;

    private volatile DeviceSoftStateEx _softStatus = DeviceSoftStateEx.None;

    private volatile DeviceSoftStateEx _lockedSoftStatus = DeviceSoftStateEx.None;

    private volatile DeviceSoftStateEx _releaseSoftStatusCondition = DeviceSoftStateEx.None;

    private volatile bool _softStatusIsLocked;

    private volatile bool _autoRelease;

    private readonly object _softStatusLockObj = new object();

    private event EventHandler<DevicePhysicalStateEx>? _physicalStatusChanged;

    private event EventHandler<DeviceSoftStateEx>? _softStatusChanged;

    public string Identifer { get; set; } = string.Empty;

    public ConnectType ConnectType { get; set; }

    public string ConnectTime { get; set; } = string.Empty;

    public IDeviceOperator? DeviceOperator { get; set; }

    public DeviceType DeviceType { get; set; }

    public abstract IAndroidDevice? Property { get; set; }

    public string? ConnectedAppType { get; set; }

    public bool AppInstallFinished { get; set; }

    public int AppVersion { get; set; }

    public Func<Task, string, bool>? ValidateCodeFunc { get; set; }

    public Func<int, bool>? InstallAppCallback { get; set; }

    public Action? RetryConnectCallback { get; set; }

    public bool IsRemove { get; set; }

    public bool SoftStatusIsLocked => _softStatusIsLocked;

    public DeviceWorkType WorkType
    {
        get => _workType;
        set
        {
            if (_workType != value)
            {
                DeviceWorkType prev = _workType;
                _workType = value;
                LogHelper.LogInstance.Info($"======device: {Identifer}#{ConnectTime}, work type: {prev} --> {_workType}======");
            }
        }
    }

    public DevicePhysicalStateEx PhysicalStatus
    {
        get => _physicalStatus;
        set
        {
            if (_physicalStatus != value)
            {
                DevicePhysicalStateEx prev = _physicalStatus;
                _physicalStatus = value;
                LogHelper.LogInstance.Info($"======device: {Identifer}#{ConnectTime}, phy status: {prev} --> {_physicalStatus}======");
                OnPhysicalStatusChanged(prev, _physicalStatus);
                FirePhysicalStatusChangedEvent(this, _physicalStatus);
                if (_physicalStatus == DevicePhysicalStateEx.Offline)
                {
                    SoftStatus = DeviceSoftStateEx.Offline;
                }
            }
        }
    }

    public DeviceSoftStateEx SoftStatus
    {
        get => _softStatus;
        set
        {
            if (_softStatus != value)
            {
                DeviceSoftStateEx prev = _softStatus;
                _softStatus = value;
                LogHelper.LogInstance.Info($"======device: {Identifer}#{ConnectTime}, soft status: {prev} --> {_softStatus}======");
                OnSoftStatusChanged(prev, _softStatus);
                FireSoftStatusChangedEvent(this, _softStatus);
            }
        }
    }

    public event EventHandler<DevicePhysicalStateEx> PhysicalStatusChanged
    {
        add
        {
            _physicalStatusChanged += value;
            Task.Run(() => value.Invoke(this, PhysicalStatus));
        }
        remove
        {
            _physicalStatusChanged -= value;
        }
    }

    public event EventHandler<DeviceSoftStateEx> SoftStatusChanged
    {
        add
        {
            _softStatusChanged += value;
            Task.Run(() => value.Invoke(this, SoftStatus));
        }
        remove
        {
            _softStatusChanged -= value;
        }
    }

    protected DeviceEx()
    {
        DeviceType = DeviceType.Normal;
    }

    public abstract void Load();

    protected virtual void OnPhysicalStatusChanged(DevicePhysicalStateEx prev, DevicePhysicalStateEx current)
    {
    }

    protected virtual void OnSoftStatusChanged(DeviceSoftStateEx prev, DeviceSoftStateEx current)
    {
    }

    public void LockSoftStatus(bool autoRelease, DeviceSoftStateEx when)
    {
        lock (_softStatusLockObj)
        {
            _releaseSoftStatusCondition = when;
            _autoRelease = autoRelease;
            _softStatusIsLocked = true;
        }
    }

    public void ReleaseSoftStatusLock()
    {
        lock (_softStatusLockObj)
        {
            _softStatusIsLocked = false;
            if (_lockedSoftStatus != DeviceSoftStateEx.None)
            {
                SoftStatus = _lockedSoftStatus;
            }
        }
    }

    public bool IsHWEnable()
    {
        if (_softStatus == DeviceSoftStateEx.Online)
        {
            if (ConnectedAppType == "Ma")
            {
                return true;
            }
            if (ConnectedAppType == "Moto")
            {
                return AppVersion >= 1200000;
            }
            return false;
        }
        return false;
    }

    public void UnloadEvent()
    {
        _softStatusChanged = null;
        _physicalStatusChanged = null;
        ValidateCodeFunc = null;
    }

    private void FirePhysicalStatusChangedEvent(object sender, DevicePhysicalStateEx e)
    {
        EventHandler<DevicePhysicalStateEx>? handler = _physicalStatusChanged;
        if (handler != null)
        {
            foreach (EventHandler<DevicePhysicalStateEx> d in handler.GetInvocationList())
            {
                Task.Run(() => d.Invoke(sender, e));
            }
        }
    }

    private void FireSoftStatusChangedEvent(object sender, DeviceSoftStateEx e)
    {
        EventHandler<DeviceSoftStateEx>? handler = _softStatusChanged;
        if (handler != null)
        {
            foreach (EventHandler<DeviceSoftStateEx> d in handler.GetInvocationList())
            {
                Task.Run(() => d.Invoke(sender, e));
            }
        }
    }

    public override string ToString()
    {
        return $"device: {Identifer}#{ConnectTime}, modelname:{Property?.ModelName}, connect type: {ConnectType}, phy status: {PhysicalStatus}, soft status: {SoftStatus}, app type: {ConnectedAppType}";
    }
}
