namespace lenovo.mbg.service.framework.smartdevice.Steps;

public enum ShellCmdStatus
{
	None = 0,
	Connecting = 1,
	Connected = 2,
	Downloading = 3,
	Completed = 4,
	Outputing = 5,
	Authenticating = 6,
	Writing = 7,
	RomUnMatchError = 8,
	AuthorizedError = 9,
	FastbootError = 10,
	FileLostError = 11,
	FastbootDegrade = 12,
	ConditionQuit = 13,
	Error = -1
}
