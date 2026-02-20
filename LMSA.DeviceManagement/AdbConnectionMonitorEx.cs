using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.devicemgt.DeviceOperator;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt;

public class AdbConnectionMonitorEx : IDisposable
{
    private class AdbScaner
    {
        private readonly AdbConnectionMonitorEx outer;

        private readonly Dictionary<string, DeviceEx> Cache = new Dictionary<string, DeviceEx>();

        private volatile bool isRunning;

        public AdbScaner(AdbConnectionMonitorEx outer)
        {
            this.outer = outer;
        }

        public void Start()
        {
            isRunning = true;
            Cache.Clear();
            List<DeviceDataEx> foundDevices = new List<DeviceDataEx>();

            while (isRunning)
            {
                foundDevices.Clear();
                try
                {
                    List<string> devices = AdbOperator.Instance.FindDevices();
                    foreach (string serial in devices)
                    {
                        foundDevices.Add(new DeviceDataEx
                        {
                            Serial = serial,
                            PhyState = DevicePhysicalStateEx.Online
                        });
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.LogInstance.Error("AdbScaner.Start scan error", ex);
                }

                foreach (DeviceDataEx item in foundDevices)
                {
                    if (item.Serial.StartsWith("emulator-"))
                    {
                        continue;
                    }

                    if (!Cache.ContainsKey(item.Serial))
                    {
                        DeviceEx device = outer.CreateAdbDevice(item.Serial);
                        DevicemgtContantClass.IsOtherToBackAndRestoreAndAutoConnection = true;
                        Cache.Add(item.Serial, device);
                        outer.m_Listener.OnConnect(device, item.PhyState);
                    }
                    else if (Cache[item.Serial].IsRemove)
                    {
                        Cache[item.Serial].IsRemove = false;
                        outer.m_Listener.OnConnect(Cache[item.Serial], item.PhyState);
                    }
                    else
                    {
                        Cache[item.Serial].PhysicalStatus = item.PhyState;
                    }
                }

                var disconnected = Cache
                    .Where(n => !n.Value.IsRemove)
                    .Select(n => n.Key)
                    .Except(foundDevices.Select(n => n.Serial))
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

    private readonly ICompositListener m_Listener;
    private readonly AdbScaner adbScaner;

    public AdbConnectionMonitorEx(ICompositListener listener)
    {
        m_Listener = listener;
        adbScaner = new AdbScaner(this);
    }

    private DeviceEx CreateAdbDevice(string serial)
    {
        var device = new SimpleAdbDeviceEx();
        device.ConnectType = ConnectType.Adb;
        device.Identifer = serial;
        return device;
    }

    public void StartMonitoring()
    {
        adbScaner.Start();
    }

    public void StopMonitoring()
    {
        adbScaner.Stop();
    }

    public void Dispose()
    {
        StopMonitoring();
    }
}
