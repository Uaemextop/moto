using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class FastbootFlashSinglepartition : BaseStep
{
	private string failedResponse;

	private static readonly Regex REG_EX = new Regex("(?<bootloader>\\(bootloader\\)\\s+)(?<key>.+):\\s+(?<value>.*)");

	private List<KeyValuePair<string, int>> commands;

	public override void Run()
	{
		string text = base.Info.Args.XML;
		string text2 = base.Info.Args.EXE;
		string name = base.Info.Args.PartitionName;
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		if (!File.Exists(text))
		{
			if (Directory.Exists(text))
			{
				base.Log.AddResult(this, Result.LOAD_RESOURCE_FAILED, "Rom resource file: " + text + " not exist!");
			}
			else
			{
				base.Log.AddResult(this, Result.ABORTED, "Mybe the key \"XMl\" in json args is error!");
			}
			return;
		}
		bool flag = ExtractBootloaderCommand(text, name);
		if (!flag)
		{
			base.Log.AddResult(this, Result.LOAD_RESOURCE_FAILED, "Rom resource file: " + text + " not exist!");
			return;
		}
		if (string.IsNullOrEmpty(text2))
		{
			text2 = "fastboot.exe";
		}
		string text3 = base.Resources.GetLocalFilePath(text2);
		if (string.IsNullOrEmpty(text3))
		{
			text3 = Configurations.FastbootPath;
		}
		foreach (KeyValuePair<string, int> command in commands)
		{
			string text4 = EncapsulationFastbootCommand(command.Key);
			string text5 = ProcessRunner.ProcessString(text3, text4, command.Value)?.ToLower();
			base.Log.AddLog("fastboot command: " + text4 + ", response: " + text5, upload: true);
			if (text5.Contains("fail") || text5.Contains("error"))
			{
				failedResponse = "exec command failed: " + text4;
				flag = false;
				break;
			}
		}
		dynamic val = base.Info.Args.IgnoreCurrStepResult ?? ((object)false);
		if (flag)
		{
			base.Log.AddResult(this, Result.PASSED);
			return;
		}
		if ((!val))
		{
			string text6 = string.Empty;
			List<string> list = ProcessRunner.ProcessList(text3, EncapsulationFastbootCommand("getvar all"), 12000);
			if (list != null)
			{
				foreach (string item in list)
				{
					Match match = REG_EX.Match(item);
					string value = match.Groups["key"].Value;
					string value2 = match.Groups["value"].Value;
					if (!string.IsNullOrEmpty(value) && value.Equals("channelid"))
					{
						base.Log.AddInfo("channelid", value2);
						break;
					}
				}
				text6 = string.Join("\r\n", list);
			}
			base.Log.AddLog("fastboot command : getvar all, response: " + text6, upload: true);
			text6 = ProcessRunner.ProcessString(text3, EncapsulationFastbootCommand("oem read_sv"), 12000)?.ToLower();
			base.Log.AddLog("fastboot command : oem read_sv, response: " + text6, upload: true);
			text6 = ProcessRunner.ProcessString(text3, EncapsulationFastbootCommand("oem partition"), 12000)?.ToLower();
			base.Log.AddLog("fastboot command : oem partition, response: " + text6, upload: true);
		}
		base.Log.AddResult(this, Result.FASTBOOT_FLASH_SINGLEPARTITION_FAILED, failedResponse);
	}

	private bool ExtractBootloaderCommand(string xml, string name)
	{
		//IL_01b7: Expected O, but got Unknown
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		string text = string.Empty;
		try
		{
			text = File.ReadAllText(xml);
			XmlDocument val = new XmlDocument();
			val.LoadXml(text);
			XmlNodeList val2 = ((XmlNode)val).SelectNodes($"//step[@partition='{name}']");
			if (val2 != null && val2.Count == 0)
			{
				failedResponse = "partition=" + name + " not exists in file: " + xml;
				base.Log.AddLog(failedResponse, upload: true);
				return false;
			}
			string directoryName = Path.GetDirectoryName(xml);
			commands = new List<KeyValuePair<string, int>>();
			foreach (XmlNode item in val2)
			{
				XmlNode namedItem;
				string text2 = (((namedItem = ((XmlNamedNodeMap)item.Attributes).GetNamedItem("operation")) != null) ? namedItem.Value : string.Empty).Trim();
				string text3 = (((namedItem = ((XmlNamedNodeMap)item.Attributes).GetNamedItem("partition")) != null) ? namedItem.Value : string.Empty).Trim();
				string path = (((namedItem = ((XmlNamedNodeMap)item.Attributes).GetNamedItem("filename")) != null) ? namedItem.Value : string.Empty).Trim();
				(((namedItem = ((XmlNamedNodeMap)item.Attributes).GetNamedItem("MD5")) != null) ? namedItem.Value : string.Empty).Trim();
				string text4 = Path.Combine(directoryName, path);
				string key = text2 + " " + text3 + " \"" + text4 + "\"";
				commands.Add(new KeyValuePair<string, int>(key, 300000));
			}
			return true;
		}
		catch (XmlException ex)
		{
			XmlException ex2 = ex;
			base.Log.AddLog("Error xml content: " + text, upload: false, (Exception)(object)ex2);
			return false;
		}
		catch (Exception ex3)
		{
			base.Log.AddLog("Read xml for exucte bootloader command failed!", upload: false, ex3);
			return false;
		}
	}
}
