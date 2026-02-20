using System;
using log4net;

namespace lenovo.mbg.service.framework.common
{
    /// <summary>
    /// Provides structured logging methods for the LMSA framework.
    /// </summary>
    public static class Log
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Log));

        /// <summary>
        /// Adds a general log message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="upload">Whether this log entry should be marked for upload.</param>
        public static void AddLog(string message, bool upload = false)
        {
            string prefix = upload ? "[UPLOAD] " : "";
            _log.Info($"{prefix}{message}");
        }

        /// <summary>
        /// Adds an informational log message.
        /// </summary>
        public static void AddInfo(string message)
        {
            _log.Info(message);
        }

        /// <summary>
        /// Logs the result of an operation.
        /// </summary>
        /// <param name="source">The object that produced the result.</param>
        /// <param name="result">The result of the operation.</param>
        /// <param name="errorMessage">Optional error message for failures.</param>
        public static void AddResult(object source, Result result, string? errorMessage)
        {
            string sourceName = source?.GetType().Name ?? "Unknown";

            if (result == Result.PASSED)
            {
                _log.Info($"[{sourceName}] Result: {result}");
            }
            else
            {
                string msg = string.IsNullOrEmpty(errorMessage) ? "" : $" - {errorMessage}";
                _log.Error($"[{sourceName}] Result: {result}{msg}");
            }
        }

        /// <summary>
        /// Logs an error with an optional exception.
        /// </summary>
        public static void AddError(string message, Exception? exception = null)
        {
            if (exception != null)
                _log.Error(message, exception);
            else
                _log.Error(message);
        }

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        public static void AddDebug(string message)
        {
            _log.Debug(message);
        }
    }
}
