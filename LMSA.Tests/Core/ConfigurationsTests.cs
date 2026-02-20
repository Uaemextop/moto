using lenovo.mbg.service.common.utilities;

namespace LMSA.Tests.Core;

public class ConfigurationsTests
{
    [Fact]
    public void AdbPath_ContainsAdbExe()
    {
        string path = Configurations.AdbPath;
        Assert.Contains("adb.exe", path);
    }

    [Fact]
    public void FastbootPath_ContainsFastbootExe()
    {
        string path = Configurations.FastbootPath;
        Assert.Contains("fastboot.exe", path);
    }

    [Fact]
    public void ProgramDataPath_IsNotEmpty()
    {
        string path = Configurations.ProgramDataPath;
        Assert.False(string.IsNullOrEmpty(path));
        Assert.Contains("LMSA", path);
    }

    [Fact]
    public void RescueResultMap_InitializedWithZeroCounts()
    {
        Configurations.ResetRescueResultMap();
        var map = Configurations.RescueResultMap;

        Assert.Equal(0, map[true]);
        Assert.Equal(0, map[false]);
    }

    [Fact]
    public void AddRescueResult_IncrementsCounts()
    {
        Configurations.ResetRescueResultMap();

        Configurations.AddRescueResult(true);
        Configurations.AddRescueResult(true);
        Configurations.AddRescueResult(false);

        Assert.Equal(2, Configurations.RescueResultMap[true]);
        Assert.Equal(1, Configurations.RescueResultMap[false]);
    }

    [Fact]
    public void ResetRescueResultMap_ResetsCounts()
    {
        Configurations.AddRescueResult(true);
        Configurations.ResetRescueResultMap();

        Assert.Equal(0, Configurations.RescueResultMap[true]);
        Assert.Equal(0, Configurations.RescueResultMap[false]);
    }

    [Fact]
    public void SevenZipDllPath_ContainsSevenZip()
    {
        string path = Configurations.SevenZipDllPath;
        Assert.Contains("7z", path);
    }

    [Fact]
    public void TRANSFER_FILE_MAX_SIZE_Is4GB()
    {
        Assert.Equal(4294967296L, Configurations.TRANSFER_FILE_MAX_SIZE);
    }
}
