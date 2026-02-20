using System;
using System.Collections.Generic;
using System.IO;

namespace lenovo.mbg.service.common.utilities;

public class Configurations
{
    private static string _DownloadPath;
    private static string _BackupPath;
    private static string _GifSavePath;
    private static string _ProgramDataPath = null;
    private static string serviceBaseUrl = null;
    private static string serviceInterfaceUrl = null;
    private static string _baseHttpUrl = null;
    private static readonly object serviceBaseUrlLock = new object();

    private static Dictionary<bool, int> _RescueResultMap = new Dictionary<bool, int>
    {
        { true, 0 },
        { false, 0 }
    };

    public const long TRANSFER_FILE_MAX_SIZE = 4294967296L;

    public static string ToolPath => Path.Combine(DownloadPath, "ToolFiles" + Path.DirectorySeparatorChar);

    public static string RomsPath => Path.Combine(DownloadPath, "RomFiles" + Path.DirectorySeparatorChar);

    public static string ApkPath => Path.Combine(DownloadPath, "ApkFiles" + Path.DirectorySeparatorChar);

    public static string IconPath => Path.Combine(DownloadPath, "IconFiles" + Path.DirectorySeparatorChar);

    public static string CountryCodePath => Path.Combine(DownloadPath, "CountryCodeFiles" + Path.DirectorySeparatorChar);

    public static string JsonPath => Path.Combine(DownloadPath, "JsonFiles" + Path.DirectorySeparatorChar);

    public static string BannerPath => Path.Combine(DownloadPath, "BannerFiles" + Path.DirectorySeparatorChar);

    public static string XamlPath => Path.Combine(DownloadPath, "XamlFiles" + Path.DirectorySeparatorChar);

    public static string UnknownPath => Path.Combine(DownloadPath, "UnknownFiles" + Path.DirectorySeparatorChar);

    public static string DownloadingSavePath => Path.Combine(DownloadInfoSavePath, "download_resources.json");

    public static string DownloadedSavePath => Path.Combine(DownloadInfoSavePath, "downloaded_resources.json");

    public static string DownloadedMatchPath => Path.Combine(DownloadInfoSavePath, "downloaded_match.json");

    public static string RescueManualMatchFile => "rescueddevice.json.dpapi";

    public static string DefaultOptionsFileName => "options.json.dpapi";

    public static string RescueRecordsFile => "flashrecords.json.dpapi";

    public static string UserRequestRecordsFile => "user_request_records.json.dpapi";

    public static string AppCacheDir => Path.Combine(ProgramDataPath, "App");

    public static string AppIconCacheDir => Path.Combine(ProgramDataPath, "AppIcon");

    public static string PicCacheDir => Path.Combine(ProgramDataPath, "Pic");

    public static string PicOriginalCacheDir => Path.Combine(ProgramDataPath, "PicOriginal");

    public static string MusicCacheDir => Path.Combine(ProgramDataPath, "Music");

    public static string VideoCacheDir => Path.Combine(ProgramDataPath, "Video");

    public static string ContactCacheDir => Path.Combine(ProgramDataPath, "Contact");

    public static string PhoneBackupCacheDir
    {
        get
        {
            string text = Path.Combine(ProgramDataPath, "Backup");
            if (!Directory.Exists(text))
            {
                Directory.CreateDirectory(text);
            }
            return text;
        }
    }

    public static string ScreencapDir => Path.Combine(ProgramDataPath, "Screencap");

    public static string TempDir
    {
        get
        {
            string text = Path.Combine(ProgramDataPath, "Temp");
            if (!Directory.Exists(text))
            {
                Directory.CreateDirectory(text);
            }
            return text;
        }
    }

    public static string ProgramDataPath
    {
        get
        {
            if (_ProgramDataPath == null)
            {
                _ProgramDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                _ProgramDataPath = Path.Combine(_ProgramDataPath, "LMSA");
                if (!string.IsNullOrEmpty(StorageDirName))
                {
                    _ProgramDataPath = Path.Combine(_ProgramDataPath, StorageDirName);
                }
            }
            return _ProgramDataPath;
        }
    }

    public static string ProgramDataRSARootPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "RSA");

    public static string DownloadInfoSavePath
    {
        get
        {
            string text = Path.Combine(ProgramDataPath, "Download");
            if (!Directory.Exists(text))
            {
                Directory.CreateDirectory(text);
            }
            return text;
        }
    }

    public static string DownloadPath
    {
        get
        {
            if (string.IsNullOrEmpty(_DownloadPath))
            {
                _DownloadPath = Path.Combine(ProgramDataRSARootPath, "Download");
            }
            return _DownloadPath;
        }
        set
        {
            _DownloadPath = value;
        }
    }

    public static string BackupPath
    {
        get
        {
            if (string.IsNullOrEmpty(_BackupPath))
            {
                _BackupPath = Path.Combine(ProgramDataRSARootPath, "Backup");
            }
            if (!Directory.Exists(_BackupPath))
            {
                Directory.CreateDirectory(_BackupPath);
            }
            return _BackupPath;
        }
        set
        {
            _BackupPath = value;
        }
    }

    public static string GifSavePath
    {
        get
        {
            if (string.IsNullOrEmpty(_GifSavePath))
            {
                _GifSavePath = Path.Combine(ProgramDataRSARootPath, "Gif");
            }
            if (!Directory.Exists(_GifSavePath))
            {
                Directory.CreateDirectory(_GifSavePath);
            }
            return _GifSavePath;
        }
        set
        {
            _GifSavePath = value;
        }
    }

    public static string TRANSFER_FILE_ERROR_TXT_PATH => Path.Combine(TempDir, "larger_max_size_file_list.txt");

    public static string ServiceBaseUrl
    {
        get
        {
            if (string.IsNullOrEmpty(serviceBaseUrl))
            {
                lock (serviceBaseUrlLock)
                {
                    serviceBaseUrl = "https://lsa.lenovo.com";
                }
            }
            return serviceBaseUrl;
        }
        set
        {
            lock (serviceBaseUrlLock)
            {
                serviceBaseUrl = value;
                serviceInterfaceUrl = null;
            }
        }
    }

    public static string ServiceInterfaceUrl
    {
        get
        {
            if (string.IsNullOrEmpty(serviceInterfaceUrl))
            {
                serviceInterfaceUrl = ServiceBaseUrl + "/Interface";
            }
            return serviceInterfaceUrl;
        }
    }

    public static string BaseHttpUrl
    {
        get
        {
            if (string.IsNullOrEmpty(_baseHttpUrl))
            {
                _baseHttpUrl = ServiceBaseUrl;
            }
            return _baseHttpUrl;
        }
        set => _baseHttpUrl = value;
    }

    public static bool IsReleaseVersion => "https://lsa.lenovo.com".Equals(ServiceBaseUrl, StringComparison.InvariantCultureIgnoreCase);

    public static string AdbPath => Path.Combine(".", "adb.exe");

    public static string RescueFailedLogPath => Path.Combine(".", "rescuefailedtmp.log");

    public static string FastbootPath => Path.Combine(".", "fastboot.exe");

    public static string SevenZipDllPath => Path.Combine(".", "7zSharp", Environment.Is64BitProcess ? "7z64.dll" : "7z.dll");

    public static Dictionary<string, string> FileSaveLocalPath => new Dictionary<string, string>
    {
        { "ROM", RomsPath },
        { "APK", ApkPath },
        { "TOOL", ToolPath },
        { "ICON", IconPath },
        { "COUNTRYCODE", CountryCodePath },
        { "JSON", JsonPath },
        { "BANNER", BannerPath },
        { "XAML", XamlPath },
        { "UNKNOWN", UnknownPath }
    };

    public static string StorageDirName { get; set; }

    public static Dictionary<bool, int> RescueResultMap => _RescueResultMap;

    public static void AddRescueResult(bool success)
    {
        _RescueResultMap[success] = ++_RescueResultMap[success];
    }

    public static void ResetRescueResultMap()
    {
        _RescueResultMap = new Dictionary<bool, int>
        {
            { true, 0 },
            { false, 0 }
        };
    }
}
