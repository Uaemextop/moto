using lenovo.mbg.service.common.log;

namespace LMSA.Tests;

public class BusinessLogTests
{
    [Fact]
    public void Write_Method_AppendsToCache()
    {
        var log = new BusinessLog();
        log.Write("TestMethod", "test message", LogLevel.INFO, null);

        string result = log.ToString();
        Assert.Contains("TestMethod", result);
        Assert.Contains("test message", result);
        Assert.Contains("INFO", result);
    }

    [Fact]
    public void Write_WithException_IncludesExceptionInCache()
    {
        var log = new BusinessLog();
        var ex = new Exception("test exception");
        log.Write("TestMethod", "error occurred", LogLevel.ERROE, ex);

        string result = log.ToString();
        Assert.Contains("test exception", result);
        Assert.Contains("error occurred", result);
    }

    [Fact]
    public void Clear_RemovesAllContent()
    {
        var log = new BusinessLog();
        log.Write("TestMethod", "test message", LogLevel.INFO, null);

        Assert.NotEmpty(log.ToString());

        log.Clear();
        Assert.Empty(log.ToString());
    }

    [Fact]
    public void Write_MessageOnly_AppendsToCache()
    {
        var log = new BusinessLog();
        log.Write("simple message", null);

        Assert.Contains("simple message", log.ToString());
    }

    [Fact]
    public void LogLevel_ValuesAreCorrect()
    {
        Assert.Equal(1u, (uint)LogLevel.DEBUG);
        Assert.Equal(2u, (uint)LogLevel.INFO);
        Assert.Equal(3u, (uint)LogLevel.WARN);
        Assert.Equal(4u, (uint)LogLevel.ERROE);
        Assert.Equal(5u, (uint)LogLevel.FATAL);
    }
}
