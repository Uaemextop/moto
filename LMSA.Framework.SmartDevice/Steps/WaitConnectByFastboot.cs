using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class WaitConnectByFastboot : BaseStep
{
	public override void Run()
	{
		Result result = Result.PASSED;
		if (base.TimeoutMilliseconds <= 0)
		{
			base.TimeoutMilliseconds = 20000;
		}
		string response = null;
		DeviceEx deviceEx = GetDevice(ConnectType.Fastboot);
		if (deviceEx == null && base.Recipe.Device != null)
		{
			deviceEx = HostProxy.deviceManager.ConntectedDevices.FirstOrDefault((DeviceEx n) => n.Identifer == base.Recipe.Device.Identifer);
			if (deviceEx != null)
			{
				base.Recipe.SetRecipeDevice(deviceEx);
			}
		}
		if (deviceEx == null)
		{
			int num = 0;
			int num2;
			do
			{
				ClearDeviceMonitorConnectedList();
				num++;
				Task<bool?> msgbxTask = base.Recipe.UcDevice.MessageBox.WaitByFastboot(base.Log.UseCase, base.Resources.Get(RecipeResources.ModelName) + "#" + base.Resources.Get(RecipeResources.RealModelName));
				num2 = FindDevice(msgbxTask);
				if (num2 == -1)
				{
					break;
				}
				base.Recipe.UcDevice.MessageBox.Close(true);
				if (num2 == 1)
				{
					break;
				}
				if ((Retry > 0 && base.Info.Args.RetryPromptText != null) && ((!base.Recipe.UcDevice.MessageBox.Show(base.Name, base.Info.Args.RetryPromptText.ToString()).Wait(base.TimeoutMilliseconds)) ? true : false))
				{
					num2 = 0;
					base.Recipe.UcDevice.MessageBox.Close(true);
					break;
				}
				base.Log.AddLog($"Not find fastboot device,connect timeout ! Execute count :{num}");
				ErrorConnectProcess();
			}
			while (Retry-- > 0);
			switch (num2)
			{
			case -1:
				result = Result.MANUAL_QUIT;
				response = "customer closes the connection pop-up window";
				break;
			case 1:
				result = Result.PASSED;
				break;
			default:
				result = Result.FASTBOOT_CONNECT_FAILED;
				response = "connect timeout";
				break;
			}
		}
		Thread.Sleep(2000);
		PrintConnectedDevice(result != Result.PASSED);
		base.Log.AddResult(this, result, response);
	}

	protected int FindDevice(Task<bool?> msgbxTask)
	{
		DateTime now = DateTime.Now;
		do
		{
			if (msgbxTask.IsCompleted)
			{
				return -1;
			}
			if (GetDevice(ConnectType.Fastboot) != null)
			{
				return 1;
			}
			Thread.Sleep(500);
		}
		while (DateTime.Now.Subtract(now).TotalMilliseconds < (double)base.TimeoutMilliseconds);
		return 0;
	}
}
