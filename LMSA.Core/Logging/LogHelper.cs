using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace lenovo.mbg.service.common.log;

public class LogHelper
{
    public static string ConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");

    private static List<string> m_UnsafeText = new List<string>();

    protected BusinessLog _businessLog;

    private static object locker = new object();

    private static LogHelper? m_Instance;

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

    private static void Init()
    {
        m_Instance = new LogHelper();
    }

    public void Debug(string message, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        WriteToConsole("DEBUG", currentMethod, message);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.DEBUG);
        }
    }

    public void Debug(string message, Exception exception, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        WriteToConsole("DEBUG", currentMethod, message, exception);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.DEBUG, exception);
        }
    }

    public void Info(string message, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        WriteToConsole("INFO", currentMethod, message);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.INFO);
        }
    }

    public void Info(string message, Exception exception, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        WriteToConsole("INFO", currentMethod, message, exception);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.INFO, exception);
        }
    }

    public void Warn(string message, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        WriteToConsole("WARN", currentMethod, message);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.WARN);
        }
    }

    public void Warn(string message, Exception exception, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        WriteToConsole("WARN", currentMethod, message, exception);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.WARN, exception);
        }
    }

    public void Error(string message, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        WriteToConsole("ERROR", currentMethod, message);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.ERROE);
        }
    }

    public void Error(string message, Exception exception, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        WriteToConsole("ERROR", currentMethod, message, exception);
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

    private static string MessageDesensitization(string _msg)
    {
        m_UnsafeText.ForEach(delegate (string m)
        {
            _msg = _msg.Replace(m, "***");
        });
        return _msg;
    }

    public void WriteLogForUser(string message, int resultCode)
    {
        try
        {
            message = MessageDesensitization(message);
            message = Regex.Replace(Regex.Replace(message, "(-User\\s*)\\\\\"[^\\\\\"]+\\\\\"", "$1\"********\"", RegexOptions.IgnoreCase | RegexOptions.Multiline), "(-Password\\s*)\\\\\"[^\\\\\"]+\\\\\"", "$1\"********\"", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string text = "not start";
            switch (resultCode)
            {
                case 0:
                    text = "fail";
                    break;
                case 1:
                    text = "pass";
                    break;
                case 2:
                    text = "quit";
                    break;
            }
            string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
            string contents = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - [{text}] - {message}{Environment.NewLine}";
            File.AppendAllText(Path.Combine(logDir, $"{DateTime.Now:yyyy-MM}-friendly.log"), contents);
        }
        catch (Exception arg)
        {
            Error($"WriteLogForUser - message:[{message}] exception:[{arg}]");
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
        var stackTrace = new StackTrace(2, fNeedFileInfo: true);
        var frame = stackTrace.GetFrame(0);
        return frame?.GetMethod()?.Name ?? "Unknown";
    }

    private static void WriteToConsole(string level, string method, string message, Exception? exception = null)
    {
        Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {method} - {message}");
        if (exception != null)
        {
            Console.WriteLine(exception.ToString());
        }
    }

    private LogHelper()
    {
        _businessLog = new BusinessLog();
    }
}
