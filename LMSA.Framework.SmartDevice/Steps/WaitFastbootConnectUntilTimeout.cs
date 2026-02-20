using System;
using System.Collections.Generic;
using System.Management;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class WaitFastbootConnectUntilTimeout : BaseStep
{
	protected bool quit;

	private string TAG => GetType().FullName;

	public override void Run()
	{
		bool flag = false;
		quit = false;
		DriversHelper.CheckMotorolaDriverExeInstalled(delegate(string _arg)
		{
			base.Recipe.UcDevice.MessageBox.SetMainWindowDriverBtnStatus(_arg);
		});
		if (GetDevice(ConnectType.Fastboot) != null)
		{
			flag = true;
		}
		else
		{
			do
			{
				ShowConnectMessagebox();
				int num = 0;
				int num2 = base.Info.Args.Timeout ?? ((object)300000);
				while (!quit && num <= num2)
				{
					if (GetDevice(ConnectType.Fastboot) != null)
					{
						flag = true;
						break;
					}
					Thread.Sleep(1000);
					num += 1000;
				}
				base.Recipe.UcDevice.MessageBox.Close(true);
				if (quit)
				{
					break;
				}
				if (!flag)
				{
					string text = "Fastboot device isn't find!";
					bool flag2 = PrintDevStatus();
					if (flag2)
					{
						text += " will install driver";
					}
					base.Log.AddLog(text, upload: true);
					if (Retry > 0 && flag2)
					{
						string output = string.Empty;
						DriversHelper.CheckAndInstallInfDriver(DriverType.MTK, null, out output);
					}
					if (Retry > 0 && base.Info.Args.RetryText != null)
					{
						base.Recipe.UcDevice.MessageBox.Show(base.Info.Name, base.Info.Args.RetryText.ToString()).Wait();
					}
				}
			}
			while (!flag && Retry-- > 0);
		}
		if (quit)
		{
			base.Log.AddResult(this, Result.MANUAL_QUIT, "customer closes the connection pop-up window");
		}
		else if (!flag)
		{
			base.Log.AddResult(this, Result.FAILED, "Fastboot device isn't find!");
		}
		else
		{
			base.Log.AddResult(this, Result.PASSED);
		}
	}

	private void ShowConnectMessagebox()
	{
		if (base.Info.Args.ConnectTutorials is JObject jObject && jObject.Value<JArray>("Steps") != null)
		{
			string title = jObject.Value<string>("Title");
			JArray steps = jObject.Value<JArray>("Steps");
			bool autoPlay = jObject.Value<bool>("AutoPlay");
			double interval = jObject.Value<double>("Interval");
			string noteText = jObject.Value<string>("NoteText");
			LogHelper.LogInstance.Debug("Args.ConnectTutorials.Steps will show");
			Task.Run(delegate
			{
				if (!base.Recipe.UcDevice.MessageBox.AutoCloseConnectTutorials(title, steps, -1, autoPlay, interval, showPlayControl: true, showClose: true, popupWhenClose: true, noteText).HasValue)
				{
					quit = true;
				}
			});
		}
		else if (base.Info.Args.ConnectSteps != null)
		{
			ConnectStepInfo _connectStep = new ConnectStepInfo
			{
				NoteText = base.Info.Args.ConnectSteps.NoteText?.ToString(),
				RetryText = base.Info.Args.ConnectSteps.RetryText?.ToString(),
				WidthRatio = (base.Info.Args.ConnectSteps.WidthRatio?.ToString() ?? "1:2:2")
			};
			Task.Run(delegate
			{
				List<ConnectSteps> list = new List<ConnectSteps>();
				foreach (dynamic item in base.Info.Args.ConnectSteps.Steps)
				{
					list.Add(new ConnectSteps
					{
						StepImage = item.Image.ToString(),
						StepContent = item.Content.ToString()
					});
				}
				_connectStep.Steps = list;
				if (!base.Recipe.UcDevice.MessageBox.AutoCloseMoreStep(base.Name, _connectStep, -1, popupWhenClose: true).HasValue)
				{
					quit = true;
				}
			});
		}
		else
		{
			if (!((base.Info.Args.PromptText != null) ? true : false))
			{
				return;
			}
			Task.Run(delegate
			{
				dynamic val = base.Info.Args.Image?.ToString();
				dynamic val2 = base.Info.Args.PromptText.ToString();
				if (base.Recipe.UcDevice.MessageBox.AutoClose(base.Name, val2, val, showClose: true, popupWhenClose: true) == null)
				{
					quit = true;
				}
			});
		}
	}

	private bool PrintDevStatus()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		ManagementObjectCollection obj = new ManagementObjectSearcher("Select * From Win32_PnPEntity where service='WinUSB' or Status='ERROR'").Get();
		List<string> list = new List<string>();
		ManagementObjectCollection.ManagementObjectEnumerator enumerator = obj.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ManagementBaseObject current = enumerator.Current;
				string text = current.GetPropertyValue("Status") as string;
				string value = current.GetPropertyValue("Service") as string;
				if (!text.Equals("ERROR", StringComparison.CurrentCultureIgnoreCase) || string.IsNullOrEmpty(value))
				{
					list.Add(string.Format("Name: {0}\t  Status: {1}", current.GetPropertyValue("Name"), text));
				}
			}
		}
		finally
		{
			((IDisposable)enumerator)?.Dispose();
		}
		string output = string.Empty;
		if (list.Exists((string n) => n.Equals("Name: Android\t  Status: Error", StringComparison.CurrentCultureIgnoreCase)) || (list.Exists((string n) => n.Equals("Name: \t  Status: Error", StringComparison.CurrentCultureIgnoreCase)) && !DriversHelper.CheckAndInstallInfDriver(DriverType.MTK, null, out output)))
		{
			result = true;
		}
		if (base.Info.Args.Print == true && list.Count > 0)
		{
			base.Log.AddLog("::::::::::::::::::::::::::Detect device status:::::::::::::::::::::::::::::::::", upload: true);
			base.Log.AddLog(string.Join("\n", list), upload: true);
			base.Log.AddLog(":::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::", upload: true);
		}
		return result;
	}
}
