using lenovo.mbg.service.framework.devicemgt.DeviceOperator;

namespace LMSA.Tests;

public class AdbOperatorTests
{
    [Fact]
    public void Instance_ReturnsSingleton()
    {
        var instance1 = AdbOperator.Instance;
        var instance2 = AdbOperator.Instance;

        Assert.NotNull(instance1);
        Assert.Same(instance1, instance2);
    }
}
