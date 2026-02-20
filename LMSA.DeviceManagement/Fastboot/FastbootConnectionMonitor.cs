using lenovo.mbg.service.framework.common;
using log4net;
using System;
using System.Collections.Generic;
using System.Threading;

namespace lenovo.mbg.service.framework.devicemgt
{
    /// <summary>
    /// Monitors Fastboot device connections by polling fastboot devices.
    /// </summary>
    public class FastbootConnectionMonitor : IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(FastbootConnectionMonitor));

        private readonly FastbootOperator _fastbootOperator;
        private readonly HashSet<string> _knownDevices = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly object _lock = new object();

        private Timer? _pollTimer;
        private bool _disposed;

        /// <summary>Polling interval in milliseconds.</summary>
        public int PollIntervalMs { get; set; } = 2000;

        /// <summary>Fired when a new fastboot device is detected.</summary>
        public event EventHandler<string>? DeviceConnected;

        /// <summary>Fired when a fastboot device disconnects.</summary>
        public event EventHandler<string>? DeviceDisconnected;

        public FastbootConnectionMonitor() : this(new FastbootOperator()) { }

        public FastbootConnectionMonitor(FastbootOperator fastbootOperator)
        {
            _fastbootOperator = fastbootOperator ?? throw new ArgumentNullException(nameof(fastbootOperator));
        }

        /// <summary>
        /// Starts polling for fastboot device changes.
        /// </summary>
        public void Start()
        {
            _log.Info("FastbootConnectionMonitor started");
            _pollTimer = new Timer(_ => PollDevices(), null, 0, PollIntervalMs);        }

        /// <summary>
        /// Stops polling.
        /// </summary>
        public void Stop()
        {
            _pollTimer?.Dispose();
            _pollTimer = null;
            _log.Info("FastbootConnectionMonitor stopped");
        }

        /// <summary>
        /// Gets a snapshot of currently known fastboot device serial numbers.
        /// </summary>
        public IReadOnlyCollection<string> GetKnownDevices()
        {
            lock (_lock)
            {
                return new HashSet<string>(_knownDevices, StringComparer.OrdinalIgnoreCase);
            }
        }

        private void PollDevices()
        {
            try
            {
                var current = DetectFastbootDevices();

                lock (_lock)
                {
                    foreach (string serial in current)
                    {
                        if (_knownDevices.Add(serial))
                        {
                            _log.Info($"Fastboot device connected: {serial}");
                            DeviceConnected?.Invoke(this, serial);
                        }
                    }

                    var disconnected = new List<string>();
                    foreach (string serial in _knownDevices)
                    {
                        if (!current.Contains(serial))
                            disconnected.Add(serial);
                    }

                    foreach (string serial in disconnected)
                    {
                        _knownDevices.Remove(serial);
                        _log.Info($"Fastboot device disconnected: {serial}");
                        DeviceDisconnected?.Invoke(this, serial);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error polling fastboot devices", ex);
            }
        }

        private HashSet<string> DetectFastbootDevices()
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            string output = _fastbootOperator.Execute("devices", 5000);

            foreach (string line in output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.Contains("fastboot"))
                {
                    string serial = line.Split('\t')[0].Trim();
                    if (!string.IsNullOrEmpty(serial))
                        result.Add(serial);
                }
            }

            return result;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Stop();
                _disposed = true;
            }
        }
    }
}
