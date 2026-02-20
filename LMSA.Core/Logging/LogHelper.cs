using System;
using System.Diagnostics;
using System.IO;
using log4net;
using log4net.Config;

namespace lenovo.mbg.service.common.log;

/// <summary>
/// Centralized logging helper wrapping log4net.
/// </summary>
public class LogHelper
{
    public static string ConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");

    private static readonly object _locker = new object();

    private static LogHelper? _instance;

    private readonly ILog _log;

    public static LogHelper LogInstance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }
            lock (_locker)
            {
                if (_instance == null)
                {
                    Init();
                }
            }
            return _instance!;
        }
    }

    private LogHelper()
    {
        _log = LogManager.GetLogger(typeof(LogHelper));
    }

    public static void SetConfig()
    {
        XmlConfigurator.Configure();
    }

    public static void SetConfig(string configPath)
    {
        XmlConfigurator.Configure(new FileInfo(configPath));
    }

    private static void Init()
    {
        if (File.Exists(ConfigFilePath))
        {
            XmlConfigurator.Configure(new FileInfo(ConfigFilePath));
        }
        else
        {
            BasicConfigurator.Configure();
        }
        _instance = new LogHelper();
    }

    public void Debug(string message, bool upload = false)
    {
        string method = GetCurrentMethod();
        _log.Debug(method + " - " + message);
    }

    public void Debug(string message, Exception exception, bool upload = false)
    {
        string method = GetCurrentMethod();
        _log.Debug(method + " - " + message, exception);
    }

    public void Info(string message, bool upload = false)
    {
        string method = GetCurrentMethod();
        _log.Info(method + " - " + message);
    }

    public void Info(string message, Exception exception, bool upload = false)
    {
        string method = GetCurrentMethod();
        _log.Info(method + " - " + message, exception);
    }

    public void Warn(string message, bool upload = false)
    {
        string method = GetCurrentMethod();
        _log.Warn(method + " - " + message);
    }

    public void Warn(string message, Exception exception, bool upload = false)
    {
        string method = GetCurrentMethod();
        _log.Warn(method + " - " + message, exception);
    }

    public void Error(string message, bool upload = false)
    {
        string method = GetCurrentMethod();
        _log.Error(method + " - " + message);
    }

    public void Error(string message, Exception exception, bool upload = false)
    {
        string method = GetCurrentMethod();
        _log.Error(method + " - " + message, exception);
    }

    public void WriteLogForUser(string message, int resultCode)
    {
        try
        {
            string status = resultCode switch
            {
                0 => "fail",
                1 => "pass",
                2 => "quit",
                _ => "not start"
            };
            string contents = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - [{status}] - {message}{Environment.NewLine}";
            string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            Directory.CreateDirectory(logDir);
            File.AppendAllText(Path.Combine(logDir, $"{DateTime.Now:yyyy-MM}-friendly.log"), contents);
        }
        catch (Exception ex)
        {
            Error($"WriteLogForUser - message:[{message}] exception:[{ex}]");
        }
    }

    internal static string GetCurrentMethod()
    {
        return new StackTrace(2, fNeedFileInfo: true).GetFrame(0)?.GetMethod()?.Name ?? "Unknown";
    }
}
