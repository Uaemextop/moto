using SharpAdbClient;

namespace lenovo.mbg.service.framework.devicemgt;

public sealed class AdbClientWrapper
{
    public AdbClientWrapper(IAdbClient? adbClient = null)
    {
        Client = adbClient ?? new AdbClient();
    }

    public IAdbClient Client { get; }
}
