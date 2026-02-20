using System;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt;

public class DeviceConnectionManagerEx : ICompositListener
{
	private readonly AdbConnectionMonitorEx adbMonitor;
	private readonly FBConnectionMonitorEx fastbootMonitor;

	public event Action<DeviceEx, DevicePhysicalStateEx> DeviceConnected;
	public event Action<DeviceEx> DeviceDisconnected;

	public DeviceConnectionManagerEx(string adbPath)
	{
		adbMonitor = new AdbConnectionMonitorEx(this, adbPath);
		fastbootMonitor = new FBConnectionMonitorEx(this);
	}

	public void StartMonitoring()
	{
		adbMonitor.StartMonitoring();
		fastbootMonitor.StartMonitoring();
	}

	public void StopMonitoring()
	{
		adbMonitor.StopMonitoring();
		fastbootMonitor.StopMonitoring();
	}

	public void OnConnect(DeviceEx device, DevicePhysicalStateEx phyState)
	{
		device.PhysicalStatus = phyState;
		DeviceConnected?.Invoke(device, phyState);
	}

	public void OnDisconnect(DeviceEx device)
	{
		DeviceDisconnected?.Invoke(device);
	}
}
