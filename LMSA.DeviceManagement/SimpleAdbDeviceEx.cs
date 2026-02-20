using lenovo.mbg.service.framework.devicemgt.DeviceInfo;
using lenovo.mbg.service.framework.devicemgt.DeviceOperator;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt;

public class SimpleAdbDeviceEx : DeviceEx
{
    private IAndroidDevice _property;

    public override IAndroidDevice Property
    {
        get => _property;
        set => _property = value;
    }

    public SimpleAdbDeviceEx()
    {
        DeviceOperator = AdbOperator.Instance;
        Property = new AndroidDeviceProperty();
    }

    public override void Load()
    {
        ((ILoadDeviceData)Property).Load();
    }
}
