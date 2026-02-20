using System;
using System.Collections.Generic;
using lenovo.mbg.service.framework.common;
using log4net;

namespace lenovo.mbg.service.framework.devicemgt
{
    /// <summary>
    /// Implements Fastboot operations for device communication in bootloader mode.
    /// </summary>
    public class FastbootOperator : IDeviceOperator
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(FastbootOperator));
        private readonly IProcessRunner _processRunner;
        private readonly string _fastbootPath;
        private string? _targetDevice;

        /// <summary>
        /// Operation-specific timeout map (milliseconds).
        /// </summary>
        public static readonly Dictionary<string, int> OperationToTimeout = new Dictionary<string, int>
        {
            { "flash", Configurations.Timeouts.Flash },
            { "erase", Configurations.Timeouts.Erase },
            { "oem", Configurations.Timeouts.Oem },
            { "getvar", Configurations.Timeouts.GetVar },
            { "reboot", Configurations.Timeouts.Reboot },
            { "reboot-bootloader", Configurations.Timeouts.RebootBootloader },
            { "format", Configurations.Timeouts.Format },
            { "flashall", Configurations.Timeouts.FlashAll },
            { "continue", Configurations.Timeouts.Continue }
        };

        public FastbootOperator(IProcessRunner processRunner, string? fastbootPath = null)
        {
            _processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));
            _fastbootPath = fastbootPath ?? Configurations.FastbootPath;
        }

        /// <summary>
        /// Sets the target device serial for subsequent commands.
        /// </summary>
        public void SetTargetDevice(string deviceID)
        {
            _targetDevice = deviceID;
        }

        /// <summary>
        /// Encapsulates a fastboot command with device serial if set.
        /// </summary>
        public string EncapsulationFastbootCommand(string command)
        {
            if (!string.IsNullOrEmpty(_targetDevice))
                return $"-s {_targetDevice} {command}";
            return command;
        }

        public string Command(string command, int timeout = -1, string deviceID = "")
        {
            if (!string.IsNullOrEmpty(deviceID))
            {
                string? oldTarget = _targetDevice;
                _targetDevice = deviceID;
                string result = ExecuteFastboot(command, timeout);
                _targetDevice = oldTarget;
                return result;
            }

            return ExecuteFastboot(command, timeout);
        }

        private string ExecuteFastboot(string command, int timeout)
        {
            int resolvedTimeout = timeout > 0 ? timeout : ResolveTimeout(command);
            string encapsulated = EncapsulationFastbootCommand(command);

            _log.Info($"Fastboot command: {encapsulated} (timeout: {resolvedTimeout}ms)");
            string response = _processRunner.ProcessString(_fastbootPath, encapsulated, resolvedTimeout);
            _log.Debug($"Fastboot response: {response}");

            Log.AddLog($"fastboot command: {encapsulated}, response: {response}", upload: true);

            // Check for critical exit code
            if (_processRunner.LastExitCode == -1073741515 && string.IsNullOrEmpty(response))
            {
                _log.Error("Fastboot critical error: exit code -1073741515 with empty response");
            }

            return response;
        }

        private List<string> ExecuteFastbootList(string command, int timeout)
        {
            int resolvedTimeout = timeout > 0 ? timeout : ResolveTimeout(command);
            string encapsulated = EncapsulationFastbootCommand(command);

            _log.Info($"Fastboot command (list): {encapsulated} (timeout: {resolvedTimeout}ms)");
            var lines = _processRunner.ProcessList(_fastbootPath, encapsulated, resolvedTimeout);
            _log.Debug($"Fastboot response lines: {lines.Count}");

            return lines;
        }

        /// <summary>
        /// Resolves the timeout for a given command based on the operation type.
        /// </summary>
        private static int ResolveTimeout(string command)
        {
            if (string.IsNullOrEmpty(command)) return Configurations.Timeouts.Standard;

            string operation = command.Split(' ')[0].ToLower();
            return OperationToTimeout.TryGetValue(operation, out int timeout)
                ? timeout
                : Configurations.Timeouts.Standard;
        }

        /// <summary>
        /// Flashes a partition with the specified image file.
        /// </summary>
        public string Flash(string partition, string filePath, string deviceID = "")
        {
            return Command($"flash {partition} \"{filePath}\"", Configurations.Timeouts.Flash, deviceID);
        }

        /// <summary>
        /// Erases a partition.
        /// </summary>
        public string Erase(string partition, string deviceID = "")
        {
            return Command($"erase {partition}", Configurations.Timeouts.Erase, deviceID);
        }

        /// <summary>
        /// Formats a partition.
        /// </summary>
        public string Format(string partition, string deviceID = "")
        {
            return Command($"format {partition}", Configurations.Timeouts.Format, deviceID);
        }

        /// <summary>
        /// Gets all device variables.
        /// </summary>
        public List<string> GetVarAll(string deviceID = "")
        {
            if (!string.IsNullOrEmpty(deviceID))
            {
                string? oldTarget = _targetDevice;
                _targetDevice = deviceID;
                var result = ExecuteFastbootList("getvar all", Configurations.Timeouts.Standard);
                _targetDevice = oldTarget;
                return result;
            }
            return ExecuteFastbootList("getvar all", Configurations.Timeouts.Standard);
        }

        /// <summary>
        /// Gets a specific device variable.
        /// </summary>
        public string GetVar(string variable, string deviceID = "")
        {
            return Command($"getvar {variable}", Configurations.Timeouts.GetVar, deviceID);
        }

        /// <summary>
        /// Reads the secure version information via OEM command.
        /// </summary>
        public string OemReadSv(string deviceID = "")
        {
            return Command("oem read_sv", Configurations.Timeouts.Standard, deviceID);
        }

        /// <summary>
        /// Gets partition information via OEM command.
        /// </summary>
        public string OemPartition(string deviceID = "")
        {
            return Command("oem partition", Configurations.Timeouts.Standard, deviceID);
        }

        /// <summary>
        /// Dumps the logfs partition via OEM command.
        /// </summary>
        public string OemPartitionDumpLogfs(string deviceID = "")
        {
            return Command("oem partition dump logfs", Configurations.Timeouts.ShellCommand, deviceID);
        }

        /// <summary>
        /// Sets fastboot mode via OEM command.
        /// </summary>
        public string OemFbModeSet(string deviceID = "")
        {
            return Command("oem fb_mode_set", Configurations.Timeouts.Oem, deviceID);
        }

        /// <summary>
        /// Clears fastboot mode via OEM command.
        /// </summary>
        public string OemFbModeClear(string deviceID = "")
        {
            return Command("oem fb_mode_clear", Configurations.Timeouts.Oem, deviceID);
        }

        /// <summary>
        /// Flashes all partitions from an XML configuration.
        /// </summary>
        public string FlashAll(string deviceID = "")
        {
            return Command("flashall", Configurations.Timeouts.FlashAll, deviceID);
        }

        /// <summary>
        /// Reboots the device from fastboot mode.
        /// </summary>
        public string Reboot(string deviceID = "")
        {
            return Command("reboot", Configurations.Timeouts.Reboot, deviceID);
        }

        /// <summary>
        /// Reboots the device to bootloader from fastboot mode.
        /// </summary>
        public string RebootBootloader(string deviceID = "")
        {
            return Command("reboot-bootloader", Configurations.Timeouts.RebootBootloader, deviceID);
        }

        /// <summary>
        /// Continues the boot process from fastboot mode.
        /// </summary>
        public string Continue(string deviceID = "")
        {
            return Command("continue", Configurations.Timeouts.Continue, deviceID);
        }

        /// <summary>
        /// Checks for anti-rollback violations in a flash response.
        /// </summary>
        public bool CheckAntiRollback(string response)
        {
            return ErrorDetector.HasAntiRollbackViolation(response);
        }

        /// <summary>
        /// Flashes a partition with retry logic and error checking.
        /// Returns the Result of the operation.
        /// </summary>
        public Result FlashWithVerification(string partition, string filePath, int retries = 3, string deviceID = "")
        {
            string response = RetryHelper.Execute(
                () => Flash(partition, filePath, deviceID),
                retries,
                r => ErrorDetector.HasError(r) && !ErrorDetector.HasAntiRollbackViolation(r));

            Result result = ErrorDetector.EvaluateResponse(response);

            if (result == Result.PASSED)
                Log.AddResult(this, Result.PASSED, null);
            else
                Log.AddResult(this, result, $"Flash {partition} failed: {response}");

            return result;
        }
    }
}
