using FluentAssertions;
using lenovo.mbg.service.framework.services.Device;
using Xunit;

namespace LMSA.Tests.DeviceManagement;

public class DeviceEnumTests
{
    [Fact]
    public void DevicePhysicalStateEx_None_HasValueMinusOne()
    {
        ((int)DevicePhysicalStateEx.None).Should().Be(-1);
    }

    [Fact]
    public void DevicePhysicalStateEx_Offline_HasValueZero()
    {
        ((int)DevicePhysicalStateEx.Offline).Should().Be(0);
    }

    [Fact]
    public void DevicePhysicalStateEx_Online_HasValueTwo()
    {
        ((int)DevicePhysicalStateEx.Online).Should().Be(2);
    }

    [Fact]
    public void ConnectType_Adb_HasValueOne()
    {
        ((int)ConnectType.Adb).Should().Be(1);
    }

    [Fact]
    public void ConnectType_Fastboot_HasValueTwo()
    {
        ((int)ConnectType.Fastboot).Should().Be(2);
    }

    [Fact]
    public void ConnectType_IsFlags()
    {
        var connectType = ConnectType.Adb | ConnectType.Fastboot;
        ((int)connectType).Should().Be(3);
    }
}
