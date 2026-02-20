using lenovo.mbg.service.framework.services.Device;
using Xunit;

namespace LMSA.Tests;

/// <summary>
/// Unit tests for device state management.
/// </summary>
public class DeviceExTests
{
    private class TestDeviceEx : DeviceEx
    {
        public override IAndroidDevice? Property { get; set; }

        public override void Load() { }
    }

    [Fact]
    public void DeviceEx_InitialState_IsNone()
    {
        // Arrange & Act
        var device = new TestDeviceEx();

        // Assert
        Assert.Equal(DevicePhysicalStateEx.None, device.PhysicalStatus);
        Assert.Equal(DeviceSoftStateEx.None, device.SoftStatus);
        Assert.Equal(DeviceWorkType.None, device.WorkType);
        Assert.Equal(DeviceType.Normal, device.DeviceType);
    }

    [Fact]
    public void DeviceEx_SetPhysicalStatus_UpdatesStatus()
    {
        // Arrange
        var device = new TestDeviceEx { Identifer = "test-device", ConnectTime = "2026-02-20" };

        // Act
        device.PhysicalStatus = DevicePhysicalStateEx.Online;

        // Assert
        Assert.Equal(DevicePhysicalStateEx.Online, device.PhysicalStatus);
    }

    [Fact]
    public void DeviceEx_SetOfflinePhysicalStatus_SetsSoftStatusOffline()
    {
        // Arrange
        var device = new TestDeviceEx { Identifer = "test-device", ConnectTime = "2026-02-20" };
        device.SoftStatus = DeviceSoftStateEx.Online;

        // Act
        device.PhysicalStatus = DevicePhysicalStateEx.Offline;

        // Allow async event to fire
        System.Threading.Thread.Sleep(100);

        // Assert - when physical goes offline, soft should also go offline
        Assert.Equal(DeviceSoftStateEx.Offline, device.SoftStatus);
    }

    [Fact]
    public void DeviceEx_SetWorkType_UpdatesWorkType()
    {
        // Arrange
        var device = new TestDeviceEx { Identifer = "test-device", ConnectTime = "2026-02-20" };

        // Act
        device.WorkType = DeviceWorkType.Rescue;

        // Assert
        Assert.Equal(DeviceWorkType.Rescue, device.WorkType);
    }

    [Fact]
    public void DeviceEx_ToString_ContainsIdentifier()
    {
        // Arrange
        var device = new TestDeviceEx { Identifer = "ABC123", ConnectTime = "2026-02-20" };

        // Act
        string result = device.ToString();

        // Assert
        Assert.Contains("ABC123", result);
    }

    [Fact]
    public void DeviceEx_IsHWEnable_WithMaAppOnline_ReturnsTrue()
    {
        // Arrange
        var device = new TestDeviceEx { Identifer = "test", ConnectTime = "2026-02-20" };
        device.ConnectedAppType = "Ma";
        device.SoftStatus = DeviceSoftStateEx.Online;

        // Assert
        Assert.True(device.IsHWEnable());
    }

    [Fact]
    public void DeviceEx_IsHWEnable_WhenOffline_ReturnsFalse()
    {
        // Arrange
        var device = new TestDeviceEx { Identifer = "test", ConnectTime = "2026-02-20" };
        device.ConnectedAppType = "Ma";

        // Assert - SoftStatus is None by default
        Assert.False(device.IsHWEnable());
    }
}
