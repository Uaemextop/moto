using System;
using System.Collections.Generic;
using System.IO;

namespace lenovo.mbg.service.common.utilities;

public class Configurations
{
    private static string? _ProgramDataPath = null;

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
                if (StorageDirName != null && StorageDirName != string.Empty)
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
            string text = Path.Combine(ProgramDataPath, "Backup");
            if (!Directory.Exists(text))
            {
                Directory.CreateDirectory(text);
            }
            return text;
        }
    }

    public static string TRANSFER_FILE_ERROR_TXT_PATH => Path.Combine(TempDir, "larger_max_size_file_list.txt");

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
