using lenovo.mbg.service.common.log;

namespace LMSA.Tests.Core;

public class LogLevelTests
{
    [Fact]
    public void LogLevel_HasCorrectValues()
    {
        Assert.Equal(1u, (uint)LogLevel.DEBUG);
        Assert.Equal(2u, (uint)LogLevel.INFO);
        Assert.Equal(3u, (uint)LogLevel.WARN);
        Assert.Equal(4u, (uint)LogLevel.ERROE);
        Assert.Equal(5u, (uint)LogLevel.FATAL);
    }
}
