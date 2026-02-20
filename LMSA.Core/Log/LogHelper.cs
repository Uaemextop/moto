using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace lenovo.mbg.service.common.log;

/// <summary>
/// Centralized logging helper with message desensitization and structured log output.
/// Singleton pattern providing Debug/Info/Warn/Error methods with optional upload flag.
/// </summary>
public class LogHelper
{
    private static List<string> m_UnsafeText = new List<string>();
    protected BusinessLog _businessLog;
    private static readonly object locker = new object();
    private static LogHelper? m_Instance;
    private readonly ILogger _logger;

    public static LogHelper LogInstance
    {
        get
        {
            if (m_Instance == null)
            {
                lock (locker)
                {
                    if (m_Instance == null)
                    {
                        m_Instance = new LogHelper();
                    }
                }
            }
            return m_Instance;
        }
    }

    public void Debug(string message, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        _logger.LogDebug("{Method} - {Message}", currentMethod, message);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.DEBUG);
        }
    }

    public void Debug(string message, Exception exception, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        _logger.LogDebug(exception, "{Method} - {Message}", currentMethod, message);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.DEBUG, exception);
        }
    }

    public void Info(string message, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        _logger.LogInformation("{Method} - {Message}", currentMethod, message);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.INFO);
        }
    }

    public void Info(string message, Exception exception, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        _logger.LogInformation(exception, "{Method} - {Message}", currentMethod, message);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.INFO, exception);
        }
    }

    public void Warn(string message, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        _logger.LogWarning("{Method} - {Message}", currentMethod, message);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.WARN);
        }
    }

    public void Warn(string message, Exception exception, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        _logger.LogWarning(exception, "{Method} - {Message}", currentMethod, message);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.WARN, exception);
        }
    }

    public void Error(string message, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        _logger.LogError("{Method} - {Message}", currentMethod, message);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.ERROE);
        }
    }

    public void Error(string message, Exception exception, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        _logger.LogError(exception, "{Method} - {Message}", currentMethod, message);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.ERROE, exception);
        }
    }

    public void AddUnsafeText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }
        try
        {
            if (!m_UnsafeText.Contains(text))
            {
                m_UnsafeText.Add(text);
            }
        }
        catch
        {
        }
    }

    public void WriteLogForUser(string message, int resultCode)
    {
        try
        {
            message = MessageDesensitization(message);
            message = Regex.Replace(
                Regex.Replace(message, @"(-User\s*)\\""[^\\""]+\\""", @"$1""********""", RegexOptions.IgnoreCase | RegexOptions.Multiline),
                @"(-Password\s*)\\""[^\\""]+\\""", @"$1""********""", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            string resultText = resultCode switch
            {
                0 => "fail",
                1 => "pass",
                2 => "quit",
                _ => "not start"
            };

            string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }

            string contents = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - [{resultText}] - {message}{Environment.NewLine}";
            File.AppendAllText(Path.Combine(logDir, $"{DateTime.Now:yyyy-MM}-friendly.log"), contents);
        }
        catch (Exception ex)
        {
            Error($"WriteLogForUser - message:[{message}] exception:[{ex}]");
        }
    }

    private static string MessageDesensitization(string msg)
    {
        m_UnsafeText.ForEach(m =>
        {
            msg = msg.Replace(m, "***");
        });
        return msg;
    }

    private void WriteLogAsync(string message, string method, LogLevel level)
    {
        WriteLogAsync(message, method, level, null);
    }

    private void WriteLogAsync(string message, string method, LogLevel level, Exception? exception)
    {
        _businessLog?.Write(method, message, level, exception);
    }

    internal static string GetCurrentMethod()
    {
        try
        {
            return new StackTrace(2, fNeedFileInfo: true).GetFrame(0)?.GetMethod()?.Name ?? "Unknown";
        }
        catch
        {
            return "Unknown";
        }
    }

    private LogHelper()
    {
        _businessLog = new BusinessLog();
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
        });
        _logger = loggerFactory.CreateLogger<LogHelper>();
    }
}
