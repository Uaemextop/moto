using lenovo.mbg.service.framework.services;

namespace LMSA.Tests;

public class ServiceFrameworkTests
{
    private class TestPlugin : PluginBase
    {
        public bool WasInitialized { get; private set; }

        public override void Init()
        {
            WasInitialized = true;
        }
    }

    [Fact]
    public void PluginBase_CanClose_ReturnsTrueByDefault()
    {
        var plugin = new TestPlugin();
        Assert.True(plugin.CanClose());
    }

    [Fact]
    public void PluginBase_IsExecuteWork_ReturnsFalseByDefault()
    {
        var plugin = new TestPlugin();
        Assert.False(plugin.IsExecuteWork());
    }

    [Fact]
    public void PluginBase_IsNonBusinessPage_ReturnsTrueByDefault()
    {
        var plugin = new TestPlugin();
        Assert.True(plugin.IsNonBusinessPage());
    }

    [Fact]
    public void PluginBase_Init_CanBeOverridden()
    {
        var plugin = new TestPlugin();
        plugin.Init();
        Assert.True(plugin.WasInitialized);
    }

    [Fact]
    public void PluginBase_GetService_ReturnsNullForUnrelatedType()
    {
        var plugin = new TestPlugin();
        var result = plugin.GetService(typeof(string));
        Assert.Null(result);
    }

    [Fact]
    public void PluginBase_GetService_ReturnsSelfForMatchingType()
    {
        var plugin = new TestPlugin();
        var result = plugin.GetService(typeof(TestPlugin));
        Assert.Same(plugin, result);
    }

    [Fact]
    public void PluginBase_OnSelected_DoesNotThrow()
    {
        var plugin = new TestPlugin();
        var exception = Record.Exception(() => plugin.OnSelected("test"));
        Assert.Null(exception);
    }

    [Fact]
    public void PluginBase_OnSelecting_DoesNotThrow()
    {
        var plugin = new TestPlugin();
        var exception = Record.Exception(() => plugin.OnSelecting("test"));
        Assert.Null(exception);
    }

    [Fact]
    public void PluginBase_OnInit_DoesNotThrow()
    {
        var plugin = new TestPlugin();
        var exception = Record.Exception(() => plugin.OnInit(null));
        Assert.Null(exception);
    }

    [Fact]
    public void PluginBase_Dispose_DoesNotThrow()
    {
        var plugin = new TestPlugin();
        var exception = Record.Exception(() => plugin.Dispose());
        Assert.Null(exception);
    }

    [Fact]
    public void NotifyEventProxy_CallsAction()
    {
        var proxy = new NotifyEventProxy();
        string receivedParam = null;
        proxy.OnNotifyLanguageChangeAction = (param) => receivedParam = param;

        proxy.CallNotifyLanguageChangeAction("en-US");

        Assert.Equal("en-US", receivedParam);
    }

    [Fact]
    public void NotifyEventProxy_NullAction_DoesNotThrow()
    {
        var proxy = new NotifyEventProxy();
        proxy.OnNotifyLanguageChangeAction = null;

        var exception = Record.Exception(() => proxy.CallNotifyLanguageChangeAction("test"));
        Assert.Null(exception);
    }

    [Fact]
    public void ServiceProviderUtil_GetService_ReturnsTypedService()
    {
        // ServiceProviderUtil is a static extension method for IServiceProvider
        // Testing it with a simple mock
        var provider = new SimpleServiceProvider();
        var result = provider.GetService<string>();
        Assert.Equal("test", result);
    }

    private class SimpleServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(string))
                return "test";
            return null;
        }
    }
}
