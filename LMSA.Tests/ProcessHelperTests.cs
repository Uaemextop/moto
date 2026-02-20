using lenovo.mbg.service.framework.devicemgt.DeviceOperator;

namespace LMSA.Tests;

public class ProcessHelperTests
{
    [Fact]
    public void Instance_ReturnsSingleton()
    {
        var instance1 = ProcessHelper.Instance;
        var instance2 = ProcessHelper.Instance;

        Assert.NotNull(instance1);
        Assert.Same(instance1, instance2);
    }

    [Fact]
    public void Do_ExecutesEchoCommand()
    {
        var helper = ProcessHelper.Instance;
        string result = helper.Do("/bin/echo", "hello world", 5000);

        Assert.Contains("hello world", result);
    }
}
