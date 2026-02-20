using System.Collections.Generic;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class ShellResponseQdowloader : ShellResponse
{
	protected Dictionary<string, ShellCmdStatus> QComResponseToStatus = new Dictionary<string, ShellCmdStatus>
	{
		{
			"Status=status_flash_dut_connected",
			ShellCmdStatus.Connected
		},
		{
			"Status=status_flash_download_percent_",
			ShellCmdStatus.Downloading
		},
		{
			"Status=status_flash_download_end",
			ShellCmdStatus.Completed
		},
		{
			"Status=status_flash_download_failed",
			ShellCmdStatus.Error
		}
	};

	public override Dictionary<string, ShellCmdStatus> ResponseToStatus => QComResponseToStatus;

	public override ShellCmdType ShellCmd => ShellCmdType.QComFlashTool;

	public override string ComputedPercent(string response, string key)
	{
		return response.Substring(key.Length);
	}
}
