using System;
using System.IO;

namespace lenovo.mbg.service.common.utilities;

public static class Configurations
{
    public static string AdbPath => Path.Combine(".", "adb.exe");

    public static string FastbootPath => Path.Combine(".", "fastboot.exe");

    public static string RescueFailedLogPath => Path.Combine(".", "rescuefailedtmp.log");

    public static string SevenZipDllPath => Path.Combine(".", "7zSharp", Environment.Is64BitProcess ? "7z64.dll" : "7z.dll");

    public static string ProgramDataPath
    {
        get
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            return Path.Combine(path, "LMSA");
        }
    }

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

    public static string LogDir
    {
        get
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
    }
}
