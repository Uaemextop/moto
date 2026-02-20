using System;
using System.Text;
using System.Threading;

namespace lenovo.mbg.service.common.log;

public class BusinessLog
{
    private readonly object locker = new object();

    public StringBuilder LogCache { get; private set; }

    public BusinessLog()
    {
        LogCache = new StringBuilder();
    }

    public void Write(string method, string message, LogLevel level, Exception? exception)
    {
        string formattedMessage = $"{DateTime.Now:G} [{Thread.CurrentThread.ManagedThreadId.ToString().PadLeft(2)}] [{level.ToString().PadLeft(5)}] {method} - {message}";
        Write(formattedMessage, exception);
    }

    public void Write(string message, Exception? exception)
    {
        lock (locker)
        {
            LogCache.AppendLine(message);
            if (exception != null)
            {
                LogCache.AppendLine(exception.ToString());
            }
        }
    }

    public void Clear()
    {
        lock (locker)
        {
            LogCache.Clear();
        }
    }

    public override string ToString()
    {
        lock (locker)
        {
            return LogCache.ToString();
        }
    }
}
