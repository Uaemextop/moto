using lenovo.mbg.service.framework.services.Download;

namespace LMSA.Tests;

public class DownloadInfoTests
{
    [Fact]
    public void DefaultConstructor_InitializesDefaults()
    {
        var info = new DownloadInfo();

        Assert.Equal("0KB/S", info.Speed);
        Assert.Equal(0.0, info.Progress);
        Assert.Equal("UNKNOWN", info.FileType);
        Assert.True(info.ShowInUI);
    }

    [Fact]
    public void ParameterizedConstructor_SetsProperties()
    {
        var info = new DownloadInfo("http://example.com/file.zip", "/local/path", 1024, "abc123", "ROM");

        Assert.Equal("http://example.com/file.zip", info.FileUrl);
        Assert.Equal("/local/path", info.LocalPath);
        Assert.Equal(1024, info.FileSize);
        Assert.Equal("abc123", info.MD5);
        Assert.Equal("ROM", info.FileType);
    }

    [Fact]
    public void DownloadUrl_ExtractsFileUrl()
    {
        var info = new DownloadInfo();
        info.DownloadUrl = "http://example.com/firmware.zip?token=abc";

        Assert.Equal("http://example.com/firmware.zip", info.FileUrl);
    }

    [Fact]
    public void FileSize_UpdatesFileSizeStr()
    {
        var info = new DownloadInfo();
        info.FileSize = 1048576; // 1MB

        Assert.Contains("MB", info.FileSizeStr);
    }

    [Fact]
    public void FileSize_Zero_ReturnsZeroMB()
    {
        var info = new DownloadInfo();
        info.FileSize = 0;

        Assert.Equal("0MB", info.FileSizeStr);
    }

    [Fact]
    public void FileType_ConvertsToUpperCase()
    {
        var info = new DownloadInfo();
        info.FileType = "apk";
        Assert.Equal("APK", info.FileType);

        info.FileType = "rom";
        Assert.Equal("ROM", info.FileType);
    }

    [Fact]
    public void FileType_UnknownValue_ReturnsUnknown()
    {
        var info = new DownloadInfo();
        info.FileType = "something_random";
        Assert.Equal("UNKNOWN", info.FileType);
    }

    [Fact]
    public void CanDownload_ReturnsTrue()
    {
        var info = new DownloadInfo();
        Assert.True(info.CanDownload());
    }

    [Fact]
    public void PropertyChanged_FiresOnStatusChange()
    {
        var info = new DownloadInfo();
        string? changedProperty = null;
        info.PropertyChanged += (s, e) => changedProperty = e.PropertyName;

        info.Status = DownloadStatus.DOWNLOADING;

        Assert.Equal("Status", changedProperty);
    }

    [Fact]
    public void FireComplete_InvokesCallback()
    {
        bool callbackInvoked = false;
        var info = new DownloadInfo();
        info.OnComplete = status => callbackInvoked = true;

        info.FireComplete(DownloadStatus.SUCCESS);

        Assert.True(callbackInvoked);
    }

    [Fact]
    public void FireComplete_NullCallback_DoesNotThrow()
    {
        var info = new DownloadInfo();
        info.OnComplete = null;

        var ex = Record.Exception(() => info.FireComplete(DownloadStatus.SUCCESS));
        Assert.Null(ex);
    }
}
