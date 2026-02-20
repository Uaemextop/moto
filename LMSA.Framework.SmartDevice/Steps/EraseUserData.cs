using System.Windows;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class EraseUserData : BaseStep
{
	public override void Run()
	{
		bool flag = false;
		bool flag2 = false;
		bool? result;
		if (base.Info.Args.UnFastboot == true)
		{
			result = base.Recipe.UcDevice.MessageBox.EraseData().Result;
		}
		else
		{
			IAndroidDevice androidDevice = null;
			androidDevice = ((base.Recipe.Device.ConnectType != ConnectType.Adb) ? base.Recipe.Device.Property : GetDevice(ConnectType.Fastboot)?.Property);
			string text = base.Log.Info["fdr-allowed"];
			if (string.IsNullOrEmpty(text))
			{
				text = androidDevice?.GetPropertyValue("fdr-allowed")?.ToLower();
			}
			flag2 = text?.ToLower() == "no";
			LogHelper.LogInstance.Info($"read fdr-allowed: {text}, enterprise device: {flag2}");
			if (flag2)
			{
				result = base.Recipe.UcDevice.MessageBox.Show("K2130", "K1618", "K2129", "K0208", showClose: true, (MessageBoxImage)64, null, isPrivacy: false, isWarnYellow: true).Result;
				flag = result != true;
			}
			else
			{
				result = base.Recipe.UcDevice.MessageBox.EraseData().Result;
			}
		}
		if (flag)
		{
			base.Log.AddInfo("erase_personal_data", false.ToString());
			base.Log.AddResult(this, Result.MANUAL_QUIT, base.RunResult);
			return;
		}
		if (flag2)
		{
			base.RunResult = "unerase";
			base.Log.AddInfo("erase_personal_data", false.ToString());
		}
		else
		{
			base.RunResult = ((result != true) ? "unerase" : "erase");
			base.Log.AddInfo("erase_personal_data", (result == true).ToString());
		}
		base.Log.AddResult(this, Result.PASSED, base.RunResult);
	}
}
