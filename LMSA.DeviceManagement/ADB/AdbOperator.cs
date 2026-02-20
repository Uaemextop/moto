using System;
using System.Collections.Generic;
using lenovo.mbg.service.framework.common;
using log4net;

namespace lenovo.mbg.service.framework.devicemgt
{
    /// <summary>
    /// Implements ADB operations for device communication.
    /// </summary>
    public class AdbOperator : IDeviceOperator
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(AdbOperator));
        private readonly IProcessRunner _processRunner;
        private readonly string _adbPath;

        public AdbOperator(IProcessRunner processRunner, string? adbPath = null)
        {
            _processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));
            _adbPath = adbPath ?? Configurations.AdbPath;
        }

        public string Command(string command, int timeout = -1, string deviceID = "")
        {
            string fullCommand = !string.IsNullOrEmpty(deviceID)
                ? $"-s {deviceID} {command}"
                : command;

            _log.Info($"ADB command: {fullCommand}");
            string response = _processRunner.ProcessString(_adbPath, fullCommand, timeout);
            _log.Debug($"ADB response: {response}");

            Log.AddLog($"adb command: {fullCommand}, response: {response}", upload: true);
            return response;
        }

        /// <summary>
        /// Executes an ADB shell command.
        /// </summary>
        public string Shell(string shellCommand, int timeout = -1, string deviceID = "")
        {
            return Command($"shell {shellCommand}", timeout, deviceID);
        }

        /// <summary>
        /// Gets all device properties via 'getprop'.
        /// </summary>
        public string GetProperties(string deviceID = "")
        {
            return Shell("getprop", Configurations.Timeouts.ShellCommand, deviceID);
        }

        /// <summary>
        /// Gets a specific device property.
        /// </summary>
        public string GetProperty(string propertyName, string deviceID = "")
        {
            return Shell($"getprop {propertyName}", Configurations.Timeouts.ShellCommand, deviceID);
        }

        /// <summary>
        /// Installs an APK on the device.
        /// </summary>
        /// <param name="apkPath">Path to the APK file.</param>
        /// <param name="reinstall">Whether to reinstall if already installed.</param>
        /// <param name="deviceID">Optional device serial.</param>
        public string InstallPackage(string apkPath, bool reinstall = true, string deviceID = "")
        {
            string flag = reinstall ? "-r " : "";
            return Command($"install {flag}\"{apkPath}\"", Configurations.Timeouts.Flash, deviceID);
        }

        /// <summary>
        /// Uninstalls a package from the device.
        /// </summary>
        public string UninstallPackage(string packageName, string deviceID = "")
        {
            return Command($"uninstall {packageName}", Configurations.Timeouts.Erase, deviceID);
        }

        /// <summary>
        /// Pushes a file to the device.
        /// </summary>
        public string Push(string localPath, string remotePath, string deviceID = "")
        {
            return Command($"push \"{localPath}\" \"{remotePath}\"", Configurations.Timeouts.Flash, deviceID);
        }

        /// <summary>
        /// Pulls a file from the device.
        /// </summary>
        public string Pull(string remotePath, string localPath, string deviceID = "")
        {
            return Command($"pull \"{remotePath}\" \"{localPath}\"", Configurations.Timeouts.Flash, deviceID);
        }

        /// <summary>
        /// Reboots the device to the specified mode.
        /// </summary>
        public string Reboot(string mode = "", string deviceID = "")
        {
            string cmd = string.IsNullOrEmpty(mode) ? "reboot" : $"reboot {mode}";
            return Command(cmd, Configurations.Timeouts.Reboot, deviceID);
        }

        /// <summary>
        /// Reboots the device to the bootloader.
        /// </summary>
        public string RebootBootloader(string deviceID = "")
        {
            return Reboot("bootloader", deviceID);
        }

        /// <summary>
        /// Reboots the device to recovery mode.
        /// </summary>
        public string RebootRecovery(string deviceID = "")
        {
            return Reboot("recovery", deviceID);
        }

        /// <summary>
        /// Reboots the device to EDL (Emergency Download) mode.
        /// </summary>
        public string RebootEdl(string deviceID = "")
        {
            return Reboot("edl", deviceID);
        }

        /// <summary>
        /// Starts an activity on the device.
        /// </summary>
        public string StartActivity(string packageName, string activityName, string deviceID = "")
        {
            return Shell($"am start -n {packageName}/{activityName}", Configurations.Timeouts.ShellCommand, deviceID);
        }

        /// <summary>
        /// Force-stops an application on the device.
        /// </summary>
        public string ForceStop(string packageName, string deviceID = "")
        {
            return Shell($"am force-stop {packageName}", Configurations.Timeouts.ShellCommand, deviceID);
        }

        /// <summary>
        /// Gets the current focused window on the device.
        /// </summary>
        public string GetCurrentFocus(string deviceID = "")
        {
            return Shell("\"dumpsys window | grep mCurrentFocus\"", Configurations.Timeouts.ShellCommand, deviceID);
        }

        /// <summary>
        /// Gets the Android SDK version of the device.
        /// </summary>
        public string GetSdkVersion(string deviceID = "")
        {
            return GetProperty("ro.build.version.sdk", deviceID);
        }

        /// <summary>
        /// Gets the full build version of the device.
        /// </summary>
        public string GetBuildVersion(string deviceID = "")
        {
            return GetProperty("ro.build.version.full", deviceID);
        }

        /// <summary>
        /// Lists connected devices.
        /// </summary>
        public List<string> GetDevices()
        {
            string response = Command("devices", Configurations.Timeouts.Standard);
            var devices = new List<string>();

            if (string.IsNullOrEmpty(response)) return devices;

            foreach (var line in response.Split('\n'))
            {
                string trimmed = line.Trim();
                if (!string.IsNullOrEmpty(trimmed) && !trimmed.StartsWith("List of") && trimmed.Contains('\t'))
                {
                    devices.Add(trimmed.Split('\t')[0]);
                }
            }

            return devices;
        }

        /// <summary>
        /// Creates a TCP port forward.
        /// </summary>
        public string CreateForward(string localPort, string remotePort, string deviceID = "")
        {
            return Command($"forward tcp:{localPort} tcp:{remotePort}", Configurations.Timeouts.Standard, deviceID);
        }

        /// <summary>
        /// Removes a TCP port forward.
        /// </summary>
        public string RemoveForward(string localPort, string deviceID = "")
        {
            return Command($"forward --remove tcp:{localPort}", Configurations.Timeouts.Standard, deviceID);
        }

        /// <summary>
        /// Removes all TCP port forwards for a device.
        /// </summary>
        public string RemoveAllForwards(string deviceID = "")
        {
            return Command("forward --remove-all", Configurations.Timeouts.Standard, deviceID);
        }

        /// <summary>
        /// Connects to a device over TCP/IP.
        /// </summary>
        public string Connect(string ipAddress, int port = 5555)
        {
            return Command($"connect {ipAddress}:{port}", Configurations.Timeouts.Standard);
        }

        /// <summary>
        /// Disconnects a device connected over TCP/IP.
        /// </summary>
        public string Disconnect(string ipAddress = "")
        {
            string target = string.IsNullOrEmpty(ipAddress) ? "" : $" {ipAddress}";
            return Command($"disconnect{target}", Configurations.Timeouts.Standard);
        }
    }
}
