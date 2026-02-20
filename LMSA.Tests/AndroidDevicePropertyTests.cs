using lenovo.mbg.service.framework.devicemgt.DeviceInfo;

namespace LMSA.Tests;

public class AndroidDevicePropertyTests
{
    private AndroidDeviceProperty CreateDeviceWithProps(Dictionary<string, string> props)
    {
        var propInfo = new PropInfo();
        foreach (var kv in props)
        {
            propInfo.AddOrUpdateProp(new PropItem { Key = kv.Key, Value = kv.Value });
        }
        return new AndroidDeviceProperty(propInfo);
    }

    [Fact]
    public void NewDevice_HasEmptyProperties()
    {
        var device = new AndroidDeviceProperty();
        Assert.Equal(string.Empty, device.ModelId);
    }

    [Fact]
    public void AndroidVersion_ReturnsCorrectValue()
    {
        var device = CreateDeviceWithProps(new Dictionary<string, string>
        {
            { "ro.build.version.release", "13" }
        });
        Assert.Equal("13", device.AndroidVersion);
    }

    [Fact]
    public void Brand_ReturnsCorrectValue()
    {
        var device = CreateDeviceWithProps(new Dictionary<string, string>
        {
            { "ro.product.brand", "motorola" }
        });
        Assert.Equal("motorola", device.Brand);
    }

    [Fact]
    public void ModelName_ForMotorola_UsesHardwareSku()
    {
        var device = CreateDeviceWithProps(new Dictionary<string, string>
        {
            { "ro.product.brand", "motorola" },
            { "ro.product.model", "moto g power" },
            { "ro.boot.hardware.sku", "XT2345-1" }
        });
        Assert.Equal("XT2345-1", device.ModelName);
    }

    [Fact]
    public void ModelName_ForLenovo_UsesProductModel()
    {
        var device = CreateDeviceWithProps(new Dictionary<string, string>
        {
            { "ro.product.brand", "Lenovo" },
            { "ro.product.model", "Lenovo TB-8504F" }
        });
        Assert.Equal("Lenovo TB-8504F", device.ModelName);
    }

    [Fact]
    public void IMEI1_ReturnsFirstAvailableValue()
    {
        var device = CreateDeviceWithProps(new Dictionary<string, string>
        {
            { "device.imei1", "123456789012345" }
        });
        Assert.Equal("123456789012345", device.IMEI1);
    }

    [Fact]
    public void SN_ReturnsFirstValidValue()
    {
        var device = CreateDeviceWithProps(new Dictionary<string, string>
        {
            { "ro.serialno", "ABCD12345" }
        });
        Assert.Equal("ABCD12345", device.SN);
    }

    [Fact]
    public void SN_SkipsUnknownValues()
    {
        var device = CreateDeviceWithProps(new Dictionary<string, string>
        {
            { "sys.customsn.showcode", "UNKNOWN" },
            { "ro.serialno", "VALID_SN" }
        });
        Assert.Equal("VALID_SN", device.SN);
    }

    [Fact]
    public void ApiLevel_ParsesIntCorrectly()
    {
        var device = CreateDeviceWithProps(new Dictionary<string, string>
        {
            { "ro.build.version.sdk", "33" }
        });
        Assert.Equal(33, device.ApiLevel);
    }

    [Fact]
    public void BatteryQuantityPercentage_ParsesDoubleCorrectly()
    {
        var device = CreateDeviceWithProps(new Dictionary<string, string>
        {
            { "battery.quantity", "85.5" }
        });
        Assert.Equal(85.5, device.BatteryQuantityPercentage);
    }

    [Fact]
    public void SimCount_SingleSim_Returns1()
    {
        var device = CreateDeviceWithProps(new Dictionary<string, string>
        {
            { "persist.radio.multisim.config", "SS" }
        });
        Assert.Equal(1, device.SimCount);
    }

    [Fact]
    public void SimCount_DualSim_Returns2()
    {
        var device = CreateDeviceWithProps(new Dictionary<string, string>
        {
            { "persist.radio.multisim.config", "DSDS" }
        });
        Assert.Equal(2, device.SimCount);
    }

    [Fact]
    public void Category_ForTablet_ReturnsTablet()
    {
        var device = CreateDeviceWithProps(new Dictionary<string, string>
        {
            { "ro.product.model", "Lenovo PB2-690Y" },
            { "ro.product.brand", "Lenovo" }
        });
        Assert.Equal("tablet", device.Category);
    }

    [Fact]
    public void FsgVersion_FallsBackToAlternatives()
    {
        var device = CreateDeviceWithProps(new Dictionary<string, string>
        {
            { "gsm.version.baseband", "1.0.0" }
        });
        Assert.Equal("1.0.0", device.FsgVersion);
    }

    [Fact]
    public void Others_ReturnsDictionaryOfAllProps()
    {
        var device = CreateDeviceWithProps(new Dictionary<string, string>
        {
            { "key1", "val1" },
            { "key2", "val2" }
        });
        var others = device.Others;
        Assert.Equal(2, others.Count);
        Assert.Equal("val1", others["key1"]);
    }

    [Fact]
    public void GetPropertyValue_ReturnsCorrectValue()
    {
        var device = CreateDeviceWithProps(new Dictionary<string, string>
        {
            { "custom.prop", "custom_value" }
        });
        Assert.Equal("custom_value", device.GetPropertyValue("custom.prop"));
    }
}
