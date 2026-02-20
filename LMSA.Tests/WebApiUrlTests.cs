using lenovo.mbg.service.common.webservices;
using Xunit;

namespace LMSA.Tests;

public class WebApiUrlTests
{
    [Fact]
    public void AllEndpoints_HaveCorrectBaseUrl()
    {
        Assert.StartsWith("https://lsa.lenovo.com/Interface", WebApiUrl.GET_PUBLIC_KEY);
        Assert.StartsWith("https://lsa.lenovo.com/Interface", WebApiUrl.INIT_TOKEN);
        Assert.StartsWith("https://lsa.lenovo.com/Interface", WebApiUrl.USER_LOGIN);
        Assert.StartsWith("https://lsa.lenovo.com/Interface", WebApiUrl.CALL_B2B_ORDERS_URL);
        Assert.StartsWith("https://lsa.lenovo.com/Interface", WebApiUrl.CALL_B2B_QUERY_ORDER_URL);
    }

    [Fact]
    public void B2BEndpoints_HaveCorrectPaths()
    {
        Assert.Contains("/vip/getB2BInfo.jhtml", WebApiUrl.CALL_B2B_ORDERS_URL);
        Assert.Contains("/vip/getActiveB2BInfos.jhtml", WebApiUrl.CALL_B2B_ACTIVE_ORDERS_URL);
        Assert.Contains("/vip/getEnableB2BOrder.jhtml", WebApiUrl.CALL_B2B_QUERY_ORDER_URL);
        Assert.Contains("/vip/getOrderNum.jhtml", WebApiUrl.CALL_B2B_GET_ORDERID_URL);
        Assert.Contains("/vip/buy.jhtml", WebApiUrl.CALL_B2B_ORDER_BUY_URL);
        Assert.Contains("/vip/card.jhtml", WebApiUrl.CALL_B2B_GET_PRICE_URL);
    }

    [Fact]
    public void RescueEndpoints_HaveCorrectPaths()
    {
        Assert.Contains("/rescueDevice/getParamType.jhtml", WebApiUrl.GET_UPGRADEFLASH_MATCH_TYPES);
        Assert.Contains("/rescueDevice/getRomMatchParams.jhtml", WebApiUrl.RESUCE_AUTOMATCH_GETPARAMS_MAPPING);
        Assert.Contains("/rescueDevice/getNewResource.jhtml", WebApiUrl.RESUCE_AUTOMATCH_GETROM);
        Assert.Contains("/rescueDevice/getRescueModelRecipe.jhtml", WebApiUrl.GET_FASTBOOTDATA_RECIPE);
    }

    [Fact]
    public void OriginalMisspellings_ArePreserved()
    {
        // Webwervice (not WebService) - intentional misspelling from original
        Assert.Contains("/priv/getRomList.jhtml", WebApiUrl.Webwervice_Get_RomResources);
        // RESUCE (not RESCUE) - intentional misspelling from original
        Assert.Contains("/rescueDevice/", WebApiUrl.RESUCE_AUTOMATCH_GETROM);
    }

    [Fact]
    public void NetworkCheckUrl_IsCorrect()
    {
        Assert.Equal("https://lsa.lenovo.com/lmsa-web/index.jsp", WebApiUrl.NETWORK_CONNECT_CHECK);
    }

    [Fact]
    public void LenovoIdFormat_ContainsPlaceholder()
    {
        Assert.Contains("{0}", WebApiUrl.FORMAT_LENOVOID_ACCOUNT);
    }
}
