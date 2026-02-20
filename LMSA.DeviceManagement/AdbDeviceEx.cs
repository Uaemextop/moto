using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt;

public class AdbDeviceEx : DeviceEx
{
	public override IAndroidDevice Property { get; set; }

	public AdbDeviceEx()
	{
		Property = new AndroidDeviceProperty();
	}

	public override void Load()
	{
	}
}

public class AndroidDeviceProperty : IAndroidDevice
{
	public string ModelName { get; set; }
	public string IMEI1 { get; set; }
	public string SN { get; set; }
}
