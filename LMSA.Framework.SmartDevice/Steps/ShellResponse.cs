using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public abstract class ShellResponse
{
	public abstract Dictionary<string, ShellCmdStatus> ResponseToStatus { get; }

	public abstract ShellCmdType ShellCmd { get; }

	public abstract string ComputedPercent(string response, string key);

	public virtual void Init(string category = null, object data = null)
	{
	}

	public virtual ShellCmdStatus ParseResponse(string response, out string responseKey)
	{
		ShellCmdStatus result = ShellCmdStatus.None;
		responseKey = string.Empty;
		foreach (string key in ResponseToStatus.Keys)
		{
			if (Regex.IsMatch(response, key, RegexOptions.IgnoreCase))
			{
				responseKey = key;
				result = ResponseToStatus[key];
			}
		}
		return result;
	}

	public virtual double GetDownloadProgressPercent(string response, string key)
	{
		double result = 0.0;
		if (double.TryParse(ComputedPercent(response, key), out var result2))
		{
			result = ((100.0 - result2 < 0.02) ? 100.0 : result2);
		}
		return result;
	}

	public void CleanUp()
	{
		GlobalFun.TryDeleteFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Qualcomm\\QFIL\\QFIL.config"));
	}

	public void WriteInput(string message, string serialNumber, string logId, string clientReqType, string prodId, string keyType, string keyName, string inputFileName, Process process)
	{
	}
}
