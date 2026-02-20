using System;
using FluentAssertions;
using lenovo.mbg.service.common.log;
using Xunit;

namespace LMSA.Tests.Logging;

public class BusinessLogTests
{
    [Fact]
    public void Write_WithMessageAndNoException_AppendsToCache()
    {
        var log = new BusinessLog();
        log.Write("TestMethod", "Test message", LogLevel.INFO, null);
        log.ToString().Should().Contain("Test message");
    }

    [Fact]
    public void Write_WithException_AppendsBothMessageAndException()
    {
        var log = new BusinessLog();
        var ex = new InvalidOperationException("Test exception");
        log.Write("TestMethod", "Test message", LogLevel.ERROE, ex);
        string result = log.ToString();
        result.Should().Contain("Test message");
        result.Should().Contain("Test exception");
    }

    [Fact]
    public void Clear_RemovesAllLoggedMessages()
    {
        var log = new BusinessLog();
        log.Write("TestMethod", "Test message", LogLevel.INFO, null);
        log.Clear();
        log.ToString().Should().BeEmpty();
    }

    [Fact]
    public void Write_IncludesLogLevel_InOutput()
    {
        var log = new BusinessLog();
        log.Write("TestMethod", "Debug message", LogLevel.DEBUG, null);
        log.ToString().Should().Contain("DEBUG");
    }

    [Fact]
    public void Write_IncludesMethodName_InOutput()
    {
        var log = new BusinessLog();
        log.Write("MyMethod", "Some message", LogLevel.WARN, null);
        log.ToString().Should().Contain("MyMethod");
    }
}
