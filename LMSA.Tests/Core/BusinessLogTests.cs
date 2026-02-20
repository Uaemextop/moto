using lenovo.mbg.service.common.log;

namespace LMSA.Tests.Core;

public class BusinessLogTests
{
    [Fact]
    public void Write_AppendsMessageToCache()
    {
        var log = new BusinessLog();

        log.Write("TestMethod", "Test message", LogLevel.INFO, null);

        string output = log.ToString();
        Assert.Contains("Test message", output);
        Assert.Contains("TestMethod", output);
        Assert.Contains("INFO", output);
    }

    [Fact]
    public void Write_WithException_AppendsExceptionToCache()
    {
        var log = new BusinessLog();
        var ex = new InvalidOperationException("test exception");

        log.Write("TestMethod", "Test message", LogLevel.ERROE, ex);

        string output = log.ToString();
        Assert.Contains("test exception", output);
    }

    [Fact]
    public void Clear_RemovesAllCachedContent()
    {
        var log = new BusinessLog();
        log.Write("Method", "message", LogLevel.DEBUG, null);

        log.Clear();

        Assert.Equal(string.Empty, log.ToString());
    }

    [Fact]
    public void Constructor_InitializesEmptyCache()
    {
        var log = new BusinessLog();
        Assert.NotNull(log.LogCache);
        Assert.Equal(string.Empty, log.ToString());
    }

    [Fact]
    public void Write_MessageOnly_AppendsToCache()
    {
        var log = new BusinessLog();

        log.Write("Simple message", null);

        Assert.Contains("Simple message", log.ToString());
    }
}
