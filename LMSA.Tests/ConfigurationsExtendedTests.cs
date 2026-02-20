using lenovo.mbg.service.common.utilities;

namespace LMSA.Tests;

public class ConfigurationsExtendedTests
{
    [Fact]
    public void ServiceBaseUrl_ReturnsDefaultLenovoUrl()
    {
        string url = Configurations.ServiceBaseUrl;
        Assert.NotNull(url);
        Assert.Equal("https://lsa.lenovo.com", url);
    }

    [Fact]
    public void ServiceInterfaceUrl_AppendsInterface()
    {
        string url = Configurations.ServiceInterfaceUrl;
        Assert.NotNull(url);
        Assert.EndsWith("/Interface", url);
    }

    [Fact]
    public void BaseHttpUrl_ReturnsNonNull()
    {
        string url = Configurations.BaseHttpUrl;
        Assert.NotNull(url);
    }

    [Fact]
    public void IsReleaseVersion_TrueForDefaultUrl()
    {
        Assert.True(Configurations.IsReleaseVersion);
    }

    [Fact]
    public void OsVersionName_ReturnsNonNull()
    {
        string name = LMSAContext.OsVersionName;
        Assert.NotNull(name);
        Assert.NotEqual("unknown", name);
    }
}
