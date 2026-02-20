using Xunit;
using FluentAssertions;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.devicemgt.DeviceOperator;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.socket;

namespace LMSA.Tests;

public class DeviceManagementTests
{
    [Fact]
    public void AdbOperator_ShouldImplement_IDeviceOperator()
    {
        var op = new AdbOperator();
        op.Should().BeAssignableTo<IDeviceOperator>();
    }

    [Fact]
    public void FastbootOperator_ShouldExist()
    {
        var op = new FastbootOperator();
        op.Should().NotBeNull();
    }

    [Fact]
    public void ConnectSteps_ShouldExist()
    {
        typeof(ConnectSteps).Should().NotBeNull();
    }

    [Fact]
    public void ConnectStepStatus_ShouldHave_Values()
    {
        Enum.GetValues<ConnectStepStatus>().Should().NotBeEmpty();
    }

    [Fact]
    public void ConnectErrorCode_ShouldHave_Values()
    {
        Enum.GetValues<ConnectErrorCode>().Should().NotBeEmpty();
    }

    [Fact]
    public void DeviceConnectionManagerEx_ShouldExtend_Abstract()
    {
        var mgr = new DeviceConnectionManagerEx();
        mgr.Should().BeAssignableTo<AbstractDeviceConnectionManagerEx>();
        mgr.ConntectedDevices.Should().NotBeNull();
    }

    [Fact]
    public void FastbootDeviceEx_ShouldExtend_DeviceEx()
    {
        var device = new FastbootDeviceEx();
        device.Should().BeAssignableTo<DeviceEx>();
    }

    [Fact]
    public void DevicemgtContantClass_ShouldExist()
    {
        typeof(DevicemgtContantClass).Should().NotBeNull();
    }

    [Fact]
    public void PropInfo_ShouldManage_Properties()
    {
        var info = new PropInfo();
        info.AddOrUpdateProp("test.key", "test.value");
        info.GetProp("test.key").Should().Be("test.value");
    }

    [Fact]
    public void PropInfo_GetIntProp_ShouldParse()
    {
        var info = new PropInfo();
        info.AddOrUpdateProp("api.level", "33");
        info.GetIntProp("api.level").Should().Be(33);
        info.GetIntProp("missing.key", -1).Should().Be(-1);
    }

    [Fact]
    public void PropInfo_Reset_ShouldClearAll()
    {
        var info = new PropInfo();
        info.AddOrUpdateProp("key1", "val1");
        info.AddOrUpdateProp("key2", "val2");
        info.Reset();
        info.GetProp("key1").Should().BeNull();
    }
}
