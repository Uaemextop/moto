using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using log4net;

namespace lenovo.mbg.service.framework.common
{
    /// <summary>
    /// Executes external processes (adb, fastboot) and captures their output.
    /// </summary>
    public static class ProcessRunner
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ProcessRunner));

        /// <summary>
        /// Executes a process and returns all output as a single string.
        /// </summary>
        /// <param name="executable">Path to the executable.</param>
        /// <param name="arguments">Command-line arguments.</param>
        /// <param name="timeoutMs">Timeout in milliseconds. Use -1 for infinite.</param>
        /// <returns>Combined stdout and stderr output.</returns>
        public static string ProcessString(string executable, string arguments, int timeoutMs = -1)
        {
            try
            {
                var sb = new StringBuilder();
                using var process = CreateProcess(executable, arguments);

                process.OutputDataReceived += (_, e) => { if (e.Data != null) sb.AppendLine(e.Data); };
                process.ErrorDataReceived += (_, e) => { if (e.Data != null) sb.AppendLine(e.Data); };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                bool finished = timeoutMs <= 0
                    ? process.WaitForExit(int.MaxValue)
                    : process.WaitForExit(timeoutMs);

                if (!finished)
                {
                    KillProcessSafe(process);
                    _log.Warn($"Process timed out after {timeoutMs}ms: {executable} {arguments}");
                }
                else
                {
                    // Ensure all async output has been flushed after process exit
                    process.WaitForExit();
                }

                string output = sb.ToString().TrimEnd();
                _log.Debug($"ProcessString [{executable} {arguments}] => {output}");
                return output;
            }
            catch (Exception ex)
            {
                _log.Error($"ProcessString failed for [{executable} {arguments}]", ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Executes a process and returns output as a list of lines.
        /// </summary>
        /// <param name="executable">Path to the executable.</param>
        /// <param name="arguments">Command-line arguments.</param>
        /// <param name="timeoutMs">Timeout in milliseconds. Use -1 for infinite.</param>
        /// <returns>List of output lines.</returns>
        public static List<string> ProcessList(string executable, string arguments, int timeoutMs = -1)
        {
            var lines = new List<string>();
            try
            {
                using var process = CreateProcess(executable, arguments);

                process.OutputDataReceived += (_, e) => { if (e.Data != null) lines.Add(e.Data); };
                process.ErrorDataReceived += (_, e) => { if (e.Data != null) lines.Add(e.Data); };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                bool finished = timeoutMs <= 0
                    ? process.WaitForExit(int.MaxValue)
                    : process.WaitForExit(timeoutMs);

                if (!finished)
                {
                    KillProcessSafe(process);
                    _log.Warn($"Process timed out after {timeoutMs}ms: {executable} {arguments}");
                }
                else
                {
                    // Ensure all async output has been flushed after process exit
                    process.WaitForExit();
                }

                _log.Debug($"ProcessList [{executable} {arguments}] => {lines.Count} lines");
            }
            catch (Exception ex)
            {
                _log.Error($"ProcessList failed for [{executable} {arguments}]", ex);
            }

            return lines;
        }

        /// <summary>
        /// Kills all running adb.exe processes.
        /// </summary>
        public static void KillAdbProcesses()
        {
            KillProcessesByName("adb");
        }

        /// <summary>
        /// Kills all running fastboot.exe processes.
        /// </summary>
        public static void KillFastbootProcesses()
        {
            KillProcessesByName("fastboot");
        }

        private static Process CreateProcess(string executable, string arguments)
        {
            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = executable,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                }
            };
        }

        private static void KillProcessSafe(Process process)
        {
            try
            {
                if (!process.HasExited)
                    process.Kill();
            }
            catch (Exception ex)
            {
                _log.Warn($"Failed to kill process: {ex.Message}");
            }
        }

        private static void KillProcessesByName(string name)
        {
            foreach (var process in Process.GetProcessesByName(name))
            {
                try
                {
                    process.Kill();
                    _log.Info($"Killed process: {name} (PID {process.Id})");
                }
                catch (Exception ex)
                {
                    _log.Warn($"Failed to kill {name} process: {ex.Message}");
                }
                finally
                {
                    process.Dispose();
                }
            }
        }
    }
}
