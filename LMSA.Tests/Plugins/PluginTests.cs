using System;
using System.IO;
using lenovo.mbg.service.lmsa.plugins;
using Xunit;
using FluentAssertions;

namespace LMSA.Tests.Plugins
{
    public class TestPlugin : IPlugin
    {
        public string PluginID => "test-plugin-001";
        public string Name => "Test Plugin";
        public string Description => "A test plugin for unit testing";
        public string Version => "1.0.0";
        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            IsInitialized = true;
        }

        public void Shutdown()
        {
            IsInitialized = false;
        }
    }

    public class PluginLoaderTests
    {
        [Fact]
        public void Constructor_ThrowsOnNullDirectory()
        {
            var act = () => new PluginLoader(null!);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void LoadAll_HandlesNonExistentDirectory()
        {
            var loader = new PluginLoader("/nonexistent/path");
            loader.LoadAll();
            loader.LoadedPlugins.Should().BeEmpty();
        }

        [Fact]
        public void LoadPlugin_HandlesNonExistentDll()
        {
            var loader = new PluginLoader("/tmp");
            var plugin = loader.LoadPlugin("/nonexistent/plugin.dll");
            plugin.Should().BeNull();
        }

        [Fact]
        public void GetPlugin_ReturnsNullForUnknownId()
        {
            var loader = new PluginLoader("/tmp");
            var plugin = loader.GetPlugin("unknown-id");
            plugin.Should().BeNull();
        }

        [Fact]
        public void UnloadPlugin_ReturnsFalseForUnknownId()
        {
            var loader = new PluginLoader("/tmp");
            loader.UnloadPlugin("unknown-id").Should().BeFalse();
        }
    }

    public class PluginInterfaceTests
    {
        [Fact]
        public void TestPlugin_ImplementsInterface()
        {
            var plugin = new TestPlugin();

            plugin.PluginID.Should().Be("test-plugin-001");
            plugin.Name.Should().Be("Test Plugin");
            plugin.Description.Should().NotBeEmpty();
            plugin.Version.Should().Be("1.0.0");
            plugin.IsInitialized.Should().BeFalse();
        }

        [Fact]
        public void TestPlugin_InitializeAndShutdown()
        {
            var plugin = new TestPlugin();

            plugin.Initialize();
            plugin.IsInitialized.Should().BeTrue();

            plugin.Shutdown();
            plugin.IsInitialized.Should().BeFalse();
        }
    }
}
