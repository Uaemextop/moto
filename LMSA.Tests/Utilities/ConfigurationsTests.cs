using FluentAssertions;
using lenovo.mbg.service.common.utilities;
using Xunit;

namespace LMSA.Tests.Utilities;

public class ConfigurationsTests
{
    [Fact]
    public void AdbPath_ContainsAdb()
    {
        Configurations.AdbPath.Should().Contain("adb");
    }

    [Fact]
    public void FastbootPath_ContainsFastboot()
    {
        Configurations.FastbootPath.Should().Contain("fastboot");
    }

    [Fact]
    public void ProgramDataPath_ContainsLMSA()
    {
        Configurations.ProgramDataPath.Should().Contain("LMSA");
    }

    [Fact]
    public void SevenZipDllPath_Contains7z()
    {
        Configurations.SevenZipDllPath.Should().Contain("7z");
    }
}
