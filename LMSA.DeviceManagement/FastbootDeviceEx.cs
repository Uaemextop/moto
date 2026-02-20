using System.Threading.Tasks;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.devicemgt.DeviceInfo;
using lenovo.mbg.service.framework.devicemgt.DeviceOperator;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt;

public class FastbootDeviceEx : DeviceEx
{
    private IAndroidDevice _property;

    public override IAndroidDevice Property
    {
        get => _property;
        set => _property = value;
    }

    public FastbootDeviceEx()
    {
        Property = new FastbootAndroidDevice(this);
        DeviceOperator = FastbootOperator.Instance;
    }

    public override void Load()
    {
        ((ILoadDeviceData)Property).Load();
    }

    protected override void OnPhysicalStatusChanged(DevicePhysicalStateEx prev, DevicePhysicalStateEx current)
    {
        if (current != DevicePhysicalStateEx.Online)
        {
            return;
        }
        SoftStatus = DeviceSoftStateEx.Connecting;
        Task.Run(() => Load()).ContinueWith(_ =>
        {
            if (PhysicalStatus == DevicePhysicalStateEx.Online)
            {
                LogHelper.LogInstance.Info($"FastbootDeviceEx: ModelName:{Property.ModelName}, Imei1:{Property.IMEI1}");
                if (!string.IsNullOrEmpty(Property.ModelName) || !string.IsNullOrEmpty(Property.IMEI1))
                {
                    SoftStatus = DeviceSoftStateEx.Online;
                }
                else
                {
                    SoftStatus = DeviceSoftStateEx.Offline;
                }
            }
            else
            {
                SoftStatus = DeviceSoftStateEx.Offline;
            }
        });
    }
}
