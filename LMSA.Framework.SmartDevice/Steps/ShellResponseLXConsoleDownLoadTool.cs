using System.Collections.Generic;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class ShellResponseLXConsoleDownLoadTool : ShellResponse
{
	protected Dictionary<string, ShellCmdStatus> DownLoadToolResponseToStatus = new Dictionary<string, ShellCmdStatus>
	{
		{
			"begin down file...",
			ShellCmdStatus.Connected
		},
		{
			"start download file",
			ShellCmdStatus.Downloading
		},
		{
			"open device fail",
			ShellCmdStatus.Error
		},
		{
			"download finish",
			ShellCmdStatus.Completed
		}
	};

	public override Dictionary<string, ShellCmdStatus> ResponseToStatus => DownLoadToolResponseToStatus;

	public override ShellCmdType ShellCmd => ShellCmdType.LXDownloadTool;

	public override string ComputedPercent(string response, string key)
	{
		return "0";
	}
}
