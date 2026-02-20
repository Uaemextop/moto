using lenovo.mbg.service.common.webservices.WebApiModel;

namespace LMSA.Tests;

public class WebApiModelTests
{
    [Fact]
    public void RSAKey_CanSetProperties()
    {
        var key = new RSAKey();
        key.PublicKey = "public";
        key.PrivateKey = "private";

        Assert.Equal("public", key.PublicKey);
        Assert.Equal("private", key.PrivateKey);
    }

    [Fact]
    public void ToolVersionModel_CanSetProperties()
    {
        var model = new ToolVersionModel();
        model.Id = "1";
        model.VersionNumber = "5.0.0";
        model.FilePath = "/path/to/file";
        model.FileSize = 1024;
        model.MD5 = "abc123";
        model.IsForce = true;
        model.ReleaseDate = "2024-01-01";
        model.ReleaseNotes = "New release";

        Assert.Equal("1", model.Id);
        Assert.Equal("5.0.0", model.VersionNumber);
        Assert.Equal(1024, model.FileSize);
        Assert.True(model.IsForce);
    }

    [Fact]
    public void FlashedDevModel_CanSetProperties()
    {
        var model = new FlashedDevModel();
        model.Imei = "123456789012345";
        model.ModelName = "moto g power";
        model.Category = "phone";
        model.createDate = 1704067200000;

        Assert.Equal("123456789012345", model.Imei);
        Assert.Equal("moto g power", model.ModelName);
        Assert.Equal(2024, model.FlashDate.Year);
    }

    [Fact]
    public void OrderItem_CanSetProperties()
    {
        var order = new OrderItem();
        order.orderId = 123;
        order.orderStatus = "active";
        order.type = "premium";
        order.enable = true;
        order.display = true;

        Assert.Equal(123, order.orderId);
        Assert.Equal("active", order.orderStatus);
        Assert.True(order.enable);
    }

    [Fact]
    public void RespOrders_CanSetProperties()
    {
        var orders = new RespOrders();
        orders.username = "testuser";
        orders.email = "test@example.com";
        orders.freeAmount = 5;
        orders.multiDevice = true;
        orders.enableOrderDtos = new List<OrderItem> { new OrderItem { orderId = 1 } };

        Assert.Equal("testuser", orders.username);
        Assert.Equal(5, orders.freeAmount);
        Assert.True(orders.multiDevice);
        Assert.Single(orders.enableOrderDtos);
    }

    [Fact]
    public void PriceInfo_CanSetProperties()
    {
        var price = new PriceInfo();
        price.sku = "SKU001";
        price.price = 9.99f;
        price.cardName = "Basic";
        price.country = "US";
        price.monetaryUnit = "USD";

        Assert.Equal("SKU001", price.sku);
        Assert.Equal(9.99f, price.price);
        Assert.Equal("USD", price.monetaryUnit);
    }
}
