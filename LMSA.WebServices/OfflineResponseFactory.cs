using System;
using System.Collections.Generic;
using lenovo.mbg.service.common.webservices.WebApiModel;
using lenovo.mbg.service.lmsa.core;
using lenovo.mbg.service.lmsa.Login.Protocol;

namespace lenovo.mbg.service.common.webservices;

/// <summary>
/// Factory that creates default response objects for offline/standalone operation.
/// When the application operates without server connectivity, these defaults
/// ensure full functionality.
/// </summary>
public static class OfflineResponseFactory
{
    /// <summary>
    /// Creates a default RespOrders for offline operation with full feature access.
    /// </summary>
    public static RespOrders CreateDefaultOrders(string username = "local-user", string email = "")
    {
        var defaultOrder = CreateDefaultOrderItem();
        return new RespOrders
        {
            id = "offline",
            username = username,
            email = email,
            vipName = "OFFLINE",
            freeAmount = int.MaxValue,
            usedFreeAmount = 0,
            multiDevice = true,
            enableOrderDtos = new List<OrderItem> { defaultOrder },
            unableOrderDtos = new List<OrderItem>(),
            popUp = false,
            popMode = -1
        };
    }

    /// <summary>
    /// Creates a default OrderItem for offline operation that is enabled and displayed.
    /// </summary>
    public static OrderItem CreateDefaultOrderItem()
    {
        long farFuture = new DateTimeOffset(2099, 12, 31, 23, 59, 59, TimeSpan.Zero).ToUnixTimeMilliseconds();
        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        return new OrderItem
        {
            orderId = 1,
            rid = "offline-rid",
            orderStatus = "ACTIVE",
            type = "OFFLINE",
            usingDate = now,
            purchaseAmount = 0,
            effectiveDate = now,
            expiredDate = farFuture,
            imeiCount = int.MaxValue,
            imeiUsedCount = 0,
            macAddressRsa = string.Empty,
            unit = "OFFLINE",
            display = true,
            refund = false,
            enable = true,
            orderLevelDesc = "Offline Mode (Open Source)",
            serverOrderStatus = "ACTIVE",
            imeiDtos = new List<FlashedDevModel>()
        };
    }

    /// <summary>
    /// Creates a default B2BUserInfo for offline operation.
    /// </summary>
    public static B2BUserInfo CreateDefaultB2BUserInfo()
    {
        return new B2BUserInfo
        {
            B2bMode = "OFFLINE",
            IsMultiDev = true,
            B2bButtonDisplay = true
        };
    }

    /// <summary>
    /// Applies offline defaults to a server response.
    /// If FeatureConfiguration.OfflineMode is true, ensures the response
    /// has full feature access for standalone operation.
    /// Returns the response with defaults applied, or creates a new default if null.
    /// </summary>
    public static RespOrders ApplyOfflineDefaults(RespOrders response)
    {
        if (!FeatureConfiguration.OfflineMode)
            return response;

        if (response == null)
            return CreateDefaultOrders();

        response.enableOrderDtos ??= new List<OrderItem>();
        if (response.enableOrderDtos.Count == 0)
            response.enableOrderDtos.Add(CreateDefaultOrderItem());

        if (FeatureConfiguration.MultiDeviceSupport)
            response.multiDevice = true;

        if (FeatureConfiguration.SuppressB2BPopups)
        {
            response.popUp = false;
            response.popMode = -1;
        }

        if (FeatureConfiguration.DefaultOrdersEnabled)
        {
            foreach (var order in response.enableOrderDtos)
            {
                order.enable = true;
                order.display = true;
            }
        }

        response.freeAmount = int.MaxValue;
        response.usedFreeAmount = 0;

        return response;
    }
}
