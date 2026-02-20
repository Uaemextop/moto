using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt;

public class DeviceConnectionManagerEx : AbstractDeviceConnectionManagerEx
{
    private DeviceEx masterDevice;
    private Func<Task, string, bool> validateCodeFunc;
    private readonly List<DeviceEx> connectedDevices = new List<DeviceEx>();

    public override Func<Task, string, bool> ValidateCodeFunc
    {
        get => validateCodeFunc;
        set => validateCodeFunc = value;
    }

    public override DeviceEx MasterDevice
    {
        get => masterDevice;
        protected set => masterDevice = value;
    }

    public override List<Tuple<string, string>> WirelessWaitForConnectEndPoints => new List<Tuple<string, string>>();

    public override IList<DeviceEx> ConntectedDevices => connectedDevices;

    public override event WirelessMornitoringAddressChangedHandler WifiMonitoringEndPointChanged;

    public override event EventHandler<DeviceEx> Connecte;

    public override event EventHandler<DeviceEx> DisConnecte;

    public override event EventHandler<MasterDeviceChangedEventArgs> MasterDeviceChanged;

    public DeviceConnectionManagerEx()
    {
    }

    public override void Start()
    {
    }

    public override void Stop()
    {
    }

    public override void Dispose()
    {
        Stop();
    }

    public override void SwitchDevice(string id)
    {
    }

    public override void SwitchAfterDevice(string id)
    {
    }

    public override bool IsMasterConnectedByHelperForAndroid11()
    {
        return false;
    }
}
