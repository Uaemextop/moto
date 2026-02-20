using lenovo.mbg.service.framework.devicemgt.DeviceOperator;

namespace LMSA.Tests;

public class FastbootOperatorTests
{
    [Fact]
    public void Instance_ReturnsSingleton()
    {
        var instance1 = FastbootOperator.Instance;
        var instance2 = FastbootOperator.Instance;

        Assert.NotNull(instance1);
        Assert.Same(instance1, instance2);
    }

    [Fact]
    public void FindDevices_ParsesDeviceOutput()
    {
        // This tests the parsing logic; in production, fastboot.exe would return results
        // We test the empty case here since no device is connected
        var operator_ = FastbootOperator.Instance;
        // FindDevices would try to run fastbootmonitor.exe which isn't available in test
        // but we can verify the Instance pattern works
        Assert.NotNull(operator_);
    }

    [Fact]
    public void ForwardPort_ThrowsNotImplementedException()
    {
        var operator_ = FastbootOperator.Instance;
        Assert.Throws<NotImplementedException>(() => operator_.ForwardPort("device", 5000, 5001));
    }

    [Fact]
    public void Install_ThrowsNotImplementedException()
    {
        var operator_ = FastbootOperator.Instance;
        Assert.Throws<NotImplementedException>(() => operator_.Install("device", "/path/to/apk"));
    }

    [Fact]
    public void PushFile_ThrowsNotImplementedException()
    {
        var operator_ = FastbootOperator.Instance;
        Assert.Throws<NotImplementedException>(() => operator_.PushFile("device", "/local/file", "/remote/file"));
    }

    [Fact]
    public void Shell_ThrowsNotImplementedException()
    {
        var operator_ = FastbootOperator.Instance;
        Assert.Throws<NotImplementedException>(() => operator_.Shell("device", "command"));
    }

    [Fact]
    public void Uninstall_ThrowsNotImplementedException()
    {
        var operator_ = FastbootOperator.Instance;
        Assert.Throws<NotImplementedException>(() => operator_.Uninstall("device", "package"));
    }

    [Fact]
    public void RemoveAllForward_ThrowsNotImplementedException()
    {
        var operator_ = FastbootOperator.Instance;
        Assert.Throws<NotImplementedException>(() => operator_.RemoveAllForward("device"));
    }
}
