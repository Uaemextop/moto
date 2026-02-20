using lenovo.mbg.service.framework.services.Model;

namespace LMSA.Tests;

public class BusinessModelTests
{
    [Fact]
    public void BusinessType_HasExpectedRescueValues()
    {
        Assert.Equal(3010, (int)BusinessType.RESCUE_IMEI_SEARCH);
        Assert.Equal(3030, (int)BusinessType.RESCUE_MANUAL_PHONE_FLASH);
        Assert.Equal(3050, (int)BusinessType.RESCUE_AUTO_PHONE_FLASH);
    }

    [Fact]
    public void BusinessType_HasExpectedHomeValue()
    {
        Assert.Equal(1000, (int)BusinessType.HOME);
    }

    [Fact]
    public void BusinessStatus_HasCorrectValues()
    {
        Assert.Equal(0, (int)BusinessStatus.CLICK);
        Assert.Equal(10, (int)BusinessStatus.SUCCESS);
        Assert.Equal(20, (int)BusinessStatus.FALIED);
        Assert.Equal(30, (int)BusinessStatus.QUIT);
    }

    [Fact]
    public void BusinessModel_CanSetFields()
    {
        var model = new BusinessModel();
        model.business = BusinessType.HOME;
        model.businessName = "Home";

        Assert.Equal(BusinessType.HOME, model.business);
        Assert.Equal("Home", model.businessName);
    }
}
