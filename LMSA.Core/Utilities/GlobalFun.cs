using System.Diagnostics;
using log4net;

namespace lenovo.mbg.service.framework.common
{
    /// <summary>
    /// Provides utility functions such as process killing.
    /// </summary>
    public static class GlobalFun
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(GlobalFun));

        /// <summary>
        /// Kills all processes matching the given name.
        /// </summary>
        /// <param name="processName">Process name without extension (e.g., "adb", "fastboot").</param>
        public static void KillProcess(string processName)
        {
            try
            {
                var processes = Process.GetProcessesByName(processName);
                foreach (var proc in processes)
                {
                    try
                    {
                        if (!proc.HasExited)
                        {
                            proc.Kill();
                            _log.Info($"Killed process: {processName} (PID: {proc.Id})");
                        }
                    }
                    catch (System.Exception ex)
                    {
                        _log.Warn($"Failed to kill process {processName} (PID: {proc.Id}): {ex.Message}");
                    }
                    finally
                    {
                        proc.Dispose();
                    }
                }
            }
            catch (System.Exception ex)
            {
                _log.Error($"Error finding processes named '{processName}'", ex);
            }
        }
    }
}
