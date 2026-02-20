using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace lenovo.mbg.service.framework.services.Download;

/// <summary>
/// Represents information about a file download including status, progress, and metadata.
/// </summary>
[Serializable]
public class DownloadInfo : EventArgs, INotifyPropertyChanged
{
    private bool isManualMatch;
    private string _originalFileName = string.Empty;
    private string _FileName = string.Empty;
    private string _Speed = "0KB/S";
    private double _Progress;
    private DownloadStatus _Status;
    private long _LocalFileSize;
    private long _FileSize;
    private string _FileType = "UNKNOWN";
    private string _DownloadUrl = string.Empty;
    private string _LocalPath = string.Empty;
    private bool _ShowInUI = true;
    private string _MD5 = string.Empty;
    private string _NeedTakesTime = string.Empty;
    private bool _UnZip;
    private string _LocalFileSizeStr = "0MB";
    private string _FileSizeStr = "0MB";
    private string _ErrorMessage = string.Empty;

    public Action<DownloadStatus>? OnComplete;

    public string FileType
    {
        get => _FileType;
        set
        {
            _FileType = FileTypeConverter(value);
            FirePropertyChangedEvent(nameof(FileType));
        }
    }

    public string FileUrl { get; private set; } = string.Empty;

    public string DownloadUrl
    {
        get => _DownloadUrl;
        set
        {
            _DownloadUrl = value;
            FileUrl = _DownloadUrl.Split('?')[0];
            OriginalFileName = GetFileName(FileUrl);
        }
    }

    public string FileName
    {
        get => _FileName;
        private set
        {
            _FileName = value;
            FirePropertyChangedEvent(nameof(FileName));
        }
    }

    public string OriginalFileName
    {
        get => _originalFileName;
        private set
        {
            _originalFileName = value;
            var regex = new Regex(@"-|\\|\/|:|\*|\?|\<|\>|""");
            FileName = regex.Replace(Uri.UnescapeDataString(_originalFileName), "_");
        }
    }

    public string LocalPath
    {
        get => _LocalPath;
        set
        {
            _LocalPath = value;
            FirePropertyChangedEvent(nameof(LocalPath));
        }
    }

    public long FileSize
    {
        get => _FileSize;
        set
        {
            _FileSize = value;
            FileSizeStr = ConvertLong2String2(value);
            FirePropertyChangedEvent(nameof(FileSize));
        }
    }

    public string MD5
    {
        get => _MD5;
        set
        {
            _MD5 = value;
            FirePropertyChangedEvent(nameof(MD5));
        }
    }

    public bool ShowInUI
    {
        get => _ShowInUI;
        set
        {
            _ShowInUI = value;
            FirePropertyChangedEvent(nameof(ShowInUI));
        }
    }

    public DateTime CreateDateTime { get; set; }

    public string Speed
    {
        get => _Speed;
        set
        {
            _Speed = value;
            FirePropertyChangedEvent(nameof(Speed));
        }
    }

    public string NeedTakesTime
    {
        get => _NeedTakesTime;
        set
        {
            _NeedTakesTime = value;
            FirePropertyChangedEvent(nameof(NeedTakesTime));
        }
    }

    public double Progress
    {
        get => _Progress;
        set
        {
            _Progress = value;
            FirePropertyChangedEvent(nameof(Progress));
        }
    }

    public DownloadStatus Status
    {
        get => _Status;
        set
        {
            _Status = value;
            FirePropertyChangedEvent(nameof(Status));
        }
    }

    public long LocalFileSize
    {
        get => _LocalFileSize;
        set
        {
            _LocalFileSize = value;
            LocalFileSizeStr = ConvertLong2String2(value);
            FirePropertyChangedEvent(nameof(LocalFileSize));
        }
    }

    public bool UnZip
    {
        get => _UnZip;
        set
        {
            _UnZip = value;
            FirePropertyChangedEvent(nameof(UnZip));
        }
    }

    public string? ZipPwd { get; set; }

    public string LocalFileSizeStr
    {
        get => _LocalFileSizeStr;
        set
        {
            _LocalFileSizeStr = value;
            FirePropertyChangedEvent(nameof(LocalFileSizeStr));
        }
    }

    public string FileSizeStr
    {
        get => _FileSizeStr;
        set
        {
            _FileSizeStr = value;
            FirePropertyChangedEvent(nameof(FileSizeStr));
        }
    }

    public string ErrorMessage
    {
        get => _ErrorMessage;
        set
        {
            _ErrorMessage = value;
            FirePropertyChangedEvent(nameof(ErrorMessage));
        }
    }

    public bool IsManualMatch
    {
        get => isManualMatch;
        set
        {
            isManualMatch = value;
            FirePropertyChangedEvent(nameof(IsManualMatch));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public virtual bool CanDownload()
    {
        return true;
    }

    public DownloadInfo()
    {
        Speed = "0KB/S";
        Progress = 0.0;
        FileType = "UNKNOWN";
        CreateDateTime = DateTime.Now;
    }

    public DownloadInfo(string fileUrl, string localPath, long fileSize, string md5, string fileType = "UNKNOWN")
        : this(fileUrl, localPath, fileSize, md5, fileType, null)
    {
    }

    public DownloadInfo(string fileUrl, string localPath, long fileSize, string md5, string fileType, Action<DownloadStatus>? onComplete, bool showInUI = false)
    {
        FileType = fileType;
        DownloadUrl = fileUrl;
        LocalPath = localPath;
        FileSize = fileSize;
        MD5 = md5;
        CreateDateTime = DateTime.Now;
        ShowInUI = true;
        Speed = "0KB/S";
        Progress = 0.0;
        OnComplete = onComplete;
    }

    public void FireComplete(DownloadStatus status)
    {
        try
        {
            OnComplete?.Invoke(status);
        }
        catch (Exception)
        {
        }
    }

    protected string GetFileName(string fileUrl)
    {
        string[] parts = Regex.Split(fileUrl, @"\\|/");
        if (parts != null && parts.Length != 0)
        {
            return parts[^1];
        }
        return fileUrl;
    }

    protected void FirePropertyChangedEvent(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private static string FileTypeConverter(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            return "UNKNOWN";
        }
        return data.ToUpper() switch
        {
            "APK" => "APK",
            "ICON" => "ICON",
            "ROM" => "ROM",
            "TOOL" => "TOOL",
            "COUNTRYCODE" or "COUNTRY_CODE" => "COUNTRYCODE",
            "JSON" => "JSON",
            "BANNER" or "BANNER_ICON" => "BANNER",
            "XAML" => "XAML",
            _ => "UNKNOWN"
        };
    }

    private static string ConvertLong2String2(long bytes)
    {
        string format = "F1";
        float num = bytes;
        if (bytes == 0L)
        {
            return "0MB";
        }
        if (bytes > 1000)
        {
            if (bytes >= 1024000)
            {
                if (bytes >= 1024000000)
                {
                    return (num / 1.0737418E+09f).ToString(format) + "GB";
                }
                return (num / 1048576f).ToString(format) + "MB";
            }
            return (num / 1024f).ToString(format) + "KB";
        }
        return bytes + "B";
    }
}
