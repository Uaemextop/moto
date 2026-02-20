namespace lenovo.mbg.service.common.log;

public class LogHelper
{
    private static LogHelper m_Instance;
    private static readonly object locker = new object();

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

    public void Info(string message)
    {
        // Stub implementation
    }

    public void Debug(string message)
    {
        // Stub implementation
    }

    public void Error(string message)
    {
        // Stub implementation
    }

    public void Warn(string message)
    {
        // Stub implementation
    }
}
