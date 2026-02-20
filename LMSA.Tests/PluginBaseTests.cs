using lenovo.mbg.service.framework.services;
using Xunit;

namespace LMSA.Tests;

/// <summary>
/// Unit tests for the plugin framework.
/// </summary>
public class PluginBaseTests
{
    private class TestPlugin : PluginBase
    {
        private bool _initCalled;

        public bool InitCalled => _initCalled;

        public override void Init()
        {
            _initCalled = true;
        }

        public override object CreateControl(IMessageBox iMessage)
        {
            return new object();
        }
    }

    private class TestMessageBox : IMessageBox
    {
        public string? LastMessage { get; private set; }
        public bool LastConfirmResult { get; set; } = true;

        public void Show(string message) => LastMessage = message;

        public bool Confirm(string message)
        {
            LastMessage = message;
            return LastConfirmResult;
        }
    }

    [Fact]
    public void PluginBase_Init_IsCalledCorrectly()
    {
        // Arrange
        var plugin = new TestPlugin();

        // Act
        plugin.Init();

        // Assert
        Assert.True(plugin.InitCalled);
    }

    [Fact]
    public void PluginBase_CanClose_DefaultsToTrue()
    {
        // Arrange
        var plugin = new TestPlugin();

        // Assert
        Assert.True(plugin.CanClose());
    }

    [Fact]
    public void PluginBase_IsExecuteWork_DefaultsToFalse()
    {
        // Arrange
        var plugin = new TestPlugin();

        // Assert
        Assert.False(plugin.IsExecuteWork());
    }

    [Fact]
    public void PluginBase_IsNonBusinessPage_DefaultsToTrue()
    {
        // Arrange
        var plugin = new TestPlugin();

        // Assert
        Assert.True(plugin.IsNonBusinessPage());
    }

    [Fact]
    public void PluginBase_CreateControl_ReturnsNonNull()
    {
        // Arrange
        var plugin = new TestPlugin();
        var msgBox = new TestMessageBox();

        // Act
        object control = plugin.CreateControl(msgBox);

        // Assert
        Assert.NotNull(control);
    }

    [Fact]
    public void PluginBase_GetService_WithMatchingType_ReturnsSelf()
    {
        // Arrange
        var plugin = new TestPlugin();

        // Act
        object? service = plugin.GetService(typeof(TestPlugin));

        // Assert
        Assert.Same(plugin, service);
    }

    [Fact]
    public void PluginBase_GetService_WithNonMatchingType_ReturnsNull()
    {
        // Arrange
        var plugin = new TestPlugin();

        // Act
        object? service = plugin.GetService(typeof(string));

        // Assert
        Assert.Null(service);
    }
}
