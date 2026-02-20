using lenovo.mbg.service.framework.services.Device;
using Xunit;

namespace LMSA.Tests;

/// <summary>
/// Unit tests for device enums.
/// </summary>
public class DeviceEnumTests
{
    [Fact]
    public void ConnectType_HasExpectedValues()
    {
        Assert.Equal(1, (int)ConnectType.Adb);
        Assert.Equal(2, (int)ConnectType.Fastboot);
        Assert.Equal(4, (int)ConnectType.Wifi);
        Assert.Equal(5, (int)ConnectType.UNKNOW);
    }

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
    public void DeviceWorkType_HasExpectedValues()
    {
        Assert.Equal(1u, (uint)DeviceWorkType.None);
        Assert.Equal(2u, (uint)DeviceWorkType.Rescue);
        Assert.Equal(4u, (uint)DeviceWorkType.ReadFastboot);
    }

    [Fact]
    public void DeviceType_HasExpectedValues()
    {
        Assert.Equal(0, (int)DeviceType.Master);
        Assert.Equal(1, (int)DeviceType.Slave);
        Assert.Equal(2, (int)DeviceType.Normal);
    }

    [Fact]
    public void ConnectType_IsFlagsEnum()
    {
        // Verify Flags attribute - can combine values
        ConnectType combined = ConnectType.Adb | ConnectType.Fastboot;
        Assert.Equal(3, (int)combined);
    }

    [Fact]
    public void DeviceWorkType_IsFlagsEnum()
    {
        // Verify Flags attribute
        DeviceWorkType combined = DeviceWorkType.Rescue | DeviceWorkType.ReadFastboot;
        Assert.Equal(6u, (uint)combined);
    }
}
