using FluentAssertions;
using lenovo.mbg.service.lmsa.plugin;
using System;
using Xunit;

namespace LMSA.Tests.Plugins
{
    public class PluginLoaderTests
    {
        [Fact]
        public void PluginLoader_DefaultConstructor_UsesDefaultPluginsDirectory()
        {
            // Act - constructor should not throw
            FluentActions.Invoking(() => new PluginLoader())
                .Should().NotThrow();
        }

        [Fact]
        public void PluginLoader_CustomDirectory_AcceptsPath()
        {
            var loader = new PluginLoader("/tmp/plugins");
            loader.Should().NotBeNull();
        }

        [Fact]
        public void PluginLoader_NullDirectory_ThrowsArgumentNullException()
        {
            FluentActions.Invoking(() => new PluginLoader(null!))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void LoadAll_WhenDirectoryNotExists_ReturnsZero()
        {
            // Arrange
            var loader = new PluginLoader("/nonexistent/plugins/path");

            // Act
            int count = loader.LoadAll();

            // Assert
            count.Should().Be(0);
        }

        [Fact]
        public void LoadedPlugins_AfterLoadAll_IsEmptyWhenNoPlugins()
        {
            var loader = new PluginLoader("/tmp/empty_plugins_dir");
            loader.LoadAll();
            loader.LoadedPlugins.Should().BeEmpty();
        }

        [Fact]
        public void GetPlugin_NonExistentId_ReturnsNull()
        {
            var loader = new PluginLoader("/tmp/plugins");
            var plugin = loader.GetPlugin("nonexistent-id");
            plugin.Should().BeNull();
        }

        [Fact]
        public void UnloadAll_WhenNoPluginsLoaded_DoesNotThrow()
        {
            var loader = new PluginLoader("/tmp/plugins");
            FluentActions.Invoking(() => loader.UnloadAll())
                .Should().NotThrow();
        }
    }
}
