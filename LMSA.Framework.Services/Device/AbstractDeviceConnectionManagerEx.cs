using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace lenovo.mbg.service.framework.services.Device;

public abstract class AbstractDeviceConnectionManagerEx : IDisposable
{
    public abstract Func<Task, string, bool> ValidateCodeFunc { get; set; }

    public abstract DeviceEx MasterDevice { get; protected set; }

    public abstract List<Tuple<string, string>> WirelessWaitForConnectEndPoints { get; }

    public abstract IList<DeviceEx> ConntectedDevices { get; }

    public abstract event WirelessMornitoringAddressChangedHandler WifiMonitoringEndPointChanged;

    public abstract event EventHandler<DeviceEx> Connecte;

    public abstract event EventHandler<DeviceEx> DisConnecte;

    public event Action<bool> ConnectChange;

    public event Action<int> ShowConnectIconStatus;

    public abstract event EventHandler<MasterDeviceChangedEventArgs> MasterDeviceChanged;

    public virtual void RaiseConnectChange(bool status)
    {
        this.ConnectChange?.Invoke(status);
    }

    public virtual void SetDeviceConnectIconStatus(int status)
    {
        this.ShowConnectIconStatus?.Invoke(status);
    }

    public abstract void Start();

    public abstract void Stop();

    public abstract void Dispose();

    public abstract void SwitchDevice(string id);

    public abstract void SwitchAfterDevice(string id);

    public abstract bool IsMasterConnectedByHelperForAndroid11();
}
