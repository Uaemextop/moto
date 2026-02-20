using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.devicemgt.DeviceOperator;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt;

public class FBConnectionMonitorEx
{
    private class FastbootScaner
    {
        private readonly FBConnectionMonitorEx outer;

        private readonly Dictionary<string, DeviceEx> Cache = new Dictionary<string, DeviceEx>();

        private volatile bool isRunning;

        public FastbootScaner(FBConnectionMonitorEx outer)
        {
            this.outer = outer;
        }

        public void Start()
        {
            isRunning = true;
            Cache.Clear();

            while (isRunning)
            {
                List<string> devices = new List<string>();
                try
                {
                    devices = FastbootOperator.Instance.FindDevices();
                }
                catch (Exception ex)
                {
                    LogHelper.LogInstance.Error("FastbootScaner.Start scan error", ex);
                }

                foreach (string serial in devices)
                {
                    if (!Cache.ContainsKey(serial))
                    {
                        var device = new FastbootDeviceEx();
                        device.ConnectType = ConnectType.Fastboot;
                        device.Identifer = serial;
                        Cache.Add(serial, device);
                        outer.m_listener.OnConnect(device, DevicePhysicalStateEx.Online);
                    }
                    else if (Cache[serial].IsRemove)
                    {
                        Cache[serial].IsRemove = false;
                        outer.m_listener.OnConnect(Cache[serial], DevicePhysicalStateEx.Online);
                    }
                }

                var disconnected = Cache
                    .Where(n => !n.Value.IsRemove)
                    .Select(n => n.Key)
                    .Except(devices)
                    .ToList();

                foreach (string serial in disconnected)
                {
                    DeviceEx deviceEx = Cache[serial];
                    if (deviceEx.WorkType == DeviceWorkType.None)
                    {
                        Cache.Remove(serial);
                    }
                    else
                    {
                        deviceEx.IsRemove = true;
                    }
                    outer.m_listener.OnDisconnect(deviceEx);
                }

                Thread.Sleep(1000);
            }
        }

        public void Stop()
        {
            isRunning = false;
        }
    }

    private FastbootScaner scaner;
    private ICompositListener m_listener;

    public FBConnectionMonitorEx(ICompositListener listener)
    {
        m_listener = listener;
        scaner = new FastbootScaner(this);
    }

    public void StartMonitoring()
    {
        scaner.Start();
    }

    public void StopMonitoring()
    {
        scaner.Stop();
    }
}
