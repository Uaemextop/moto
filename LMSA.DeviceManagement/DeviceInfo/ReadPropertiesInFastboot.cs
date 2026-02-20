using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt.DeviceInfo;

/// <summary>
/// Reads device properties from a device in fastboot mode via getvar commands.
/// </summary>
public class ReadPropertiesInFastboot
{
    private static readonly Regex REG_EX = new Regex(@"(?<bootloader>\(bootloader\)\s+)(?<key>.+):\s+(?<value>.*)");

    protected DeviceEx _device;

    private List<string> _keyWord = new List<string> { "Finished", "FAILED", "OKAY", "completed" };

    public Dictionary<string, string> Props { get; private set; }

    public ReadPropertiesInFastboot(DeviceEx device)
    {
        _device = device;
        Props = new Dictionary<string, string>();
    }

    public void Run()
    {
        Dictionary<string, string> data = ReadAll();
        if (data.Count > 0)
        {
            Props.Clear();
            Props = new Dictionary<string, string>(data);
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
        Props.TryGetValue(element, out string? value);
        return value ?? string.Empty;
    }

    private Dictionary<string, string> ReadAll()
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        List<string> list = new List<string>();
        string text = "getvar all";
        try
        {
            if (!string.IsNullOrEmpty(_device.Identifer))
            {
                text = "-s " + _device.Identifer + " " + text;
            }
            for (int i = 0; i < 4; i++)
            {
                list = ProcessRunner.ProcessListKeyWord(Configurations.FastbootPath, text, 5000, _keyWord);
                if (list.Count > 0 && list.Exists(n => n.Contains("FAILED")))
                {
                    LogHelper.LogInstance.Info("Fastboot device execute " + text + " Failed!");
                    LogHelper.LogInstance.Info(string.Join("\r\n", list));
                    break;
                }
                if (list.Count > 0 && list.Exists(n => n.Contains("execute occur an error")))
                {
                    LogHelper.LogInstance.Info(string.Join("\r\n", list));
                    break;
                }
                if (list.Count > 0 && !list.Exists(n => n.Contains("execute error, commnad timeout")))
                {
                    LogHelper.LogInstance.Info("Fastboot device execute " + text + " succeeded!");
                    break;
                }
                LogHelper.LogInstance.Info($"Fastboot getvar all Other Response: {string.Join(", ", list)}");
            }
            foreach (string item in list)
            {
                Match match = REG_EX.Match(item);
                string key = match.Groups["key"].Value;
                string value = match.Groups["value"].Value;
                if (!string.IsNullOrEmpty(key) && !dictionary.ContainsKey(key))
                {
                    dictionary.Add(key, value?.Trim() ?? string.Empty);
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
            string[] array = text.Split(' ');
            Props["version-baseband"] = array.Length == 1 ? array[0].Trim() : (array.Length > 1 ? array[1].Trim() : text);
        }
    }

    private void ConvertFingerPrint()
    {
        string text = Convert("ro.build.fingerprint");
        if (string.IsNullOrEmpty(text)) return;
        string[] array = text.Split('/');
        if (array.Length > 3)
        {
            Props["softwareVersion"] = array[3].Trim();
        }
        if (array.Length > 2)
        {
            string[] array2 = array[2].Split(':');
            if (array2.Length > 1)
            {
                Props["androidVer"] = array2[1]?.Trim() ?? string.Empty;
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
            Props["emmc"] = text.Split(' ')[0];
        }
    }

    private void ConvertRamSize()
    {
        string text = Convert("ram");
        if (!string.IsNullOrEmpty(text))
        {
            Props["ram"] = text.Split(' ')[0];
        }
    }

    private void ReadSimConfig()
    {
        string text = "oem hw dualsim";
        if (!string.IsNullOrEmpty(_device.Identifer))
        {
            text = "-s " + _device.Identifer + " " + text;
        }
        int retries = 3;
        List<string> list;
        do
        {
            list = ProcessRunner.ProcessList(Configurations.FastbootPath, text, 20000);
            string response = list == null ? "" : string.Join("\r\n", list);
            LogHelper.LogInstance.Info("read sim count: " + text + ", response: " + response);
            if (!string.IsNullOrEmpty(response) && !response.Contains("error") && !response.Contains("commnad timeout"))
            {
                break;
            }
            Thread.Sleep(1000);
        } while (--retries > 0);

        string? simType = null;
        if (list != null)
        {
            foreach (string item in list)
            {
                if (item.Contains("dualsim"))
                {
                    string[] parts = item.Split(':');
                    if (parts.Length > 1 && parts[1].Trim() == "true")
                    {
                        simType = "Dual";
                    }
                }
            }
        }
        if (simType == null)
        {
            simType = GetProp("dualsim");
            simType = string.IsNullOrEmpty(simType) || simType.ToLower() != "true" ? "Single" : "Dual";
        }
        Props["oem hw dualsim"] = simType;
    }

    private string Convert(string element)
    {
        if (!Props.TryGetValue(element, out string? value))
        {
            int index = 0;
            value = string.Empty;
            bool found;
            do
            {
                found = Props.TryGetValue($"{element}[{index}]", out string? part);
                if (found)
                {
                    value += part;
                    index++;
                }
                else
                {
                    value = value.Trim();
                }
            } while (found);
        }
        return value ?? string.Empty;
    }
}
