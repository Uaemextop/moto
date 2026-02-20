using System.Collections.Generic;
using System.Linq;
using System.Threading;
using lenovo.mbg.service.framework.devicemgt.DeviceOperator;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt;

/// <summary>
/// Monitors fastboot device connections and disconnections.
/// </summary>
public class FBConnectionMonitorEx
{
    private class FastbootScaner
    {
        private readonly FBConnectionMonitorEx _outer;

        private readonly Dictionary<string, DeviceEx> _cache = new Dictionary<string, DeviceEx>();

        private volatile bool _isRunning;

        public FastbootScaner(FBConnectionMonitorEx outer)
        {
            _outer = outer;
        }

        public void Start()
        {
            _isRunning = true;
            ScanDevice();
        }

        private void ScanDevice()
        {
            _cache.Clear();
            while (_isRunning)
            {
                List<string> list = _outer._operator.FindDevices();
                if (list.Count == 0)
                {
                    list = _outer._operator.FindDevices(FastbootOperator.fastbootExe);
                }
                foreach (string item in list)
                {
                    if (!_cache.ContainsKey(item))
                    {
                        FastbootDeviceEx fastbootDeviceEx = new FastbootDeviceEx
                        {
                            ConnectType = ConnectType.Fastboot,
                            Identifer = item
                        };
                        _cache.Add(item, fastbootDeviceEx);
                        _outer._listener.OnConnect(fastbootDeviceEx, DevicePhysicalStateEx.Online);
                    }
                    else if (_cache[item].IsRemove)
                    {
                        _cache[item].IsRemove = false;
                        _outer._listener.OnConnect(_cache[item], DevicePhysicalStateEx.Online);
                    }
                }
                foreach (string id in _cache
                    .Where(n => !n.Value.IsRemove)
                    .Select(n => n.Key)
                    .Except(list)
                    .ToList())
                {
                    DeviceEx deviceEx = _cache[id];
                    if (deviceEx.WorkType == DeviceWorkType.None)
                    {
                        _cache.Remove(id);
                    }
                    else
                    {
                        deviceEx.IsRemove = true;
                    }
                    _outer._listener.OnDisconnect(deviceEx);
                }
                Thread.Sleep(1000);
            }
        }

        public void Stop()
        {
            _isRunning = false;
        }
    }

    private readonly FastbootScaner _scaner;

    private readonly ICompositListener _listener;

    private readonly FastbootOperator _operator = new FastbootOperator();

    public FBConnectionMonitorEx(ICompositListener listener)
    {
        _listener = listener;
        _scaner = new FastbootScaner(this);
    }

    public void StartMonitoring()
    {
        _scaner.Start();
    }

    public void StopMonitoring()
    {
        _scaner.Stop();
    }
}
