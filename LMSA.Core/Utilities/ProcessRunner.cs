using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using log4net;

namespace lenovo.mbg.service.framework.common
{
    /// <summary>
    /// Executes external processes (adb, fastboot) with timeout support.
    /// </summary>
    public class ProcessRunner : IProcessRunner
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ProcessRunner));
        private int _lastExitCode;

        public int LastExitCode => _lastExitCode;

        public string ProcessString(string executablePath, string arguments, int timeout = -1)
        {
            ValidateArguments(executablePath, arguments);

            var output = new StringBuilder();
            var error = new StringBuilder();

            try
            {
                using var process = CreateProcess(executablePath, arguments);
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null) output.AppendLine(e.Data);
                };
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null) error.AppendLine(e.Data);
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                bool exited = timeout > 0
                    ? process.WaitForExit(timeout)
                    : (process.WaitForExit(30000), true).Item2; // Default 30s max wait

                if (!exited)
                {
                    SafeKillProcess(process);
                    _lastExitCode = -1;
                    _log.Warn($"Process timed out after {timeout}ms: {executablePath} {arguments}");
                    return output.ToString() + error.ToString();
                }

                // Ensure all async output is flushed
                process.WaitForExit();
                _lastExitCode = process.ExitCode;
            }
            catch (Exception ex)
            {
                _log.Error($"Process execution failed: {executablePath} {arguments}", ex);
                _lastExitCode = -1;
                return string.Empty;
            }

            string result = output.ToString() + error.ToString();
            return result.TrimEnd('\r', '\n');
        }

        public List<string> ProcessList(string executablePath, string arguments, int timeout = -1)
        {
            ValidateArguments(executablePath, arguments);

            var lines = new List<string>();

            try
            {
                using var process = CreateProcess(executablePath, arguments);
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null) lines.Add(e.Data);
                };
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null) lines.Add(e.Data);
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                bool exited = timeout > 0
                    ? process.WaitForExit(timeout)
                    : (process.WaitForExit(30000), true).Item2;

                if (!exited)
                {
                    SafeKillProcess(process);
                    _lastExitCode = -1;
                    _log.Warn($"Process timed out after {timeout}ms: {executablePath} {arguments}");
                    return lines;
                }

                // Ensure all async output is flushed
                process.WaitForExit();
                _lastExitCode = process.ExitCode;
            }
            catch (Exception ex)
            {
                _log.Error($"Process execution failed: {executablePath} {arguments}", ex);
                _lastExitCode = -1;
            }

            return lines;
        }

        private static Process CreateProcess(string executablePath, string arguments)
        {
            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = executablePath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
        }

        private static void ValidateArguments(string executablePath, string arguments)
        {
            if (string.IsNullOrWhiteSpace(executablePath))
                throw new ArgumentException("Executable path cannot be empty.", nameof(executablePath));

            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            // Basic command injection protection: reject shell metacharacters in executable path
            if (executablePath.Contains('|') || executablePath.Contains('&') ||
                executablePath.Contains(';') || executablePath.Contains('`'))
            {
                throw new ArgumentException("Executable path contains invalid characters.", nameof(executablePath));
            }
        }

        private static void SafeKillProcess(Process process)
        {
            try
            {
                if (!process.HasExited)
                    process.Kill();
            }
            catch (InvalidOperationException)
            {
                // Process already exited
            }
        }
    }
}
