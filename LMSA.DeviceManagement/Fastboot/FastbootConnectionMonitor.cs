using System;
using log4net;

namespace lenovo.mbg.service.framework.devicemgt
{
    /// <summary>
    /// Monitors Fastboot device connections and reports state changes.
    /// </summary>
    public class FastbootConnectionMonitor
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(FastbootConnectionMonitor));

        /// <summary>
        /// Event raised when a device is detected in fastboot mode.
        /// </summary>
        public event EventHandler<DeviceConnectionEventArgs>? DeviceConnected;

        /// <summary>
        /// Event raised when a fastboot device disconnects.
        /// </summary>
        public event EventHandler<DeviceConnectionEventArgs>? DeviceDisconnected;

        private readonly FastbootOperator _fastbootOperator;
        private bool _isMonitoring;

        public bool IsMonitoring => _isMonitoring;

        public FastbootConnectionMonitor(FastbootOperator fastbootOperator)
        {
            _fastbootOperator = fastbootOperator ?? throw new ArgumentNullException(nameof(fastbootOperator));
        }

        /// <summary>
        /// Starts monitoring for Fastboot device connections.
        /// </summary>
        public void Start()
        {
            _isMonitoring = true;
            _log.Info("Fastboot connection monitoring started");
        }

        /// <summary>
        /// Stops monitoring for Fastboot device connections.
        /// </summary>
        public void Stop()
        {
            _isMonitoring = false;
            _log.Info("Fastboot connection monitoring stopped");
        }

        protected virtual void OnDeviceConnected(string deviceId)
        {
            DeviceConnected?.Invoke(this, new DeviceConnectionEventArgs(deviceId, "fastboot"));
        }

        protected virtual void OnDeviceDisconnected(string deviceId)
        {
            DeviceDisconnected?.Invoke(this, new DeviceConnectionEventArgs(deviceId, "offline"));
        }
    }
}
