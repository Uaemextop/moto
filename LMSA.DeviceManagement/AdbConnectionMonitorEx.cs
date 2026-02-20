using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SharpAdbClient;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt;

public class AdbConnectionMonitorEx : IDisposable
{
	private class AdbScaner
	{
		private readonly AdbConnectionMonitorEx outer;

		private readonly Dictionary<string, DeviceEx> Cache = new Dictionary<string, DeviceEx>();

		private volatile bool isRunning;

		private Thread scanThread;

		public AdbScaner(AdbConnectionMonitorEx outer)
		{
			this.outer = outer;
		}

		public void Start()
		{
			isRunning = true;
			scanThread = new Thread(ScanLoop);
			scanThread.IsBackground = true;
			scanThread.Start();
		}

		private void ScanLoop()
		{
			Cache.Clear();
			List<DeviceDataEx> foundDevices = new List<DeviceDataEx>();
			List<DeviceData> list = new List<DeviceData>();

			while (isRunning)
			{
				foundDevices.Clear();
				try
				{
					list = m_AdbClient.GetDevices().ToList();
				}
				catch
				{
					list.Clear();
				}

				foreach (DeviceData device in list)
				{
					foundDevices.Add(new DeviceDataEx
					{
						Data = device,
						PhyState = (DevicePhysicalStateEx)(int)device.State
					});
				}

				foreach (DeviceDataEx item in foundDevices)
				{
					if (!item.Data.Serial.StartsWith("emulator-"))
					{
						if (!Cache.ContainsKey(item.Data.Serial))
						{
							AdbDeviceEx adbDeviceEx = new AdbDeviceEx
							{
								ConnectType = ConnectType.Adb,
								Identifer = item.Data.Serial,
								ConnectTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
							};
							Cache.Add(item.Data.Serial, adbDeviceEx);
							outer.m_Listener.OnConnect(adbDeviceEx, item.PhyState);
						}
						else if (Cache[item.Data.Serial].IsRemove)
						{
							Cache[item.Data.Serial].IsRemove = false;
							outer.m_Listener.OnConnect(Cache[item.Data.Serial], item.PhyState);
						}
						else
						{
							Cache[item.Data.Serial].PhysicalStatus = item.PhyState;
						}
					}
				}

				foreach (string item2 in (from n in Cache
					where !n.Value.IsRemove
					select n.Key).Except(foundDevices.Select((DeviceDataEx n) => n.Data.Serial)).ToList())
				{
					DeviceEx deviceEx = Cache[item2];
					if (deviceEx.WorkType == DeviceWorkType.None)
					{
						Cache.Remove(item2);
					}
					else
					{
						deviceEx.IsRemove = true;
					}
					outer.m_Listener.OnDisconnect(deviceEx);
				}

				Thread.Sleep(1000);
			}
		}

		public void Stop()
		{
			isRunning = false;
		}
	}

	public static IAdbClient m_AdbClient = new AdbClient();

	private readonly ICompositListener m_Listener;

	private readonly string m_AdbExeFileFullName;

	private readonly AdbScaner adbScaner;

	public AdbConnectionMonitorEx(ICompositListener listener, string adbFileName)
	{
		m_Listener = listener;
		m_AdbExeFileFullName = adbFileName;
		adbScaner = new AdbScaner(this);
	}

	public void StartMonitoring()
	{
		int num = 3;
		do
		{
			try
			{
				AdbServer.Instance.StartServer(m_AdbExeFileFullName, restartServerIfNewer: true);
				break;
			}
			catch
			{
			}
		}
		while (num-- > 0);
		adbScaner.Start();
	}

	public void StopMonitoring()
	{
		adbScaner.Stop();
		TryKillAdb();
	}

	private void TryKillAdb()
	{
		try
		{
			if (m_AdbClient != null)
			{
				m_AdbClient.KillAdb();
			}
		}
		catch (Exception)
		{
		}
	}

	public void Dispose()
	{
		StopMonitoring();
	}
}
