using System;
using lenovo.mbg.service.framework.devicemgt.DeviceInfo;
using lenovo.mbg.service.framework.devicemgt.DeviceOperator;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt;

// Stub implementation - AdbDeviceEx will be fully implemented in a subsequent step
public class AdbDeviceEx : TcpAndroidDevice
{
    public AdbDeviceEx()
    {
        base.DeviceOperator = new AdbOperator();
    }

    public override void Load()
    {
        try
        {
            Property = new AndroidDeviceProperty();
        }
        catch
        {
        }
    }

    protected override void OnPhysicalStatusChanged(DevicePhysicalStateEx prev, DevicePhysicalStateEx current)
    {
        base.OnPhysicalStatusChanged(prev, current);
    }
}
