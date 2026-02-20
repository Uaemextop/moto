using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using log4net;
using log4net.Config;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.common.log;

public class LogHelper
{
	public static string ConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");

	private static List<string> m_UnsafeText = new List<string>();

	private static object locker = new object();

	private static LogHelper m_Instance;

	private ILog Log => LogManager.GetLogger("");

	public static LogHelper LogInstance
	{
		get
		{
			if (m_Instance == null)
			{
				lock (locker)
				{
					if (m_Instance == null)
					{
						Init();
					}
				}
			}
			return m_Instance;
		}
	}

	public static void SetConfig()
	{
		XmlConfigurator.Configure();
	}

	public static void SetConfig(string configPath)
	{
		XmlConfigurator.Configure(new FileInfo(configPath));
	}

	private static void Init()
	{
		if (File.Exists(ConfigFilePath))
		{
			XmlConfigurator.Configure(new FileInfo(ConfigFilePath));
		}
		m_Instance = new LogHelper();
	}

	public void Debug(string message, bool upload = false)
	{
		message = MessageDesensitization(message);
		string currentMethod = GetCurrentMethod();
		Log.Debug(currentMethod + " - " + message);
	}

	public void Debug(string message, Exception exception, bool upload = false)
	{
		message = MessageDesensitization(message);
		string currentMethod = GetCurrentMethod();
		Log.Debug(currentMethod + " - " + message, exception);
	}

	public void Info(string message, bool upload = false)
	{
		message = MessageDesensitization(message);
		string currentMethod = GetCurrentMethod();
		Log.Info(currentMethod + " - " + message);
	}

	public void Info(string message, Exception exception, bool upload = false)
	{
		message = MessageDesensitization(message);
		string currentMethod = GetCurrentMethod();
		Log.Info(currentMethod + " - " + message, exception);
	}

	public void Warn(string message, bool upload = false)
	{
		message = MessageDesensitization(message);
		string currentMethod = GetCurrentMethod();
		Log.Warn(currentMethod + " - " + message);
	}

	public void Warn(string message, Exception exception, bool upload = false)
	{
		message = MessageDesensitization(message);
		string currentMethod = GetCurrentMethod();
		Log.Warn(currentMethod + " - " + message, exception);
	}

	public void Error(string message, bool upload = false)
	{
		message = MessageDesensitization(message);
		string currentMethod = GetCurrentMethod();
		Log.Error(currentMethod + " - " + message);
	}

	public void Error(string message, Exception exception, bool upload = false)
	{
		message = MessageDesensitization(message);
		string currentMethod = GetCurrentMethod();
		Log.Error(currentMethod + " - " + message, exception);
	}

	public void AddUnsafeText(string _text)
	{
		if (string.IsNullOrEmpty(_text))
		{
			return;
		}
		try
		{
			if (!m_UnsafeText.Contains(_text))
			{
				m_UnsafeText.Add(_text);
			}
		}
		catch
		{
		}
	}

	public void AnalyzeUnsafeText(string _msg)
	{
		if (string.IsNullOrEmpty(_msg))
		{
			return;
		}
		try
		{
			List<string> obj = new List<string> { "content.name", "content.fullName" };
			JObject jObject = JObject.Parse(_msg);
			foreach (string item in obj)
			{
				string text = jObject.SelectToken(item)?.ToString();
				if (!string.IsNullOrEmpty(text))
				{
					m_UnsafeText.Add(text);
				}
			}
		}
		catch
		{
		}
	}

	private static string MessageDesensitization(string _msg)
	{
		m_UnsafeText.ForEach(delegate(string m)
		{
			_msg = _msg.Replace(m, "***");
		});
		return _msg;
	}

	public void WriteLogForUser(string message, int resultCode)
	{
		try
		{
			message = MessageDesensitization(message);
			message = Regex.Replace(Regex.Replace(message, "(-User\\s*)\\\\\"[^\\\\\"]+\\\\\"", "$1\"********\"", RegexOptions.IgnoreCase | RegexOptions.Multiline), "(-Password\\s*)\\\\\"[^\\\\\"]+\\\\\"", "$1\"********\"", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			string text = "not start";
			switch (resultCode)
			{
			case 0:
				text = "fail";
				break;
			case 1:
				text = "pass";
				break;
			case 2:
				text = "quit";
				break;
			}
			string contents = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - [{text}] - {message}{Environment.NewLine}";
			string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
			if (!Directory.Exists(logDir))
			{
				Directory.CreateDirectory(logDir);
			}
			File.AppendAllText(Path.Combine(logDir, $"{DateTime.Now:yyyy-MM}-friendly.log"), contents);
		}
		catch (Exception arg)
		{
			Error($"WriteLogForUser - message:[{message}] exception:[{arg}]");
		}
	}

	internal static string GetCurrentMethod()
	{
		return new StackTrace(2, fNeedFileInfo: true).GetFrame(0).GetMethod().Name;
	}

	private LogHelper()
	{
	}
}
