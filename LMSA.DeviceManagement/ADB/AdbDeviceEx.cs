using lenovo.mbg.service.framework.devicemgt.DeviceOperator;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt;

public class AdbDeviceEx : DeviceEx
{
    public override IAndroidDevice? Property { get; set; }

    public AdbDeviceEx()
    {
        DeviceOperator = new AdbOperator();
    }

    public override void Load()
    {
    }
}
