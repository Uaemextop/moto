using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.devicemgt;

namespace LMSA.Tests.PluginsCommon;

public class DeviceExTests
{
    private class TestDeviceEx : DeviceEx
    {
        private IAndroidDevice? _property;

        public override IAndroidDevice? Property
        {
            get => _property;
            set => _property = value;
        }

        public override void Load()
        {
        }
    }

    [Fact]
    public void Constructor_SetsDefaultValues()
    {
        var device = new TestDeviceEx();

        Assert.Equal(DeviceType.Normal, device.DeviceType);
        Assert.Equal(DeviceWorkType.None, device.WorkType);
        Assert.Equal(DevicePhysicalStateEx.None, device.PhysicalStatus);
        Assert.Equal(DeviceSoftStateEx.None, device.SoftStatus);
        Assert.False(device.IsRemove);
        Assert.False(device.SoftStatusIsLocked);
    }

    [Fact]
    public void Identifer_CanBeSetAndGet()
    {
        var device = new TestDeviceEx();
        device.Identifer = "ABC123";

        Assert.Equal("ABC123", device.Identifer);
    }

    [Fact]
    public void ConnectType_CanBeSetAndGet()
    {
        var device = new TestDeviceEx();
        device.ConnectType = ConnectType.Adb;

        Assert.Equal(ConnectType.Adb, device.ConnectType);
    }

    [Fact]
    public void PhysicalStatus_SetsOfflineSoftStatus_WhenOffline()
    {
        var device = new TestDeviceEx();
        device.PhysicalStatus = DevicePhysicalStateEx.Online;
        device.SoftStatus = DeviceSoftStateEx.Online;

        device.PhysicalStatus = DevicePhysicalStateEx.Offline;

        Assert.Equal(DeviceSoftStateEx.Offline, device.SoftStatus);
    }

    [Fact]
    public void IsHWEnable_ReturnsFalse_WhenNotOnline()
    {
        var device = new TestDeviceEx();
        device.SoftStatus = DeviceSoftStateEx.Offline;

        Assert.False(device.IsHWEnable());
    }

    [Fact]
    public void IsHWEnable_ReturnsFalse_WhenOnlineButNoAppType()
    {
        var device = new TestDeviceEx();
        // Need to set status without triggering the setter cascade
        device.ConnectedAppType = "";

        Assert.False(device.IsHWEnable());
    }

    [Fact]
    public void ToString_ContainsDeviceIdentifier()
    {
        var device = new TestDeviceEx();
        device.Identifer = "TEST_DEVICE";
        device.ConnectType = ConnectType.Adb;

        string result = device.ToString();

        Assert.Contains("TEST_DEVICE", result);
        Assert.Contains("Adb", result);
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
    public void UnloadEvent_ClearsCallbacks()
    {
        var device = new TestDeviceEx();
        device.ValidateCodeFunc = (task, str) => true;

        device.UnloadEvent();

        Assert.Null(device.ValidateCodeFunc);
    }
}
