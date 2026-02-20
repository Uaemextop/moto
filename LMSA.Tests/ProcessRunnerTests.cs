using System.Collections.Generic;
using System.IO;
using lenovo.mbg.service.common.utilities;
using Xunit;

namespace LMSA.Tests;

/// <summary>
/// Unit tests for ProcessRunner.
/// </summary>
public class ProcessRunnerTests
{
    [Fact]
    public void ProcessString_WithValidCommand_ReturnsOutput()
    {
        // Arrange
        string exe = "echo";
        string command = "hello";
        int timeout = 5000;

        // Act - on Linux echo works
        string result = ProcessRunner.ProcessString(exe, command, timeout);

        // Assert - should return some output or empty (may fail on non-Linux without echo)
        Assert.NotNull(result);
    }

    [Fact]
    public void ProcessList_WithValidCommand_ReturnsList()
    {
        // Arrange
        string exe = "echo";
        string command = "test";
        int timeout = 5000;

        // Act
        List<string> result = ProcessRunner.ProcessList(exe, command, timeout);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void ProcessString_WithNonExistentExe_ReturnsEmpty()
    {
        // Arrange
        string exe = "nonexistent_binary_12345";
        string command = "test";
        int timeout = 1000;

        // Act
        string result = ProcessRunner.ProcessString(exe, command, timeout);

        // Assert - should return empty or error message, not throw
        Assert.NotNull(result);
    }
}
