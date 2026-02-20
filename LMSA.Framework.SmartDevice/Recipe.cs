using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json.Linq;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.common.webservices.WebApiServices;
using lenovo.mbg.service.framework.resources;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.smartdevice.Steps;

namespace lenovo.mbg.service.framework.smartdevice;

public class Recipe
{
	protected readonly string RuntimeCheckStepName = "RuntimeCheck";

	public List<string> DeviceMonitorConnectedList = new List<string>();

	public List<string> DeviceMonitorCacheIds = new List<string>();

	public bool IsLoadPnPSignedDriver;

	public RecipeInfo Info { get; private set; }

	public ResultLogger Log => UcDevice.Log;

	public List<BaseStep> Steps { get; private set; }

	public SortedList<string, dynamic> Cache { get; private set; }

	public RecipeResources Resources { get; private set; }

	public DeviceEx Device => UcDevice.Device;

	public UseCaseDevice UcDevice { get; set; }

	public int ComPort { get; set; }

	public bool IsEraseDataFailed { get; set; }

	public bool DeviceMonitorRunning { get; set; }

	public Recipe(UseCaseDevice device)
	{
		Resources = device.Resources;
		UcDevice = device;
		Cache = new SortedList<string, object>();
		SetRecipeDevice(device.Device);
	}

	public virtual void SetRecipeDevice(DeviceEx device)
	{
		if (device != null)
		{
			if (Device != null)
			{
				Device.WorkType = DeviceWorkType.None;
			}
			device.WorkType = ((Log.UseCase == UseCase.LMSA_Recovery) ? DeviceWorkType.Rescue : DeviceWorkType.ReadFastboot);
			UcDevice.Device = device;
			Log.AddLog($"==========Set recipe device id: {Device?.Identifer}, modelname: {Device?.Property?.ModelName}, imei: {Device?.Property?.IMEI1}, connecttype: {Device?.ConnectType}, softstatus: {Device?.SoftStatus}, worktype: {Device?.WorkType} ==========", upload: true);
		}
	}

	public void FreeEventHandler(bool realFlash)
	{
		UcDevice.EventHandle.Set();
		if (realFlash)
		{
			UcDevice.RealFlash = true;
			UcDevice.Log.NotifyAsync(RecipeMessageType.REALFLASH, default(RecipeMessage));
		}
	}

	public void Load(RecipeInfo info)
	{
		Info = info;
		Steps = new List<BaseStep>();
		double num = 0.0;
		double num2 = 100.0 / (double)info.Steps.Count;
		foreach (StepInfo step in info.Steps)
		{
			double num3 = 1.0;
			if (step.Args != null && step.Args["ProgressFactor"] != null)
			{
				num3 = step.Args["ProgressFactor"];
			}
			num += num3 * num2;
		}
		double num4 = 0.0;
		int num5 = 1;
		if (Log.UseCase == UseCase.LMSA_Recovery)
		{
			Steps.Add(ConstructRunTimeCheckStep());
			num5 = 2;
		}
		foreach (StepInfo step2 in info.Steps)
		{
			BaseStep baseStep = StepHelper.LoadNew<BaseStep>(step2.Step);
			step2.Args.ProgressStart = num4;
			baseStep.Index = num5++;
			double num6 = 1.0;
			if (step2.Args["ProgressFactor"] != null)
			{
				num6 = step2.Args["ProgressFactor"];
			}
			double num7 = num6 * num2 / num;
			double num8 = num4 + num7 * 100.0;
			step2.Args.ProgressEnd = num8;
			num4 = num8;
			baseStep.Load(this, step2);
			Steps.Add(baseStep);
		}
		Steps.Last().Info.Args.ProgressEnd = 100.0;
	}

	private BaseStep ConstructRunTimeCheckStep()
	{
		JObject stepContent = new JObject
		{
			{ "Name", "Recipe execution environment check" },
			{ "Step", RuntimeCheckStepName },
			{
				"Args",
				new JObject()
			}
		};
		StepInfo stepInfo = new StepInfo();
		stepInfo.Load(stepContent);
		stepInfo.Args.ProgressStart = 0.0;
		stepInfo.Args.ProgressEnd = 0.0;
		BaseStep baseStep = StepHelper.LoadNew<BaseStep>(stepInfo.Step);
		baseStep.Index = 1;
		baseStep.Load(this, stepInfo);
		return baseStep;
	}

	private bool UnzipTool(out string toolDir)
	{
		string text = Resources.Get(RecipeResources.ToolZip);
		toolDir = Resources.Get(RecipeResources.TooL);
		if (!string.IsNullOrEmpty(text) && File.Exists(text) && !GlobalFun.Exists(toolDir) && !Rsd.Instance.UnzipTool(text, toolDir, out var response))
		{
			GlobalCmdHelper.Instance.Execute(new
			{
				type = GlobalCmdType.DELETE_ROM_AFTER_RESCUE,
				data = Path.GetFileName(text)
			});
			Log.AddLog("tool unzip failed: " + response, upload: true);
			UcDevice.MessageBox.Show("K0071", "K1600", "K0327", null, showClose: false, (MessageBoxImage)48).Wait();
			return false;
		}
		return true;
	}

	public void Run()
	{
		if (UnzipTool(out var toolDir))
		{
			Log.CurrentProgress = 0.0;
			string text = string.Empty;
			DriverInstall();
			bool flag = false;
			UcDevice.RecipeLocked = true;
			StartDeviceMonitorAsync();
			foreach (BaseStep step in Steps)
			{
				text = step.Info.Args.SuccessPromptText ?? string.Empty;
				bool flag2 = step.Info.Args.SkipCurrentStep ?? ((object)false);
				bool flag3 = step.Info.Args.AllowSkip ?? ((object)true);
				if (flag2 || (Log.OverallResult != Result.PASSED && flag3 && !flag))
				{
					Log.AddResult(step, flag2 ? Result.SKIPPED : Result.CANCELED);
					continue;
				}
				flag = false;
				try
				{
					Log.CurrentProgress = ((step.Info.Args.ProgressEnd >= 100) ? ((object)100) : step.Info.Args.ProgressEnd);
					if (step.VerifyPreContionMet())
					{
						Log.AddStart(step);
						step.Setup();
						List<BaseStep> subSteps = step.SubSteps;
						if (subSteps != null && subSteps.Count > 0)
						{
							RunSubStep(step);
						}
						else
						{
							RunStep(step);
						}
					}
					else
					{
						Log.AddResult(step, Result.SKIPPED);
					}
				}
				catch (Exception ex)
				{
					step.RunResult = Result.ABORTED.ToString();
					Log.AddResult(step, Result.ABORTED, ex.ToString());
				}
				finally
				{
					if (string.IsNullOrEmpty(step.RunResult))
					{
						step.RunResult = Log.OverallResult.ToString();
					}
					flag = step.IgnoreCurrStepResult;
					if (step.StepResult == Result.FAILED)
					{
						string text2 = step.Info.Args.ErrorPromptText?.ToString();
						if (!string.IsNullOrEmpty(text2))
						{
							UcDevice.MessageBox.Show(step.Info.Name, text2, "K0327", null, showClose: false, (MessageBoxImage)48).Wait();
						}
					}
				}
			}
			UcDevice.RecipeLocked = false;
			UcDevice.MessageBox.SetMainWindowDriverBtnStatus("none");
			GlobalFun.DeleteDirectoryEx(toolDir);
			if (Log.OverallResult == Result.PASSED)
			{
				if (IsEraseDataFailed && Log.OverallResult == Result.PASSED)
				{
					Log.FailedResult = Result.FASTBOOT_FLASH_ERASEDATE_FAILED;
				}
				GetDeviceInfo();
				if (!string.IsNullOrEmpty(text))
				{
					Log.SuccessPromptText = text;
				}
			}
			StopDeviceMonitor();
			Thread.Sleep(1000);
			Log.Dispose();
		}
		else
		{
			Log.NotifyAsync(RecipeMessageType.UNDO, default(RecipeMessage));
		}
		if (!UcDevice.EventHandle.SafeWaitHandle.IsClosed)
		{
			FreeEventHandler(realFlash: false);
		}
	}

	public void StartDeviceMonitorAsync()
	{
		Task.Run(async delegate
		{
			DeviceMonitorRunning = true;
			IsLoadPnPSignedDriver = false;
			DeviceMonitorConnectedList = new List<string>();
			DeviceMonitorCacheIds = new List<string>();
			int idx = 0;
			do
			{
				List<string> list = new List<string>();
				ManagementObjectSearcher val = new ManagementObjectSearcher("Select * From Win32_PnPEntity");
				try
				{
					ManagementObjectSearcher val2 = new ManagementObjectSearcher("Select * From Win32_USBController");
					try
					{
						List<ManagementBaseObject> list2 = new List<ManagementBaseObject>();
						var enumerator = val.Get().GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								ManagementBaseObject current = enumerator.Current;
								list2.Add(current);
							}
						}
						finally
						{
							((IDisposable)enumerator)?.Dispose();
						}
						enumerator = val2.Get().GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								ManagementBaseObject current2 = enumerator.Current;
								list2.Add(current2);
							}
						}
						finally
						{
							((IDisposable)enumerator)?.Dispose();
						}
						string dateTime = DateTime.Now.ToString("HH:mm:ss.ffffff");
						foreach (ManagementBaseObject item in list2)
						{
							try
							{
								string text = item.GetPropertyValue("DeviceID") as string;
								if (!string.IsNullOrEmpty(text))
								{
									string text2 = item.GetPropertyValue("Name") as string;
									string text3 = item.GetPropertyValue("Manufacturer") as string;
									string text4 = item.GetPropertyValue("Status") as string;
									list.Add(text);
									if (idx == 0 && !DeviceMonitorCacheIds.Contains(text))
									{
										DeviceMonitorCacheIds.Add(text);
									}
									else if (idx > 0 && !DeviceMonitorCacheIds.Contains(text))
									{
										IsLoadPnPSignedDriver = true;
										DeviceMonitorCacheIds.Add(text);
										string text5 = dateTime + ",Name: " + text2 + ", DeviceId: " + text + ", Manufacturer: " + text3 + ", Status: " + text4;
										if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text4) && text4.Equals("ok", StringComparison.CurrentCultureIgnoreCase))
										{
											ManagementObjectSearcher val3 = new ManagementObjectSearcher("Select * From Win32_PnPSignedDriver");
											try
											{
												enumerator = val3.Get().GetEnumerator();
												try
												{
													while (enumerator.MoveNext())
													{
														ManagementObject val4 = (ManagementObject)enumerator.Current;
														if (((ManagementBaseObject)val4).GetPropertyValue("DeviceID") as string == text)
														{
															string text6 = ((ManagementBaseObject)val4).GetPropertyValue("DriverProviderName") as string;
															string text7 = ((ManagementBaseObject)val4).GetPropertyValue("DriverVersion") as string;
															string text8 = ((ManagementBaseObject)val4).GetPropertyValue("DriverDate") as string;
															string text9 = ((ManagementBaseObject)val4).GetPropertyValue("Signer") as string;
															text5 = text5 + ", DriverProviderName: " + text6 + ", DriverVersion: " + text7 + ", DriverDate: " + text8 + ", Signer: " + text9;
															break;
														}
													}
												}
												finally
												{
													((IDisposable)enumerator)?.Dispose();
												}
											}
											finally
											{
												((IDisposable)val3)?.Dispose();
											}
										}
										DeviceMonitorConnectedList.Add(text5);
										IsLoadPnPSignedDriver = false;
										Log.AddLog("device connected: " + text5, upload: true);
									}
								}
							}
							catch
							{
							}
						}
						DeviceMonitorCacheIds.Except(list).ToList().ForEach(delegate(string n)
						{
							Log.AddLog("device removed: " + dateTime + "，" + n, upload: true);
							DeviceMonitorCacheIds.Remove(n);
						});
					}
					finally
					{
						((IDisposable)val2)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
				int num = idx + 1;
				idx = num;
				await Task.Delay(1000).ConfigureAwait(continueOnCapturedContext: false);
			}
			while (DeviceMonitorRunning);
		});
	}

	public void StopDeviceMonitor()
	{
		DeviceMonitorRunning = false;
	}

	public void Dispose()
	{
		if (Steps != null && Steps.Count != 0)
		{
			Steps.ForEach(delegate(BaseStep p)
			{
				p.Dispose();
			});
		}
	}

	private void GetDeviceInfo()
	{
		if (Device != null && Device.Property != null)
		{
			Log.AddLog($"device info [{Device.ConnectType}]: {JsonHelper.SerializeObject2Json(Device.Property.Others)}");
		}
	}

	private void RunStep(BaseStep step)
	{
		step.Run();
		if (step.Info.Args.Retesting == true)
		{
			Log.AddLog($"Re-running step {step.Info.Name} ({step.Info.Step})", upload: true);
			step.Run();
		}
	}

	private void RunSubStep(BaseStep step)
	{
		if ("before".Equals(step.condition, StringComparison.InvariantCultureIgnoreCase))
		{
			foreach (BaseStep subStep in step.SubSteps)
			{
				Log.AddLog($"before::Running substep '{subStep.Info.Name} ({subStep.Info.Step})'");
				subStep.Run();
			}
			RunStep(step);
			return;
		}
		if ("after".Equals(step.condition, StringComparison.InvariantCultureIgnoreCase))
		{
			RunStep(step);
			{
				foreach (BaseStep subStep2 in step.SubSteps)
				{
					Log.AddLog($"after::Running substep '{subStep2.Info.Name} ({subStep2.Info.Step})'");
					subStep2.Run();
				}
				return;
			}
		}
		RunStep(step);
	}

	private void DriverInstall()
	{
		DriverType drivertype = DriverType.Lenovo;
		string text = Resources.Get(RecipeResources.RealModelName);
		string value = Resources.Get("Platform");
		if (!CheckDriverWhiteList(text))
		{
			if (Resources.Get("IsFastboot") == true.ToString())
			{
				drivertype = DriverType.Motorola;
				string text2 = DriversHelper.CheckMotorolaDriverExeInstalled(delegate(string _arg)
				{
					UcDevice.MessageBox.SetMainWindowDriverBtnStatus(_arg);
				});
				LogHelper.LogInstance.Debug(text2);
				Log.AddLog(text2, upload: true);
			}
			else if ("MTK".Equals(value, StringComparison.CurrentCultureIgnoreCase))
			{
				drivertype = DriverType.MTK;
			}
			else if ("Unisoc".Equals(value, StringComparison.CurrentCultureIgnoreCase))
			{
				drivertype = ((string.IsNullOrEmpty(text) || !Regex.IsMatch(text, "L19111", RegexOptions.IgnoreCase)) ? DriverType.Unisoc : DriverType.Unisoc_L19111);
			}
			if (!string.IsNullOrEmpty(text))
			{
				if (Regex.IsMatch(text, "SP101FU", RegexOptions.IgnoreCase))
				{
					drivertype = DriverType.PNP;
				}
				else if (Regex.IsMatch(text, "CD-17302F", RegexOptions.IgnoreCase))
				{
					drivertype = DriverType.ADBDRIVER;
				}
			}
			DriversHelper.CheckAndInstallInfDriver(drivertype, null, out var output);
			if (!string.IsNullOrEmpty(output))
			{
				LogHelper.LogInstance.Debug(output);
				Log.AddLog(output, upload: true);
			}
		}
		UcDevice.MessageBox.SetMainWindowDriverBtnStatus("installed");
	}

	private bool CheckDriverWhiteList(string _modelName)
	{
		string output = string.Empty;
		if (string.IsNullOrEmpty(_modelName))
		{
			return false;
		}
		List<string> list = new ApiService().RequestContent<List<string>>(WebApiUrl.RESUCE_CHECK_MODEL_NAME_DRIVERS, new
		{
			modelName = _modelName
		});
		if (list != null && list.Count > 0)
		{
			foreach (string item in list)
			{
				if (!Enum.TryParse<DriverType>(item, out var result))
				{
					continue;
				}
				if (result == DriverType.Motorola)
				{
					string text = DriversHelper.CheckMotorolaDriverExeInstalled(delegate(string _arg)
					{
						UcDevice.MessageBox.SetMainWindowDriverBtnStatus(_arg);
					});
					LogHelper.LogInstance.Debug(text);
					Log.AddLog(text, upload: true);
				}
				DriversHelper.CheckAndInstallInfDriver(result, null, out output);
				if (!string.IsNullOrEmpty(output))
				{
					LogHelper.LogInstance.Debug(output);
					Log.AddLog(output, upload: true);
				}
			}
			return true;
		}
		foreach (KeyValuePair<string, List<DriverType>> driver_White_ in DriversHelper.Driver_White_List)
		{
			if (!Regex.IsMatch(_modelName, driver_White_.Key, RegexOptions.IgnoreCase))
			{
				continue;
			}
			foreach (DriverType item2 in driver_White_.Value)
			{
				if (item2 == DriverType.Motorola)
				{
					string text2 = DriversHelper.CheckMotorolaDriverExeInstalled(delegate(string _arg)
					{
						UcDevice.MessageBox.SetMainWindowDriverBtnStatus(_arg);
					});
					LogHelper.LogInstance.Debug(text2);
					Log.AddLog(text2, upload: true);
				}
				DriversHelper.CheckAndInstallInfDriver(item2, null, out output);
				if (!string.IsNullOrEmpty(output))
				{
					LogHelper.LogInstance.Debug(output);
					Log.AddLog(output, upload: true);
				}
			}
			return true;
		}
		return false;
	}
}
