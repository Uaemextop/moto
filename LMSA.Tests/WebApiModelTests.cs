using System;
using System.Collections.Generic;
using lenovo.mbg.service.common.webservices.WebApiModel;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.Login.Protocol;
using Xunit;

namespace LMSA.Tests;

public class WebApiModelTests
{
    [Fact]
    public void RespOrders_AllPropertiesSettable()
    {
        var model = new RespOrders
        {
            id = "123",
            username = "user",
            email = "test@test.com",
            vipName = "VIP",
            freeAmount = 10,
            usedFreeAmount = 2,
            multiDevice = true,
            enableOrderDtos = new List<OrderItem>(),
            unableOrderDtos = new List<OrderItem>(),
            popUp = true,
            popMode = 1
        };

        Assert.Equal("123", model.id);
        Assert.Equal("user", model.username);
        Assert.Equal("test@test.com", model.email);
        Assert.Equal("VIP", model.vipName);
        Assert.Equal(10, model.freeAmount);
        Assert.Equal(2, model.usedFreeAmount);
        Assert.True(model.multiDevice);
        Assert.NotNull(model.enableOrderDtos);
        Assert.NotNull(model.unableOrderDtos);
        Assert.True(model.popUp);
        Assert.Equal(1, model.popMode);
    }

    [Fact]
    public void OrderItem_AllPropertiesSettable()
    {
        var model = new OrderItem
        {
            orderId = 42,
            rid = "rid-1",
            orderStatus = "ACTIVE",
            type = "PREMIUM",
            usingDate = 1000L,
            purchaseAmount = 5,
            effectiveDate = 2000L,
            expiredDate = 3000L,
            imeiCount = 100,
            imeiUsedCount = 3,
            macAddressRsa = "mac",
            unit = "UNIT",
            display = true,
            refund = true,
            enable = true,
            orderLevelDesc = "Premium",
            serverOrderStatus = "ACTIVE",
            imeiDtos = new List<FlashedDevModel>()
        };

        Assert.Equal(42, model.orderId);
        Assert.Equal("rid-1", model.rid);
        Assert.Equal("ACTIVE", model.orderStatus);
        Assert.Equal("PREMIUM", model.type);
        Assert.Equal(1000L, model.usingDate);
        Assert.Equal(5, model.purchaseAmount);
        Assert.Equal(2000L, model.effectiveDate);
        Assert.Equal(3000L, model.expiredDate);
        Assert.Equal(100, model.imeiCount);
        Assert.Equal(3, model.imeiUsedCount);
        Assert.Equal("mac", model.macAddressRsa);
        Assert.Equal("UNIT", model.unit);
        Assert.True(model.display);
        Assert.True(model.refund);
        Assert.True(model.enable);
        Assert.Equal("Premium", model.orderLevelDesc);
        Assert.Equal("ACTIVE", model.serverOrderStatus);
        Assert.NotNull(model.imeiDtos);
    }

    [Fact]
    public void FlashedDevModel_AllPropertiesSettable()
    {
        var model = new FlashedDevModel
        {
            Imei = "123456789",
            ModelName = "XT2201",
            Category = "Phone",
            createDate = 1700000000000L
        };

        Assert.Equal("123456789", model.Imei);
        Assert.Equal("XT2201", model.ModelName);
        Assert.Equal("Phone", model.Category);
        Assert.Equal(1700000000000L, model.createDate);
    }

    [Fact]
    public void FlashedDevModel_FlashDate_ConvertsFromMillis()
    {
        var model = new FlashedDevModel
        {
            createDate = 1700000000000L
        };

        var expected = new DateTime(1970, 1, 1).AddMilliseconds(1700000000000L);
        Assert.Equal(expected, model.FlashDate);
    }

    [Fact]
    public void PriceInfo_AllPropertiesSettable()
    {
        var model = new PriceInfo
        {
            sku = "sku1",
            price = 1.99f,
            cardName = "Basic",
            cardDesc = "Basic plan",
            country = "US",
            monetaryUnit = "$"
        };

        Assert.Equal("sku1", model.sku);
        Assert.Equal(1.99f, model.price);
        Assert.Equal("Basic", model.cardName);
        Assert.Equal("Basic plan", model.cardDesc);
        Assert.Equal("US", model.country);
        Assert.Equal("$", model.monetaryUnit);
    }

    [Fact]
    public void B2BUserInfo_AllPropertiesSettable()
    {
        var model = new B2BUserInfo
        {
            B2bMode = "PREMIUM",
            IsMultiDev = true,
            B2bButtonDisplay = true
        };

        Assert.Equal("PREMIUM", model.B2bMode);
        Assert.True(model.IsMultiDev);
        Assert.True(model.B2bButtonDisplay);
    }

    [Fact]
    public void ResponseModel_AllPropertiesSettable()
    {
        var model = new ResponseModel<RespOrders>
        {
            success = true,
            code = "0000",
            desc = "Success",
            content = new RespOrders()
        };

        Assert.True(model.success);
        Assert.Equal("0000", model.code);
        Assert.Equal("Success", model.desc);
        Assert.NotNull(model.content);
    }

    [Fact]
    public void UserInfo_AllPropertiesSettable()
    {
        var model = new UserInfo
        {
            UserId = "uid-1",
            UserName = "testuser",
            EmailAddress = "test@test.com",
            Country = "US",
            FullName = "Test User",
            PhoneNumber = "555-0100",
            UserSource = "lenovoid",
            IsB2BSupportMultDev = true,
            Config = new Dictionary<string, object> { { "key", "value" } }
        };

        Assert.Equal("uid-1", model.UserId);
        Assert.Equal("testuser", model.UserName);
        Assert.Equal("test@test.com", model.EmailAddress);
        Assert.Equal("US", model.Country);
        Assert.Equal("Test User", model.FullName);
        Assert.Equal("555-0100", model.PhoneNumber);
        Assert.Equal("lenovoid", model.UserSource);
        Assert.True(model.IsB2BSupportMultDev);
        Assert.Single(model.Config);
    }
}
