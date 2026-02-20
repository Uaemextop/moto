using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.common.webservices.WebApiModel;

namespace LMSA.Tests;

public class WebApiTests
{
    [Fact]
    public void HttpMethod_HasCorrectValues()
    {
        Assert.Equal(0, (int)lenovo.mbg.service.common.webservices.HttpMethod.POST);
        Assert.Equal(1, (int)lenovo.mbg.service.common.webservices.HttpMethod.GET);
    }

    [Fact]
    public void WebApiContext_GUID_IsNotEmpty()
    {
        Assert.NotNull(WebApiContext.GUID);
        Assert.NotEmpty(WebApiContext.GUID);
    }

    [Fact]
    public void WebApiContext_GUID_IsValidFormat()
    {
        Assert.True(Guid.TryParse(WebApiContext.GUID, out _));
    }

    [Fact]
    public void WebApiContext_Constants_AreCorrect()
    {
        Assert.Equal("ERROR", WebApiContext.REQUEST_CODE_ERROR);
        Assert.Equal("0000", WebApiContext.REQUEST_CODE_0000);
        Assert.Equal("3010", WebApiContext.REQUEST_CODE_3010);
        Assert.Equal("402", WebApiContext.REQUEST_CODE_TOKENTIMEOUT);
    }

    [Fact]
    public void WebApiContext_REQUEST_AUTHOR_HEADERS_ContainsGuid()
    {
        var headers = WebApiContext.REQUEST_AUTHOR_HEADERS;
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("guid"));
        Assert.Equal(WebApiContext.GUID, headers["guid"]);
    }

    [Fact]
    public void WebApiContext_LANGUAGE_ReturnsNonNull()
    {
        string lang = WebApiContext.LANGUAGE;
        Assert.NotNull(lang);
    }

    [Fact]
    public void WebApiContext_CLIENT_VERSION_ReturnsNonNull()
    {
        string version = WebApiContext.CLIENT_VERSION;
        Assert.NotNull(version);
    }

    [Fact]
    public void WebApiUrl_HasAllEndpoints()
    {
        Assert.Contains("/common/rsa.jhtml", WebApiUrl.GET_PUBLIC_KEY);
        Assert.Contains("/client/initToken.jhtml", WebApiUrl.INIT_TOKEN);
        Assert.Contains("/device/getDeviceInfo.jhtml", WebApiUrl.GET_DEVICE_INFO);
        Assert.Contains("/device/getDeviceIcon.jhtml", WebApiUrl.GET_DEVICE_ICON);
        Assert.Contains("/rescueDevice/getNewResource.jhtml", WebApiUrl.RESUCE_AUTOMATCH_GETROM);
        Assert.Contains("/user/login.jhtml", WebApiUrl.USER_LOGIN);
        Assert.Contains("/user/logout.jhtml", WebApiUrl.USER_LOGOUT);
    }

    [Fact]
    public void WebApiUrl_NETWORK_CONNECT_CHECK_IsAbsoluteUrl()
    {
        Assert.StartsWith("https://", WebApiUrl.NETWORK_CONNECT_CHECK);
    }

    [Fact]
    public void ResponseModel_CanSetProperties()
    {
        var model = new ResponseModel<string>();
        model.success = true;
        model.code = "0000";
        model.desc = "OK";
        model.content = "test content";

        Assert.True(model.success);
        Assert.Equal("0000", model.code);
        Assert.Equal("OK", model.desc);
        Assert.Equal("test content", model.content);
    }

    [Fact]
    public void ResponseModel_DefaultValues()
    {
        var model = new ResponseModel<object>();
        Assert.False(model.success);
        Assert.Null(model.code);
        Assert.Null(model.desc);
        Assert.Null(model.content);
    }

    [Fact]
    public void RequestModel_SetsDefaultValues()
    {
        var model = new RequestModel(new { name = "test" });

        Assert.NotNull(model.language);
        Assert.NotNull(model.client);
        Assert.True(model.client.ContainsKey("version"));
        Assert.NotNull(model.dparams);
    }

    [Fact]
    public void RequestModel_ToString_ReturnsJson()
    {
        var model = new RequestModel(new { name = "test" });
        string json = model.ToString();

        Assert.NotNull(json);
        Assert.Contains("language", json);
        Assert.Contains("client", json);
        Assert.Contains("dparams", json);
    }

    [Fact]
    public void BaseRequestModel_ToString_ReturnsJson()
    {
        var model = new BaseRequestModel();
        string json = model.ToString();

        Assert.NotNull(json);
        Assert.Equal("{}", json);
    }
}
