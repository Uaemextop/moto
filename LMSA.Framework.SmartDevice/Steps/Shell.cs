using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using Newtonsoft.Json.Linq;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.smartdevice.ODMSocketServer;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class Shell : BaseStep
{
	private delegate bool WNDENUMPROC(IntPtr hWnd, int lParam);

	private delegate bool CHILDWINDOWPROC(IntPtr hwnd, int lParam);

	protected int timeout = 1200000;

	protected bool IsConnected;

	private bool mError;

	private bool mCompleted;

	protected bool quit;

	protected bool hasPort;

	private string failedResponse;

	private int terminatedRetry = 2;

	private ShellCmdStatus ToolErrorStatus;

	private AutoResetEvent autoLockHandler;

	private DateTime startTime;

	private List<string> decryptFiles;

	private int responseCount;

	private int responseTriggerCount;

	private int logMonitorType;

	private bool autoCloseWhenGoOnResponse;

	private string QuitConditionMessage;

	private string FailConditionMessage;

	private bool IsRetry;

	private Task ConnectTutorialsTask;

	private const string FileLostString = "No such file or directory";

	private readonly List<JObject> QuitConditionList = new List<JObject>
	{
		new JObject
		{
			{
				"Condition",
				new JArray("No such file or directory")
			},
			{ "Message", null }
		}
	};

	private readonly List<JObject> FailConditionList = new List<JObject>();

	private List<string> monitorResult;

	public bool isMonitoring;

	private System.Timers.Timer monitorTimer;

	[DllImport("user32.dll")]
	private static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, int lParam);

	[DllImport("user32.dll")]
	private static extern int GetWindowTextW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpString, int nMaxCount);

	[DllImport("user32.dll")]
	private static extern int GetClassNameW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpString, int nMaxCount);

	[DllImport("user32.dll")]
	private static extern int EnumChildWindows(IntPtr hWndParent, CHILDWINDOWPROC lpfn, int lParam);

	public override void Run()
	{
		if (base.TimeoutMilliseconds <= 0)
		{
			base.TimeoutMilliseconds = timeout;
		}
		if (Retry > 0)
		{
			terminatedRetry = Retry;
		}
		if (base.Info.Args.QuitConditions is JArray { HasValues: not false } jArray)
		{
			foreach (JObject item3 in jArray)
			{
				QuitConditionList.Add(item3);
			}
		}
		if (base.Info.Args.FailConditions is JArray { HasValues: not false } jArray2)
		{
			foreach (JObject item4 in jArray2)
			{
				FailConditionList.Add(item4);
			}
		}
		List<Tuple<string, string, ShellResponse>> list = Init();
		if (ToolErrorStatus == ShellCmdStatus.FileLostError)
		{
			base.Log.AddResult(this, Result.LOAD_RESOURCE_FAILED, failedResponse);
			return;
		}
		Result result = Result.PASSED;
		int num = base.Info.Args.FlashRetry ?? ((object)0);
		do
		{
			ClearDeviceMonitorConnectedList();
			if (base.Info.Args.MonitorPort != null)
			{
				string vidpid = base.Info.Args.MonitorPort;
				StartMonitoring(vidpid);
			}
			string text = null;
			startTime = DateTime.Now;
			foreach (Tuple<string, string, ShellResponse> item5 in list)
			{
				result = DoFlash(item5.Item1, item5.Item2, item5.Item3);
			}
			IsRetry = false;
			switch (result)
			{
			case Result.FAILED:
				if (!string.IsNullOrEmpty(FailConditionMessage))
				{
					int num2 = (int)((double)base.TimeoutMilliseconds - DateTime.Now.Subtract(startTime).TotalMilliseconds);
					if (num2 < 10000)
					{
						num2 = 10000;
					}
					base.Recipe.UcDevice.MessageBox.AutoClose(base.Name, FailConditionMessage, null, new List<string> { "K0327" }, num2, null, showClose: false, popupWhenClose: false, format: true, true);
				}
				if (ToolErrorStatus == ShellCmdStatus.FastbootError || ToolErrorStatus == ShellCmdStatus.FileLostError)
				{
					break;
				}
				if (!IsConnected && Retry > 0)
				{
					if (base.Info.Args.ConnectTutorials is JObject jObject && jObject.Value<JArray>("ReSteps") != null)
					{
						Retry--;
						IsRetry = true;
					}
					text = base.Info.Args.ConnectTutorials?.RetryText ?? base.Info.Args.ConnectSteps?.RetryText ?? base.Info.Args.ReconnectPromptText;
					if (!IsRetry && text != null)
					{
						Retry--;
						base.Log.AddLog("device not connected, will try again", upload: true);
						IsRetry = true;
					}
				}
				else if (IsConnected && num > 0)
				{
					text = base.Info.Args.ConnectTutorials?.FlashRetryText ?? base.Info.Args.ConnectSteps?.FlashRetryText;
					if (text != null)
					{
						num--;
						base.Log.AddLog("Unisoc device rescue failed, will try again", upload: true);
						IsRetry = true;
					}
				}
				break;
			case Result.SHELL_EXE_TERMINATED_EXIT:
			case Result.SHELL_EXE_START_FAILED:
				if (terminatedRetry-- > 0)
				{
					IsRetry = true;
					Thread.Sleep(20000);
				}
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				int num3 = (int)((double)base.TimeoutMilliseconds - DateTime.Now.Subtract(startTime).TotalMilliseconds);
				if (num3 < 10000)
				{
					num3 = 10000;
				}
				if (!base.Recipe.UcDevice.MessageBox.Show(base.Info.Name, text, "K0327", null, showClose: false, (MessageBoxImage)64).Wait(num3))
				{
					base.Recipe.UcDevice.MessageBox.Close(true);
				}
			}
			ErrorConnectProcess(!quit && !hasPort && !IsConnected && result == Result.FAILED);
			if (!IsRetry && !quit && !mCompleted && !IsConnected && Retry-- > 0)
			{
				IsRetry = true;
			}
		}
		while (IsRetry);
		switch (result)
		{
		case Result.FAILED:
			if (ToolErrorStatus == ShellCmdStatus.RomUnMatchError)
			{
				result = Result.ROM_UNMATCH_FAILED;
			}
			else if (ToolErrorStatus == ShellCmdStatus.AuthorizedError)
			{
				result = Result.AUTRORIZED_FAILED;
			}
			else if (ToolErrorStatus == ShellCmdStatus.FastbootError)
			{
				result = Result.SHELL_RESCUE_FAILED;
			}
			else if (IsConnected)
			{
				result = (CheckComport() ? Result.SHELL_RESCUE_FAILED : Result.PROCESS_FORCED_TEREMINATION);
			}
			else
			{
				result = Result.SHELL_CONNECTED_FAILED;
				if (monitorResult != null)
				{
					base.Log.AddLog(string.Join("\r\n", monitorResult), upload: true);
				}
			}
			if (string.IsNullOrEmpty(failedResponse))
			{
				failedResponse = "shell execute timeout";
			}
			if (list.Select((Tuple<string, string, ShellResponse> n) => n.Item3).Count((ShellResponse n) => n.ShellCmd == ShellCmdType.MTekCfcFlashTool) > 0)
			{
				string text3 = EncapsulationFastbootCommand("getvar all");
				List<string> values = ProcessRunner.ProcessList(LoadToolPath("fastboot.exe"), text3, 5000);
				base.Log.AddLog("command : " + text3 + ", response: " + string.Join("\r\n", values), upload: true);
			}
			break;
		case Result.SHELL_EXE_TERMINATED_EXIT:
		case Result.SHELL_EXE_START_FAILED:
		{
			failedResponse = ((result == Result.SHELL_EXE_START_FAILED) ? "The tool process has not started" : "The tool process terminated abnormally");
			base.Recipe.UcDevice.MessageBox.Show("K0071", "K1832", "K0327", null, showClose: false, (MessageBoxImage)48).Wait();
			string text2 = base.Resources.Get(RecipeResources.TooL);
			if (!Directory.Exists(text2))
			{
				base.Log.AddLog("tool path: " + text2 + " not exists", upload: true);
			}
			else
			{
				List<string> allFiles = GlobalFun.GetAllFiles(text2);
				if (allFiles != null && allFiles.Count > 0)
				{
					base.Log.AddLog($"tool path: {text2}, total files: {allFiles.Count}", upload: true);
					foreach (string item6 in allFiles)
					{
						FileInfo fileInfo = new FileInfo(item6);
						base.Log.AddLog($"{fileInfo.FullName}, {fileInfo.Length}, {fileInfo.LastWriteTime}", upload: true);
					}
				}
				else
				{
					base.Log.AddLog("tool path: " + text2 + ", total files: 0", upload: true);
				}
			}
			base.Log.AddInfo("rescuemark", "11");
			break;
		}
		}
		StopMonitoring();
		FreeLock();
		base.Recipe.UcDevice.MessageBox.Close(true);
		ODMServerMain.CloseAllSockets();
		if (decryptFiles != null && decryptFiles.Count > 0)
		{
			Task.Run(delegate
			{
				decryptFiles.ForEach(delegate(string n)
				{
					GlobalFun.TryDeleteFile(n);
				});
			});
		}
		if (!string.IsNullOrEmpty(QuitConditionMessage))
		{
			base.Recipe.UcDevice.MessageBox.AutoClose(base.Name, QuitConditionMessage, null, new List<string> { "K0327" }, 10000, null, showClose: false, popupWhenClose: false, format: true, true);
		}
		base.Log.AddResult(this, result, failedResponse);
	}

	private List<Tuple<string, string, ShellResponse>> Init()
	{
		string category = base.Resources.Get("category");
		string text = base.Info.Args.EXE;
		string text2 = base.Info.Args.Command;
		List<Tuple<string, string, ShellResponse>> list = new List<Tuple<string, string, ShellResponse>>();
		if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
		{
			if (text.StartsWith("$"))
			{
				string key = text.Substring(1);
				text = base.Cache[key];
			}
			List<object> list2 = new List<object>();
			if (base.Info.Args.Format != null)
			{
				foreach (object item3 in base.Info.Args.Format)
				{
					string text3 = (string)(dynamic)item3;
					object item = text3;
					if (text3.StartsWith("$"))
					{
						string key2 = text3.Substring(1);
						item = base.Cache[key2];
					}
					list2.Add(item);
				}
				text2 = string.Format(text2, list2.ToArray());
			}
			string value = Regex.Match(text2, "\"(?<key>.*scatter.*\\.txt)").Groups["key"].Value;
			if (File.Exists(value))
			{
				base.Log.AddLog("check sactter file: " + value, upload: true);
				MatchCollection matchCollection = Regex.Matches(File.ReadAllText(value), "(?<key>file_name):\\s+(?<value>.*)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
				string path = base.Resources.Get(RecipeResources.Rom);
				foreach (Match item4 in matchCollection)
				{
					string value2 = item4.Groups["value"].Value;
					if (!string.IsNullOrEmpty(value2) && !"NONE".Equals(value2, StringComparison.CurrentCultureIgnoreCase))
					{
						string text4 = Path.Combine(path, value2);
						if (!File.Exists(text4))
						{
							failedResponse = text4 + " not exists";
							ToolErrorStatus = ShellCmdStatus.FileLostError;
							return list;
						}
					}
				}
			}
			ShellResponse shellResponse = ShellResponseFactory.CreateInstance(text);
			shellResponse.Init(category, list2);
			list.Add(new Tuple<string, string, ShellResponse>(text, text2, shellResponse));
		}
		else
		{
			string text5 = base.Info.Args.ComPort ?? string.Empty;
			string name = base.Info.Args.StartupFile ?? "Flash.cmd";
			string localFilePath = base.Resources.GetLocalFilePath("toolFolder");
			if (!string.IsNullOrEmpty(text5))
			{
				string key3 = (base.Cache.ContainsKey(text5) ? text5 : "comport");
				object arg = base.Cache[key3];
				text5 = $"COM{arg}";
			}
			string recoveryCmd = base.Resources.GetRecoveryCmd(name);
			if (string.IsNullOrWhiteSpace(recoveryCmd))
			{
				ToolErrorStatus = ShellCmdStatus.FileLostError;
				failedResponse = "Recipe Shell StartupFile doesn't exist!";
				return list;
			}
			string[] array = Regex.Replace(Regex.Replace(File.ReadAllText(recoveryCmd).Trim(), "%~dp0", Path.GetDirectoryName(recoveryCmd) + "\\", RegexOptions.IgnoreCase).Trim(), "pause", string.Empty, RegexOptions.IgnoreCase).Trim().Replace(".\\portname", ".\\" + text5)
				.Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text6 in array)
			{
				string text7 = text6.Substring(0, text6.IndexOf(' '));
				string item2 = text6.Substring(text6.IndexOf(' '));
				string[] files = Directory.GetFiles(localFilePath, text7, SearchOption.AllDirectories);
				if (files.Length == 0)
				{
					ToolErrorStatus = ShellCmdStatus.FileLostError;
					failedResponse = "Recipe Shell FlashToolExe:[" + text7 + "] doesn't exist!";
					return list;
				}
				ShellResponse shellResponse2 = ShellResponseFactory.CreateInstance(files[0]);
				shellResponse2.Init(category);
				list.Add(new Tuple<string, string, ShellResponse>(files[0], item2, shellResponse2));
			}
			try
			{
				string text8 = base.Info.Args.DecryptFileType;
				if (!string.IsNullOrEmpty(text8))
				{
					decryptFiles = GlobalFun.DecryptRomFile(base.Resources.Get("Rom"), text8);
				}
			}
			catch (Exception)
			{
			}
			if (list.Count == 0)
			{
				ToolErrorStatus = ShellCmdStatus.FileLostError;
				failedResponse = recoveryCmd + " content is incorrect";
			}
		}
		return list;
	}

	protected Result DoFlash(string exe, string command, ShellResponse shellResponse)
	{
		Result result = Result.PASSED;
		IsConnected = false;
		mCompleted = false;
		mError = false;
		quit = false;
		hasPort = false;
		ConnectTutorialsTask = null;
		responseCount = 0;
		failedResponse = null;
		bool flag = true;
		bool flag2 = false;
		ToolErrorStatus = ShellCmdStatus.None;
		Process process = new Process();
		process.StartInfo.FileName = "\"" + exe + "\"";
		process.StartInfo.WorkingDirectory = Path.GetDirectoryName(exe);
		process.StartInfo.Arguments = command;
		process.StartInfo.ErrorDialog = true;
		process.StartInfo.RedirectStandardInput = true;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.Verb = "runas";
		process.EnableRaisingEvents = true;
		process.StartInfo.CreateNoWindow = true;
		process.OutputDataReceived += delegate(object s, DataReceivedEventArgs e)
		{
			Redirected(e, shellResponse);
		};
		process.ErrorDataReceived += delegate(object s, DataReceivedEventArgs e)
		{
			Redirected(e, shellResponse);
		};
		try
		{
			process.Start();
		}
		catch (Exception arg)
		{
			base.Log.AddLog($"exe: {exe}, command {command}, exeption: {arg}", upload: true);
			return Result.SHELL_EXE_START_FAILED;
		}
		base.Log.AddLog($"exe: {exe}, pid: {process.Id}, command {command}", upload: true);
		process.BeginOutputReadLine();
		process.BeginErrorReadLine();
		if (base.Info.Args.Input != null)
		{
			string text = base.Info.Args.Input;
			if (text.StartsWith("$"))
			{
				string key = text.Substring(1);
				text = base.Cache[key];
			}
			if (text != string.Empty)
			{
				process.StandardInput.WriteLine(text);
			}
		}
		if (base.Info.Args.ShowRescuingMask == true)
		{
			base.Recipe.FreeEventHandler(realFlash: true);
		}
		if (shellResponse.ShellCmd != ShellCmdType.CmdDloader && shellResponse.ShellCmd != ShellCmdType.CmdDloaderTablet && shellResponse.ShellCmd != ShellCmdType.MTekFlashTool && shellResponse.ShellCmd != ShellCmdType.MTekSpFlashTool)
		{
			Task.Run(() => ShowMessageWhenStartToolAsync(shellResponse));
		}
		bool flag3 = false;
		int num = 3000;
		do
		{
			if (!quit)
			{
				flag3 = process.WaitForExit(num);
			}
			if (flag3)
			{
				flag = false;
				if (!quit)
				{
					Thread.Sleep(num);
				}
			}
			if (mCompleted || mError || quit)
			{
				flag = false;
				if (!flag3 && !quit)
				{
					flag3 = process.WaitForExit(num * 3);
				}
				break;
			}
		}
		while (!flag3 && DateTime.Now.Subtract(startTime).TotalMilliseconds < (double)base.TimeoutMilliseconds);
		if (!quit)
		{
			Thread.Sleep(num);
		}
		if (!flag3)
		{
			try
			{
				process.Kill();
				flag3 = true;
			}
			catch (Exception ex)
			{
				base.Log.AddLog("fail to shut down the shell command process: " + ex.Message);
			}
		}
		if (quit)
		{
			result = ((ToolErrorStatus == ShellCmdStatus.FileLostError) ? Result.LOAD_RESOURCE_FAILED : ((ToolErrorStatus == ShellCmdStatus.FastbootDegrade) ? Result.FASTBOOT_DEGRADE_QUIT : ((ToolErrorStatus != ShellCmdStatus.ConditionQuit) ? Result.MANUAL_QUIT : Result.INTERCEPTOR_QUIT)));
		}
		else if (mCompleted)
		{
			result = Result.PASSED;
		}
		else if (!mError && !quit && !flag)
		{
			flag2 = responseCount == 0;
			failedResponse = "the flash tool process has exited";
			result = (flag2 ? Result.SHELL_EXE_TERMINATED_EXIT : Result.FAILED);
		}
		else if (mError || !mCompleted || flag)
		{
			result = Result.FAILED;
		}
		base.Log.AddLog($"shell completed: {mCompleted}, error: {mError}, quit: {quit}, terminated exit: {flag2}, timeout-{base.TimeoutMilliseconds}: {flag}", upload: true);
		FreeLock();
		base.Recipe.UcDevice.MessageBox.Close(true);
		return result;
	}

	private void Redirected(DataReceivedEventArgs e, ShellResponse shellResponse)
	{
		if (string.IsNullOrEmpty(e.Data) || quit)
		{
			return;
		}
		responseCount++;
		if (autoCloseWhenGoOnResponse && logMonitorType > 0 && responseTriggerCount < responseCount)
		{
			if (autoLockHandler != null && !autoLockHandler.SafeWaitHandle.IsClosed)
			{
				autoLockHandler.Set();
				autoLockHandler = null;
			}
			base.Recipe.UcDevice.MessageBox.Close(true);
		}
		string text = e.Data.Trim();
		string responseKey;
		ShellCmdStatus shellCmdStatus = shellResponse.ParseResponse(text, out responseKey);
		base.Log.AddLog($"status: {shellCmdStatus}, shell response: {text}", upload: true);
		if (shellResponse.ShellCmd == ShellCmdType.CmdDloader && text.StartsWith("Port :", StringComparison.CurrentCultureIgnoreCase))
		{
			hasPort = true;
		}
		int num = ConditionCheck(text);
		if (num != 0)
		{
			if (text.Contains("No such file or directory"))
			{
				ToolErrorStatus = ShellCmdStatus.FileLostError;
			}
			else if (text.Contains("STATUS_SEC_VIOLATE_ANTI_ROLLBACK"))
			{
				ToolErrorStatus = ShellCmdStatus.FastbootDegrade;
			}
			else
			{
				ToolErrorStatus = ShellCmdStatus.ConditionQuit;
			}
			if (num == 1)
			{
				quit = true;
			}
			else
			{
				mError = true;
			}
			return;
		}
		ShowMessageAnalyzeResponse(text, shellResponse);
		AnalyzeComport(text);
		switch (shellCmdStatus)
		{
		case ShellCmdStatus.Error:
		case ShellCmdStatus.RomUnMatchError:
		case ShellCmdStatus.AuthorizedError:
		case ShellCmdStatus.FastbootError:
			if (!mError && string.IsNullOrEmpty(failedResponse))
			{
				failedResponse = text;
			}
			if (ToolErrorStatus == ShellCmdStatus.None)
			{
				if (shellCmdStatus == ShellCmdStatus.AuthorizedError)
				{
					IsConnected = true;
				}
				ToolErrorStatus = shellCmdStatus;
			}
			mError = true;
			break;
		case ShellCmdStatus.Connected:
			if (!IsConnected)
			{
				base.Recipe.UcDevice.MessageBox.Close(true);
				base.Recipe.FreeEventHandler(realFlash: true);
				ShowMessageWhenConnectedSuccessAsync();
			}
			IsConnected = true;
			break;
		case ShellCmdStatus.Downloading:
		{
			double downloadProgressPercent = shellResponse.GetDownloadProgressPercent(text, responseKey);
			ProgressUpdate(downloadProgressPercent);
			break;
		}
		case ShellCmdStatus.Completed:
			mCompleted = true;
			break;
		case ShellCmdStatus.None:
		case ShellCmdStatus.Connecting:
		case ShellCmdStatus.Outputing:
		case ShellCmdStatus.Authenticating:
		case ShellCmdStatus.Writing:
		case ShellCmdStatus.FileLostError:
		case ShellCmdStatus.FastbootDegrade:
		case ShellCmdStatus.ConditionQuit:
			break;
		}
	}

	private async Task ShowMessageWhenStartToolAsync(ShellResponse shellResponse)
	{
		bool flag = true;
		if ((shellResponse.ShellCmd == ShellCmdType.MTekFlashTool || shellResponse.ShellCmd == ShellCmdType.MTekSpFlashTool) && base.Cache.ContainsKey("Read Device Mode") && base.Recipe.Device != null)
		{
			flag = base.Recipe.Device.ConnectType != ConnectType.Adb;
		}
		if (flag && base.Info.Args.ConnectTutorials is JObject jObject && ((IsRetry && jObject.Value<JArray>("ReSteps") != null) || jObject.Value<JArray>("Steps") != null))
		{
			string title = jObject.Value<string>("Title");
			JArray steps = jObject.Value<JArray>((IsRetry && jObject.Value<JArray>("ReSteps") != null) ? "ReSteps" : "Steps");
			bool autoPlay = jObject.Value<bool>("AutoPlay");
			double interval = jObject.Value<double>("Interval");
			string noteText = jObject.Value<string>("NoteText");
			LogHelper.LogInstance.Debug("Args.ConnectTutorials.Steps will show");
			if (!base.Recipe.UcDevice.MessageBox.AutoCloseConnectTutorials(title, steps, -1, autoPlay, interval, showPlayControl: true, showClose: true, popupWhenClose: true, noteText).HasValue)
			{
				quit = true;
			}
		}
		else if (flag && base.Info.Args.ConnectSteps != null)
		{
			ConnectStepInfo _connectStep = new ConnectStepInfo();
			_connectStep.NoteText = base.Info.Args.ConnectSteps.NoteText?.ToString();
			_connectStep.RetryText = base.Info.Args.ConnectSteps.RetryText?.ToString();
			_connectStep.WidthRatio = base.Info.Args.ConnectSteps.WidthRatio?.ToString() ?? "1:2:2";
			await Task.Run(delegate
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
			if (!((flag && base.Info.Args.PromptText != null) ? true : false))
			{
				return;
			}
			await Task.Run(delegate
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

	private void ShowMessageWhenConnectedSuccessAsync()
	{
		Task.Run(delegate
		{
			if (base.Info.Args.ConnectTutorials is JObject jObject && jObject.Value<JArray>("ReleaseSteps") != null)
			{
				JArray steps = jObject.Value<JArray>("ReleaseSteps");
				jObject.Value<string>("NoteText");
				LogHelper.LogInstance.Debug("Args.ConnectTutorials.ReleaseSteps will show");
				base.Recipe.UcDevice.MessageBox.AutoCloseConnectTutorials(null, steps, 10000, autoPlay: false, 5000.0, showPlayControl: false, showClose: false);
			}
			else if (base.Info.Args.ExPromptText != null)
			{
				string image = base.Info.Args.ExImage?.ToString();
				string message = base.Info.Args.ExPromptText.ToString();
				base.Recipe.UcDevice.MessageBox.AutoClose(base.Name, message, image, null, 10000, null, showClose: false, popupWhenClose: false, format: true, true);
			}
		});
	}

	private void ShowMessageAnalyzeResponse(string response, ShellResponse shellResponse)
	{
		if (base.Info.Args.LogMonitorActions != null)
		{
			foreach (dynamic iter in base.Info.Args.LogMonitorActions)
			{
				if (!(response.Contains(iter.MatchText.ToString()) ? true : false))
				{
					continue;
				}
				if (iter.ActionType == "Command")
				{
					string text = iter.EXE;
					if (text.StartsWith("$"))
					{
						text = base.Cache[text.TrimStart(new char[1] { '$' })];
					}
					string text2 = iter.Command.ToString();
					if (text.ToLower().Contains("fastboot"))
					{
						string text3 = base.Recipe.Device?.Identifer;
						if (!string.IsNullOrEmpty(text3))
						{
							text2 = "-s " + text3 + " " + text2;
						}
					}
					string text4 = ProcessRunner.ProcessString(text, text2, 6000);
					base.Log.AddLog("Excute shell command: " + text + " " + text2 + ", response: " + text4 + "!", upload: true);
				}
				else
				{
					if (!((iter.Steps != null && iter.ActionType == "PromptText") ? true : false))
					{
						continue;
					}
					responseTriggerCount = responseCount;
					int millisecondsDelay = iter.Delay ?? ((object)0);
					autoCloseWhenGoOnResponse = iter.AutoCloseWhenGoOnResponse ?? ((object)false);
					if (iter.Steps.Count == 1)
					{
						dynamic img = iter.Steps[0].Image?.ToString();
						dynamic content = iter.Steps[0].Content.ToString();
						Task.Delay(millisecondsDelay).ContinueWith(delegate
						{
							if (!autoCloseWhenGoOnResponse || responseTriggerCount == responseCount)
							{
								Task.Run(delegate
								{
									if (autoLockHandler == null)
									{
										autoLockHandler = new AutoResetEvent(initialState: false);
									}
									logMonitorType = 1;
									int num = (int)((double)base.TimeoutMilliseconds - DateTime.Now.Subtract(startTime).TotalMilliseconds);
									bool? flag = base.Recipe.UcDevice.MessageBox.AutoClose(iter.NoteText?.ToString() ?? base.Name, content, img, new List<string> { "K0327" }, num, showClose: false, popupWhenClose: true, format: false, autoCloseResult: false);
									logMonitorType = 0;
									if (!flag.HasValue)
									{
										quit = true;
									}
									else if (flag == false)
									{
										mError = true;
									}
									autoLockHandler?.Set();
								});
							}
						});
					}
					else
					{
						if (!((iter.Steps.Count == 2 || iter.Steps.Count == 3) ? true : false))
						{
							continue;
						}
						ConnectStepInfo multiInfo = new ConnectStepInfo();
						multiInfo.NoteText = iter.NoteText?.ToString();
						multiInfo.WidthRatio = iter.WidthRatio?.ToString() ?? "1:2:2";
						multiInfo.Steps = new List<ConnectSteps>();
						foreach (dynamic item in iter.Steps)
						{
							multiInfo.Steps.Add(new ConnectSteps
							{
								StepImage = item.Image?.ToString(),
								StepContent = item.Content.ToString()
							});
						}
						Task.Delay(millisecondsDelay).ContinueWith(delegate
						{
							if (!autoCloseWhenGoOnResponse || responseTriggerCount == responseCount)
							{
								Task.Run(delegate
								{
									logMonitorType = 2;
									base.Recipe.UcDevice.MessageBox.AutoCloseMoreStep(base.Name, multiInfo);
									logMonitorType = 0;
								});
							}
						});
					}
				}
			}
		}
		if (response.Contains("too many links"))
		{
			base.Recipe.UcDevice.MessageBox.Show(base.Name, "K1452", "K0327", null, showClose: false, (MessageBoxImage)48).Wait(60000);
		}
		else if (!IsConnected && (response.StartsWith("Scanning USB port", StringComparison.CurrentCultureIgnoreCase) || response.StartsWith("Detecting download device", StringComparison.CurrentCultureIgnoreCase) || response.StartsWith("scan device START", StringComparison.CurrentCultureIgnoreCase)) && ConnectTutorialsTask == null)
		{
			base.Log.AddLog("the response meet the connection pop-up conditions, will show connection popup window");
			ConnectTutorialsTask = Task.Run(() => ShowMessageWhenStartToolAsync(shellResponse));
		}
	}

	protected void AnalyzeComport(string data)
	{
		if (!string.IsNullOrEmpty(data) && !base.Cache.ContainsKey("comport"))
		{
			string value = Regex.Match(data, "\\(COM(?<value>\\d+)\\)").Groups["value"].Value;
			if (!string.IsNullOrEmpty(value) && !base.Cache.ContainsKey("comport"))
			{
				base.Cache.Add("comport", value);
			}
		}
	}

	private bool CheckComport()
	{
		if (!base.Cache.ContainsKey("comport"))
		{
			return true;
		}
		List<string> comInfo = GlobalFun.GetComInfo();
		if (comInfo == null || comInfo.Count == 0)
		{
			return false;
		}
		return comInfo.Exists((string n) => n.Contains(string.Format("(COM{0})", (object?)base.Cache["comport"])));
	}

	private int ConditionCheck(string response)
	{
		string tmp = response.ToLower();
		foreach (JObject quitCondition in QuitConditionList)
		{
			List<string> list = quitCondition.Value<JArray>("Condition").Values<string>().ToList();
			if (list != null && list.Exists((string n) => tmp.Contains(n.ToLower())))
			{
				QuitConditionMessage = quitCondition.Value<string>("Message");
				string value = quitCondition.Value<string>("RescueMark");
				base.Log.AddInfo("rescuemark", value);
				base.Log.AddLog("shell response quit contains: " + string.Join(" | ", list), upload: true);
				return 1;
			}
		}
		foreach (JObject failCondition in FailConditionList)
		{
			List<string> list2 = failCondition.Value<JArray>("Condition").Values<string>().ToList();
			if (list2 != null && list2.Exists((string n) => tmp.Contains(n.ToLower())))
			{
				FailConditionMessage = failCondition.Value<string>("Message");
				base.Log.AddLog("shell response fail contains: " + string.Join(" | ", list2), upload: true);
				return 2;
			}
		}
		return 0;
	}

	protected void FreeLock()
	{
		if (autoLockHandler != null)
		{
			autoLockHandler.WaitOne();
			autoLockHandler.Dispose();
			autoLockHandler = null;
		}
	}

	private void StartMonitoring(string vidpid)
	{
		if (isMonitoring)
		{
			return;
		}
		isMonitoring = true;
		monitorResult = new List<string>();
		base.Log.AddLog("Begin monitor device vidpid :" + vidpid, upload: true);
		monitorTimer = new System.Timers.Timer(1000.0);
		monitorTimer.Elapsed += delegate
		{
			if (!isMonitoring)
			{
				return;
			}
			try
			{
				List<string> values = ProcessRunner.ProcessList("pnputil.exe", "/enum-devices /connected", 60000);
				string output = string.Join("\r\n", values);
				string text = ParseAndFilterDevices(output, vidpid);
				string text2 = ((monitorResult.Count > 0) ? monitorResult.Last() : "");
				if (string.IsNullOrEmpty(text))
				{
					if (!text2.Contains("not found device"))
					{
						monitorResult.Add($"[{DateTime.Now}] :not found device:{vidpid}");
					}
				}
				else if (!text2.Contains(text))
				{
					monitorResult.Add($"[{DateTime.Now}]:{text}");
				}
			}
			catch (Exception arg)
			{
				monitorResult.Add($"shell StartMonitoring error,vidpid: {vidpid}, exeption: {arg}");
			}
		};
		monitorTimer.AutoReset = true;
		monitorTimer.Enabled = true;
		monitorTimer.Start();
	}

	private void StopMonitoring()
	{
		if (isMonitoring)
		{
			isMonitoring = false;
			monitorTimer?.Stop();
			monitorTimer?.Dispose();
			base.Log.AddLog("Stop monitor device vidpid", upload: true);
		}
	}

	private string ParseAndFilterDevices(string output, string searchPattern)
	{
		List<string> list = new List<string>();
		string[] array = output.Split(new string[2] { "\r\n", "\n" }, StringSplitOptions.None);
		bool flag = false;
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text.Contains(searchPattern))
			{
				flag = true;
				list.Add(text);
				continue;
			}
			if ((string.IsNullOrWhiteSpace(text) || text.Contains(" ID")) && flag)
			{
				flag = false;
			}
			if (flag && text.Contains(":"))
			{
				list.Add(text);
			}
		}
		return string.Join("\r\n", list);
	}
}
