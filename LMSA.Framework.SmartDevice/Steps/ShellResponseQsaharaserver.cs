using System.Collections.Generic;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class ShellResponseQsaharaserver : ShellResponse
{
	protected Dictionary<string, ShellCmdStatus> QFileSaharaResponseToStatus = new Dictionary<string, ShellCmdStatus>
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
			"Sahara protocol completed",
			ShellCmdStatus.Completed
		},
		{
			"File transferred successfully",
			ShellCmdStatus.Outputing
		},
		{
			"Download Succeed",
			ShellCmdStatus.Completed
		},
		{
			"Download Fail",
			ShellCmdStatus.Error
		},
		{
			"All Finished Successfully",
			ShellCmdStatus.Completed
		},
		{
			"There is a chance your target is in SAHARA mode!!",
			ShellCmdStatus.Error
		},
		{
			"ERROR: XML not formed correctly",
			ShellCmdStatus.Error
		}
	};

	public override Dictionary<string, ShellCmdStatus> ResponseToStatus => QFileSaharaResponseToStatus;

	public override ShellCmdType ShellCmd => ShellCmdType.QFileSaharaTool;

	public override string ComputedPercent(string response, string key)
	{
		return "0";
	}
}
