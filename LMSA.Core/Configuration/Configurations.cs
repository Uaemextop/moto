using System;
using System.IO;

namespace lenovo.mbg.service.common.utilities;

/// <summary>
/// Application-wide configuration and path management.
/// </summary>
public class Configurations
{
    private static string? _programDataPath;

    private static string? _downloadPath;

    private static string? _backupPath;

    public const long TRANSFER_FILE_MAX_SIZE = 4294967296L;

    public static string ProgramDataPath
    {
        get
        {
            if (_programDataPath == null)
            {
                _programDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                _programDataPath = Path.Combine(_programDataPath, "LMSA");
                if (StorageDirName != null && StorageDirName != string.Empty)
                {
                    _programDataPath = Path.Combine(_programDataPath, StorageDirName);
                }
            }
            return _programDataPath;
        }
    }

    public static string? StorageDirName { get; set; }

    public static string DownloadPath
    {
        get => _downloadPath ??= Path.Combine(ProgramDataPath, "Download");
        set => _downloadPath = value;
    }

    public static string BackupPath
    {
        get
        {
            _backupPath ??= Path.Combine(ProgramDataPath, "Backup");
            if (!Directory.Exists(_backupPath))
            {
                Directory.CreateDirectory(_backupPath);
            }
            return _backupPath;
        }
        set => _backupPath = value;
    }

    public static string ToolPath => Path.Combine(DownloadPath, "ToolFiles\\");

    public static string RomsPath => Path.Combine(DownloadPath, "RomFiles\\");

    public static string ApkPath => Path.Combine(DownloadPath, "ApkFiles\\");

    public static string IconPath => Path.Combine(DownloadPath, "IconFiles\\");

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

    public static string AdbPath => Path.Combine(".", "adb.exe");

    public static string FastbootPath => Path.Combine(".", "fastboot.exe");

    public static string RescueFailedLogPath => Path.Combine(".", "rescuefailedtmp.log");

    public static string SevenZipDllPath => Path.Combine(".", "7zSharp", Environment.Is64BitProcess ? "7z64.dll" : "7z.dll");

    public static int AppVersionCode { get; set; } = 1;

    public static int AppMinVersionCodeOfMA { get; set; } = 0;

    public static int AppMinVersionCodeOfMoto { get; set; } = 0;

    public static string? ServiceBaseUrl { get; set; }

    public static string? MotoHelperSecurityVersion { get; set; }

    public static string? MotoApkRandomKeyVersion { get; set; }

    public static string QrCodeDownloadMaUrl => "https://download.lenovo.com/lsa/ma.apk";
}
