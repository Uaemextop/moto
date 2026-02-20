using System;
using System.Collections.Generic;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class ShellResponseCmdDloader : ShellResponse
{
	protected Dictionary<string, ShellCmdStatus> CmdDloaderResponseToStatus = new Dictionary<string, ShellCmdStatus>
	{
		{
			"Connecting",
			ShellCmdStatus.Connected
		},
		{
			"Downloading...",
			ShellCmdStatus.Downloading
		},
		{
			"DownLoad Passed",
			ShellCmdStatus.Completed
		},
		{
			"DownLoad Failed",
			ShellCmdStatus.Error
		},
		{
			"login http Get Fail!",
			ShellCmdStatus.AuthorizedError
		},
		{
			"Not find valid download devices",
			ShellCmdStatus.Error
		},
		{
			"WM_CLOSE",
			ShellCmdStatus.Error
		},
		{
			"login tsdc server fail",
			ShellCmdStatus.Error
		},
		{
			"Failure to load PAC file",
			ShellCmdStatus.Error
		}
	};

	protected Dictionary<string, ShellCmdStatus> CmdDloaderTabletResponseToStatus = new Dictionary<string, ShellCmdStatus>
	{
		{
			"Connecting",
			ShellCmdStatus.Connected
		},
		{
			"Downloading...",
			ShellCmdStatus.Downloading
		},
		{
			"DownLoad Passed",
			ShellCmdStatus.Completed
		},
		{
			"DownLoad Failed",
			ShellCmdStatus.Error
		},
		{
			"login http Get Fail!",
			ShellCmdStatus.AuthorizedError
		},
		{
			"Not find valid download devices",
			ShellCmdStatus.Error
		},
		{
			"Loading firmware",
			ShellCmdStatus.Connected
		},
		{
			"Download Image Total\\(",
			ShellCmdStatus.Downloading
		},
		{
			"Upgrade firmware ok",
			ShellCmdStatus.Completed
		},
		{
			"Download Firmware Fail",
			ShellCmdStatus.Error
		},
		{
			"Load PAC file successfully",
			ShellCmdStatus.Connected
		}
	};

	private ShellCmdType shellCMd = ShellCmdType.CmdDloader;

	private Dictionary<string, ShellCmdStatus> responseToStatus;

	private double mPercentage;

	public override Dictionary<string, ShellCmdStatus> ResponseToStatus => responseToStatus;

	public override ShellCmdType ShellCmd => shellCMd;

	public override void Init(string category = null, object data = null)
	{
		bool flag = !string.IsNullOrEmpty(category) && category.Equals("Tablet", StringComparison.OrdinalIgnoreCase);
		shellCMd = (flag ? ShellCmdType.CmdDloaderTablet : ShellCmdType.CmdDloader);
		responseToStatus = (flag ? CmdDloaderTabletResponseToStatus : CmdDloaderResponseToStatus);
	}

	public override string ComputedPercent(string response, string key)
	{
		if (ShellCmd == ShellCmdType.CmdDloader)
		{
			mPercentage += 3.3333333333333335;
			return $"{mPercentage:0.00}";
		}
		return "0";
	}
}
