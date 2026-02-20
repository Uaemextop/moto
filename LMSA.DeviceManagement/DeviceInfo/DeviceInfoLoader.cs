using lenovo.mbg.service.framework.common;
using log4net;
using System;
using System.Collections.Generic;

namespace lenovo.mbg.service.framework.devicemgt
{
    /// <summary>
    /// Retrieves and parses device information from both ADB and Fastboot modes.
    /// </summary>
    public class DeviceInfoLoader
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(DeviceInfoLoader));

        private readonly AdbOperator _adbOperator;
        private readonly FastbootOperator _fastbootOperator;

        /// <summary>
        /// Initializes a new DeviceInfoLoader with default operators.
        /// </summary>
        /// <param name="deviceId">Optional device serial number.</param>
        public DeviceInfoLoader(string deviceId = "")
        {
            _adbOperator = new AdbOperator();
            _fastbootOperator = new FastbootOperator(deviceId);
        }

        /// <summary>
        /// Initializes a new DeviceInfoLoader with provided operators (for testing).
        /// </summary>
        public DeviceInfoLoader(AdbOperator adbOperator, FastbootOperator fastbootOperator)
        {
            _adbOperator = adbOperator ?? throw new ArgumentNullException(nameof(adbOperator));
            _fastbootOperator = fastbootOperator ?? throw new ArgumentNullException(nameof(fastbootOperator));
        }

        /// <summary>
        /// Reads all device properties from a device in ADB mode.
        /// </summary>
        /// <param name="deviceId">Device serial number.</param>
        /// <returns>Populated DeviceInfo instance.</returns>
        public DeviceInfo ReadPropertiesFromAdb(string deviceId = "")
        {
            var info = new DeviceInfo { SerialNumber = deviceId, State = DevicePhysicalStateEx.Online };

            try
            {
                var properties = _adbOperator.GetAllProperties(deviceId);
                info.AdbProperties = properties;

                if (properties.TryGetValue("ro.product.model", out string? model) && model != null)
                    info.Model = model;

                if (properties.TryGetValue("ro.product.manufacturer", out string? manufacturer) && manufacturer != null)
                    info.Manufacturer = manufacturer;

                if (properties.TryGetValue("ro.build.version.sdk", out string? sdk) && sdk != null)
                    info.AndroidSdkVersion = sdk;

                if (properties.TryGetValue("ro.build.version.full", out string? buildVersion) && buildVersion != null)
                    info.BuildVersion = buildVersion;

                Log.AddLog($"Read ADB properties for device {deviceId}: {info}", upload: true);
            }
            catch (Exception ex)
            {
                _log.Error($"Failed to read ADB properties for device {deviceId}", ex);
            }

            return info;
        }

        /// <summary>
        /// Reads device variables from a device in Fastboot mode.
        /// </summary>
        /// <returns>Populated DeviceInfo instance with fastboot variables.</returns>
        public DeviceInfo ReadPropertiesInFastboot()
        {
            var info = new DeviceInfo { State = DevicePhysicalStateEx.Fastboot };

            try
            {
                var vars = _fastbootOperator.GetVarAll();
                info.FastbootVariables = vars;

                if (vars.TryGetValue("product", out string? product) && product != null)
                    info.Model = product;

                if (vars.TryGetValue("serialno", out string? serial) && serial != null)
                    info.SerialNumber = serial;

                Log.AddLog($"Read fastboot variables: {info}", upload: true);
            }
            catch (Exception ex)
            {
                _log.Error("Failed to read fastboot variables", ex);
            }

            return info;
        }

        /// <summary>
        /// Reads the secure version from a device in fastboot mode.
        /// </summary>
        /// <returns>Secure version string, or empty if not available.</returns>
        public string ReadSecureVersion()
        {
            try
            {
                string response = _fastbootOperator.OemReadSv();
                Log.AddLog($"oem read_sv response: {response}", upload: true);
                return response;
            }
            catch (Exception ex)
            {
                _log.Error("Failed to read secure version", ex);
                return string.Empty;
            }
        }
    }
}
