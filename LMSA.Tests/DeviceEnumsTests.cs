using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.services.Download;
using lenovo.mbg.service.framework.services.Model;

namespace LMSA.Tests;

public class DeviceEnumsTests
{
    [Fact]
    public void DevicePhysicalStateEx_HasExpectedValues()
    {
        Assert.Equal(-1, (int)DevicePhysicalStateEx.None);
        Assert.Equal(0, (int)DevicePhysicalStateEx.Offline);
        Assert.Equal(2, (int)DevicePhysicalStateEx.Online);
        Assert.Equal(7, (int)DevicePhysicalStateEx.Unauthorized);
        Assert.Equal(9, (int)DevicePhysicalStateEx.UsbDebugSwitchClosed);
    }

    [Fact]
    public void DeviceSoftStateEx_HasExpectedValues()
    {
        Assert.Equal(-1, (int)DeviceSoftStateEx.None);
        Assert.Equal(0, (int)DeviceSoftStateEx.Offline);
        Assert.Equal(1, (int)DeviceSoftStateEx.Connecting);
        Assert.Equal(2, (int)DeviceSoftStateEx.Online);
    }

    [Fact]
    public void ConnectType_HasFlagsAttribute()
    {
        Assert.Equal(1, (int)ConnectType.Adb);
        Assert.Equal(2, (int)ConnectType.Fastboot);
        Assert.Equal(4, (int)ConnectType.Wifi);
    }

    [Fact]
    public void DeviceWorkType_HasExpectedFlags()
    {
        Assert.Equal(1u, (uint)DeviceWorkType.None);
        Assert.Equal(2u, (uint)DeviceWorkType.Rescue);
        Assert.Equal(4u, (uint)DeviceWorkType.ReadFastboot);
    }

    [Fact]
    public void DownloadStatus_HasAllExpectedValues()
    {
        Assert.Equal(0, (int)DownloadStatus.WAITTING);
        Assert.Equal(1, (int)DownloadStatus.DOWNLOADING);
        Assert.Equal(5, (int)DownloadStatus.SUCCESS);
    }

    [Fact]
    public void BusinessStatus_HasExpectedValues()
    {
        Assert.Equal(0, (int)BusinessStatus.CLICK);
        Assert.Equal(10, (int)BusinessStatus.SUCCESS);
        Assert.Equal(20, (int)BusinessStatus.FALIED);
        Assert.Equal(30, (int)BusinessStatus.QUIT);
    }

    [Fact]
    public void BusinessType_HasExpectedValues()
    {
        Assert.Equal(1000, (int)BusinessType.HOME);
        Assert.Equal(1100, (int)BusinessType.APP);
        Assert.Equal(3010, (int)BusinessType.RESCUE_IMEI_SEARCH);
        Assert.Equal(9900, (int)BusinessType.SURVEY_QUIT);
    }
}
