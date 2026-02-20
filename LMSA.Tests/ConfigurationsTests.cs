using lenovo.mbg.service.common.utilities;
using Xunit;

namespace LMSA.Tests;

/// <summary>
/// Unit tests for Configurations.
/// </summary>
public class ConfigurationsTests
{
    [Fact]
    public void AdbPath_ReturnsExpectedPath()
    {
        // Act
        string path = Configurations.AdbPath;

        // Assert
        Assert.NotNull(path);
        Assert.Contains("adb.exe", path);
    }

    [Fact]
    public void FastbootPath_ReturnsExpectedPath()
    {
        // Act
        string path = Configurations.FastbootPath;

        // Assert
        Assert.NotNull(path);
        Assert.Contains("fastboot.exe", path);
    }

    [Fact]
    public void ProgramDataPath_ContainsLMSA()
    {
        // Act
        string path = Configurations.ProgramDataPath;

        // Assert
        Assert.NotNull(path);
        Assert.Contains("LMSA", path);
    }

    [Fact]
    public void DownloadPath_IsConfigurable()
    {
        // Arrange
        string newPath = "/tmp/test-download";

        // Act
        Configurations.DownloadPath = newPath;

        // Assert
        Assert.Equal(newPath, Configurations.DownloadPath);
    }

    [Fact]
    public void AppVersionCode_CanBeSetAndGet()
    {
        // Arrange
        int expectedVersion = 42;

        // Act
        Configurations.AppVersionCode = expectedVersion;

        // Assert
        Assert.Equal(expectedVersion, Configurations.AppVersionCode);
    }
}
