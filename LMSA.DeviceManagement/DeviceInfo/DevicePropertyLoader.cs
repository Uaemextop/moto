using System;
using System.Collections.Generic;
using System.Linq;
using log4net;

namespace lenovo.mbg.service.framework.devicemgt
{
    /// <summary>
    /// Parses and provides access to device properties from ADB getprop and fastboot getvar.
    /// </summary>
    public class DevicePropertyLoader
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(DevicePropertyLoader));
        private readonly Dictionary<string, string> _properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the number of loaded properties.
        /// </summary>
        public int Count => _properties.Count;

        /// <summary>
        /// Parses ADB getprop output (format: [key]: [value]).
        /// </summary>
        public void LoadFromGetProp(string getpropOutput)
        {
            if (string.IsNullOrEmpty(getpropOutput)) return;

            foreach (var line in getpropOutput.Split('\n'))
            {
                string trimmed = line.Trim();
                if (trimmed.StartsWith("[") && trimmed.Contains("]: ["))
                {
                    int keyStart = 1;
                    int keyEnd = trimmed.IndexOf(']');
                    int valueStart = trimmed.IndexOf("]: [") + 4;
                    int valueEnd = trimmed.LastIndexOf(']');

                    if (keyEnd > keyStart && valueEnd > valueStart)
                    {
                        string key = trimmed.Substring(keyStart, keyEnd - keyStart);
                        string value = trimmed.Substring(valueStart, valueEnd - valueStart);
                        _properties[key] = value;
                    }
                }
            }

            _log.Info($"Loaded {_properties.Count} properties from getprop");
        }

        /// <summary>
        /// Parses fastboot getvar all output (format: (bootloader) key: value or key: value).
        /// </summary>
        public void LoadFromFastbootVars(List<string> lines)
        {
            if (lines == null) return;

            foreach (var line in lines)
            {
                string trimmed = line.Trim();

                // Remove "(bootloader) " prefix if present
                if (trimmed.StartsWith("(bootloader)"))
                    trimmed = trimmed.Substring("(bootloader)".Length).Trim();

                int colonIndex = trimmed.IndexOf(':');
                if (colonIndex > 0)
                {
                    string key = trimmed.Substring(0, colonIndex).Trim();
                    string value = trimmed.Substring(colonIndex + 1).Trim();
                    if (!string.IsNullOrEmpty(key))
                        _properties[key] = value;
                }
            }

            _log.Info($"Loaded {_properties.Count} properties from fastboot getvar");
        }

        /// <summary>
        /// Gets a property value by key.
        /// </summary>
        public string GetProperty(string key, string defaultValue = "")
        {
            return _properties.TryGetValue(key, out string? value) ? value : defaultValue;
        }

        /// <summary>
        /// Checks whether a property key exists.
        /// </summary>
        public bool HasProperty(string key)
        {
            return _properties.ContainsKey(key);
        }

        /// <summary>
        /// Gets all properties as a read-only dictionary.
        /// </summary>
        public IReadOnlyDictionary<string, string> GetAllProperties()
        {
            return _properties;
        }

        // Convenience accessors for common properties

        public string DeviceModel => GetProperty("ro.product.model");
        public string Manufacturer => GetProperty("ro.product.manufacturer");
        public string SerialNumber => GetProperty("ro.serialno");
        public string AndroidVersion => GetProperty("ro.build.version.release");
        public string SdkVersion => GetProperty("ro.build.version.sdk");
        public string BuildVersion => GetProperty("ro.build.version.full");
        public string ProductName => GetProperty("ro.product.name");
        public string Imei => GetProperty("persist.radio.imei");
    }
}
