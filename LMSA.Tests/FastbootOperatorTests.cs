using lenovo.mbg.service.framework.devicemgt.DeviceOperator;

namespace LMSA.Tests;

public class FastbootOperatorTests
{
    [Fact]
    public void FastbootExe_IsSet()
    {
        Assert.NotNull(FastbootOperator.fastbootExe);
        Assert.Contains("fastboot", FastbootOperator.fastbootExe);
    }

    [Fact]
    public void FastbootMonitorExe_IsSet()
    {
        Assert.NotNull(FastbootOperator.fastbootMonitorExe);
        Assert.Contains("fastbootmonitor", FastbootOperator.fastbootMonitorExe);
    }

    [Fact]
    public void ForwardPort_ThrowsNotImplemented()
    {
        var op = new FastbootOperator();
        Assert.Throws<NotImplementedException>(() => op.ForwardPort("device1", 5555, 5555));
    }

    [Fact]
    public void Install_ThrowsNotImplemented()
    {
        var op = new FastbootOperator();
        Assert.Throws<NotImplementedException>(() => op.Install("device1", "/path/to/apk"));
    }

    [Fact]
    public void PushFile_ThrowsNotImplemented()
    {
        var op = new FastbootOperator();
        Assert.Throws<NotImplementedException>(() => op.PushFile("device1", "/local", "/remote"));
    }

    [Fact]
    public void Shell_ThrowsNotImplemented()
    {
        var op = new FastbootOperator();
        Assert.Throws<NotImplementedException>(() => op.Shell("device1", "ls"));
    }

    [Fact]
    public void Uninstall_ThrowsNotImplemented()
    {
        var op = new FastbootOperator();
        Assert.Throws<NotImplementedException>(() => op.Uninstall("device1", "com.test"));
    }

    [Fact]
    public void RemoveAllForward_ThrowsNotImplemented()
    {
        var op = new FastbootOperator();
        Assert.Throws<NotImplementedException>(() => op.RemoveAllForward("device1"));
    }
}
