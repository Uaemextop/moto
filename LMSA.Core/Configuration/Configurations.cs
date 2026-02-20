using System;
using System.IO;

namespace lenovo.mbg.service.framework.common
{
    /// <summary>
    /// Provides application-wide configuration paths and settings.
    /// </summary>
    public static class Configurations
    {
        private static string? _baseDirectory;

        /// <summary>
        /// Gets or sets the base directory for tool paths. Defaults to AppDomain base directory.
        /// </summary>
        public static string BaseDirectory
        {
            get => _baseDirectory ?? AppDomain.CurrentDomain.BaseDirectory;
            set => _baseDirectory = value;
        }

        public static string AdbPath => Path.Combine(BaseDirectory, "adb.exe");
        public static string FastbootPath => Path.Combine(BaseDirectory, "fastboot.exe");
        public static string FastbootMonitorPath => Path.Combine(BaseDirectory, "fastbootmonitor.exe");

        public static string LogDirectory => Path.Combine(BaseDirectory, "logs");
        public static string PluginsDirectory => Path.Combine(BaseDirectory, "plugins");
        public static string LangDirectory => Path.Combine(BaseDirectory, "lang");

        public static string RescueFailedLogPath => Path.Combine(LogDirectory, "rescue_failed.log");
        public static string PluginConfigPath => Path.Combine(BaseDirectory, "plugins.xml");
        public static string DownloadConfigPath => Path.Combine(BaseDirectory, "download-config.xml");

        /// <summary>
        /// Default timeout values in milliseconds for different operations.
        /// </summary>
        public static class Timeouts
        {
            public const int Standard = 12000;
            public const int ShellCommand = 20000;
            public const int Flash = 300000;
            public const int Erase = 60000;
            public const int Format = 60000;
            public const int Oem = 60000;
            public const int GetVar = 20000;
            public const int Reboot = 20000;
            public const int RebootBootloader = 10000;
            public const int FlashAll = 600000;
            public const int Continue = 10000;
        }
    }
}
