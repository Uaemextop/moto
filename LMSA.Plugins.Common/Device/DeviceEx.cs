using System;
using System.Threading.Tasks;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.services;

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

    public string Identifer { get; set; }

    public ConnectType ConnectType { get; set; }

    public string ConnectTime { get; set; }

    public IDeviceOperator DeviceOperator { get; set; }

    public DeviceType DeviceType { get; set; }

    public abstract IAndroidDevice Property { get; set; }

    public string ConnectedAppType { get; set; }

    public bool AppInstallFinished { get; set; }

    public int AppVersion { get; set; }

    public Func<Task, string, bool> ValidateCodeFunc { get; set; }

    public DeviceWorkType WorkType
    {
        get
        {
            return workType;
        }
        set
        {
            if (workType != value)
            {
                workType = value;
            }
        }
    }

    public bool IsRemove { get; set; }

    public DevicePhysicalStateEx PhysicalStatus
    {
        get
        {
            return physicalStatus;
        }
        set
        {
            if (physicalStatus != value)
            {
                DevicePhysicalStateEx prev = physicalStatus;
                physicalStatus = value;
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
        get
        {
            return softStatusIsLocked;
        }
        private set
        {
            softStatusIsLocked = value;
        }
    }

    public DeviceSoftStateEx SoftStatus
    {
        get
        {
            return softStatus;
        }
        set
        {
            if (softStatus != value)
            {
                DeviceSoftStateEx prev = softStatus;
                softStatus = value;
                OnSoftStatusChanged(prev, softStatus);
                FireSoftStatusChangedEvent(this, softStatus);
            }
        }
    }

    public Func<int, bool> InstallAppCallback { get; set; }

    public Action RetryConnectCallback { get; set; }

    private event EventHandler<DevicePhysicalStateEx> physicalStatusChanged;

    public event EventHandler<DevicePhysicalStateEx> PhysicalStatusChanged
    {
        add
        {
            physicalStatusChanged += value;
            value?.Invoke(this, PhysicalStatus);
        }
        remove
        {
            physicalStatusChanged -= value;
        }
    }

    private event EventHandler<DeviceSoftStateEx> softStatusChanged;

    public event EventHandler<DeviceSoftStateEx> SoftStatusChanged
    {
        add
        {
            softStatusChanged += value;
            value?.Invoke(this, SoftStatus);
        }
        remove
        {
            softStatusChanged -= value;
        }
    }

    public DeviceEx()
    {
        DeviceType = DeviceType.Normal;
    }

    private void FirePhysicalStatusChangedEvent(object sender, DevicePhysicalStateEx e)
    {
        physicalStatusChanged?.Invoke(sender, e);
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
        softStatusChanged?.Invoke(sender, e);
    }

    protected virtual void OnSoftStatusChanged(DeviceSoftStateEx prev, DeviceSoftStateEx current)
    {
    }

    public bool IsHWEnable()
    {
        if (softStatus == DeviceSoftStateEx.Online)
        {
            if (!(ConnectedAppType == ConnectedAppTypesDefine.Ma))
            {
                if (ConnectedAppType == ConnectedAppTypesDefine.Moto)
                {
                    return AppVersion >= 1200000;
                }
                return false;
            }
            return true;
        }
        return false;
    }

    public override string ToString()
    {
        return $"device: {Identifer}#{ConnectTime}, modelname:{Property?.ModelName}, connect type: {ConnectType}, phy status: {PhysicalStatus}, soft status: {SoftStatus}, app type: {ConnectedAppType}";
    }

    public void UnloadEvent()
    {
        softStatusChanged = null;
        physicalStatusChanged = null;
        ValidateCodeFunc = null;
    }
}
