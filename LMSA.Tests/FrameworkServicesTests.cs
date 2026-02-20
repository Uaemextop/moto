using Xunit;
using FluentAssertions;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.services.Model;

namespace LMSA.Tests;

public class FrameworkServicesTests
{
    [Fact]
    public void DevicePhysicalStateEx_ShouldHave_CorrectValues()
    {
        ((int)DevicePhysicalStateEx.None).Should().Be(-1);
        ((int)DevicePhysicalStateEx.Offline).Should().Be(0);
        ((int)DevicePhysicalStateEx.Online).Should().Be(2);
        ((int)DevicePhysicalStateEx.Unauthorized).Should().Be(7);
        ((int)DevicePhysicalStateEx.UsbDebugSwitchClosed).Should().Be(9);
    }

    [Fact]
    public void DeviceType_ShouldHave_AllTypes()
    {
        Enum.GetValues<DeviceType>().Should().HaveCountGreaterThan(1);
    }

    [Fact]
    public void ConnectType_ShouldHave_AllTypes()
    {
        Enum.GetValues<ConnectType>().Should().HaveCountGreaterThan(1);
    }

    [Fact]
    public void DeviceEx_ShouldBeAbstract()
    {
        typeof(DeviceEx).IsAbstract.Should().BeTrue();
    }

    [Fact]
    public void BusinessData_ShouldStore_Properties()
    {
        var data = new BusinessData();
        data.Should().NotBeNull();
    }

    [Fact]
    public void BusinessType_ShouldHave_Values()
    {
        Enum.GetValues<BusinessType>().Should().NotBeEmpty();
    }

    [Fact]
    public void BusinessStatus_ShouldHave_Values()
    {
        Enum.GetValues<BusinessStatus>().Should().NotBeEmpty();
    }

    [Fact]
    public void PluginExportAttribute_ShouldStore_PluginId()
    {
        var attr = new PluginExportAttribute(typeof(IPlugin), "test-guid");
        attr.PluginId.Should().Be("test-guid");
    }

    [Fact]
    public void RuntimeContext_ShouldExist()
    {
        typeof(RuntimeContext).Should().NotBeNull();
    }
}
