using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.services;

namespace LMSA.Tests;

public class TestDeviceEx : DeviceEx
{
    private IAndroidDevice _property;

    public override IAndroidDevice Property
    {
        get => _property;
        set => _property = value;
    }

    public override void Load()
    {
    }
}

public class DeviceExTests
{
    [Fact]
    public void NewDevice_HasDefaultValues()
    {
        var device = new TestDeviceEx();

        Assert.Equal(DeviceType.Normal, device.DeviceType);
        Assert.Equal(DevicePhysicalStateEx.None, device.PhysicalStatus);
        Assert.Equal(DeviceSoftStateEx.None, device.SoftStatus);
        Assert.Equal(DeviceWorkType.None, device.WorkType);
        Assert.False(device.IsRemove);
        Assert.False(device.AppInstallFinished);
        Assert.False(device.SoftStatusIsLocked);
    }

    [Fact]
    public void PhysicalStatus_WhenChangedToOffline_SetsSoftStatusToOffline()
    {
        var device = new TestDeviceEx();
        device.PhysicalStatus = DevicePhysicalStateEx.Online;
        device.SoftStatus = DeviceSoftStateEx.Online;

        device.PhysicalStatus = DevicePhysicalStateEx.Offline;

        Assert.Equal(DeviceSoftStateEx.Offline, device.SoftStatus);
    }

    [Fact]
    public void PhysicalStatusChanged_EventFires()
    {
        var device = new TestDeviceEx();
        bool eventFired = false;
        DevicePhysicalStateEx? receivedState = null;

        device.PhysicalStatusChanged += (sender, state) =>
        {
            eventFired = true;
            receivedState = state;
        };

        device.PhysicalStatus = DevicePhysicalStateEx.Online;

        Assert.True(eventFired);
        Assert.Equal(DevicePhysicalStateEx.Online, receivedState);
    }

    [Fact]
    public void SoftStatusChanged_EventFires()
    {
        var device = new TestDeviceEx();
        bool eventFired = false;
        DeviceSoftStateEx? receivedState = null;

        device.SoftStatusChanged += (sender, state) =>
        {
            eventFired = true;
            receivedState = state;
        };

        device.SoftStatus = DeviceSoftStateEx.Online;

        Assert.True(eventFired);
        Assert.Equal(DeviceSoftStateEx.Online, receivedState);
    }

    [Fact]
    public void IsHWEnable_WhenOnlineWithMa_ReturnsTrue()
    {
        var device = new TestDeviceEx();
        device.SoftStatus = DeviceSoftStateEx.Online;
        device.ConnectedAppType = "Ma";

        Assert.True(device.IsHWEnable());
    }

    [Fact]
    public void IsHWEnable_WhenOffline_ReturnsFalse()
    {
        var device = new TestDeviceEx();
        device.SoftStatus = DeviceSoftStateEx.Offline;
        device.ConnectedAppType = "Ma";

        Assert.False(device.IsHWEnable());
    }

    [Fact]
    public void IsHWEnable_WithMoto_RequiresMinVersion()
    {
        var device = new TestDeviceEx();
        device.SoftStatus = DeviceSoftStateEx.Online;
        device.ConnectedAppType = "Moto";

        device.AppVersion = 1000000;
        Assert.False(device.IsHWEnable());

        device.AppVersion = 1200000;
        Assert.True(device.IsHWEnable());
    }

    [Fact]
    public void LockSoftStatus_SetsLockFlag()
    {
        var device = new TestDeviceEx();

        device.LockSoftStatus(true, DeviceSoftStateEx.Online);

        Assert.True(device.SoftStatusIsLocked);
    }

    [Fact]
    public void ReleaseSoftStatusLock_ClearsLockFlag()
    {
        var device = new TestDeviceEx();
        device.LockSoftStatus(true, DeviceSoftStateEx.Online);

        device.ReleaseSoftStatusLock();

        Assert.False(device.SoftStatusIsLocked);
    }

    [Fact]
    public void UnloadEvent_ClearsEventHandlers()
    {
        var device = new TestDeviceEx();
        device.PhysicalStatusChanged += (s, e) => { };
        device.SoftStatusChanged += (s, e) => { };

        device.UnloadEvent();

        // Should not throw
        Assert.Null(device.ValidateCodeFunc);
    }

    [Fact]
    public void ToString_ReturnsDeviceInfo()
    {
        var device = new TestDeviceEx();
        device.Identifer = "ABC123";
        device.ConnectTime = "2024-01-01";
        device.ConnectedAppType = "Ma";

        string str = device.ToString();
        Assert.Contains("ABC123", str);
        Assert.Contains("2024-01-01", str);
        Assert.Contains("Ma", str);
    }
}
