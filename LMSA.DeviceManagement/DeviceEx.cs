using System;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt;

public abstract class DeviceEx
{
	private DeviceWorkType workType = DeviceWorkType.None;

	private volatile DevicePhysicalStateEx physicalStatus = DevicePhysicalStateEx.None;

	private volatile DeviceSoftStateEx softStatus = DeviceSoftStateEx.None;

	public string Identifer { get; set; }

	public ConnectType ConnectType { get; set; }

	public string ConnectTime { get; set; }

	public IDeviceOperator DeviceOperator { get; set; }

	public abstract IAndroidDevice Property { get; set; }

	public string ConnectedAppType { get; set; }

	public bool AppInstallFinished { get; set; }

	public int AppVersion { get; set; }

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
				if (physicalStatus == DevicePhysicalStateEx.Offline)
				{
					SoftStatus = DeviceSoftStateEx.Offline;
				}
			}
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
			}
		}
	}

	public abstract void Load();

	protected virtual void OnPhysicalStatusChanged(DevicePhysicalStateEx prev, DevicePhysicalStateEx current)
	{
	}

	protected virtual void OnSoftStatusChanged(DeviceSoftStateEx prev, DeviceSoftStateEx current)
	{
	}

	public override string ToString()
	{
		return $"device: {Identifer}#{ConnectTime}, modelname:{Property?.ModelName}, connect type: {ConnectType}, phy status: {PhysicalStatus}, soft status: {SoftStatus}, app type: {ConnectedAppType}";
	}
}
