using lenovo.mbg.service.framework.services;

namespace LMSA.Tests.PluginsCommon;

public class PluginTests
{
    private class TestPlugin : PluginBase
    {
        public bool InitCalled { get; private set; }

        public override void Init()
        {
            InitCalled = true;
        }
    }

    [Fact]
    public void PluginBase_CanClose_ReturnsTrue()
    {
        var plugin = new TestPlugin();
        Assert.True(plugin.CanClose());
    }

    [Fact]
    public void PluginBase_IsExecuteWork_ReturnsFalse()
    {
        var plugin = new TestPlugin();
        Assert.False(plugin.IsExecuteWork());
    }

    [Fact]
    public void PluginBase_IsNonBusinessPage_ReturnsTrue()
    {
        var plugin = new TestPlugin();
        Assert.True(plugin.IsNonBusinessPage());
    }

    [Fact]
    public void PluginBase_Init_CanBeOverridden()
    {
        var plugin = new TestPlugin();
        plugin.Init();
        Assert.True(plugin.InitCalled);
    }

    [Fact]
    public void PluginBase_GetService_ReturnsThis_WhenTypeMatches()
    {
        var plugin = new TestPlugin();
        var result = plugin.GetService(typeof(TestPlugin));
        Assert.Same(plugin, result);
    }

    [Fact]
    public void PluginBase_GetService_ReturnsNull_WhenTypeDoesNotMatch()
    {
        var plugin = new TestPlugin();
        var result = plugin.GetService(typeof(string));
        Assert.Null(result);
    }

    [Fact]
    public void PluginExportAttribute_StoresPluginId()
    {
        var attr = new PluginExportAttribute(typeof(IPlugin), "test-guid-123");

        Assert.Equal("test-guid-123", attr.PluginId);
        Assert.Equal(typeof(IPlugin), attr.ContractType);
    }
}
