using lenovo.mbg.service.framework.common;
using Xunit;
using FluentAssertions;

namespace LMSA.Tests.Core
{
    public class ConfigurationsTests
    {
        [Fact]
        public void AdbPath_ContainsAdbExe()
        {
            Configurations.AdbPath.Should().EndWith("adb.exe");
        }

        [Fact]
        public void FastbootPath_ContainsFastbootExe()
        {
            Configurations.FastbootPath.Should().EndWith("fastboot.exe");
        }

        [Fact]
        public void FastbootMonitorPath_ContainsFastbootMonitorExe()
        {
            Configurations.FastbootMonitorPath.Should().EndWith("fastbootmonitor.exe");
        }

        [Fact]
        public void BaseDirectory_CanBeOverridden()
        {
            string original = Configurations.BaseDirectory;
            try
            {
                Configurations.BaseDirectory = "/custom/path";
                Configurations.BaseDirectory.Should().Be("/custom/path");
                Configurations.AdbPath.Should().StartWith("/custom/path");
            }
            finally
            {
                Configurations.BaseDirectory = null;
            }
        }

        [Fact]
        public void Timeouts_HaveCorrectValues()
        {
            Configurations.Timeouts.Flash.Should().Be(300000);
            Configurations.Timeouts.Erase.Should().Be(60000);
            Configurations.Timeouts.FlashAll.Should().Be(600000);
            Configurations.Timeouts.Reboot.Should().Be(20000);
            Configurations.Timeouts.RebootBootloader.Should().Be(10000);
            Configurations.Timeouts.Continue.Should().Be(10000);
            Configurations.Timeouts.Standard.Should().Be(12000);
            Configurations.Timeouts.GetVar.Should().Be(20000);
            Configurations.Timeouts.Format.Should().Be(60000);
            Configurations.Timeouts.Oem.Should().Be(60000);
            Configurations.Timeouts.ShellCommand.Should().Be(20000);
        }

        [Fact]
        public void LogDirectory_IsSubdirectoryOfBase()
        {
            Configurations.LogDirectory.Should().Contain("logs");
        }

        [Fact]
        public void PluginsDirectory_IsSubdirectoryOfBase()
        {
            Configurations.PluginsDirectory.Should().Contain("plugins");
        }
    }

    public class ProcessRunnerTests
    {
        [Fact]
        public void ProcessString_ThrowsOnEmptyExecutablePath()
        {
            var runner = new ProcessRunner();
            var act = () => runner.ProcessString("", "args");
            act.Should().Throw<System.ArgumentException>();
        }

        [Fact]
        public void ProcessString_ThrowsOnNullArguments()
        {
            var runner = new ProcessRunner();
            var act = () => runner.ProcessString("/bin/echo", null!);
            act.Should().Throw<System.ArgumentNullException>();
        }

        [Fact]
        public void ProcessString_RejectsShellMetacharactersInPath()
        {
            var runner = new ProcessRunner();
            var act = () => runner.ProcessString("/bin/echo | rm -rf", "test");
            act.Should().Throw<System.ArgumentException>();
        }

        [Fact]
        public void ProcessString_ExecutesSimpleCommand()
        {
            var runner = new ProcessRunner();
            var result = runner.ProcessString("/usr/bin/echo", "hello world", 5000);
            result.Should().Contain("hello world");
            runner.LastExitCode.Should().Be(0);
        }

        [Fact]
        public void ProcessList_ExecutesSimpleCommand()
        {
            var runner = new ProcessRunner();
            var result = runner.ProcessList("/usr/bin/echo", "line1", 5000);
            result.Should().Contain("line1");
        }

        [Fact]
        public void ProcessList_ThrowsOnEmptyExecutablePath()
        {
            var runner = new ProcessRunner();
            var act = () => runner.ProcessList("", "args");
            act.Should().Throw<System.ArgumentException>();
        }

        [Fact]
        public void ProcessString_ReturnsEmptyOnNonexistentPath()
        {
            var runner = new ProcessRunner();
            var result = runner.ProcessString("/nonexistent/binary", "args", 1000);
            result.Should().BeEmpty();
            runner.LastExitCode.Should().Be(-1);
        }

        [Fact]
        public void ProcessString_RejectsAmpersandInPath()
        {
            var runner = new ProcessRunner();
            var act = () => runner.ProcessString("/bin/echo & /bin/rm", "test");
            act.Should().Throw<System.ArgumentException>();
        }

        [Fact]
        public void ProcessString_RejectsSemicolonInPath()
        {
            var runner = new ProcessRunner();
            var act = () => runner.ProcessString("/bin/echo; /bin/rm", "test");
            act.Should().Throw<System.ArgumentException>();
        }

        [Fact]
        public void ProcessString_RejectsBacktickInPath()
        {
            var runner = new ProcessRunner();
            var act = () => runner.ProcessString("/bin/echo`rm`", "test");
            act.Should().Throw<System.ArgumentException>();
        }
    }
}
