using lenovo.mbg.service.framework.common;
using log4net;
using System;
using System.Collections.Generic;

namespace lenovo.mbg.service.framework.devicemgt
{
    /// <summary>
    /// Timeout constants for fastboot operations (milliseconds).
    /// </summary>
    internal static class FastbootTimeouts
    {
        public const int Flash = 300_000;       // 5 minutes
        public const int Erase = 60_000;        // 1 minute
        public const int Format = 60_000;       // 1 minute
        public const int FlashAll = 600_000;    // 10 minutes
        public const int GetVar = 12_000;       // 12 seconds
        public const int Oem = 12_000;          // 12 seconds
        public const int OemDumpLogfs = 20_000; // 20 seconds
        public const int Reboot = 20_000;       // 20 seconds
        public const int RebootBootloader = 10_000; // 10 seconds
        public const int Continue = 10_000;     // 10 seconds
    }

    /// <summary>
    /// Wraps fastboot command execution for device flashing and rescue operations.
    /// All commands are executed via fastboot.exe through ProcessRunner.
    /// </summary>
    public class FastbootOperator
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(FastbootOperator));

        private readonly string _deviceId;

        /// <summary>
        /// Initializes a new FastbootOperator.
        /// </summary>
        /// <param name="deviceId">Optional device serial number for multi-device setups.</param>
        public FastbootOperator(string deviceId = "")
        {
            _deviceId = deviceId;
        }

        /// <summary>
        /// Executes a raw fastboot command and returns the output.
        /// </summary>
        /// <param name="command">The fastboot command (e.g., "getvar all").</param>
        /// <param name="timeoutMs">Timeout in milliseconds.</param>
        /// <returns>Combined stdout/stderr output.</returns>
        public string Execute(string command, int timeoutMs = -1)
        {
            string fastbootPath = Configurations.FastbootPath;
            string encapsulated = EncapsulationFastbootCommand(command);

            Log.AddLog($"fastboot command: {encapsulated}", upload: true);
            string response = ProcessRunner.ProcessString(fastbootPath, encapsulated, timeoutMs);
            Log.AddLog($"fastboot response: {response}", upload: true);

            return response;
        }

        /// <summary>
        /// Flashes a partition image file (5-minute timeout).
        /// </summary>
        /// <param name="partition">Target partition name (e.g., "boot", "system").</param>
        /// <param name="imageFile">Full path to the image file.</param>
        /// <returns>Fastboot response string.</returns>
        public string FlashPartition(string partition, string imageFile)
        {
            return Execute($"flash {partition} \"{imageFile}\"", FastbootTimeouts.Flash);
        }

        /// <summary>
        /// Erases a partition (1-minute timeout).
        /// Common targets: userdata, metadata.
        /// </summary>
        /// <param name="partition">Partition name to erase.</param>
        /// <returns>Fastboot response string.</returns>
        public string ErasePartition(string partition)
        {
            return Execute($"erase {partition}", FastbootTimeouts.Erase);
        }

        /// <summary>
        /// Formats a partition (1-minute timeout).
        /// </summary>
        /// <param name="partition">Partition name to format.</param>
        /// <returns>Fastboot response string.</returns>
        public string FormatPartition(string partition)
        {
            return Execute($"format {partition}", FastbootTimeouts.Format);
        }

        /// <summary>
        /// Runs flashall using an XML configuration (10-minute timeout).
        /// </summary>
        /// <returns>Fastboot response string.</returns>
        public string FlashAll()
        {
            return Execute("flashall", FastbootTimeouts.FlashAll);
        }

        /// <summary>
        /// Retrieves all device variables via 'getvar all' (12-second timeout).
        /// </summary>
        /// <returns>Dictionary of variable name to value.</returns>
        public Dictionary<string, string> GetVarAll()
        {
            string response = Execute("getvar all", FastbootTimeouts.GetVar);
            return ParseFastbootVars(response);
        }

        /// <summary>
        /// Retrieves a single fastboot variable (12-second timeout).
        /// </summary>
        /// <param name="variable">Variable name (e.g., "product", "version-bootloader").</param>
        /// <returns>Variable value string.</returns>
        public string GetVar(string variable)
        {
            return Execute($"getvar {variable}", FastbootTimeouts.GetVar);
        }

        /// <summary>
        /// Reads the secure version via OEM command (12-second timeout).
        /// </summary>
        /// <returns>OEM read_sv response.</returns>
        public string OemReadSv()
        {
            return Execute("oem read_sv", FastbootTimeouts.Oem);
        }

        /// <summary>
        /// Reads partition information via OEM command (12-second timeout).
        /// </summary>
        /// <returns>Partition information response.</returns>
        public string OemPartition()
        {
            return Execute("oem partition", FastbootTimeouts.Oem);
        }

        /// <summary>
        /// Dumps the logfs partition via OEM command (20-second timeout).
        /// </summary>
        /// <returns>Logfs dump response.</returns>
        public string OemPartitionDumpLogfs()
        {
            return Execute("oem partition dump logfs", FastbootTimeouts.OemDumpLogfs);
        }

        /// <summary>
        /// Sets fastboot mode via OEM command.
        /// </summary>
        public string OemFbModeSet()
        {
            return Execute("oem fb_mode_set", FastbootTimeouts.Oem);
        }

        /// <summary>
        /// Clears fastboot mode via OEM command.
        /// </summary>
        public string OemFbModeClear()
        {
            return Execute("oem fb_mode_clear", FastbootTimeouts.Oem);
        }

        /// <summary>
        /// Reboots the device normally (20-second timeout).
        /// </summary>
        public string Reboot()
        {
            return Execute("reboot", FastbootTimeouts.Reboot);
        }

        /// <summary>
        /// Reboots back to bootloader (10-second timeout).
        /// </summary>
        public string RebootBootloader()
        {
            return Execute("reboot-bootloader", FastbootTimeouts.RebootBootloader);
        }

        /// <summary>
        /// Continues normal boot from bootloader (10-second timeout).
        /// </summary>
        public string Continue()
        {
            return Execute("continue", FastbootTimeouts.Continue);
        }

        /// <summary>
        /// Checks if a fastboot response indicates an anti-rollback violation.
        /// </summary>
        /// <param name="response">The fastboot command output.</param>
        /// <returns>True if anti-rollback violation detected.</returns>
        public static bool IsAntiRollbackViolation(string response)
        {
            return response != null &&
                   response.Contains("STATUS_SEC_VIOLATE_ANTI_ROLLBACK",
                       StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks if a fastboot response indicates an error.
        /// </summary>
        /// <param name="response">The fastboot command output.</param>
        /// <returns>True if response contains an error indicator.</returns>
        public static bool IsError(string response)
        {
            if (string.IsNullOrEmpty(response))
                return true;
            string lower = response.ToLower();
            return lower.Contains("error") || lower.Contains("fail");
        }

        /// <summary>
        /// Encapsulates a fastboot command with device selector when a device ID is set.
        /// </summary>
        private string EncapsulationFastbootCommand(string command)
        {
            return string.IsNullOrEmpty(_deviceId)
                ? command
                : $"-s {_deviceId} {command}";
        }

        /// <summary>
        /// Parses fastboot 'getvar all' output into a key-value dictionary.
        /// </summary>
        private static Dictionary<string, string> ParseFastbootVars(string response)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrEmpty(response))
                return result;

            foreach (string line in response.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int colonIdx = line.IndexOf(':');
                if (colonIdx > 0)
                {
                    string key = line.Substring(0, colonIdx).Trim();
                    string value = line.Substring(colonIdx + 1).Trim();
                    result[key] = value;
                }
            }

            return result;
        }
    }
}
