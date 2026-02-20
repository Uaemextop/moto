using lenovo.mbg.service.framework.devicemgt.DeviceInfo;

namespace LMSA.Tests.DeviceManagement;

public class PropInfoTests
{
    [Fact]
    public void PropInfo_Constructor_InitializesEmptyProps()
    {
        var propInfo = new PropInfo();
        Assert.NotNull(propInfo.Props);
        Assert.Empty(propInfo.Props);
    }

    [Fact]
    public void AddOrUpdateProp_AddsSingleItem()
    {
        var propInfo = new PropInfo();
        propInfo.AddOrUpdateProp(new PropItem { Key = "key1", Value = "value1" });

        Assert.Single(propInfo.Props);
        Assert.Equal("value1", propInfo.GetProp("key1"));
    }

    [Fact]
    public void AddOrUpdateProp_UpdatesExistingItem()
    {
        var propInfo = new PropInfo();
        propInfo.AddOrUpdateProp(new PropItem { Key = "key1", Value = "value1" });
        propInfo.AddOrUpdateProp(new PropItem { Key = "key1", Value = "updated" });

        Assert.Single(propInfo.Props);
        Assert.Equal("updated", propInfo.GetProp("key1"));
    }

    [Fact]
    public void AddOrUpdateProp_AddsList()
    {
        var propInfo = new PropInfo();
        var items = new List<PropItem>
        {
            new PropItem { Key = "key1", Value = "value1" },
            new PropItem { Key = "key2", Value = "value2" }
        };
        propInfo.AddOrUpdateProp(items);

        Assert.Equal(2, propInfo.Props.Count);
        Assert.Equal("value1", propInfo.GetProp("key1"));
        Assert.Equal("value2", propInfo.GetProp("key2"));
    }

    [Fact]
    public void AddOrUpdateProp_WithKeyMapping_MapsKeys()
    {
        var propInfo = new PropInfo();
        var items = new List<PropItem>
        {
            new PropItem { Key = "imei1", Value = "123456789" }
        };
        var mapping = new Dictionary<string, string>
        {
            { "imei1", "device.imei1" }
        };
        propInfo.AddOrUpdateProp(items, mapping);

        Assert.Equal("123456789", propInfo.GetProp("device.imei1"));
    }

    [Fact]
    public void GetProp_ReturnsNull_WhenKeyNotFound()
    {
        var propInfo = new PropInfo();
        Assert.Null(propInfo.GetProp("nonexistent"));
    }

    [Fact]
    public void GetIntProp_ReturnsIntValue()
    {
        var propInfo = new PropInfo();
        propInfo.AddOrUpdateProp(new PropItem { Key = "sdk", Value = "30" });

        Assert.Equal(30, propInfo.GetIntProp("sdk"));
    }

    [Fact]
    public void GetIntProp_ReturnsZero_WhenNotNumeric()
    {
        var propInfo = new PropInfo();
        propInfo.AddOrUpdateProp(new PropItem { Key = "text", Value = "abc" });

        Assert.Equal(0, propInfo.GetIntProp("text"));
    }

    [Fact]
    public void GetIntProp_ReturnsZero_WhenKeyNotFound()
    {
        var propInfo = new PropInfo();
        Assert.Equal(0, propInfo.GetIntProp("nonexistent"));
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
    public void Reset_AddsNewValue_WhenKeyNotFound()
    {
        var propInfo = new PropInfo();
        propInfo.Reset("key1", "value1");

        Assert.Equal("value1", propInfo.GetProp("key1"));
    }

    [Fact]
    public void AddOrUpdateProp_NullList_DoesNotThrow()
    {
        var propInfo = new PropInfo();
        var exception = Record.Exception(() => propInfo.AddOrUpdateProp((List<PropItem>?)null));
        Assert.Null(exception);
    }
}
