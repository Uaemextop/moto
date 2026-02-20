using System.Collections.Generic;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class ShellResponseCfcflash : ShellResponse
{
	protected readonly Dictionary<string, ShellCmdStatus> MTekCfcResponseToStatus = new Dictionary<string, ShellCmdStatus>
	{
		{
			"all download successful!!!",
			ShellCmdStatus.Completed
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
			"fastboot: error",
			ShellCmdStatus.FastbootError
		},
		{
			"error:serialnocannotbenull",
			ShellCmdStatus.Error
		},
		{
			"Flashemmcsplloadererror",
			ShellCmdStatus.Error
		},
		{
			"Flashufssplloadererror",
			ShellCmdStatus.Error
		},
		{
			"Flashuboot_aerror",
			ShellCmdStatus.Error
		},
		{
			"Flashtrustos_aerror",
			ShellCmdStatus.Error
		},
		{
			"Flashteecfg_aerror",
			ShellCmdStatus.Error
		},
		{
			"Flashsml_aerror",
			ShellCmdStatus.Error
		},
		{
			"Eraseuboot_logerror",
			ShellCmdStatus.Error
		},
		{
			"backupnverror",
			ShellCmdStatus.Error
		},
		{
			"Flashl_fixnv1_aerror",
			ShellCmdStatus.Error
		},
		{
			"Flashgpterror",
			ShellCmdStatus.Error
		},
		{
			"Flashvbmeta_aerror",
			ShellCmdStatus.Error
		},
		{
			"Flashvbmeta_system_aerror",
			ShellCmdStatus.Error
		},
		{
			"Flashvbmeta_system_ext_aerror",
			ShellCmdStatus.Error
		},
		{
			"Flashvbmeta_vendor_aerror",
			ShellCmdStatus.Error
		},
		{
			"Flashvbmeta_product_aerror",
			ShellCmdStatus.Error
		},
		{
			"Flashvbmeta_odm_aerror",
			ShellCmdStatus.Error
		},
		{
			"Flashl_modem_aerror",
			ShellCmdStatus.Error
		},
		{
			"Flashl_deltanv_aerror",
			ShellCmdStatus.Error
		},
		{
			"Flashl_ldsp_aerror",
			ShellCmdStatus.Error
		},
		{
			"Flashl_gdsp_aerror",
			ShellCmdStatus.Error
		},
		{
			"Flashl_agdsp_aerror",
			ShellCmdStatus.Error
		},
		{
			"Flashpm_sys_aerror",
			ShellCmdStatus.Error
		},
		{
			"Flashboot_aerror",
			ShellCmdStatus.Error
		},
		{
			"Flashvendor_boot_aerror",
			ShellCmdStatus.Error
		},
		{
			"Flashinit_boot_aerror",
			ShellCmdStatus.Error
		},
		{
			"Flashdtbo_aerror",
			ShellCmdStatus.Error
		},
		{
			"Flashsupererror",
			ShellCmdStatus.Error
		},
		{
			"Flashcacheerror",
			ShellCmdStatus.Error
		},
		{
			"Flashblackboxerror",
			ShellCmdStatus.Error
		},
		{
			"Flashelabelerror",
			ShellCmdStatus.Error
		},
		{
			"Flashuserdataerror",
			ShellCmdStatus.Error
		},
		{
			"Flashlogoerror",
			ShellCmdStatus.Error
		},
		{
			"Flashfbootlogoerror",
			ShellCmdStatus.Error
		},
		{
			"Erasel_runtimenv1error",
			ShellCmdStatus.Error
		},
		{
			"Erasemetadataerror",
			ShellCmdStatus.Error
		},
		{
			"Erasesysdumpdberror",
			ShellCmdStatus.Error
		},
		{
			"Flashsplloadererror",
			ShellCmdStatus.Error
		},
		{
			"Erro:enduserflashfailed!!!",
			ShellCmdStatus.Error
		}
	};

	public override Dictionary<string, ShellCmdStatus> ResponseToStatus => MTekCfcResponseToStatus;

	public override ShellCmdType ShellCmd => ShellCmdType.MTekCfcFlashTool;

	public override string ComputedPercent(string response, string key)
	{
		return "0";
	}
}
