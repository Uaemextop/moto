using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.devicemgt.DeviceInfo;
using lenovo.mbg.service.framework.services.Device;

namespace LMSA.Tests;

public class ConnectionMonitorTests
{
    [Fact]
    public void DeviceDataEx_CanSetProperties()
    {
        var data = new DeviceDataEx();
        data.Serial = "ABC123";
        data.PhyState = DevicePhysicalStateEx.Online;

        Assert.Equal("ABC123", data.Serial);
        Assert.Equal(DevicePhysicalStateEx.Online, data.PhyState);
    }

    [Fact]
    public void DevicemgtContantClass_DefaultValues()
    {
        Assert.False(DevicemgtContantClass.IsBackAndRestoreFrm);
        Assert.IsType<bool>(DevicemgtContantClass.IsOtherToBackAndRestoreAndAutoConnection);
    }

    [Fact]
    public void DevicemgtContantClass_CanSetValues()
    {
        DevicemgtContantClass.IsBackAndRestoreFrm = true;
        Assert.True(DevicemgtContantClass.IsBackAndRestoreFrm);
        DevicemgtContantClass.IsBackAndRestoreFrm = false;
    }

    [Fact]
    public void FastbootDeviceEx_HasCorrectDefaults()
    {
        var device = new FastbootDeviceEx();

        Assert.NotNull(device.Property);
        Assert.NotNull(device.DeviceOperator);
        Assert.IsType<FastbootAndroidDevice>(device.Property);
    }

    [Fact]
    public void SimpleAdbDeviceEx_HasCorrectDefaults()
    {
        var device = new SimpleAdbDeviceEx();

        Assert.NotNull(device.Property);
        Assert.NotNull(device.DeviceOperator);
        Assert.IsType<AndroidDeviceProperty>(device.Property);
    }

    [Fact]
    public void FastbootAndroidDevice_HasCorrectDefaults()
    {
        var device = new FastbootDeviceEx();
        var fbDevice = device.Property;

        Assert.Equal(0.0, fbDevice.BatteryQuantityPercentage);
        Assert.Equal(string.Empty, fbDevice.Brand);
        Assert.Equal(-1, fbDevice.ApiLevel);
        Assert.Equal("phone", fbDevice.Category);
        Assert.Equal(string.Empty, fbDevice.PN);
        Assert.Equal(string.Empty, fbDevice.RomVersion);
    }

    [Fact]
    public void ReadPropertiesInFastboot_GetProp_ReturnsNullForMissingKey()
    {
        var device = new FastbootDeviceEx();
        var reader = new ReadPropertiesInFastboot(device);

        string result = reader.GetProp("nonexistent");
        Assert.Null(result);
    }

    [Fact]
    public void ReadPropertiesInFastboot_Props_EmptyByDefault()
    {
        var device = new FastbootDeviceEx();
        var reader = new ReadPropertiesInFastboot(device);

        Assert.NotNull(reader.Props);
        Assert.Empty(reader.Props);
    }

    [Fact]
    public void AdbConnectionMonitorEx_CanBeCreated()
    {
        var listener = new TestCompositListener();
        using var monitor = new AdbConnectionMonitorEx(listener);
        Assert.NotNull(monitor);
    }

    [Fact]
    public void FBConnectionMonitorEx_CanBeCreated()
    {
        var listener = new TestCompositListener();
        var monitor = new FBConnectionMonitorEx(listener);
        Assert.NotNull(monitor);
    }

    private class TestCompositListener : ICompositListener
    {
        public List<lenovo.mbg.service.framework.services.DeviceEx> ConnectedDevices { get; } = new();
        public List<lenovo.mbg.service.framework.services.DeviceEx> DisconnectedDevices { get; } = new();

        public void OnConnect(lenovo.mbg.service.framework.services.DeviceEx device, DevicePhysicalStateEx phyState)
        {
            ConnectedDevices.Add(device);
        }

        public void OnDisconnect(lenovo.mbg.service.framework.services.DeviceEx device)
        {
            DisconnectedDevices.Add(device);
        }

        public void OnWifiMonitoringEndPointChanged(List<Tuple<string, string>> endpoints)
        {
        }
    }
}
