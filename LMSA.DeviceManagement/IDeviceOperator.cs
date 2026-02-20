namespace lenovo.mbg.service.framework.services.Device;

public interface IDeviceOperator
{
	string Command(string command, int timeout = -1, string deviceID = "");
}
