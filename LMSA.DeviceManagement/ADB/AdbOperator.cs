using lenovo.mbg.service.framework.common;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;

namespace lenovo.mbg.service.framework.devicemgt
{
    /// <summary>
    /// Wraps ADB command execution for device management operations.
    /// All commands are executed via adb.exe through ProcessRunner.
    /// </summary>
    public class AdbOperator
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(AdbOperator));

        /// <summary>
        /// Executes an ADB command and returns the combined output.
        /// </summary>
        /// <param name="command">The ADB command (e.g., "shell getprop").</param>
        /// <param name="timeoutMs">Timeout in milliseconds. Use -1 for no timeout.</param>
        /// <param name="deviceId">Optional device serial number for multi-device setups.</param>
        /// <returns>Combined stdout/stderr output string.</returns>
        public string Command(string command, int timeoutMs = -1, string deviceId = "")
        {
            string adbPath = Configurations.AdbPath;
            string fullCommand = !string.IsNullOrEmpty(deviceId)
                ? $"-s {deviceId} {command}"
                : command;

            Log.AddLog($"adb command: {fullCommand}", upload: true);
            string response = ProcessRunner.ProcessString(adbPath, fullCommand, timeoutMs);
            Log.AddLog($"adb response: {response}", upload: true);

            return response;
        }

        /// <summary>
        /// Executes an ADB command and returns output as a list of lines.
        /// </summary>
        /// <param name="command">The ADB command.</param>
        /// <param name="timeoutMs">Timeout in milliseconds.</param>
        /// <param name="deviceId">Optional device serial number.</param>
        /// <returns>List of output lines.</returns>
        public List<string> CommandList(string command, int timeoutMs = -1, string deviceId = "")
        {
            string adbPath = Configurations.AdbPath;
            string fullCommand = !string.IsNullOrEmpty(deviceId)
                ? $"-s {deviceId} {command}"
                : command;

            Log.AddLog($"adb command (list): {fullCommand}", upload: true);
            return ProcessRunner.ProcessList(adbPath, fullCommand, timeoutMs);
        }

        /// <summary>
        /// Installs an APK on the device. Uses -r flag to allow reinstall.
        /// </summary>
        /// <param name="apkPath">Full path to the APK file.</param>
        /// <param name="deviceId">Optional device serial number.</param>
        /// <returns>ADB install response.</returns>
        public string InstallPackage(string apkPath, string deviceId = "")
        {
            if (!File.Exists(apkPath))
            {
                _log.Error($"APK not found: {apkPath}");
                return "error: APK file not found";
            }
            return Command($"install -r \"{apkPath}\"", Configurations.DefaultTimeoutMs, deviceId);
        }

        /// <summary>
        /// Uninstalls a package from the device.
        /// </summary>
        /// <param name="packageName">The package name (e.g., com.example.app).</param>
        /// <param name="deviceId">Optional device serial number.</param>
        /// <returns>ADB uninstall response.</returns>
        public string UninstallPackage(string packageName, string deviceId = "")
        {
            return Command($"uninstall {packageName}", Configurations.DefaultTimeoutMs, deviceId);
        }

        /// <summary>
        /// Pulls a file from the device to the local machine.
        /// </summary>
        /// <param name="remotePath">Path on the device.</param>
        /// <param name="localPath">Local destination path.</param>
        /// <param name="deviceId">Optional device serial number.</param>
        /// <returns>ADB pull response.</returns>
        public string PullFile(string remotePath, string localPath, string deviceId = "")
        {
            return Command($"pull \"{remotePath}\" \"{localPath}\"",
                Configurations.DefaultTimeoutMs, deviceId);
        }

        /// <summary>
        /// Pushes a local file to the device.
        /// </summary>
        /// <param name="localPath">Local source file path.</param>
        /// <param name="remotePath">Destination path on the device.</param>
        /// <param name="deviceId">Optional device serial number.</param>
        /// <returns>ADB push response.</returns>
        public string PushFile(string localPath, string remotePath, string deviceId = "")
        {
            if (!File.Exists(localPath) && !Directory.Exists(localPath))
            {
                _log.Error($"Local file/directory not found: {localPath}");
                return "error: local file not found";
            }
            return Command($"push \"{localPath}\" \"{remotePath}\"",
                Configurations.DefaultTimeoutMs, deviceId);
        }

        /// <summary>
        /// Executes a shell command on the device.
        /// </summary>
        /// <param name="shellCommand">The shell command to run.</param>
        /// <param name="deviceId">Optional device serial number.</param>
        /// <returns>Shell command output.</returns>
        public string Shell(string shellCommand, string deviceId = "")
        {
            return Command($"shell {shellCommand}", Configurations.DefaultTimeoutMs, deviceId);
        }

        /// <summary>
        /// Gets all device properties via 'getprop'.
        /// </summary>
        /// <param name="deviceId">Optional device serial number.</param>
        /// <returns>Dictionary of property name to value.</returns>
        public Dictionary<string, string> GetAllProperties(string deviceId = "")
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var lines = CommandList("shell getprop", Configurations.DefaultTimeoutMs, deviceId);

            foreach (string line in lines)
            {
                int bracketStart = line.IndexOf('[');
                int bracketEnd = line.IndexOf(']');
                int valueBracketStart = line.LastIndexOf('[');
                int valueBracketEnd = line.LastIndexOf(']');

                if (bracketStart >= 0 && bracketEnd > bracketStart
                    && valueBracketStart > bracketEnd && valueBracketEnd > valueBracketStart)
                {
                    string key = line.Substring(bracketStart + 1, bracketEnd - bracketStart - 1);
                    string value = line.Substring(valueBracketStart + 1, valueBracketEnd - valueBracketStart - 1);
                    result[key] = value;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets a single device property.
        /// </summary>
        /// <param name="propertyName">The property name (e.g., "ro.build.version.sdk").</param>
        /// <param name="deviceId">Optional device serial number.</param>
        /// <returns>Property value, or empty string if not found.</returns>
        public string GetProperty(string propertyName, string deviceId = "")
        {
            return Command($"shell getprop {propertyName}", Configurations.DefaultTimeoutMs, deviceId)
                .Trim();
        }

        /// <summary>
        /// Reboots the device normally.
        /// </summary>
        public string Reboot(string deviceId = "") =>
            Command("reboot", Configurations.DefaultTimeoutMs, deviceId);

        /// <summary>
        /// Reboots the device into the bootloader (fastboot mode).
        /// </summary>
        public string RebootBootloader(string deviceId = "") =>
            Command("reboot bootloader", Configurations.DefaultTimeoutMs, deviceId);

        /// <summary>
        /// Reboots the device into recovery mode.
        /// </summary>
        public string RebootRecovery(string deviceId = "") =>
            Command("reboot recovery", Configurations.DefaultTimeoutMs, deviceId);

        /// <summary>
        /// Reboots the device into EDL (Emergency Download Mode).
        /// </summary>
        public string RebootEdl(string deviceId = "") =>
            Command("reboot edl", Configurations.DefaultTimeoutMs, deviceId);

        /// <summary>
        /// Starts an Android activity using the activity manager.
        /// </summary>
        /// <param name="componentName">Component name (e.g., "com.pkg/.Activity").</param>
        /// <param name="deviceId">Optional device serial number.</param>
        public string StartActivity(string componentName, string deviceId = "") =>
            Command($"shell am start -n {componentName}", Configurations.DefaultTimeoutMs, deviceId);

        /// <summary>
        /// Force-stops an application.
        /// </summary>
        /// <param name="packageName">The package name to force-stop.</param>
        /// <param name="deviceId">Optional device serial number.</param>
        public string ForceStopApp(string packageName, string deviceId = "") =>
            Command($"shell am force-stop {packageName}", Configurations.DefaultTimeoutMs, deviceId);

        /// <summary>
        /// Creates a TCP port forward from the local machine to the device.
        /// </summary>
        /// <param name="localPort">Local TCP port.</param>
        /// <param name="remotePort">Remote TCP port on device.</param>
        /// <param name="deviceId">Optional device serial number.</param>
        public string CreateForward(int localPort, int remotePort, string deviceId = "") =>
            Command($"forward tcp:{localPort} tcp:{remotePort}", Configurations.DefaultTimeoutMs, deviceId);

        /// <summary>
        /// Removes a specific port forward.
        /// </summary>
        public string RemoveForward(int localPort, string deviceId = "") =>
            Command($"forward --remove tcp:{localPort}", Configurations.DefaultTimeoutMs, deviceId);

        /// <summary>
        /// Removes all port forwards.
        /// </summary>
        public string RemoveAllForwards(string deviceId = "") =>
            Command("forward --remove-all", Configurations.DefaultTimeoutMs, deviceId);

        /// <summary>
        /// Gets a list of connected ADB device serial numbers.
        /// </summary>
        /// <returns>List of device serial numbers.</returns>
        public List<string> GetDevices()
        {
            var devices = new List<string>();
            var lines = CommandList("devices");
            foreach (string line in lines)
            {
                if (line.Contains("\tdevice") || line.Contains("\trecovery"))
                {
                    string serial = line.Split('\t')[0].Trim();
                    if (!string.IsNullOrEmpty(serial))
                        devices.Add(serial);
                }
            }
            return devices;
        }
    }
}
