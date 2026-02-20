using System;
using System.IO;

namespace lenovo.mbg.service.framework.common
{
    /// <summary>
    /// Provides centralized configuration paths and settings for the LMSA application.
    /// </summary>
    public static class Configurations
    {
        private static readonly string _baseDirectory =
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            ?? AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>Gets the path to the adb.exe tool.</summary>
        public static string AdbPath => Path.Combine(_baseDirectory, "adb.exe");

        /// <summary>Gets the path to the fastboot.exe tool.</summary>
        public static string FastbootPath => Path.Combine(_baseDirectory, "fastboot.exe");

        /// <summary>Gets the path to the fastbootmonitor.exe tool.</summary>
        public static string FastbootMonitorPath => Path.Combine(_baseDirectory, "fastbootmonitor.exe");

        /// <summary>Gets the application log directory.</summary>
        public static string LogDirectory => Path.Combine(_baseDirectory, "logs");

        /// <summary>Gets the rescue failure log path.</summary>
        public static string RescueFailedLogPath => Path.Combine(LogDirectory, "rescue_failed.log");

        /// <summary>Gets the plugins directory path.</summary>
        public static string PluginsDirectory => Path.Combine(_baseDirectory, "plugins");

        /// <summary>Gets the language packs directory.</summary>
        public static string LangDirectory => Path.Combine(_baseDirectory, "lang");

        /// <summary>Gets the base application directory.</summary>
        public static string BaseDirectory => _baseDirectory;

        /// <summary>Gets the default ADB server port.</summary>
        public static int AdbServerPort => 5037;

        /// <summary>Gets the default timeout for short operations (ms).</summary>
        public static int DefaultTimeoutMs => 20000;
    }
}
