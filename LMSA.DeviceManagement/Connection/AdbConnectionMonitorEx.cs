using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt.DeviceOperator;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt;

public class AdbConnectionMonitorEx : IDisposable
{
    private class AdbScaner
    {
        private readonly AdbConnectionMonitorEx outer;
        private readonly Dictionary<string, DeviceEx> Cache = new Dictionary<string, DeviceEx>();
        private volatile bool isRunning;
        protected AdbOperator Operator = new AdbOperator();

        public AdbScaner(AdbConnectionMonitorEx outer)
        {
            this.outer = outer;
        }

        public void Start()
        {
            isRunning = true;
            Cache.Clear();
            List<string> foundDevices = new List<string>();

            while (isRunning)
            {
                foundDevices.Clear();
                foundDevices = Operator.FindDevices();

                foreach (string serial in foundDevices)
                {
                    if (!serial.StartsWith("emulator-"))
                    {
                        if (!Cache.ContainsKey(serial))
                        {
                            AdbDeviceEx adbDeviceEx = new AdbDeviceEx
                            {
                                ConnectType = ConnectType.Adb,
                                Identifer = serial,
                                ValidateCodeFunc = null
                            };
                            DevicemgtContantClass.IsOtherToBackAndRestoreAndAutoConnection = true;
                            Cache.Add(serial, adbDeviceEx);
                            outer.m_Listener.OnConnect(adbDeviceEx, DevicePhysicalStateEx.Online);
                        }
                        else if (Cache[serial].IsRemove)
                        {
                            Cache[serial].IsRemove = false;
                            outer.m_Listener.OnConnect(Cache[serial], DevicePhysicalStateEx.Online);
                        }
                    }
                }

                foreach (string item2 in Cache
                    .Where(n => !n.Value.IsRemove)
                    .Select(n => n.Key)
                    .Except(foundDevices)
                    .ToList())
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
        LogHelper.LogInstance.Info("Starting ADB monitoring with: " + m_AdbExeFileFullName);
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
