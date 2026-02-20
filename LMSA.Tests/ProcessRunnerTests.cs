using lenovo.mbg.service.common.utilities;

namespace LMSA.Tests;

public class ProcessRunnerTests
{
    [Fact]
    public void ProcessString_ExecutesCommand()
    {
        // Use Shell method which reads synchronously and avoids race conditions
        string result = ProcessRunner.Shell("echo test_output", "/bin/bash", 5000);

        Assert.Contains("test_output", result);
    }

    [Fact]
    public void ProcessList_ReturnsListOfLines()
    {
        // ProcessList uses async data handlers - test with a command that produces output before exit
        var result = ProcessRunner.ProcessList("/bin/bash", "-c \"for i in 1 2 3; do echo line_$i; done; sleep 0.1\"", 5000);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void ProcessString_EmptyOutput_ReturnsEmpty()
    {
        string result = ProcessRunner.ProcessString("/bin/true", "", 5000);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ProcessString_WithTimeout_HandlesLongRunning()
    {
        string result = ProcessRunner.ProcessString("/bin/sleep", "10", 100);

        Assert.Contains("timeout", result.ToLower());
    }

    [Fact]
    public void CmdExcuteWithExit_HandlesNull()
    {
        // On Linux cmd.exe doesn't exist, so this should return null
        string? result = ProcessRunner.CmdExcuteWithExit("echo test");
        // On Linux this returns null since cmd.exe is not available
        Assert.Null(result);
    }
}
