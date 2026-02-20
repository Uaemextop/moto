using System;
using log4net;

namespace lenovo.mbg.service.framework.devicemgt
{
    /// <summary>
    /// Monitors ADB device connections and reports state changes.
    /// </summary>
    public class AdbConnectionMonitor
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(AdbConnectionMonitor));

        /// <summary>
        /// Event raised when a device connects.
        /// </summary>
        public event EventHandler<DeviceConnectionEventArgs>? DeviceConnected;

        /// <summary>
        /// Event raised when a device disconnects.
        /// </summary>
        public event EventHandler<DeviceConnectionEventArgs>? DeviceDisconnected;

        /// <summary>
        /// Event raised when device state changes.
        /// </summary>
        public event EventHandler<DeviceConnectionEventArgs>? DeviceStateChanged;

        private readonly AdbOperator _adbOperator;
        private bool _isMonitoring;

        public bool IsMonitoring => _isMonitoring;

        public AdbConnectionMonitor(AdbOperator adbOperator)
        {
            _adbOperator = adbOperator ?? throw new ArgumentNullException(nameof(adbOperator));
        }

        /// <summary>
        /// Starts monitoring for ADB device connections.
        /// </summary>
        public void Start()
        {
            _isMonitoring = true;
            _log.Info("ADB connection monitoring started");
        }

        /// <summary>
        /// Stops monitoring for ADB device connections.
        /// </summary>
        public void Stop()
        {
            _isMonitoring = false;
            _log.Info("ADB connection monitoring stopped");
        }

        protected virtual void OnDeviceConnected(string deviceId, string state)
        {
            DeviceConnected?.Invoke(this, new DeviceConnectionEventArgs(deviceId, state));
        }

        protected virtual void OnDeviceDisconnected(string deviceId)
        {
            DeviceDisconnected?.Invoke(this, new DeviceConnectionEventArgs(deviceId, "offline"));
        }

        protected virtual void OnDeviceStateChanged(string deviceId, string state)
        {
            DeviceStateChanged?.Invoke(this, new DeviceConnectionEventArgs(deviceId, state));
        }
    }

    /// <summary>
    /// Event args for device connection events.
    /// </summary>
    public class DeviceConnectionEventArgs : EventArgs
    {
        public string DeviceId { get; }
        public string State { get; }

        public DeviceConnectionEventArgs(string deviceId, string state)
        {
            DeviceId = deviceId;
            State = state;
        }
    }
}
