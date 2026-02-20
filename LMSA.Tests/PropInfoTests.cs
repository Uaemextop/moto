using lenovo.mbg.service.framework.devicemgt.DeviceInfo;

namespace LMSA.Tests;

public class PropInfoTests
{
    [Fact]
    public void NewPropInfo_HasEmptyProps()
    {
        var propInfo = new PropInfo();
        Assert.NotNull(propInfo.Props);
        Assert.Empty(propInfo.Props);
    }

    [Fact]
    public void AddOrUpdateProp_AddsSingleProp()
    {
        var propInfo = new PropInfo();
        propInfo.AddOrUpdateProp(new PropItem { Key = "key1", Value = "value1" });

        Assert.Single(propInfo.Props);
        Assert.Equal("value1", propInfo.GetProp("key1"));
    }

    [Fact]
    public void AddOrUpdateProp_UpdatesExistingProp()
    {
        var propInfo = new PropInfo();
        propInfo.AddOrUpdateProp(new PropItem { Key = "key1", Value = "value1" });
        propInfo.AddOrUpdateProp(new PropItem { Key = "key1", Value = "value2" });

        Assert.Single(propInfo.Props);
        Assert.Equal("value2", propInfo.GetProp("key1"));
    }

    [Fact]
    public void AddOrUpdateProp_AddsList()
    {
        var propInfo = new PropInfo();
        var props = new List<PropItem>
        {
            new PropItem { Key = "key1", Value = "value1" },
            new PropItem { Key = "key2", Value = "value2" }
        };
        propInfo.AddOrUpdateProp(props);

        Assert.Equal(2, propInfo.Props.Count);
        Assert.Equal("value1", propInfo.GetProp("key1"));
        Assert.Equal("value2", propInfo.GetProp("key2"));
    }

    [Fact]
    public void GetProp_NonExistentKey_ReturnsNull()
    {
        var propInfo = new PropInfo();
        propInfo.AddOrUpdateProp(new PropItem { Key = "key1", Value = "value1" });

        Assert.Null(propInfo.GetProp("nonexistent"));
    }

    [Fact]
    public void GetProp_EmptyProps_ReturnsEmptyString()
    {
        var propInfo = new PropInfo();
        Assert.Equal(string.Empty, propInfo.GetProp("anything"));
    }

    [Fact]
    public void GetIntProp_ParsesCorrectly()
    {
        var propInfo = new PropInfo();
        propInfo.AddOrUpdateProp(new PropItem { Key = "sdk", Value = "30" });

        Assert.Equal(30, propInfo.GetIntProp("sdk"));
    }

    [Fact]
    public void GetIntProp_InvalidValue_ReturnsZero()
    {
        var propInfo = new PropInfo();
        propInfo.AddOrUpdateProp(new PropItem { Key = "sdk", Value = "invalid" });

        Assert.Equal(0, propInfo.GetIntProp("sdk"));
    }

    [Fact]
    public void GetLongProp_ParsesCorrectly()
    {
        var propInfo = new PropInfo();
        propInfo.AddOrUpdateProp(new PropItem { Key = "size", Value = "1073741824" });

        Assert.Equal(1073741824L, propInfo.GetLongProp("size"));
    }

    [Fact]
    public void Reset_UpdatesExistingValue()
    {
        var propInfo = new PropInfo();
        propInfo.AddOrUpdateProp(new PropItem { Key = "key1", Value = "old" });

        propInfo.Reset("key1", "new");

        Assert.Equal("new", propInfo.GetProp("key1"));
    }

    [Fact]
    public void AddOrUpdateProp_WithReplaceKey_ReplacesKeyNames()
    {
        var propInfo = new PropInfo();
        var items = new List<PropItem>
        {
            new PropItem { Key = "imei1", Value = "123456789" }
        };
        var replaceKey = new Dictionary<string, string>
        {
            { "imei1", "device.imei1" }
        };

        propInfo.AddOrUpdateProp(items, replaceKey);

        Assert.Equal("123456789", propInfo.GetProp("device.imei1"));
    }
}
