using lenovo.mbg.service.common.log;

namespace LMSA.Tests;

public class BusinessLogTests
{
    [Fact]
    public void Write_AppendsFormattedMessage()
    {
        var log = new BusinessLog();
        log.Write("TestMethod", "Test message", LogLevel.INFO, null);

        string output = log.ToString();
        Assert.Contains("TestMethod", output);
        Assert.Contains("Test message", output);
        Assert.Contains("INFO", output);
    }

    [Fact]
    public void Write_WithException_AppendsExceptionDetails()
    {
        var log = new BusinessLog();
        var ex = new InvalidOperationException("test error");
        log.Write("TestMethod", "Error occurred", LogLevel.ERROE, ex);

        string output = log.ToString();
        Assert.Contains("test error", output);
        Assert.Contains("Error occurred", output);
    }

    [Fact]
    public void Clear_RemovesAllContent()
    {
        var log = new BusinessLog();
        log.Write("method", "message", LogLevel.DEBUG, null);
        Assert.NotEmpty(log.ToString());

        log.Clear();
        Assert.Empty(log.ToString());
    }

    [Fact]
    public void LogCache_InitializedEmpty()
    {
        var log = new BusinessLog();
        Assert.NotNull(log.LogCache);
        Assert.Empty(log.ToString());
    }

    [Fact]
    public void Write_MultipleMessages_AccumulatesAll()
    {
        var log = new BusinessLog();
        log.Write("Method1", "First", LogLevel.INFO, null);
        log.Write("Method2", "Second", LogLevel.DEBUG, null);

        string output = log.ToString();
        Assert.Contains("First", output);
        Assert.Contains("Second", output);
    }
}
