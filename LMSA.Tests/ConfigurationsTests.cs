using lenovo.mbg.service.common.utilities;

namespace LMSA.Tests;

public class ConfigurationsTests
{
    [Fact]
    public void AdbPath_ReturnsValidPath()
    {
        string path = Configurations.AdbPath;
        Assert.NotNull(path);
        Assert.Contains("adb", path);
    }

    [Fact]
    public void FastbootPath_ReturnsValidPath()
    {
        string path = Configurations.FastbootPath;
        Assert.NotNull(path);
        Assert.Contains("fastboot", path);
    }

    [Fact]
    public void ProgramDataPath_ReturnsValidPath()
    {
        string path = Configurations.ProgramDataPath;
        Assert.NotNull(path);
        Assert.Contains("LMSA", path);
    }

    [Fact]
    public void FileSaveLocalPath_ContainsExpectedKeys()
    {
        var paths = Configurations.FileSaveLocalPath;
        Assert.NotNull(paths);
        Assert.True(paths.ContainsKey("ROM"));
        Assert.True(paths.ContainsKey("APK"));
        Assert.True(paths.ContainsKey("TOOL"));
        Assert.True(paths.ContainsKey("ICON"));
        Assert.True(paths.ContainsKey("COUNTRYCODE"));
        Assert.True(paths.ContainsKey("JSON"));
        Assert.True(paths.ContainsKey("BANNER"));
        Assert.True(paths.ContainsKey("XAML"));
        Assert.True(paths.ContainsKey("UNKNOWN"));
    }

    [Fact]
    public void RescueResultMap_InitializesWithZeros()
    {
        Configurations.ResetRescueResultMap();
        var map = Configurations.RescueResultMap;
        Assert.Equal(0, map[true]);
        Assert.Equal(0, map[false]);
    }

    [Fact]
    public void AddRescueResult_IncrementsCorrectly()
    {
        Configurations.ResetRescueResultMap();
        Configurations.AddRescueResult(true);
        Configurations.AddRescueResult(true);
        Configurations.AddRescueResult(false);

        Assert.Equal(2, Configurations.RescueResultMap[true]);
        Assert.Equal(1, Configurations.RescueResultMap[false]);
    }

    [Fact]
    public void TRANSFER_FILE_MAX_SIZE_Is4GB()
    {
        Assert.Equal(4294967296L, Configurations.TRANSFER_FILE_MAX_SIZE);
    }

    [Fact]
    public void SevenZipDllPath_ContainsCorrectBitness()
    {
        string path = Configurations.SevenZipDllPath;
        Assert.NotNull(path);
        if (Environment.Is64BitProcess)
        {
            Assert.Contains("7z64.dll", path);
        }
        else
        {
            Assert.Contains("7z.dll", path);
        }
    }

    [Fact]
    public void DownloadPath_CanBeSetAndGet()
    {
        string original = Configurations.DownloadPath;
        try
        {
            Configurations.DownloadPath = "/tmp/test_downloads";
            Assert.Equal("/tmp/test_downloads", Configurations.DownloadPath);
        }
        finally
        {
            Configurations.DownloadPath = original;
        }
    }
}
