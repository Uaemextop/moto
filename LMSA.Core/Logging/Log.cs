using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Reflection;

namespace lenovo.mbg.service.framework.common
{
    /// <summary>
    /// Central logging facility for the LMSA framework.
    /// Provides structured log methods compatible with the original decompiled patterns.
    /// </summary>
    public static class Log
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Log));

        static Log()
        {
            ConfigureLog4Net();
        }

        private static void ConfigureLog4Net()
        {
            var configFile = new FileInfo(Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? AppDomain.CurrentDomain.BaseDirectory,
                "log4net.config"));

            if (configFile.Exists)
            {
                XmlConfigurator.ConfigureAndWatch(LogManager.GetRepository(Assembly.GetCallingAssembly()), configFile);
            }
            else
            {
                BasicConfigurator.Configure(LogManager.GetRepository(Assembly.GetCallingAssembly()));
            }
        }

        /// <summary>
        /// Adds an informational log entry. Set upload=true for diagnostic uploads.
        /// </summary>
        public static void AddLog(string message, bool upload = false)
        {
            if (upload)
                _log.Info($"[UPLOAD] {message}");
            else
                _log.Info(message);
        }

        /// <summary>
        /// Logs an operation result (PASSED/FAILED/QUIT) associated with a step object.
        /// </summary>
        public static void AddResult(object step, Result result, string? errorDescription)
        {
            string stepName = step?.GetType().Name ?? "Unknown";
            if (result == Result.PASSED)
                _log.Info($"[RESULT] {stepName}: {result}");
            else
                _log.Error($"[RESULT] {stepName}: {result} - {errorDescription}");
        }

        /// <summary>
        /// Logs an info message with optional step context.
        /// </summary>
        public static void AddInfo(string message)
        {
            _log.Info(message);
        }

        /// <summary>
        /// Logs a debug-level message.
        /// </summary>
        public static void Debug(string message)
        {
            _log.Debug(message);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        public static void Warn(string message)
        {
            _log.Warn(message);
        }

        /// <summary>
        /// Logs an error message with optional exception.
        /// </summary>
        public static void Error(string message, Exception? ex = null)
        {
            if (ex != null)
                _log.Error(message, ex);
            else
                _log.Error(message);
        }
    }
}
