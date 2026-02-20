using System.Collections.Generic;

namespace lenovo.mbg.service.framework.devicemgt
{
    /// <summary>
    /// Contains device information retrieved from ADB or Fastboot.
    /// </summary>
    public class DeviceInfo
    {
        /// <summary>Gets or sets the device serial number (ADB device ID).</summary>
        public string SerialNumber { get; set; } = string.Empty;

        /// <summary>Gets or sets the current physical state of the device.</summary>
        public DevicePhysicalStateEx State { get; set; } = DevicePhysicalStateEx.Unknown;

        /// <summary>Gets or sets the device model name.</summary>
        public string Model { get; set; } = string.Empty;

        /// <summary>Gets or sets the device manufacturer.</summary>
        public string Manufacturer { get; set; } = string.Empty;

        /// <summary>Gets or sets the Android SDK version.</summary>
        public string AndroidSdkVersion { get; set; } = string.Empty;

        /// <summary>Gets or sets the full build version string.</summary>
        public string BuildVersion { get; set; } = string.Empty;

        /// <summary>Gets or sets the device IMEI (if available).</summary>
        public string Imei { get; set; } = string.Empty;

        /// <summary>Gets or sets raw fastboot variables retrieved via 'getvar all'.</summary>
        public Dictionary<string, string> FastbootVariables { get; set; } = new Dictionary<string, string>();

        /// <summary>Gets or sets raw ADB properties retrieved via 'getprop'.</summary>
        public Dictionary<string, string> AdbProperties { get; set; } = new Dictionary<string, string>();

        public override string ToString() =>
            $"{Manufacturer} {Model} [{SerialNumber}] State={State}";
    }
}
