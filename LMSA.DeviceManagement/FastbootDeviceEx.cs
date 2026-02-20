using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt;

public class FastbootDeviceEx : DeviceEx
{
	public override IAndroidDevice Property { get; set; }

	public FastbootDeviceEx()
	{
		Property = new FastbootAndroidDevice();
	}

	public override void Load()
	{
	}
}

public class FastbootAndroidDevice : IAndroidDevice
{
	public string ModelName { get; set; }
	public string IMEI1 { get; set; }
	public string SN { get; set; }
}
