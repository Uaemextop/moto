using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.devicemgt.DeviceOperator;
using lenovo.mbg.service.framework.services.Device;

namespace LMSA.Tests.DeviceManagement;

public class AdbOperatorTests
{
    [Fact]
    public void Instance_ReturnsSingletonInstance()
    {
        var instance1 = AdbOperator.Instance;
        var instance2 = AdbOperator.Instance;

        Assert.NotNull(instance1);
        Assert.Same(instance1, instance2);
    }

    [Fact]
    public void AdbOperator_ImplementsIDeviceOperator()
    {
        var op = new AdbOperator();
        Assert.IsAssignableFrom<IDeviceOperator>(op);
    }

    [Fact]
    public void FindDevices_ReturnsEmptyList_WhenAdbNotAvailable()
    {
        var op = new AdbOperator();
        var devices = op.FindDevices();
        Assert.NotNull(devices);
    }
}

public class FastbootOperatorTests
{
    [Fact]
    public void FastbootOperator_ImplementsIDeviceOperator()
    {
        var op = new FastbootOperator();
        Assert.IsAssignableFrom<IDeviceOperator>(op);
    }

    [Fact]
    public void ForwardPort_ThrowsNotImplementedException()
    {
        var op = new FastbootOperator();
        Assert.Throws<NotImplementedException>(() => op.ForwardPort("device", 1234, 5678));
    }

    [Fact]
    public void Install_ThrowsNotImplementedException()
    {
        var op = new FastbootOperator();
        Assert.Throws<NotImplementedException>(() => op.Install("device", "/path/to/apk"));
    }

    [Fact]
    public void Shell_ThrowsNotImplementedException()
    {
        var op = new FastbootOperator();
        Assert.Throws<NotImplementedException>(() => op.Shell("device", "command"));
    }

    [Fact]
    public void PushFile_ThrowsNotImplementedException()
    {
        var op = new FastbootOperator();
        Assert.Throws<NotImplementedException>(() => op.PushFile("device", "/local", "/remote"));
    }

    [Fact]
    public void Uninstall_ThrowsNotImplementedException()
    {
        var op = new FastbootOperator();
        Assert.Throws<NotImplementedException>(() => op.Uninstall("device", "com.package"));
    }

    [Fact]
    public void RemoveForward_ThrowsNotImplementedException()
    {
        var op = new FastbootOperator();
        Assert.Throws<NotImplementedException>(() => op.RemoveForward("device", 1234));
    }

    [Fact]
    public void RemoveAllForward_ThrowsNotImplementedException()
    {
        var op = new FastbootOperator();
        Assert.Throws<NotImplementedException>(() => op.RemoveAllForward("device"));
    }

    [Fact]
    public void FastbootExe_Path_IsNotEmpty()
    {
        Assert.False(string.IsNullOrEmpty(FastbootOperator.fastbootExe));
    }
}
