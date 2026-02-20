using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using lenovo.mbg.service.common.log;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class FindLocationPort : BaseStep
{
	protected bool quit;

	public override void Run()
	{
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Expected O, but got Unknown
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Expected O, but got Unknown
		DateTime now = DateTime.Now;
		int num = base.Info.Args.Timeout ?? ((object)180000);
		bool flag = false;
		bool flag2 = false;
		Task task = null;
		List<string> list = base.Info.Args.ComPorts.ToObject<List<string>>();
		do
		{
			ManagementObjectSearcher val = new ManagementObjectSearcher("Select * From Win32_PnPEntity");
			try
			{
				ManagementObjectCollection.ManagementObjectEnumerator enumerator = val.Get().GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						ManagementObject val2 = (ManagementObject)enumerator.Current;
						try
						{
							string name = ((ManagementBaseObject)val2).GetPropertyValue("Name") as string;
							if (!list.Exists((string n) => Regex.IsMatch(name, n, RegexOptions.IgnoreCase)))
							{
								continue;
							}
							object[] array = new object[2]
							{
								new string[10] { "DEVPKEY_Device_DeviceDesc", "DEVPKEY_Device_Parent", "DEVPKEY_Device_LocationInfo", "DEVPKEY_Device_LocationPaths", "DEVPKEY_Device_InstanceId", "DEVPKEY_Device_Driver", "DEVPKEY_Device_DriverProvider", "DEVPKEY_Device_DriverVersion", "DEVPKEY_Device_DriverInfPath", "DEVPKEY_Device_Manufacturer" },
								null
							};
							val2.InvokeMethod("GetDeviceProperties", array);
							ManagementBaseObject[] obj = (ManagementBaseObject[])array[1];
							PropertyData? obj2 = ((IEnumerable)obj[2].Properties).OfType<PropertyData>().FirstOrDefault((Func<PropertyData, bool>)((PropertyData p) => p.Name == "Data"));
							string text = ((obj2 != null) ? obj2.Value : null) as string;
							PropertyData? obj3 = ((IEnumerable)obj[4].Properties).OfType<PropertyData>().FirstOrDefault((Func<PropertyData, bool>)((PropertyData p) => p.Name == "Data"));
							string text2 = ((obj3 != null) ? obj3.Value : null) as string;
							PropertyData? obj4 = ((IEnumerable)obj[5].Properties).OfType<PropertyData>().FirstOrDefault((Func<PropertyData, bool>)((PropertyData p) => p.Name == "Data"));
							object obj5 = ((obj4 != null) ? obj4.Value : null);
							PropertyData? obj6 = ((IEnumerable)obj[6].Properties).OfType<PropertyData>().FirstOrDefault((Func<PropertyData, bool>)((PropertyData p) => p.Name == "Data"));
							object obj7 = ((obj6 != null) ? obj6.Value : null);
							PropertyData? obj8 = ((IEnumerable)obj[7].Properties).OfType<PropertyData>().FirstOrDefault((Func<PropertyData, bool>)((PropertyData p) => p.Name == "Data"));
							object obj9 = ((obj8 != null) ? obj8.Value : null);
							PropertyData? obj10 = ((IEnumerable)obj[8].Properties).OfType<PropertyData>().FirstOrDefault((Func<PropertyData, bool>)((PropertyData p) => p.Name == "Data"));
							object obj11 = ((obj10 != null) ? obj10.Value : null);
							PropertyData? obj12 = ((IEnumerable)obj[9].Properties).OfType<PropertyData>().FirstOrDefault((Func<PropertyData, bool>)((PropertyData p) => p.Name == "Data"));
							object obj13 = ((obj12 != null) ? obj12.Value : null);
							if (!string.IsNullOrEmpty(text))
							{
								base.Log.AddLog($"{text2} | {text} | {obj5} | {obj7} | {obj9} | {obj11} | {obj13}");
								base.Cache[list.First()] = text;
								flag = true;
								task = Task.Run(() => ShowReleaseTutorials());
								break;
							}
						}
						catch (Exception)
						{
						}
					}
				}
				finally
				{
					((IDisposable)enumerator)?.Dispose();
				}
				if (!flag && !flag2)
				{
					flag2 = true;
					Task.Run(() => ShowConnectTutorials());
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		while (!quit && !flag && DateTime.Now.Subtract(now).TotalMilliseconds < (double)num);
		task?.Wait(10000);
		base.Recipe.UcDevice.MessageBox.Close(true);
		Result result = (quit ? Result.MANUAL_QUIT : (flag ? Result.PASSED : Result.FIND_LOCATIONPORT_FAILED));
		base.Log.AddResult(this, result, (result == Result.PASSED) ? null : "find location port failed");
	}

	public async Task ShowConnectTutorials()
	{
		if (base.Info.Args.ConnectTutorials is JObject jObject && jObject.Value<JArray>("Steps") != null)
		{
			await Show(jObject, "Steps");
		}
		else
		{
			if (!((base.Info.Args.PromptText != null) ? true : false))
			{
				return;
			}
			await Task.Run(delegate
			{
				string image = base.Info.Args.Image?.ToString();
				string message = base.Info.Args.PromptText.ToString();
				if (!base.Recipe.UcDevice.MessageBox.AutoClose(base.Name, message, image, null, -1, null, showClose: true, popupWhenClose: true, format: true, true).HasValue)
				{
					quit = true;
				}
			});
		}
	}

	public async Task ShowReleaseTutorials()
	{
		if (base.Info.Args.ConnectTutorials is JObject jObject && jObject.Value<JArray>("ReleaseSteps") != null)
		{
			await Show(jObject, "ReleaseSteps");
		}
	}

	protected Task Show(JObject jobj, string stepKey)
	{
		return Task.Run(delegate
		{
			bool flag = stepKey == "ReleaseSteps";
			string title = jobj.Value<string>("Title");
			JArray steps = jobj.Value<JArray>(stepKey);
			bool autoPlay = jobj.Value<bool>("AutoPlay");
			double interval = jobj.Value<double>("Interval");
			LogHelper.LogInstance.Debug("Args.ConnectTutorials." + stepKey + " will show");
			bool? flag2 = base.Recipe.UcDevice.MessageBox.AutoCloseConnectTutorials(title, steps, -1, autoPlay, interval, !flag, !flag, !flag);
			if (!flag && !flag2.HasValue)
			{
				quit = true;
			}
		});
	}
}
