using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class ShellResponseUpgradetool : ShellResponse
{
	protected Dictionary<string, ShellCmdStatus> PnpResponseToStatus = new Dictionary<string, ShellCmdStatus>
	{
		{
			"Start to upgrade firmware",
			ShellCmdStatus.Connected
		},
		{
			"Download Image...",
			ShellCmdStatus.Downloading
		},
		{
			"Upgrade firmware ok",
			ShellCmdStatus.Completed
		},
		{
			"Download Firmware Fail",
			ShellCmdStatus.Error
		}
	};

	public override Dictionary<string, ShellCmdStatus> ResponseToStatus => PnpResponseToStatus;

	public override ShellCmdType ShellCmd => ShellCmdType.PnPTool;

	public override string ComputedPercent(string response, string key)
	{
		return Regex.Match(response, "\\((?<value>[\\d\\.]+)", RegexOptions.IgnoreCase).Groups["value"].Value;
	}
}
