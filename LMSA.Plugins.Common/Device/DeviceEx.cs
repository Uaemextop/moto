using System;
using System.Threading.Tasks;
using lenovo.mbg.service.common.log;

namespace lenovo.mbg.service.framework.services.Device;

/// <summary>
/// Abstract base class representing a connected device with physical and software state management.
/// </summary>
public abstract class DeviceEx
{
    private DeviceWorkType workType = DeviceWorkType.None;
    private volatile DevicePhysicalStateEx physicalStatus = DevicePhysicalStateEx.None;
    private volatile DeviceSoftStateEx lockedSoftStatus = DeviceSoftStateEx.None;
    private volatile DeviceSoftStateEx releaseSoftStatusCondition = DeviceSoftStateEx.None;
    private volatile bool softStatusIsLocked;
    private readonly object softStatusLockOjb = new object();
    private volatile bool autoRelease;
    private volatile DeviceSoftStateEx softStatus = DeviceSoftStateEx.None;

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

    public DeviceWorkType WorkType
    {
        get => workType;
        set
        {
            if (workType != value)
            {
                DeviceWorkType prev = workType;
                workType = value;
                LogHelper.LogInstance.Info($"======device: {Identifer}#{ConnectTime}, work type: {prev} --> {workType}======");
            }
        }
    }

    public bool IsRemove { get; set; }

    public DevicePhysicalStateEx PhysicalStatus
    {
        get => physicalStatus;
        set
        {
            if (physicalStatus != value)
            {
                DevicePhysicalStateEx prev = physicalStatus;
                physicalStatus = value;
                LogHelper.LogInstance.Info($"======device: {Identifer}#{ConnectTime}, phy status: {prev} --> {physicalStatus}======");
                OnPhysicalStatusChanged(prev, physicalStatus);
                FirePhysicalStatusChangedEvent(this, physicalStatus);
                if (physicalStatus == DevicePhysicalStateEx.Offline)
                {
                    SoftStatus = DeviceSoftStateEx.Offline;
                }
            }
        }
    }

    public bool SoftStatusIsLocked
    {
        get => softStatusIsLocked;
        private set => softStatusIsLocked = value;
    }

    public DeviceSoftStateEx SoftStatus
    {
        get => softStatus;
        set
        {
            if (softStatus != value)
            {
                DeviceSoftStateEx prev = softStatus;
                softStatus = value;
                LogHelper.LogInstance.Info($"======device: {Identifer}#{ConnectTime}, soft status: {prev} --> {softStatus}======");
                OnSoftStatusChanged(prev, softStatus);
                FireSoftStatusChangedEvent(this, softStatus);
            }
        }
    }

    public Func<int, bool>? InstallAppCallback { get; set; }
    public Action? RetryConnectCallback { get; set; }

    public event EventHandler<DevicePhysicalStateEx>? PhysicalStatusChanged;
    public event EventHandler<DeviceSoftStateEx>? SoftStatusChanged;

    public DeviceEx()
    {
        DeviceType = DeviceType.Normal;
    }

    private void FirePhysicalStatusChangedEvent(object sender, DevicePhysicalStateEx e)
    {
        PhysicalStatusChanged?.Invoke(sender, e);
    }

    public abstract void Load();

    protected virtual void OnPhysicalStatusChanged(DevicePhysicalStateEx prev, DevicePhysicalStateEx current)
    {
    }

    public void LockSoftStatus(bool autoRelease, DeviceSoftStateEx when)
    {
        lock (softStatusLockOjb)
        {
            releaseSoftStatusCondition = when;
            this.autoRelease = autoRelease;
            softStatusIsLocked = true;
        }
    }

    public void ReleaseSoftStatusLock()
    {
        lock (softStatusLockOjb)
        {
            softStatusIsLocked = false;
            if (lockedSoftStatus != DeviceSoftStateEx.None)
            {
                SoftStatus = lockedSoftStatus;
            }
        }
    }

    private void FireSoftStatusChangedEvent(object sender, DeviceSoftStateEx e)
    {
        SoftStatusChanged?.Invoke(sender, e);
    }

    protected virtual void OnSoftStatusChanged(DeviceSoftStateEx prev, DeviceSoftStateEx current)
    {
    }

    public bool IsHWEnable()
    {
        if (softStatus == DeviceSoftStateEx.Online)
        {
            if (ConnectedAppType == ConnectedAppTypesDefine.Ma)
            {
                return true;
            }
            if (ConnectedAppType == ConnectedAppTypesDefine.Moto)
            {
                return AppVersion >= 1200000;
            }
            return false;
        }
        return false;
    }

    public override string ToString()
    {
        return $"device: {Identifer}#{ConnectTime}, modelname:{Property?.ModelName}, connect type: {ConnectType}, phy status: {PhysicalStatus}, soft status: {SoftStatus}, app type: {ConnectedAppType}";
    }

    public void UnloadEvent()
    {
        PhysicalStatusChanged = null;
        SoftStatusChanged = null;
        ValidateCodeFunc = null;
    }
}
