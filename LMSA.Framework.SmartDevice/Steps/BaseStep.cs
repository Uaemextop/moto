using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json.Linq;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public abstract class BaseStep : IDisposable
{
	private static readonly Random rand = new Random();

	protected bool audited;

	protected List<string> CacheComports = new List<string>();

	protected static object locker = new object();

	public string Name => Info.Name;

	public Recipe Recipe { get; private set; }

	public StepInfo Info { get; private set; }

	protected ResultLogger Log => Recipe.Log;

	protected SortedList<string, dynamic> Cache => Recipe.Cache;

	protected RecipeResources Resources => Recipe.Resources;

	protected SortedList<string, dynamic> CheckedLimits { get; private set; }

	protected AutoResetEvent WaitEvent { get; private set; }

	protected int TimeoutMilliseconds { get; set; }

	public string condition { get; private set; }

	public List<BaseStep> SubSteps { get; private set; }

	protected List<string> SkipCommands { get; private set; }

	protected List<JObject> ConditionSkipCommands { get; private set; }

	protected string OutCondition { get; private set; }

	protected List<string> IgnoreResultCommands { get; private set; }

	public virtual int Retry { get; set; }

	public int Index { get; set; }

	public string RunResult { get; set; }

	public Result StepResult { get; set; }

	public bool IgnoreCurrStepResult { get; private set; }

	public bool IgnoreFinalResult { get; private set; }

	public bool StartDeviceMonitor { get; private set; }

	public abstract void Run();

	public virtual bool RunSubSteps()
	{
		if (SubSteps != null && SubSteps.Count > 0)
		{
			foreach (BaseStep subStep in SubSteps)
			{
				Log.AddLog($"Middle::Running substep '{subStep.Info.Name}({subStep.Info.Step})'");
				subStep.Run();
				if (Log.OverallResult == Result.QUIT || Log.OverallResult == Result.FAILED)
				{
					return false;
				}
			}
		}
		return true;
	}

	protected string AnalysisFailedResponse(string response)
	{
		if (string.IsNullOrEmpty(response))
		{
			return null;
		}
		return response.Split(new string[2] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).Reverse().ToList()
			.FirstOrDefault((string n) => Regex.IsMatch(n, "(fail)|(error)", RegexOptions.IgnoreCase));
	}

	public virtual DeviceEx GetDevice(ConnectType connectType, Predicate<object> predicate = null)
	{
		lock (locker)
		{
			IList<DeviceEx> conntectedDevices = HostProxy.deviceManager.ConntectedDevices;
			DeviceEx deviceEx = null;
			Predicate<object> tp = predicate ?? ((Predicate<object>)((object s) => true));
			if (conntectedDevices != null && conntectedDevices.Count > 0)
			{
				deviceEx = ((Recipe.Device == null) ? conntectedDevices.FirstOrDefault((DeviceEx n) => n.WorkType == DeviceWorkType.None && n.ConnectType == connectType && tp(n)) : conntectedDevices.FirstOrDefault((DeviceEx n) => n.ConnectType == connectType && n.Identifer == Recipe.Device.Identifer && tp(n)));
			}
			Recipe.SetRecipeDevice(deviceEx);
			return deviceEx;
		}
	}

	public void PrintConnectedDevice(bool print)
	{
		if (!print)
		{
			return;
		}
		Log.AddLog("current bind device: " + Recipe.Device?.Identifer, upload: true);
		IList<DeviceEx> conntectedDevices = HostProxy.deviceManager.ConntectedDevices;
		if (conntectedDevices == null || conntectedDevices.Count == 0)
		{
			Log.AddLog("No device is connected in the device list", upload: true);
		}
		foreach (DeviceEx item in conntectedDevices)
		{
			Log.AddLog($"device-{item.Identifer}, worktype: {item.WorkType}, softStatus: {item.SoftStatus}", upload: true);
		}
	}

	public void Load(Recipe recipe, StepInfo info)
	{
		Recipe = recipe;
		Info = info;
		SkipCommands = new List<string>();
		IgnoreResultCommands = new List<string>();
		Retry = 0;
		if (info.Args != null)
		{
			IgnoreCurrStepResult = Info.Args.IgnoreCurrStepResult ?? ((object)false);
			IgnoreFinalResult = Info.Args.IgnoreFinalResult ?? ((object)false);
			if (info.Args.SkipCommands != null)
			{
				SkipCommands = info.Args.SkipCommands.ToObject<List<string>>();
			}
			if (info.Args.IgnoreResultCommands != null)
			{
				IgnoreResultCommands = info.Args.IgnoreResultCommands.ToObject<List<string>>();
			}
			if (info.Args.Retry != null)
			{
				Retry = info.Args.Retry;
			}
			if (info.Args.ConditionSkipCommands != null)
			{
				OutCondition = info.Args.ConditionSkipCommands.condition;
				if (info.Args.ConditionSkipCommands.Commands != null && info.Args.ConditionSkipCommands.Commands is JArray)
				{
					ConditionSkipCommands = info.Args.ConditionSkipCommands.Commands.ToObject<List<JObject>>();
				}
			}
			if (Info.Args.StartDeviceMonitor != null)
			{
				StartDeviceMonitor = Info.Args.StartDeviceMonitor;
			}
		}
		if (info.SubSteps != null)
		{
			condition = info.SubSteps.Condition?.Value;
			if (info.SubSteps.Steps != null)
			{
				SubSteps = new List<BaseStep>();
				foreach (dynamic item in info.SubSteps.Steps)
				{
					StepInfo stepInfo = new StepInfo();
					stepInfo.Load(item);
					BaseStep baseStep = StepHelper.LoadNew<BaseStep>(stepInfo.Step);
					baseStep.Load(Recipe, stepInfo);
					SubSteps.Add(baseStep);
				}
			}
		}
		CheckedLimits = new SortedList<string, object>();
		if (Info.Args?.Timeout != null)
		{
			TimeoutMilliseconds = Info.Args?.Timeout;
			WaitEvent = new AutoResetEvent(initialState: false);
		}
	}

	public virtual bool Audit()
	{
		double? num = Info.Args.AuditPercent;
		if (!num.HasValue)
		{
			return true;
		}
		if (rand.NextDouble() >= num)
		{
			return true;
		}
		LogHelper.LogInstance.Debug($"Audit selected for {Name}({Info.Step})");
		bool flag = true;
		if (Info.Args.AuditSetup != null)
		{
			flag = Info.Args.AuditSetup;
		}
		try
		{
			if (flag)
			{
				Setup();
				if (Info.Args.AuditSettings != null)
				{
					Set(Info.Args.SettingsType.ToString(), Info.Args.AuditSettings);
				}
			}
			string type = Info.Args.PromptType;
			string text = Info.Args.PromptText;
			Result num2 = Prompt(type, text);
			audited = true;
			if (num2 == Result.PASSED)
			{
				LogHelper.LogInstance.Debug($"Audit failure for {Name}({Info.Step})");
				return false;
			}
			LogHelper.LogInstance.Debug($"Audit complete for {Name}({Info.Step})");
		}
		finally
		{
			if (flag)
			{
				TearDown();
			}
		}
		return true;
	}

	public virtual string LoadToolPath(string exe = null)
	{
		exe = exe ?? Info.Args.EXE;
		string text = Info.Args.FindLocation;
		if (string.IsNullOrEmpty(exe))
		{
			exe = "fastboot.exe";
		}
		string text2 = null;
		if (string.IsNullOrEmpty(text))
		{
			text2 = Resources.GetFastbootToolPath(exe, "ROM");
			if (string.IsNullOrEmpty(text2))
			{
				text2 = Resources.GetFastbootToolPath(exe, "TOOL");
				if (string.IsNullOrEmpty(text2))
				{
					text2 = Resources.GetFastbootToolPath(exe);
				}
			}
		}
		else if (text.Equals("TOOL"))
		{
			text2 = Resources.GetFastbootToolPath(exe, "TOOL");
			if (string.IsNullOrEmpty(text2))
			{
				text2 = Resources.GetFastbootToolPath(exe, "ROM");
				if (string.IsNullOrEmpty(text2))
				{
					text2 = Resources.GetFastbootToolPath(exe);
				}
			}
		}
		else if (text.Equals("INSTALL"))
		{
			text2 = (text2 = Resources.GetFastbootToolPath(exe));
			if (string.IsNullOrEmpty(text2))
			{
				text2 = Resources.GetFastbootToolPath(exe, "ROM");
				if (string.IsNullOrEmpty(text2))
				{
					Resources.GetFastbootToolPath(exe, "TOOL");
				}
			}
		}
		if (string.IsNullOrEmpty(text2))
		{
			Log.AddLog(exe + " not found", upload: true);
		}
		else if (!GlobalFun.Exists(text2))
		{
			Log.AddLog(text2 + " not exists", upload: true);
		}
		else
		{
			Log.AddLog("fastboot tool path: " + text2, upload: true);
		}
		return text2;
	}

	public virtual void TearDown()
	{
		foreach (string key in CheckedLimits.Keys)
		{
			string text = $"{Name}-{key}";
			double num = CheckedLimits[key].Min;
			double num2 = CheckedLimits[key].Max;
			double num3 = CheckedLimits[key].Value;
			Result result = CheckedLimits[key].Result;
			LogHelper logInstance = LogHelper.LogInstance;
			object[] args = new string[5]
			{
				text,
				num.ToString(),
				num3.ToString(),
				num2.ToString(),
				result.ToString()
			};
			logInstance.Info(string.Format("{0} - {1} < {2} < {3}: {4}", args));
		}
	}

	public virtual void Setup()
	{
	}

	protected void ProgressUpdate(double progress)
	{
		double num = Info.Args["ProgressStart"];
		double num2 = (double)Info.Args["ProgressEnd"] - num;
		double progress2 = num + num2 * (progress / 100.0);
		RecipeMessage recipeMessage = new RecipeMessage
		{
			Progress = progress2,
			UseCase = Log.UseCase,
			OverallResult = Result.PROGRESS
		};
		Log.NotifyAsync(RecipeMessageType.PROGRESS, recipeMessage);
	}

	protected virtual void Set(string settingType, dynamic settings)
	{
	}

	protected Result Prompt(string type, string text)
	{
		if (audited)
		{
			text = "Please re-check: " + text;
		}
		List<string> list = new List<string>();
		foreach (object item2 in Info.Args.ButtonContent)
		{
			string item = (string)(dynamic)item2;
			list.Add(item);
		}
		string ok = null;
		string cancel = null;
		if (list.Count == 1)
		{
			ok = list[0];
		}
		else if (list.Count == 2)
		{
			ok = list[0];
			cancel = list[1];
		}
		if (Recipe.UcDevice.MessageBox.Show(Info.Name, text, ok, cancel, showClose: false, (MessageBoxImage)64).Result != true)
		{
			return Result.QUIT;
		}
		return Result.PASSED;
	}

	public void ErrorConnectProcess(bool checkErrorConnect = true)
	{
		string value = Resources.Get("Mutil");
		if (!string.IsNullOrEmpty(value) && !bool.Parse(value) && checkErrorConnect)
		{
			while (Recipe.IsLoadPnPSignedDriver)
			{
				Thread.Sleep(1000);
			}
			ErrorConnectProcess(Recipe.DeviceMonitorConnectedList);
		}
	}

	private void ErrorConnectProcess(List<string> list)
	{
		if (!(Info.Args.BlackList is JObject { HasValues: not false } jObject) || jObject.Value<JArray>("Condition") == null)
		{
			return;
		}
		List<string> list2 = (from n in jObject.Value<JArray>("Condition")
			select n.Value<string>()).ToList();
		IEnumerator<string> enumerator = list.GetEnumerator();
		bool flag = false;
		while (!flag && enumerator.MoveNext())
		{
			flag = list2.Exists((string n) => enumerator.Current.IndexOf(n, StringComparison.CurrentCultureIgnoreCase) >= 0);
		}
		string text = jObject.Value<string>("Message");
		if (flag && !string.IsNullOrEmpty(text) && !Recipe.UcDevice.MessageBox.Show("K0711", text, "K0327", null, showClose: false, (MessageBoxImage)64).Wait(30000))
		{
			Recipe.UcDevice.MessageBox.Close(true);
		}
	}

	protected void ClearDeviceMonitorConnectedList()
	{
		if (Recipe.DeviceMonitorConnectedList == null || Recipe.DeviceMonitorConnectedList.Count == 0)
		{
			return;
		}
		Recipe.DeviceMonitorCacheIds = Recipe.DeviceMonitorCacheIds.Where((string b) => !Recipe.DeviceMonitorConnectedList.Any((string a) => a.Contains(b))).ToList();
		Recipe.DeviceMonitorConnectedList = new List<string>();
	}

	protected Task<List<string>> ComportMonitorTask(CancellationTokenSource tokenSource)
	{
		return Task.Run(delegate
		{
			List<string> result = new List<string>();
			List<string> list = new List<string>();
			new List<string>();
			CacheComports = new List<string>();
			do
			{
				List<string> comInfo = GlobalFun.GetComInfo();
				list = comInfo.Except(CacheComports).ToList();
				list.ForEach(delegate(string n)
				{
					result.Add($"device connected-{DateTime.Now:HH:mm:ss}: {n}");
				});
				CacheComports.AddRange(list);
				CacheComports.Except(comInfo).ToList().ForEach(delegate(string n)
				{
					result.Add($"device removed-{DateTime.Now:HH:mm:ss}: {n}");
					CacheComports.Remove(n);
				});
				Thread.Sleep(1000);
			}
			while (!tokenSource.IsCancellationRequested);
			if (result.Count == 0)
			{
				result.Add("no device connected");
			}
			return result;
		}, tokenSource.Token);
	}

	protected void PrintUnknownDevice()
	{
		List<string> list = new List<string>();
		ManagementObjectSearcher val = new ManagementObjectSearcher("Select * From Win32_PnPEntity where service='WINUSB' or Status='Error'");
		try
		{
			var enumerator = val.Get().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ManagementBaseObject current = enumerator.Current;
					string text = current.GetPropertyValue("Status") as string;
					if (string.IsNullOrEmpty(text) || text.Equals("error", StringComparison.CurrentCultureIgnoreCase))
					{
						string text2 = current.GetPropertyValue("Name").ToString();
						string text3 = null;
						try
						{
							text3 = current.GetPropertyValue("DeviceID").ToString();
						}
						catch
						{
						}
						list.Add("Name: " + text2 + ", DeviceId: " + text3 + ", Status: " + text);
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
			((IDisposable)val)?.Dispose();
		}
		if (list.Count > 0)
		{
			Log.AddLog("unknown device list: " + Environment.NewLine + string.Join("\r\n", list), upload: true);
		}
	}

	public bool VerifyPreContionMet()
	{
		if (Info.Args.PreCondTest != null && Info.Args.PreCondValue != null)
		{
			List<string> list = new List<string>();
			if (Info.Args.PreCondTest is JArray { HasValues: not false } jArray)
			{
				list = jArray.Values<string>().ToList();
			}
			else
			{
				list.Add((string)Info.Args.PreCondTest);
			}
			string[] source = ((string)Info.Args.PreCondValue).Split(',', ';');
			bool flag = true;
			foreach (string condTest in list)
			{
				BaseStep baseStep = Recipe.Steps.FirstOrDefault((BaseStep n) => n.Name == condTest);
				if (baseStep != null && !string.IsNullOrEmpty(baseStep.RunResult))
				{
					flag = source.Contains<string>(baseStep.RunResult, StringComparer.CurrentCultureIgnoreCase);
					if (flag)
					{
						break;
					}
				}
			}
			return flag;
		}
		return true;
	}

	protected string EncapsulationFastbootCommand(string command)
	{
		if (!string.IsNullOrEmpty(command))
		{
			string text = Recipe.Device?.Identifer;
			if (!string.IsNullOrEmpty(text))
			{
				command = "-s " + text + " " + command;
			}
		}
		return command;
	}

	public virtual void Dispose()
	{
		WaitEvent?.Dispose();
	}
}
