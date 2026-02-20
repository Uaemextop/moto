using FluentAssertions;
using lenovo.mbg.service.framework.common;
using Xunit;

namespace LMSA.Tests.Core
{
    public class ConfigurationsTests
    {
        [Fact]
        public void AdbPath_ReturnsNonEmpty()
        {
            Configurations.AdbPath.Should().NotBeNullOrEmpty();
            Configurations.AdbPath.Should().EndWith("adb.exe");
        }

        [Fact]
        public void FastbootPath_ReturnsNonEmpty()
        {
            Configurations.FastbootPath.Should().NotBeNullOrEmpty();
            Configurations.FastbootPath.Should().EndWith("fastboot.exe");
        }

        [Fact]
        public void PluginsDirectory_IsAbsolutePath()
        {
            string pluginsDir = Configurations.PluginsDirectory;
            pluginsDir.Should().NotBeNullOrEmpty();
            System.IO.Path.IsPathRooted(pluginsDir).Should().BeTrue();
        }

        [Fact]
        public void AdbServerPort_IsStandardPort()
        {
            Configurations.AdbServerPort.Should().Be(5037);
        }

        [Fact]
        public void DefaultTimeoutMs_IsPositive()
        {
            Configurations.DefaultTimeoutMs.Should().BePositive();
        }
    }
}
