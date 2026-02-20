using lenovo.mbg.service.framework.services.Model;

namespace LMSA.Tests;

public class BusinessDataTests
{
    [Fact]
    public void DefaultConstructor_InitializesEmpty()
    {
        var data = new BusinessData();

        Assert.Equal(string.Empty, data.appType);
        Assert.Equal(string.Empty, data.connectType);
        Assert.Equal(0L, data.cycleTime);
    }

    [Fact]
    public void BusinessTypeConstructor_SetsUseCaseStep()
    {
        var data = new BusinessData(BusinessType.HOME, null);

        Assert.Equal("HOME", data.useCaseStep);
        Assert.Equal(BusinessStatus.CLICK, data.status);
    }

    [Fact]
    public void Update_SetsTimeAndStatus()
    {
        var data = new BusinessData(BusinessType.APP, null);
        data.Update(5000L, BusinessStatus.SUCCESS, "extra data");

        Assert.Equal(5L, data.cycleTime);
        Assert.Equal(BusinessStatus.SUCCESS, data.status);
    }

    [Fact]
    public void Update_WithModelName_SetsModelName()
    {
        var data = new BusinessData(BusinessType.RESCUE_IMEI_SEARCH, null);
        data.Update(1000L, BusinessStatus.SUCCESS, "Moto G", null);

        Assert.Equal("Moto G", data.modelName);
    }

    [Fact]
    public void Clone_CreatesExactCopy()
    {
        var original = new BusinessData
        {
            appType = "Ma",
            connectType = "Adb",
            androidVersion = "13",
            modelName = "test_model",
            useCaseStep = "HOME",
            cycleTime = 100,
            status = BusinessStatus.SUCCESS,
            clientDate = "2026-01-01 00:00:00"
        };

        var clone = BusinessData.Clone(original);

        Assert.Equal(original.appType, clone.appType);
        Assert.Equal(original.connectType, clone.connectType);
        Assert.Equal(original.androidVersion, clone.androidVersion);
        Assert.Equal(original.modelName, clone.modelName);
        Assert.Equal(original.useCaseStep, clone.useCaseStep);
        Assert.Equal(original.cycleTime, clone.cycleTime);
        Assert.Equal(original.status, clone.status);
        Assert.Equal(original.clientDate, clone.clientDate);
    }
}
