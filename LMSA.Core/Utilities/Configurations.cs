using System;
using System.Collections.Generic;
using System.IO;

namespace lenovo.mbg.service.common.utilities;

/// <summary>
/// Application configuration providing paths for tools, downloads, backups,
/// and other application-wide settings.
/// </summary>
public class Configurations
{
    private static string? _ProgramDataPath;
    private static string _DownloadPath = string.Empty;
    private static string _BackupPath = string.Empty;
    private static Dictionary<bool, int> _RescueResultMap = new Dictionary<bool, int>
    {
        { true, 0 },
        { false, 0 }
    };

    public const long TRANSFER_FILE_MAX_SIZE = 4294967296L;

    public static string AdbPath => Path.Combine(".", "adb.exe");
    public static string FastbootPath => Path.Combine(".", "fastboot.exe");
    public static string RescueFailedLogPath => Path.Combine(".", "rescuefailedtmp.log");
    public static string SevenZipDllPath => Path.Combine(".", "7zSharp", Environment.Is64BitProcess ? "7z64.dll" : "7z.dll");

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

    public static string TempDir
    {
        get
        {
            string path = Path.Combine(ProgramDataPath, "Temp");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
    }

    public static string DownloadInfoSavePath
    {
        get
        {
            string path = Path.Combine(ProgramDataPath, "Download");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
    }

    public static string DownloadPath
    {
        get => string.IsNullOrEmpty(_DownloadPath) ? Path.Combine(ProgramDataRSARootPath, "Download") : _DownloadPath;
        set => _DownloadPath = value;
    }

    public static string BackupPath
    {
        get
        {
            string path = string.IsNullOrEmpty(_BackupPath) ? Path.Combine(ProgramDataRSARootPath, "Backup") : _BackupPath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
        set => _BackupPath = value;
    }

    public static string ToolPath => Path.Combine(DownloadPath, "ToolFiles");
    public static string RomsPath => Path.Combine(DownloadPath, "RomFiles");
    public static string ApkPath => Path.Combine(DownloadPath, "ApkFiles");
    public static string IconPath => Path.Combine(DownloadPath, "IconFiles");
    public static string CountryCodePath => Path.Combine(DownloadPath, "CountryCodeFiles");
    public static string JsonPath => Path.Combine(DownloadPath, "JsonFiles");
    public static string BannerPath => Path.Combine(DownloadPath, "BannerFiles");
    public static string XamlPath => Path.Combine(DownloadPath, "XamlFiles");
    public static string UnknownPath => Path.Combine(DownloadPath, "UnknownFiles");

    public static string DownloadingSavePath => Path.Combine(DownloadInfoSavePath, "download_resources.json");
    public static string DownloadedSavePath => Path.Combine(DownloadInfoSavePath, "downloaded_resources.json");
    public static string DownloadedMatchPath => Path.Combine(DownloadInfoSavePath, "downloaded_match.json");

    public static string AppCacheDir => Path.Combine(ProgramDataPath, "App");
    public static string AppIconCacheDir => Path.Combine(ProgramDataPath, "AppIcon");
    public static string PicCacheDir => Path.Combine(ProgramDataPath, "Pic");
    public static string MusicCacheDir => Path.Combine(ProgramDataPath, "Music");
    public static string VideoCacheDir => Path.Combine(ProgramDataPath, "Video");
    public static string ContactCacheDir => Path.Combine(ProgramDataPath, "Contact");
    public static string ScreencapDir => Path.Combine(ProgramDataPath, "Screencap");

    public static string PhoneBackupCacheDir
    {
        get
        {
            string path = Path.Combine(ProgramDataPath, "Backup");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
    }

    public static string TRANSFER_FILE_ERROR_TXT_PATH => Path.Combine(TempDir, "larger_max_size_file_list.txt");

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

    public static string? StorageDirName { get; set; }

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
