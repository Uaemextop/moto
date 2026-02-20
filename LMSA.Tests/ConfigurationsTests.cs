using lenovo.mbg.service.common.utilities;

namespace LMSA.Tests;

public class ConfigurationsTests
{
    [Fact]
    public void AdbPath_EndsWithAdbExe()
    {
        Assert.EndsWith("adb.exe", Configurations.AdbPath);
    }

    [Fact]
    public void FastbootPath_EndsWithFastbootExe()
    {
        Assert.EndsWith("fastboot.exe", Configurations.FastbootPath);
    }

    [Fact]
    public void ProgramDataPath_ContainsLMSA()
    {
        Assert.Contains("LMSA", Configurations.ProgramDataPath);
    }

    [Fact]
    public void FileSaveLocalPath_ContainsExpectedKeys()
    {
        var paths = Configurations.FileSaveLocalPath;

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
    public void RescueResultMap_InitializedWithZeros()
    {
        Configurations.ResetRescueResultMap();
        var map = Configurations.RescueResultMap;

        Assert.Equal(0, map[true]);
        Assert.Equal(0, map[false]);
    }

    [Fact]
    public void AddRescueResult_IncrementsCount()
    {
        Configurations.ResetRescueResultMap();
        Configurations.AddRescueResult(true);
        Configurations.AddRescueResult(true);
        Configurations.AddRescueResult(false);

        Assert.Equal(2, Configurations.RescueResultMap[true]);
        Assert.Equal(1, Configurations.RescueResultMap[false]);
    }

    [Fact]
    public void SevenZipDllPath_ContainsSevenZ()
    {
        Assert.Contains("7z", Configurations.SevenZipDllPath);
    }

    [Fact]
    public void DownloadPath_HasDefault()
    {
        Assert.NotNull(Configurations.DownloadPath);
        Assert.NotEmpty(Configurations.DownloadPath);
    }

    [Fact]
    public void ToolPath_DerivedFromDownloadPath()
    {
        Assert.StartsWith(Configurations.DownloadPath, Configurations.ToolPath);
    }
}
