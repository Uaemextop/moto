using lenovo.mbg.service.framework.devicemgt.DeviceInfo;
using lenovo.mbg.service.framework.devicemgt.DeviceOperator;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt;

/// <summary>
/// Represents an ADB-connected Android device.
/// </summary>
public class AdbDeviceEx : DeviceEx
{
    public override IAndroidDevice? Property { get; set; }

    public AdbDeviceEx()
    {
        DeviceOperator = new AdbOperator();
        Property = null;
    }

    public override void Load()
    {
    }
}
