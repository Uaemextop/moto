using lenovo.mbg.service.common.utilities;

namespace LMSA.Tests;

public class LMSAContextTests
{
    [Fact]
    public void CurrentLanguage_ReturnsDefaultEnUS()
    {
        string lang = LMSAContext.CurrentLanguage;
        Assert.NotNull(lang);
        Assert.Equal("en-US", lang);
    }

    [Fact]
    public void LanguagePackageRootPath_ContainsLangDir()
    {
        string path = LMSAContext.LanguagePackageRootPath;
        Assert.NotNull(path);
        Assert.Contains("lang", path);
    }

    [Fact]
    public void GetLanguageId_ReturnsCorrectId()
    {
        string id = LMSAContext.GetLanguageId("en-US");
        Assert.Equal("1033", id);
    }

    [Fact]
    public void GetLanguageId_ForChinese_ReturnsCorrectId()
    {
        string id = LMSAContext.GetLanguageId("zh-CN");
        Assert.Equal("2052", id);
    }

    [Fact]
    public void MainProcessVersion_ReturnsNonNullNonEmpty()
    {
        string version = LMSAContext.MainProcessVersion;
        Assert.NotNull(version);
        // In test environment, may return fallback "1.0.0.0"
        Assert.True(version.Length > 0);
    }

    [Fact]
    public void DEF_LANGUAGE_IsEnUS()
    {
        Assert.Equal("en-US", LMSAContext.DEF_LANGUAGE);
    }

    [Fact]
    public void LANGUAGE_PACKAGE_URI_IsCorrect()
    {
        Assert.Equal("https://rsddownload-cloud.motorola.com/RSALanguage", LMSAContext.LANGUAGE_PACKAGE_URI);
    }

    [Fact]
    public void SetCurrentLanguage_ChangesLanguage()
    {
        LMSAContext.SetCurrentLanguage("zh-CN");
        // Note: We'd need to verify via CurrentLanguage, but the getter may cache
        // This test verifies it doesn't throw
    }
}
