using System.Collections.Generic;
using FluentAssertions;
using lenovo.mbg.service.framework.devicemgt.DeviceOperator;
using lenovo.mbg.service.framework.services.Device;
using Moq;
using Xunit;

namespace LMSA.Tests.DeviceManagement;

public class FastbootOperatorTests
{
    [Fact]
    public void FindDevices_WithEmptyOutput_ReturnsEmptyList()
    {
        var operator_ = new FastbootOperator();
        // FastbootMonitor exe won't exist in test environment, but the method should handle this gracefully
        var result = operator_.FindDevices("nonexistent.exe");
        result.Should().NotBeNull();
        result.Should().BeOfType<List<string>>();
    }

    [Fact]
    public void FastbootOperator_ImplementsIDeviceOperator()
    {
        var operator_ = new FastbootOperator();
        operator_.Should().BeAssignableTo<IDeviceOperator>();
    }

    [Fact]
    public void FastbootExe_DefaultPath_ContainsFastboot()
    {
        FastbootOperator.fastbootExe.Should().Contain("fastboot");
    }

    [Fact]
    public void Install_ThrowsNotImplementedException()
    {
        var operator_ = new FastbootOperator();
        var act = () => operator_.Install("deviceID", "apkPath");
        act.Should().Throw<System.NotImplementedException>();
    }

    [Fact]
    public void Reboot_ThrowsNotImplementedException()
    {
        var operator_ = new FastbootOperator();
        var act = () => operator_.Reboot("deviceID", "mode");
        act.Should().Throw<System.NotImplementedException>();
    }
}
