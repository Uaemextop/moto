using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.framework.devicemgt.DeviceInfo;

public class ReadPropertiesInFastboot
{
    private static readonly Regex REG_EX = new Regex("(?<bootloader>\\(bootloader\\)\\s+)(?<key>.+):\\s+(?<value>.*)");

    private readonly string _deviceId;

    private readonly List<string> keyWord = new List<string> { "Finished", "FAILED", "OKAY", "completed" };

    public Dictionary<string, string> Props { get; private set; }

    public ReadPropertiesInFastboot(string deviceId)
    {
        _deviceId = deviceId;
        Props = new Dictionary<string, string>();
    }

    public void Run()
    {
        Dictionary<string, string> dictionary = ReadAll();
        if (dictionary.Count > 0)
        {
            Props.Clear();
            Props = new Dictionary<string, string>(dictionary);
        }
    }

    public string? GetProp(string element)
    {
        Props.TryGetValue(element, out var value);
        return value;
    }

    private Dictionary<string, string> ReadAll()
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        List<string> list = new List<string>();
        string command = "getvar all";
        try
        {
            string fullCommand = command;
            if (!string.IsNullOrEmpty(_deviceId))
            {
                fullCommand = "-s " + _deviceId + " " + command;
            }
            for (int i = 0; i < 4; i++)
            {
                list = ProcessRunner.ProcessListKeyWord(Configurations.FastbootPath, fullCommand, 5000, keyWord);
                if (list.Count > 0 && list.Count(n => n.Contains("FAILED")) != 0)
                {
                    LogHelper.LogInstance.Info("Fastboot device execute " + fullCommand + " Failed!");
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
                    LogHelper.LogInstance.Info("Fastboot device execute " + fullCommand + " succeeded!");
                    break;
                }
                LogHelper.LogInstance.Info($"Testlog-->Fastboot getvar all Other Resp:{string.Join(", ", list)}");
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
            LogHelper.LogInstance.Error("Fastboot device execute " + command + " Error! Exception:" + ex.Message);
        }
        return dictionary;
    }
}
