using SharpAdbClient;

namespace LMSA.Tests;

public class AdbClientWrapperTests
{
    [Fact]
    public void Constructor_WithInjectedClient_UsesInjectedInstance()
    {
        var client = new AdbClient();

        var wrapper = new lenovo.mbg.service.framework.devicemgt.AdbClientWrapper(client);

        Assert.Same(client, wrapper.Client);
    }

    [Fact]
    public void Constructor_WithoutInjectedClient_UsesSharpAdbClientSingleton()
    {
        var wrapper = new lenovo.mbg.service.framework.devicemgt.AdbClientWrapper();

        Assert.NotNull(wrapper.Client);
    }
}
