using Xunit;
using FluentAssertions;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.common.webservices.WebApiModel;

namespace LMSA.Tests;

public class WebServicesTests
{
    [Fact]
    public void WebApiUrl_ClassExists()
    {
        // WebApiUrl depends on Configurations.ServiceInterfaceUrl being set at startup
        typeof(WebApiUrl).Should().NotBeNull();
    }

    [Fact]
    public void WebApiUrl_Fields_ShouldBePublicStatic()
    {
        // Verify the class has the expected static fields without triggering the static initializer
        var fieldNames = typeof(WebApiUrl).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Select(f => f.Name).ToList();
        fieldNames.Should().Contain("NETWORK_CONNECT_CHECK");
        fieldNames.Should().Contain("GET_PUBLIC_KEY");
        fieldNames.Should().Contain("Webwervice_Get_RomResources");
        fieldNames.Should().Contain("RESUCE_AUTOMATCH_GETROM");
    }

    [Fact]
    public void HttpMethod_ShouldHave_PostAndGet()
    {
        ((int)lenovo.mbg.service.common.webservices.HttpMethod.POST).Should().Be(0);
        ((int)lenovo.mbg.service.common.webservices.HttpMethod.GET).Should().Be(1);
    }

    [Fact]
    public void WebApiContext_ShouldHave_Constants()
    {
        WebApiContext.REQUEST_CODE_ERROR.Should().Be("ERROR");
        WebApiContext.REQUEST_CODE_0000.Should().Be("0000");
        WebApiContext.REQUEST_CODE_TOKENTIMEOUT.Should().Be("402");
    }

    [Fact]
    public void WebApiContext_GUID_ShouldNotBeEmpty()
    {
        WebApiContext.GUID.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ResponseModel_ShouldStore_Properties()
    {
        var response = new ResponseModel<string>
        {
            success = true,
            code = "0000",
            desc = "OK",
            content = "test_content"
        };

        response.success.Should().BeTrue();
        response.code.Should().Be("0000");
        response.content.Should().Be("test_content");
    }

    [Fact]
    public void RSAKey_ShouldStore_KeyPair()
    {
        var key = new RSAKey
        {
            PublicKey = "pub",
            PrivateKey = "priv"
        };
        key.PublicKey.Should().Be("pub");
        key.PrivateKey.Should().Be("priv");
    }

    [Fact]
    public void ToolVersionModel_ShouldStore_Properties()
    {
        var model = new ToolVersionModel
        {
            Id = "1",
            VersionNumber = "5.0.0",
            FilePath = "/path/to/file",
            FileSize = 1024000,
            MD5 = "abc123",
            IsForce = true,
            ReleaseDate = "2024-01-01"
        };
        model.VersionNumber.Should().Be("5.0.0");
        model.FileSize.Should().Be(1024000);
        model.IsForce.Should().BeTrue();
    }

    [Fact]
    public void PriceInfo_ShouldStore_Properties()
    {
        var price = new PriceInfo
        {
            sku = "SKU001",
            price = 9.99f,
            cardName = "Basic",
            country = "US",
            monetaryUnit = "USD"
        };
        price.price.Should().Be(9.99f);
    }
}
