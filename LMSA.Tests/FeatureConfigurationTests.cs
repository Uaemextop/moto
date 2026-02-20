using lenovo.mbg.service.lmsa.core;
using Xunit;

namespace LMSA.Tests;

public class FeatureConfigurationTests
{
    [Fact]
    public void OfflineMode_DefaultsToTrue()
    {
        Assert.True(FeatureConfiguration.OfflineMode);
    }

    [Fact]
    public void ShowDeveloperOptions_DefaultsToTrue()
    {
        Assert.True(FeatureConfiguration.ShowDeveloperOptions);
    }

    [Fact]
    public void SuppressB2BPopups_DefaultsToTrue()
    {
        Assert.True(FeatureConfiguration.SuppressB2BPopups);
    }

    [Fact]
    public void MultiDeviceSupport_DefaultsToTrue()
    {
        Assert.True(FeatureConfiguration.MultiDeviceSupport);
    }

    [Fact]
    public void DefaultOrdersEnabled_DefaultsToTrue()
    {
        Assert.True(FeatureConfiguration.DefaultOrdersEnabled);
    }

    [Fact]
    public void AllProperties_AreSettable()
    {
        var original = FeatureConfiguration.OfflineMode;
        try
        {
            FeatureConfiguration.OfflineMode = false;
            Assert.False(FeatureConfiguration.OfflineMode);

            FeatureConfiguration.ShowDeveloperOptions = false;
            Assert.False(FeatureConfiguration.ShowDeveloperOptions);

            FeatureConfiguration.SuppressB2BPopups = false;
            Assert.False(FeatureConfiguration.SuppressB2BPopups);

            FeatureConfiguration.MultiDeviceSupport = false;
            Assert.False(FeatureConfiguration.MultiDeviceSupport);

            FeatureConfiguration.DefaultOrdersEnabled = false;
            Assert.False(FeatureConfiguration.DefaultOrdersEnabled);
        }
        finally
        {
            // Reset to defaults
            FeatureConfiguration.OfflineMode = true;
            FeatureConfiguration.ShowDeveloperOptions = true;
            FeatureConfiguration.SuppressB2BPopups = true;
            FeatureConfiguration.MultiDeviceSupport = true;
            FeatureConfiguration.DefaultOrdersEnabled = true;
        }
    }
}
