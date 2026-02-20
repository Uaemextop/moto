using System.Collections.Generic;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.common.webservices.WebApiModel;
using lenovo.mbg.service.lmsa.core;
using Xunit;

namespace LMSA.Tests;

public class OfflineResponseFactoryTests
{
    [Fact]
    public void CreateDefaultOrders_ReturnsValidResponse()
    {
        var result = OfflineResponseFactory.CreateDefaultOrders();

        Assert.NotNull(result);
        Assert.Equal("offline", result.id);
        Assert.Equal("OFFLINE", result.vipName);
        Assert.True(result.multiDevice);
        Assert.False(result.popUp);
        Assert.Equal(-1, result.popMode);
        Assert.Equal(int.MaxValue, result.freeAmount);
        Assert.Equal(0, result.usedFreeAmount);
    }

    [Fact]
    public void CreateDefaultOrders_HasEnabledOrders()
    {
        var result = OfflineResponseFactory.CreateDefaultOrders();

        Assert.NotNull(result.enableOrderDtos);
        Assert.Single(result.enableOrderDtos);
        Assert.True(result.enableOrderDtos[0].enable);
        Assert.True(result.enableOrderDtos[0].display);
    }

    [Fact]
    public void CreateDefaultOrders_HasEmptyUnableOrders()
    {
        var result = OfflineResponseFactory.CreateDefaultOrders();

        Assert.NotNull(result.unableOrderDtos);
        Assert.Empty(result.unableOrderDtos);
    }

    [Fact]
    public void CreateDefaultOrders_AcceptsCustomUsername()
    {
        var result = OfflineResponseFactory.CreateDefaultOrders("test-user", "test@example.com");

        Assert.Equal("test-user", result.username);
        Assert.Equal("test@example.com", result.email);
    }

    [Fact]
    public void CreateDefaultOrderItem_IsEnabledAndDisplayed()
    {
        var result = OfflineResponseFactory.CreateDefaultOrderItem();

        Assert.True(result.enable);
        Assert.True(result.display);
        Assert.Equal("ACTIVE", result.orderStatus);
        Assert.Equal("ACTIVE", result.serverOrderStatus);
        Assert.Equal(int.MaxValue, result.imeiCount);
        Assert.Equal(0, result.imeiUsedCount);
    }

    [Fact]
    public void CreateDefaultOrderItem_HasFarFutureExpiration()
    {
        var result = OfflineResponseFactory.CreateDefaultOrderItem();

        Assert.NotNull(result.expiredDate);
        Assert.True(result.expiredDate > System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
    }

    [Fact]
    public void CreateDefaultB2BUserInfo_AllFeaturesEnabled()
    {
        var result = OfflineResponseFactory.CreateDefaultB2BUserInfo();

        Assert.True(result.IsMultiDev);
        Assert.True(result.B2bButtonDisplay);
        Assert.Equal("OFFLINE", result.B2bMode);
    }

    [Fact]
    public void ApplyOfflineDefaults_NullResponse_CreatesDefault()
    {
        var result = OfflineResponseFactory.ApplyOfflineDefaults(null);

        Assert.NotNull(result);
        Assert.NotNull(result.enableOrderDtos);
        Assert.True(result.enableOrderDtos.Count > 0);
    }

    [Fact]
    public void ApplyOfflineDefaults_EmptyOrders_AddsDefault()
    {
        var response = new RespOrders
        {
            id = "test",
            username = "user",
            email = "",
            vipName = "",
            enableOrderDtos = new List<OrderItem>(),
            unableOrderDtos = new List<OrderItem>()
        };

        var result = OfflineResponseFactory.ApplyOfflineDefaults(response);

        Assert.Single(result.enableOrderDtos);
        Assert.True(result.enableOrderDtos[0].enable);
    }

    [Fact]
    public void ApplyOfflineDefaults_SuppressesPopups()
    {
        var response = new RespOrders
        {
            id = "test",
            username = "user",
            email = "",
            vipName = "",
            enableOrderDtos = new List<OrderItem> { new OrderItem { enable = true, display = true } },
            unableOrderDtos = new List<OrderItem>(),
            popUp = true,
            popMode = 2
        };

        var result = OfflineResponseFactory.ApplyOfflineDefaults(response);

        Assert.False(result.popUp);
        Assert.Equal(-1, result.popMode);
    }

    [Fact]
    public void ApplyOfflineDefaults_ForcesMultiDevice()
    {
        var response = new RespOrders
        {
            id = "test",
            username = "user",
            email = "",
            vipName = "",
            multiDevice = false,
            enableOrderDtos = new List<OrderItem> { new OrderItem() },
            unableOrderDtos = new List<OrderItem>()
        };

        var result = OfflineResponseFactory.ApplyOfflineDefaults(response);

        Assert.True(result.multiDevice);
    }

    [Fact]
    public void ApplyOfflineDefaults_ForcesOrdersEnabled()
    {
        var order = new OrderItem { enable = false, display = false };
        var response = new RespOrders
        {
            id = "test",
            username = "user",
            email = "",
            vipName = "",
            enableOrderDtos = new List<OrderItem> { order },
            unableOrderDtos = new List<OrderItem>()
        };

        var result = OfflineResponseFactory.ApplyOfflineDefaults(response);

        Assert.True(result.enableOrderDtos[0].enable);
        Assert.True(result.enableOrderDtos[0].display);
    }

    [Fact]
    public void ApplyOfflineDefaults_OfflineModeDisabled_PassesThrough()
    {
        var original = FeatureConfiguration.OfflineMode;
        try
        {
            FeatureConfiguration.OfflineMode = false;

            var response = new RespOrders
            {
                id = "test",
                username = "user",
                email = "",
                vipName = "",
                multiDevice = false,
                popUp = true,
                popMode = 2,
                enableOrderDtos = new List<OrderItem>(),
                unableOrderDtos = new List<OrderItem>()
            };

            var result = OfflineResponseFactory.ApplyOfflineDefaults(response);

            Assert.False(result.multiDevice);
            Assert.True(result.popUp);
            Assert.Equal(2, result.popMode);
            Assert.Empty(result.enableOrderDtos);
        }
        finally
        {
            FeatureConfiguration.OfflineMode = original;
        }
    }

    [Fact]
    public void ApplyOfflineDefaults_ResetsFreeAmount()
    {
        var response = new RespOrders
        {
            id = "test",
            username = "user",
            email = "",
            vipName = "",
            freeAmount = 5,
            usedFreeAmount = 5,
            enableOrderDtos = new List<OrderItem> { new OrderItem() },
            unableOrderDtos = new List<OrderItem>()
        };

        var result = OfflineResponseFactory.ApplyOfflineDefaults(response);

        Assert.Equal(int.MaxValue, result.freeAmount);
        Assert.Equal(0, result.usedFreeAmount);
    }
}
