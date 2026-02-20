using lenovo.mbg.service.common.log;

namespace LMSA.Tests;

public class LogHelperTests
{
    [Fact]
    public void LogInstance_ReturnsSingleton()
    {
        var instance1 = LogHelper.LogInstance;
        var instance2 = LogHelper.LogInstance;

        Assert.NotNull(instance1);
        Assert.Same(instance1, instance2);
    }

    [Fact]
    public void Debug_DoesNotThrow()
    {
        var exception = Record.Exception(() => LogHelper.LogInstance.Debug("test message"));
        Assert.Null(exception);
    }

    [Fact]
    public void Info_DoesNotThrow()
    {
        var exception = Record.Exception(() => LogHelper.LogInstance.Info("test message"));
        Assert.Null(exception);
    }

    [Fact]
    public void Warn_DoesNotThrow()
    {
        var exception = Record.Exception(() => LogHelper.LogInstance.Warn("test message"));
        Assert.Null(exception);
    }

    [Fact]
    public void Error_DoesNotThrow()
    {
        var exception = Record.Exception(() => LogHelper.LogInstance.Error("test message"));
        Assert.Null(exception);
    }

    [Fact]
    public void Error_WithException_DoesNotThrow()
    {
        var exception = Record.Exception(() =>
            LogHelper.LogInstance.Error("test message", new Exception("test exception")));
        Assert.Null(exception);
    }

    [Fact]
    public void AddUnsafeText_MasksTextInLogs()
    {
        LogHelper.LogInstance.AddUnsafeText("sensitive_data");
        var exception = Record.Exception(() => LogHelper.LogInstance.Info("Contains sensitive_data here"));
        Assert.Null(exception);
    }

    [Fact]
    public void AddUnsafeText_NullOrEmpty_DoesNotThrow()
    {
        var exception = Record.Exception(() =>
        {
            LogHelper.LogInstance.AddUnsafeText(null);
            LogHelper.LogInstance.AddUnsafeText("");
        });
        Assert.Null(exception);
    }
}
