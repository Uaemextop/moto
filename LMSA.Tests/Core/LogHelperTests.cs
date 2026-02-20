using lenovo.mbg.service.common.log;

namespace LMSA.Tests.Core;

public class LogHelperTests
{
    [Fact]
    public void LogInstance_ReturnsSingletonInstance()
    {
        var instance1 = LogHelper.LogInstance;
        var instance2 = LogHelper.LogInstance;

        Assert.NotNull(instance1);
        Assert.Same(instance1, instance2);
    }

    [Fact]
    public void Debug_DoesNotThrow()
    {
        var exception = Record.Exception(() => LogHelper.LogInstance.Debug("Test debug message"));
        Assert.Null(exception);
    }

    [Fact]
    public void Info_DoesNotThrow()
    {
        var exception = Record.Exception(() => LogHelper.LogInstance.Info("Test info message"));
        Assert.Null(exception);
    }

    [Fact]
    public void Warn_DoesNotThrow()
    {
        var exception = Record.Exception(() => LogHelper.LogInstance.Warn("Test warn message"));
        Assert.Null(exception);
    }

    [Fact]
    public void Error_DoesNotThrow()
    {
        var exception = Record.Exception(() => LogHelper.LogInstance.Error("Test error message"));
        Assert.Null(exception);
    }

    [Fact]
    public void Error_WithException_DoesNotThrow()
    {
        var ex = new InvalidOperationException("test");
        var exception = Record.Exception(() => LogHelper.LogInstance.Error("Test error", ex));
        Assert.Null(exception);
    }

    [Fact]
    public void AddUnsafeText_WithNullOrEmpty_DoesNotThrow()
    {
        var exception = Record.Exception(() =>
        {
            LogHelper.LogInstance.AddUnsafeText(null!);
            LogHelper.LogInstance.AddUnsafeText("");
        });
        Assert.Null(exception);
    }
}
