using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class ShellResponseFactory
{
	protected static Dictionary<string, string> ExeToShellResponse = new Dictionary<string, string>
	{
		{ "flash_tool", "ShellResponseFlashtool" },
		{ "spflashtool", "ShellResponseSpflashtool" },
		{ "qdowloader", "ShellResponseQdowloader" },
		{ "qcomdloader", "ShellResponseQdowloader" },
		{ "qfil", "ShellResponseQfil" },
		{ "upgrade_tool", "ShellResponseUpgradetool" },
		{ "cfc_flash", "ShellResponseCfcflash" },
		{ "update_flash", "ShellResponseCfcflash" },
		{ "Qsaharaserver", "ShellResponseQsaharaserver" },
		{ "fh_loader", "ShellResponseQsaharaserver" },
		{ "CmdDloader", "ShellResponseCmdDloader" },
		{ "LXConsoleDownLoadTool", "ShellResponseLXConsoleDownLoadTool" }
	};

	public static ShellResponse CreateInstance(string exe)
	{
		exe = Path.GetFileName(exe).ToLower();
		Assembly assembly = typeof(ShellResponseFactory).Assembly;
		string key = ExeToShellResponse.Keys.FirstOrDefault((string n) => exe.StartsWith(n, StringComparison.CurrentCultureIgnoreCase)) ?? "CmdDloader";
		return (ShellResponse)Activator.CreateInstance(assembly.GetType("lenovo.mbg.service.framework.smartdevice.Steps." + ExeToShellResponse[key]));
	}
}
