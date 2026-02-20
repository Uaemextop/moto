using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using log4net;
using log4net.Config;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.common.log;

public class LogHelper
{
    public static string ConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");

    private static List<string> m_UnsafeText = new List<string>();

    protected BusinessLog _businessLog;

    private static readonly object locker = new object();

    private static LogHelper? m_Instance;

    private ILog Log => LogManager.GetLogger("");

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
                        Init();
                    }
                }
            }
            return m_Instance!;
        }
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
        m_Instance = new LogHelper();
    }

    private LogHelper()
    {
        _businessLog = new BusinessLog();
    }

    public void Debug(string message, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        Log.Debug(currentMethod + " - " + message);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.DEBUG);
        }
    }

    public void Debug(string message, Exception exception, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        Log.Debug(currentMethod + " - " + message, exception);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.DEBUG, exception);
        }
    }

    public void Info(string message, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        Log.Info(currentMethod + " - " + message);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.INFO);
        }
    }

    public void Info(string message, Exception exception, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        Log.Info(currentMethod + " - " + message, exception);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.INFO, exception);
        }
    }

    public void Warn(string message, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        Log.Warn(currentMethod + " - " + message);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.WARN);
        }
    }

    public void Warn(string message, Exception exception, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        Log.Warn(currentMethod + " - " + message, exception);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.WARN, exception);
        }
    }

    public void Error(string message, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        Log.Error(currentMethod + " - " + message);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.ERROE);
        }
    }

    public void Error(string message, Exception exception, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        Log.Error(currentMethod + " - " + message, exception);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.ERROE, exception);
        }
    }

    public void AddUnsafeText(string _text)
    {
        if (string.IsNullOrEmpty(_text))
        {
            return;
        }
        try
        {
            if (!m_UnsafeText.Contains(_text))
            {
                m_UnsafeText.Add(_text);
            }
        }
        catch
        {
        }
    }

    public void AnalyzeUnsafeText(string _msg)
    {
        if (string.IsNullOrEmpty(_msg))
        {
            return;
        }
        try
        {
            List<string> fields = new List<string> { "content.name", "content.fullName" };
            JObject jObject = JObject.Parse(_msg);
            foreach (string field in fields)
            {
                string? text = jObject.SelectToken(field)?.ToString();
                if (!string.IsNullOrEmpty(text))
                {
                    m_UnsafeText.Add(text);
                }
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
                Regex.Replace(message,
                    "(-User\\s*)\\\\\"[^\\\\\"]+\\\\\"",
                    "$1\"********\"",
                    RegexOptions.IgnoreCase | RegexOptions.Multiline),
                "(-Password\\s*)\\\\\"[^\\\\\"]+\\\\\"",
                "$1\"********\"",
                RegexOptions.IgnoreCase | RegexOptions.Multiline);

            string status = resultCode switch
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
            string contents = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - [{status}] - {message}{Environment.NewLine}";
            File.AppendAllText(Path.Combine(logDir, $"{DateTime.Now:yyyy-MM}-friendly.log"), contents);
        }
        catch (Exception ex)
        {
            Error($"WriteLogForUser - message:[{message}] exception:[{ex}]");
        }
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
        return new StackTrace(2, fNeedFileInfo: true).GetFrame(0)?.GetMethod()?.Name ?? "Unknown";
    }

    private static string MessageDesensitization(string _msg)
    {
        m_UnsafeText.ForEach(m =>
        {
            _msg = _msg.Replace(m, "***");
        });
        return _msg;
    }
}
