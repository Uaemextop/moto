using lenovo.mbg.service.framework.services.Download;

namespace LMSA.Tests;

public class DownloadInfoTests
{
    [Fact]
    public void NewDownloadInfo_HasDefaultValues()
    {
        var info = new DownloadInfo();
        Assert.Equal("0KB/S", info.Speed);
        Assert.Equal(0.0, info.Progress);
        Assert.Equal("UNKNOWN", info.FileType);
        Assert.NotEqual(default(DateTime), info.CreateDateTime);
    }

    [Fact]
    public void DownloadStatus_HasAllExpectedValues()
    {
        Assert.Equal(0, (int)DownloadStatus.WAITTING);
        Assert.Equal(1, (int)DownloadStatus.DOWNLOADING);
        Assert.Equal(4, (int)DownloadStatus.FAILED);
        Assert.Equal(5, (int)DownloadStatus.SUCCESS);
    }

    [Fact]
    public void FileType_ConvertsToUppercase()
    {
        var info = new DownloadInfo();
        info.FileType = "rom";
        Assert.Equal("ROM", info.FileType);
    }

    [Fact]
    public void FileType_UnknownType_ReturnsUnknown()
    {
        var info = new DownloadInfo();
        info.FileType = "xyz";
        Assert.Equal("UNKNOWN", info.FileType);
    }

    [Fact]
    public void DownloadUrl_ExtractsFileUrl()
    {
        var info = new DownloadInfo();
        info.DownloadUrl = "https://example.com/file.zip?token=abc";

        Assert.Equal("https://example.com/file.zip", info.FileUrl);
        Assert.Contains("file.zip", info.FileName);
    }

    [Fact]
    public void FileSize_ConvertsToHumanReadable()
    {
        var info = new DownloadInfo();

        info.FileSize = 0;
        Assert.Equal("0MB", info.FileSizeStr);

        info.FileSize = 500;
        Assert.Contains("B", info.FileSizeStr);

        info.FileSize = 1048576; // 1MB
        Assert.Contains("MB", info.FileSizeStr);

        info.FileSize = 1073741824; // 1GB
        Assert.Contains("GB", info.FileSizeStr);
    }

    [Fact]
    public void Status_NotifiesPropertyChanged()
    {
        var info = new DownloadInfo();
        string changedProperty = null;
        info.PropertyChanged += (s, e) => changedProperty = e.PropertyName;

        info.Status = DownloadStatus.DOWNLOADING;

        Assert.Equal("Status", changedProperty);
        Assert.Equal(DownloadStatus.DOWNLOADING, info.Status);
    }

    [Fact]
    public void Progress_NotifiesPropertyChanged()
    {
        var info = new DownloadInfo();
        string changedProperty = null;
        info.PropertyChanged += (s, e) => changedProperty = e.PropertyName;

        info.Progress = 50.0;

        Assert.Equal("Progress", changedProperty);
        Assert.Equal(50.0, info.Progress);
    }

    [Fact]
    public void CanDownload_DefaultReturnsTrue()
    {
        var info = new DownloadInfo();
        Assert.True(info.CanDownload());
    }

    [Fact]
    public void FireComplete_CallsOnComplete()
    {
        var info = new DownloadInfo();
        DownloadStatus? calledWith = null;
        info.OnComplete = status => calledWith = status;

        info.FireComplete(DownloadStatus.SUCCESS);

        Assert.Equal(DownloadStatus.SUCCESS, calledWith);
    }

    [Fact]
    public void FireComplete_NullOnComplete_DoesNotThrow()
    {
        var info = new DownloadInfo();
        info.OnComplete = null;

        var exception = Record.Exception(() => info.FireComplete(DownloadStatus.FAILED));
        Assert.Null(exception);
    }

    [Fact]
    public void Constructor_WithParams_SetsValues()
    {
        var info = new DownloadInfo("https://example.com/fw.zip", "/tmp/fw.zip", 1048576, "abc123", "ROM");

        Assert.Equal("https://example.com/fw.zip", info.FileUrl);
        Assert.Equal("/tmp/fw.zip", info.LocalPath);
        Assert.Equal(1048576, info.FileSize);
        Assert.Equal("abc123", info.MD5);
        Assert.Equal("ROM", info.FileType);
    }
}
