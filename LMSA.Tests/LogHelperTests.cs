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
        var logger = LogHelper.LogInstance;
        var ex = Record.Exception(() => logger.Debug("test debug message"));
        Assert.Null(ex);
    }

    [Fact]
    public void Info_DoesNotThrow()
    {
        var logger = LogHelper.LogInstance;
        var ex = Record.Exception(() => logger.Info("test info message"));
        Assert.Null(ex);
    }

    [Fact]
    public void Warn_DoesNotThrow()
    {
        var logger = LogHelper.LogInstance;
        var ex = Record.Exception(() => logger.Warn("test warning message"));
        Assert.Null(ex);
    }

    [Fact]
    public void Error_DoesNotThrow()
    {
        var logger = LogHelper.LogInstance;
        var ex = Record.Exception(() => logger.Error("test error message"));
        Assert.Null(ex);
    }

    [Fact]
    public void Error_WithException_DoesNotThrow()
    {
        var logger = LogHelper.LogInstance;
        var exception = new InvalidOperationException("test exception");
        var ex = Record.Exception(() => logger.Error("test error", exception));
        Assert.Null(ex);
    }

    [Fact]
    public void AddUnsafeText_DoesNotThrow()
    {
        var logger = LogHelper.LogInstance;
        var ex = Record.Exception(() => logger.AddUnsafeText("sensitive_data"));
        Assert.Null(ex);
    }

    [Fact]
    public void AddUnsafeText_NullOrEmpty_DoesNotThrow()
    {
        var logger = LogHelper.LogInstance;
        var ex1 = Record.Exception(() => logger.AddUnsafeText(null!));
        var ex2 = Record.Exception(() => logger.AddUnsafeText(string.Empty));
        Assert.Null(ex1);
        Assert.Null(ex2);
    }
}
