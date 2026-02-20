using System.Collections.Generic;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class ShellResponseSpflashtool : ShellResponseFlashtool
{
	public override ShellCmdType ShellCmd => ShellCmdType.MTekSpFlashTool;

	public override Dictionary<string, ShellCmdStatus> ResponseToStatus => MTekResponseToStatus;

	public override void Init(string category = null, object data = null)
	{
		ComputedFileCount(data);
	}
}
