using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.services;

namespace LMSA.Tests;

public class DeviceEnumsTests
{
    [Fact]
    public void DevicePhysicalStateEx_HasCorrectValues()
    {
        Assert.Equal(-1, (int)DevicePhysicalStateEx.None);
        Assert.Equal(0, (int)DevicePhysicalStateEx.Offline);
        Assert.Equal(2, (int)DevicePhysicalStateEx.Online);
        Assert.Equal(7, (int)DevicePhysicalStateEx.Unauthorized);
        Assert.Equal(9, (int)DevicePhysicalStateEx.UsbDebugSwitchClosed);
    }

    [Fact]
    public void DeviceSoftStateEx_HasCorrectValues()
    {
        Assert.Equal(-1, (int)DeviceSoftStateEx.None);
        Assert.Equal(0, (int)DeviceSoftStateEx.Offline);
        Assert.Equal(1, (int)DeviceSoftStateEx.Connecting);
        Assert.Equal(2, (int)DeviceSoftStateEx.Online);
        Assert.Equal(3, (int)DeviceSoftStateEx.Reconncting);
        Assert.Equal(4, (int)DeviceSoftStateEx.ManualDisconnect);
    }

    [Fact]
    public void DeviceType_HasCorrectValues()
    {
        Assert.Equal(0, (int)DeviceType.Master);
        Assert.Equal(1, (int)DeviceType.Slave);
        Assert.Equal(2, (int)DeviceType.Normal);
    }

    [Fact]
    public void ConnectType_HasCorrectFlagValues()
    {
        Assert.Equal(1, (int)ConnectType.Adb);
        Assert.Equal(2, (int)ConnectType.Fastboot);
        Assert.Equal(4, (int)ConnectType.Wifi);
        Assert.Equal(5, (int)ConnectType.UNKNOW);
    }

    [Fact]
    public void DeviceWorkType_HasCorrectFlagValues()
    {
        Assert.Equal(1u, (uint)DeviceWorkType.None);
        Assert.Equal(2u, (uint)DeviceWorkType.Rescue);
        Assert.Equal(4u, (uint)DeviceWorkType.ReadFastboot);
    }

    [Fact]
    public void ConnectedAppTypesDefine_HasCorrectConstants()
    {
        Assert.Equal("Ma", ConnectedAppTypesDefine.Ma);
        Assert.Equal("Moto", ConnectedAppTypesDefine.Moto);
    }
}
