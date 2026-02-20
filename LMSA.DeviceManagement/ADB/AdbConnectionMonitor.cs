using lenovo.mbg.service.framework.common;
using log4net;
using System;
using System.Collections.Generic;
using System.Threading;

namespace lenovo.mbg.service.framework.devicemgt
{
    /// <summary>
    /// Event arguments for device state change notifications.
    /// </summary>
    public class DeviceStateChangedEventArgs : EventArgs
    {
        public string DeviceId { get; }
        public DevicePhysicalStateEx OldState { get; }
        public DevicePhysicalStateEx NewState { get; }

        public DeviceStateChangedEventArgs(string deviceId, DevicePhysicalStateEx oldState, DevicePhysicalStateEx newState)
        {
            DeviceId = deviceId;
            OldState = oldState;
            NewState = newState;
        }
    }

    /// <summary>
    /// Monitors ADB device connections and state changes.
    /// Polls the ADB server periodically to detect device additions, removals, and state changes.
    /// </summary>
    public class AdbConnectionMonitor : IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(AdbConnectionMonitor));

        private readonly AdbOperator _adbOperator;
        private readonly Dictionary<string, DevicePhysicalStateEx> _knownDevices
            = new Dictionary<string, DevicePhysicalStateEx>(StringComparer.OrdinalIgnoreCase);
        private readonly object _lock = new object();

        private Timer? _pollTimer;
        private bool _disposed;

        /// <summary>Polling interval in milliseconds.</summary>
        public int PollIntervalMs { get; set; } = 2000;

        /// <summary>Fired when a new device is detected.</summary>
        public event EventHandler<DeviceStateChangedEventArgs>? DeviceConnected;

        /// <summary>Fired when a device is disconnected.</summary>
        public event EventHandler<DeviceStateChangedEventArgs>? DeviceDisconnected;

        /// <summary>Fired when a device changes state.</summary>
        public event EventHandler<DeviceStateChangedEventArgs>? DeviceStateChanged;

        public AdbConnectionMonitor() : this(new AdbOperator()) { }

        public AdbConnectionMonitor(AdbOperator adbOperator)
        {
            _adbOperator = adbOperator ?? throw new ArgumentNullException(nameof(adbOperator));
        }

        /// <summary>
        /// Starts polling for device changes.
        /// </summary>
        public void Start()
        {
            _log.Info("AdbConnectionMonitor started");
            _pollTimer = new Timer(_ => PollDevices(), null, 0, PollIntervalMs);        }

        /// <summary>
        /// Stops polling for device changes.
        /// </summary>
        public void Stop()
        {
            _pollTimer?.Dispose();
            _pollTimer = null;
            _log.Info("AdbConnectionMonitor stopped");
        }

        /// <summary>
        /// Gets a snapshot of currently known devices and their states.
        /// </summary>
        public Dictionary<string, DevicePhysicalStateEx> GetKnownDevices()
        {
            lock (_lock)
            {
                return new Dictionary<string, DevicePhysicalStateEx>(_knownDevices, StringComparer.OrdinalIgnoreCase);
            }
        }

        private void PollDevices()
        {
            try
            {
                var currentDevices = DetectCurrentDevices();

                lock (_lock)
                {
                    // Check for new or state-changed devices
                    foreach (var kvp in currentDevices)
                    {
                        if (!_knownDevices.TryGetValue(kvp.Key, out var oldState))
                        {
                            _knownDevices[kvp.Key] = kvp.Value;
                            OnDeviceConnected(kvp.Key, DevicePhysicalStateEx.Unknown, kvp.Value);
                        }
                        else if (oldState != kvp.Value)
                        {
                            _knownDevices[kvp.Key] = kvp.Value;
                            OnDeviceStateChanged(kvp.Key, oldState, kvp.Value);
                        }
                    }

                    // Check for disconnected devices
                    var disconnected = new List<string>();
                    foreach (var kvp in _knownDevices)
                    {
                        if (!currentDevices.ContainsKey(kvp.Key))
                            disconnected.Add(kvp.Key);
                    }

                    foreach (string deviceId in disconnected)
                    {
                        var state = _knownDevices[deviceId];
                        _knownDevices.Remove(deviceId);
                        OnDeviceDisconnected(deviceId, state, DevicePhysicalStateEx.Offline);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error polling ADB devices", ex);
            }
        }

        private Dictionary<string, DevicePhysicalStateEx> DetectCurrentDevices()
        {
            var result = new Dictionary<string, DevicePhysicalStateEx>(StringComparer.OrdinalIgnoreCase);
            var lines = _adbOperator.CommandList("devices");

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("List of devices"))
                    continue;

                string[] parts = line.Split('\t');
                if (parts.Length < 2)
                    continue;

                string serial = parts[0].Trim();
                string stateStr = parts[1].Trim().ToLower();

                if (string.IsNullOrEmpty(serial))
                    continue;

                DevicePhysicalStateEx state = stateStr switch
                {
                    "device" => DevicePhysicalStateEx.Online,
                    "recovery" => DevicePhysicalStateEx.Recovery,
                    "offline" => DevicePhysicalStateEx.Offline,
                    _ => DevicePhysicalStateEx.Unknown
                };

                result[serial] = state;
            }

            return result;
        }

        private void OnDeviceConnected(string deviceId, DevicePhysicalStateEx old, DevicePhysicalStateEx newState)
        {
            _log.Info($"Device connected: {deviceId} ({newState})");
            DeviceConnected?.Invoke(this, new DeviceStateChangedEventArgs(deviceId, old, newState));
        }

        private void OnDeviceDisconnected(string deviceId, DevicePhysicalStateEx old, DevicePhysicalStateEx newState)
        {
            _log.Info($"Device disconnected: {deviceId}");
            DeviceDisconnected?.Invoke(this, new DeviceStateChangedEventArgs(deviceId, old, newState));
        }

        private void OnDeviceStateChanged(string deviceId, DevicePhysicalStateEx old, DevicePhysicalStateEx newState)
        {
            _log.Info($"Device state changed: {deviceId} {old} -> {newState}");
            DeviceStateChanged?.Invoke(this, new DeviceStateChangedEventArgs(deviceId, old, newState));
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
