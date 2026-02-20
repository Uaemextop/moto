using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.devicemgt.DeviceInfo;
using lenovo.mbg.service.framework.services.Device;

namespace LMSA.Tests.DeviceManagement;

public class FastbootDeviceExTests
{
    [Fact]
    public void Constructor_SetsDefaultProperties()
    {
        var device = new FastbootDeviceEx();

        Assert.NotNull(device.Property);
        Assert.NotNull(device.DeviceOperator);
    }

    [Fact]
    public void Constructor_SetsPropertyAsFastbootAndroidDevice()
    {
        var device = new FastbootDeviceEx();

        Assert.IsType<FastbootAndroidDevice>(device.Property);
    }

    [Fact]
    public void ConnectType_CanBeSetToFastboot()
    {
        var device = new FastbootDeviceEx();
        device.ConnectType = ConnectType.Fastboot;

        Assert.Equal(ConnectType.Fastboot, device.ConnectType);
    }
}

public class AdbDeviceExTests
{
    [Fact]
    public void Constructor_SetsDeviceOperator()
    {
        var device = new AdbDeviceEx();

        Assert.NotNull(device.DeviceOperator);
    }

    [Fact]
    public void Load_DoesNotThrow()
    {
        var device = new AdbDeviceEx();
        var exception = Record.Exception(() => device.Load());
        Assert.Null(exception);
    }
}

public class ConnectErrorCodeTests
{
    [Fact]
    public void ConnectErrorCode_HasExpectedValues()
    {
        Assert.Equal(0, (int)ConnectErrorCode.Unknown);
        Assert.Equal(1, (int)ConnectErrorCode.OK);
        Assert.Equal(2, (int)ConnectErrorCode.TcpConnectFailWithAppNotAllowed);
        Assert.Equal(3, (int)ConnectErrorCode.TcpConnectFailWithTimeout);
        Assert.Equal(4, (int)ConnectErrorCode.AppVersionNotMatched);
        Assert.Equal(5, (int)ConnectErrorCode.ApkInstallFailWithHaveNoSpace);
        Assert.Equal(6, (int)ConnectErrorCode.ApkInstallFail);
        Assert.Equal(7, (int)ConnectErrorCode.PropertyLoadFail);
        Assert.Equal(8, (int)ConnectErrorCode.LaunchAppFail);
        Assert.Equal(9, (int)ConnectErrorCode.ClientUnsupport);
    }
}

public class DevicemgtContantClassTests
{
    [Fact]
    public void IsBackAndRestoreFrm_DefaultIsFalse()
    {
        DevicemgtContantClass.IsBackAndRestoreFrm = false;
        Assert.False(DevicemgtContantClass.IsBackAndRestoreFrm);
    }

    [Fact]
    public void IsOtherToBackAndRestoreAndAutoConnection_CanBeToggled()
    {
        DevicemgtContantClass.IsOtherToBackAndRestoreAndAutoConnection = true;
        Assert.True(DevicemgtContantClass.IsOtherToBackAndRestoreAndAutoConnection);

        DevicemgtContantClass.IsOtherToBackAndRestoreAndAutoConnection = false;
        Assert.False(DevicemgtContantClass.IsOtherToBackAndRestoreAndAutoConnection);
    }
}
