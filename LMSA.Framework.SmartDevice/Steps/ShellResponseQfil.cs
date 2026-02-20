using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class ShellResponseQfil : ShellResponse
{
	protected Dictionary<string, ShellCmdStatus> QFileResponseToStatus = new Dictionary<string, ShellCmdStatus>
	{
		{
			"Start Download",
			ShellCmdStatus.Connected
		},
		{
			"{percent files transferred",
			ShellCmdStatus.Downloading
		},
		{
			"Download Succeed",
			ShellCmdStatus.Completed
		},
		{
			"Download Fail",
			ShellCmdStatus.Error
		}
	};

	public override Dictionary<string, ShellCmdStatus> ResponseToStatus => QFileResponseToStatus;

	public override ShellCmdType ShellCmd => ShellCmdType.QFileTool;

	public override string ComputedPercent(string response, string key)
	{
		string pattern = $"(?<key>.+?{key})\\s+?(?<value>[\\d\\.]+)";
		return Regex.Match(response, pattern, RegexOptions.IgnoreCase).Groups["value"].Value;
	}
}
