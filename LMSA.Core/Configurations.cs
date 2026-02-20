using System;
using System.IO;

namespace lenovo.mbg.service.common.utilities;

public class Configurations
{
	public static string AdbPath => Path.Combine(".", "adb.exe");

	public static string FastbootPath => Path.Combine(".", "fastboot.exe");

	public static int AppVersionCode { get; set; } = 1;

	public static int AppMinVersionCodeOfMoto { get; set; } = 0;

	public static int AppMinVersionCodeOfMA { get; set; } = 0;
}
