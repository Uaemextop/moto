using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class ShellResponseFlashtool : ShellResponse
{
	protected Dictionary<string, ShellCmdStatus> MTekResponseToStatus = new Dictionary<string, ShellCmdStatus>
	{
		{
			"BROM connected",
			ShellCmdStatus.Connected
		},
		{
			"connect brom successed",
			ShellCmdStatus.Connected
		},
		{
			"% of image data has been sent",
			ShellCmdStatus.Downloading
		},
		{
			"WRITE TO PARTITION",
			ShellCmdStatus.Downloading
		},
		{
			"All command exec done!",
			ShellCmdStatus.Completed
		},
		{
			"STATUS_DOWNLOAD_EXCEPTION",
			ShellCmdStatus.Error
		},
		{
			"SearchUSBPortPool failed!",
			ShellCmdStatus.Error
		},
		{
			"Failed to find USB port",
			ShellCmdStatus.Error
		},
		{
			"Connect BROM failed: S_CHIP_TYPE_NOT_MATCH",
			ShellCmdStatus.RomUnMatchError
		},
		{
			"Connect BROM failed: STATUS_SCATTER_HW_CHIP_ID_MISMATCH",
			ShellCmdStatus.RomUnMatchError
		},
		{
			"Connect BROM failed: S_BROM_CMD_STARTCMD_FAIL",
			ShellCmdStatus.Error
		},
		{
			"Connect BROM failed: S_TIMEOUT",
			ShellCmdStatus.Error
		},
		{
			"Connect BROM failed: STATUS_BROM_CMD_FAIL",
			ShellCmdStatus.Error
		},
		{
			"[BROM] Can not pass bootrom start command! Possibly target power up too early.",
			ShellCmdStatus.Error
		},
		{
			"Failed to Connect DA: S_FT_ENABLE_DRAM_FAIL",
			ShellCmdStatus.Error
		},
		{
			"[EMI] Enable DRAM Failed!",
			ShellCmdStatus.Error
		},
		{
			"Please check your load matches to your target which is to be downloaded.",
			ShellCmdStatus.Error
		},
		{
			"[DA] DA binary file contains an unsupported version in its header! Please ask for help.",
			ShellCmdStatus.Error
		},
		{
			"Failed to Connect DA: S_UNSUPPORTED_VER_OF_DA",
			ShellCmdStatus.Error
		},
		{
			"Failed to Connect DA: STATUS_DEVICE_CTRL_EXCEPTION",
			ShellCmdStatus.Error
		},
		{
			"Chip mismatch",
			ShellCmdStatus.RomUnMatchError
		},
		{
			"STATUS_SEC_DL_FORBIDDEN",
			ShellCmdStatus.AuthorizedError
		},
		{
			"Exception: err_code",
			ShellCmdStatus.Error
		},
		{
			"lib DA NOT match",
			ShellCmdStatus.RomUnMatchError
		},
		{
			"Download failed",
			ShellCmdStatus.Error
		},
		{
			"S_FT_DOWNLOAD_FAIL ",
			ShellCmdStatus.Error
		},
		{
			"Invalid parameter.",
			ShellCmdStatus.Error
		},
		{
			"connect brom failed",
			ShellCmdStatus.Error
		},
		{
			"Logger deinited",
			ShellCmdStatus.Error
		}
	};

	protected Dictionary<string, ShellCmdStatus> MTekResponseToStatus_Tablet = new Dictionary<string, ShellCmdStatus>
	{
		{
			"BROM connected",
			ShellCmdStatus.Connected
		},
		{
			"connect brom successed",
			ShellCmdStatus.Connected
		},
		{
			"% of image data has been sent",
			ShellCmdStatus.Downloading
		},
		{
			"WRITE TO PARTITION",
			ShellCmdStatus.Downloading
		},
		{
			"All command exec done!",
			ShellCmdStatus.Completed
		},
		{
			"Download Succeeded.",
			ShellCmdStatus.Completed
		},
		{
			"STATUS_DOWNLOAD_EXCEPTION",
			ShellCmdStatus.Error
		},
		{
			"SearchUSBPortPool failed!",
			ShellCmdStatus.Error
		},
		{
			"Failed to find USB port",
			ShellCmdStatus.Error
		},
		{
			"Connect BROM failed: S_CHIP_TYPE_NOT_MATCH",
			ShellCmdStatus.RomUnMatchError
		},
		{
			"Connect BROM failed: STATUS_SCATTER_HW_CHIP_ID_MISMATCH",
			ShellCmdStatus.RomUnMatchError
		},
		{
			"Connect BROM failed: S_BROM_CMD_STARTCMD_FAIL",
			ShellCmdStatus.Error
		},
		{
			"Connect BROM failed: S_TIMEOUT",
			ShellCmdStatus.Error
		},
		{
			"Connect BROM failed: STATUS_BROM_CMD_FAIL",
			ShellCmdStatus.Error
		},
		{
			"[BROM] Can not pass bootrom start command! Possibly target power up too early.",
			ShellCmdStatus.Error
		},
		{
			"Failed to Connect DA: S_FT_ENABLE_DRAM_FAIL",
			ShellCmdStatus.Error
		},
		{
			"[EMI] Enable DRAM Failed!",
			ShellCmdStatus.Error
		},
		{
			"Please check your load matches to your target which is to be downloaded.",
			ShellCmdStatus.Error
		},
		{
			"[DA] DA binary file contains an unsupported version in its header! Please ask for help.",
			ShellCmdStatus.Error
		},
		{
			"Failed to Connect DA: S_UNSUPPORTED_VER_OF_DA",
			ShellCmdStatus.Error
		},
		{
			"Failed to Connect DA: STATUS_DEVICE_CTRL_EXCEPTION",
			ShellCmdStatus.Error
		},
		{
			"Chip mismatch",
			ShellCmdStatus.RomUnMatchError
		},
		{
			"STATUS_SEC_DL_FORBIDDEN",
			ShellCmdStatus.AuthorizedError
		},
		{
			"Exception: err_code",
			ShellCmdStatus.Error
		},
		{
			"lib DA NOT match",
			ShellCmdStatus.RomUnMatchError
		},
		{
			"Download failed",
			ShellCmdStatus.Error
		},
		{
			"S_FT_DOWNLOAD_FAIL ",
			ShellCmdStatus.Error
		},
		{
			"Invalid parameter.",
			ShellCmdStatus.Error
		},
		{
			"connect brom failed",
			ShellCmdStatus.Error
		},
		{
			"Logger deinited",
			ShellCmdStatus.Error
		}
	};

	private ShellCmdType shellCMd = ShellCmdType.MTekFlashTool;

	private Dictionary<string, ShellCmdStatus> responseToStatus;

	private double mPercentage;

	protected int MTKFileCount = 1;

	public override ShellCmdType ShellCmd => shellCMd;

	public override Dictionary<string, ShellCmdStatus> ResponseToStatus => responseToStatus;

	public override void Init(string category = null, object data = null)
	{
		bool flag = !string.IsNullOrEmpty(category) && category.Equals("Tablet", StringComparison.OrdinalIgnoreCase);
		shellCMd = ((!flag) ? ShellCmdType.MTekFlashTool : ShellCmdType.MTekFlashToolTablet);
		responseToStatus = (flag ? MTekResponseToStatus_Tablet : MTekResponseToStatus);
		if (ShellCmd == ShellCmdType.MTekFlashTool)
		{
			ComputedFileCount(data);
		}
	}

	public override string ComputedPercent(string response, string key)
	{
		string text = "0";
		if (key == "WRITE TO PARTITION")
		{
			mPercentage += 100.0 / (double)MTKFileCount;
			text = $"{mPercentage:0.00}";
		}
		else if (key == "% of image data has been sent")
		{
			text = response.Substring(0, response.IndexOf('%'));
			mPercentage = double.Parse(text);
		}
		else
		{
			text = mPercentage.ToString();
		}
		return text;
	}

	protected void ComputedFileCount(object data)
	{
		if (data == null)
		{
			return;
		}
		string text = (data as List<object>).FirstOrDefault((object n) => n.ToString().EndsWith("xml"))?.ToString();
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		Dictionary<string, long> dictionary = new Dictionary<string, long>();
		foreach (Match item in Regex.Matches(File.ReadAllText(text), "<rom\\s+?index=\"(?<key>\\d+)\"\\s+?.*>(?<value>.+)</rom>", RegexOptions.Multiline))
		{
			dictionary.Add(item.Groups["key"].Value, new FileInfo(item.Groups["value"].Value).Length);
		}
		MTKFileCount = dictionary.Count;
	}
}
