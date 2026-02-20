using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.services.Model;

namespace LMSA.Tests;

public class DeviceExTests
{
    private class TestDevice : DeviceEx
    {
        private IAndroidDevice? _property;
        public override IAndroidDevice? Property { get => _property; set => _property = value; }
        public override void Load() { }
    }

    [Fact]
    public void Constructor_SetsDefaultDeviceType()
    {
        var device = new TestDevice();
        Assert.Equal(DeviceType.Normal, device.DeviceType);
    }

    [Fact]
    public void PhysicalStatus_DefaultIsNone()
    {
        var device = new TestDevice();
        Assert.Equal(DevicePhysicalStateEx.None, device.PhysicalStatus);
    }

    [Fact]
    public void SoftStatus_DefaultIsNone()
    {
        var device = new TestDevice();
        Assert.Equal(DeviceSoftStateEx.None, device.SoftStatus);
    }

    [Fact]
    public void PhysicalStatus_SettingOffline_SetsSoftStatusOffline()
    {
        var device = new TestDevice();
        device.PhysicalStatus = DevicePhysicalStateEx.Online;
        device.PhysicalStatus = DevicePhysicalStateEx.Offline;

        Assert.Equal(DeviceSoftStateEx.Offline, device.SoftStatus);
    }

    [Fact]
    public void PhysicalStatusChanged_EventFires()
    {
        var device = new TestDevice();
        DevicePhysicalStateEx? receivedState = null;
        device.PhysicalStatusChanged += (sender, state) => receivedState = state;

        device.PhysicalStatus = DevicePhysicalStateEx.Online;

        Assert.Equal(DevicePhysicalStateEx.Online, receivedState);
    }

    [Fact]
    public void SoftStatusChanged_EventFires()
    {
        var device = new TestDevice();
        DeviceSoftStateEx? receivedState = null;
        device.SoftStatusChanged += (sender, state) => receivedState = state;

        device.SoftStatus = DeviceSoftStateEx.Online;

        Assert.Equal(DeviceSoftStateEx.Online, receivedState);
    }

    [Fact]
    public void IsHWEnable_OnlineWithMa_ReturnsTrue()
    {
        var device = new TestDevice();
        device.SoftStatus = DeviceSoftStateEx.Online;
        device.ConnectedAppType = ConnectedAppTypesDefine.Ma;

        Assert.True(device.IsHWEnable());
    }

    [Fact]
    public void IsHWEnable_OnlineWithMotoHighVersion_ReturnsTrue()
    {
        var device = new TestDevice();
        device.SoftStatus = DeviceSoftStateEx.Online;
        device.ConnectedAppType = ConnectedAppTypesDefine.Moto;
        device.AppVersion = 1200000;

        Assert.True(device.IsHWEnable());
    }

    [Fact]
    public void IsHWEnable_OnlineWithMotoLowVersion_ReturnsFalse()
    {
        var device = new TestDevice();
        device.SoftStatus = DeviceSoftStateEx.Online;
        device.ConnectedAppType = ConnectedAppTypesDefine.Moto;
        device.AppVersion = 100;

        Assert.False(device.IsHWEnable());
    }

    [Fact]
    public void IsHWEnable_Offline_ReturnsFalse()
    {
        var device = new TestDevice();
        device.SoftStatus = DeviceSoftStateEx.Offline;
        device.ConnectedAppType = ConnectedAppTypesDefine.Ma;

        Assert.False(device.IsHWEnable());
    }

    [Fact]
    public void ToString_ContainsIdentifier()
    {
        var device = new TestDevice { Identifer = "ABC123" };
        Assert.Contains("ABC123", device.ToString());
    }

    [Fact]
    public void LockSoftStatus_SetsLocked()
    {
        var device = new TestDevice();
        device.LockSoftStatus(true, DeviceSoftStateEx.Online);

        Assert.True(device.SoftStatusIsLocked);
    }

    [Fact]
    public void ReleaseSoftStatusLock_UnlocksStatus()
    {
        var device = new TestDevice();
        device.LockSoftStatus(true, DeviceSoftStateEx.Online);
        Assert.True(device.SoftStatusIsLocked);

        device.ReleaseSoftStatusLock();
        Assert.False(device.SoftStatusIsLocked);
    }

    [Fact]
    public void UnloadEvent_ClearsEvents()
    {
        var device = new TestDevice();
        device.PhysicalStatusChanged += (s, e) => { };
        device.SoftStatusChanged += (s, e) => { };

        device.UnloadEvent();

        // Should not throw
        device.PhysicalStatus = DevicePhysicalStateEx.Online;
        device.SoftStatus = DeviceSoftStateEx.Online;
    }
}
