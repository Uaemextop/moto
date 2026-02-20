using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt.DeviceInfo;

public class ReadPropertiesInFastboot
{
    private static readonly Regex REG_EX = new Regex("(?<bootloader>\\(bootloader\\)\\s+)(?<key>.+):\\s+(?<value>.*)");

    protected DeviceEx device;

    private List<string> keyWord = new List<string> { "Finished", "FAILED", "OKAY", "completed" };

    public Dictionary<string, string> Props { get; private set; }

    public ReadPropertiesInFastboot(DeviceEx device)
    {
        this.device = device;
        Props = new Dictionary<string, string>();
    }

    public void Run()
    {
        Dictionary<string, string> dictionary = ReadAll();
        if (dictionary.Count > 0)
        {
            Props.Clear();
            Props = new Dictionary<string, string>(dictionary);
            ReadSimConfig();
        }
        ConvertFsgVersion();
        ConvertFingerPrint();
        ConvertBlurVersion();
        ConvertFlashSize();
        ConvertRamSize();
    }

    public string GetProp(string element)
    {
        Props.TryGetValue(element, out var value);
        return value;
    }

    private Dictionary<string, string> ReadAll()
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        List<string> list = new List<string>();
        string text = "getvar all";
        try
        {
            if (!string.IsNullOrEmpty(device.Identifer))
            {
                text = "-s " + device.Identifer + " " + text;
            }
            for (int i = 0; i < 4; i++)
            {
                list = ProcessRunner.ProcessListKeyWord(Configurations.FastbootPath, text, 5000, keyWord);
                if (list.Count > 0 && list.Count(n => n.Contains("FAILED")) != 0)
                {
                    LogHelper.LogInstance.Info("Fastboot device execute " + text + " Failed!");
                    LogHelper.LogInstance.Info(string.Join("\r\n", list));
                    break;
                }
                if (list.Count > 0 && list.Count(n => n.Contains("execute occur an error")) != 0)
                {
                    LogHelper.LogInstance.Info(string.Join("\r\n", list));
                    break;
                }
                if (list.Count > 0 && list.Count(n => n.Contains("execute error, commnad timeout")) == 0)
                {
                    LogHelper.LogInstance.Info("Fastboot device execute " + text + " succeeded!");
                    break;
                }
            }
            foreach (string item in list)
            {
                Match match = REG_EX.Match(item);
                string key = match.Groups["key"].Value;
                string value = match.Groups["value"].Value;
                if (!string.IsNullOrEmpty(key) && !dictionary.ContainsKey(key))
                {
                    dictionary.Add(key, value?.Trim());
                }
            }
        }
        catch (Exception ex)
        {
            LogHelper.LogInstance.Error("Fastboot device execute " + text + " Error! Exception:" + ex.Message);
        }
        return dictionary;
    }

    private void ConvertFsgVersion()
    {
        string text = Convert("version-baseband");
        if (!string.IsNullOrEmpty(text) && !text.Contains("not found"))
        {
            string[] array = text.Split(new char[] { ' ' });
            if (array.Length == 1)
            {
                text = array[0].Trim();
            }
            else if (array.Length > 1)
            {
                text = array[1].Trim();
            }
            Props["version-baseband"] = text;
        }
    }

    private void ConvertFingerPrint()
    {
        string text = Convert("ro.build.fingerprint");
        if (string.IsNullOrEmpty(text))
        {
            return;
        }
        string[] array = text.Split(new char[] { '/' });
        if (array.Length > 3)
        {
            Props["softwareVersion"] = array[3].Trim();
        }
        if (array.Length > 2)
        {
            string[] array2 = array[2].Split(new char[] { ':' });
            if (array2.Length > 1)
            {
                Props["androidVer"] = array2[1]?.Trim();
            }
        }
        Props["ro.build.fingerprint"] = text;
    }

    private void ConvertBlurVersion()
    {
        string value = Convert("ro.build.version.full");
        if (!string.IsNullOrEmpty(value))
        {
            Props["ro.build.version.full"] = value;
        }
    }

    private void ConvertFlashSize()
    {
        string text = Convert("emmc");
        if (!string.IsNullOrEmpty(text))
        {
            string[] array = text.Split(new char[] { ' ' });
            Props["emmc"] = array[0];
        }
    }

    private void ConvertRamSize()
    {
        string text = Convert("ram");
        if (!string.IsNullOrEmpty(text))
        {
            string[] array = text.Split(new char[] { ' ' });
            Props["ram"] = array[0];
        }
    }

    private void ReadSimConfig()
    {
        string text = "oem hw dualsim";
        if (!string.IsNullOrEmpty(device.Identifer))
        {
            text = "-s " + device.Identifer + " " + text;
        }
        int num = 3;
        List<string> list;
        do
        {
            list = ProcessRunner.ProcessList(Configurations.FastbootPath, text, 20000);
            string text2 = (list == null) ? "" : string.Join("\r\n", list);
            LogHelper.LogInstance.Info("read sim count: " + text + ", response: " + text2);
            if (!string.IsNullOrEmpty(text2) && !text2.Contains("error") && !text2.Contains("commnad timeout"))
            {
                break;
            }
            Thread.Sleep(1000);
        }
        while (--num > 0);
        string text3 = null;
        foreach (string item in list)
        {
            if (item.Contains("dualsim"))
            {
                string[] array = item.Split(new char[] { ':' });
                if (array.Length > 1 && array[1].Trim() == "true")
                {
                    text3 = "Dual";
                }
            }
        }
        if (text3 == null)
        {
            text3 = GetProp("dualsim");
            text3 = (string.IsNullOrEmpty(text3) || !(text3.ToLower() == "true")) ? "Single" : "Dual";
        }
        Props["oem hw dualsim"] = text3;
    }

    private string Convert(string element)
    {
        if (!Props.TryGetValue(element, out var value))
        {
            int num = 0;
            value = string.Empty;
            bool flag;
            do
            {
                flag = Props.TryGetValue($"{element}[{num}]", out var value2);
                if (flag)
                {
                    value += value2;
                    num++;
                }
                else
                {
                    value = value.Trim();
                }
            }
            while (flag);
        }
        return value;
    }
}
