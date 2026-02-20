using System.Collections.Generic;
using System.Linq;

namespace lenovo.mbg.service.framework.devicemgt.DeviceInfo;

public class PropItem
{
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }
}

public class PropInfo
{
    public List<PropItem> Props { get; private set; }

    public PropInfo()
    {
        Props = new List<PropItem>();
    }

    public string? GetProp(string key)
    {
        PropItem? item = Props.FirstOrDefault(p => p.Key == key);
        return item?.Value;
    }

    public int GetIntProp(string key)
    {
        string? value = GetProp(key);
        if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int result))
        {
            return result;
        }
        return 0;
    }

    public void AddOrUpdateProp(PropItem prop)
    {
        PropItem? existing = Props.FirstOrDefault(p => p.Key == prop.Key);
        if (existing != null)
        {
            existing.Value = prop.Value;
        }
        else
        {
            Props.Add(prop);
        }
    }

    public void AddOrUpdateProp(List<PropItem>? props)
    {
        if (props == null) return;
        foreach (PropItem prop in props)
        {
            AddOrUpdateProp(prop);
        }
    }

    public void AddOrUpdateProp(List<PropItem>? props, Dictionary<string, string>? keyMapping)
    {
        if (props == null) return;
        foreach (PropItem prop in props)
        {
            string key = prop.Key;
            if (keyMapping != null && keyMapping.TryGetValue(key, out string? mappedKey))
            {
                key = mappedKey;
            }
            AddOrUpdateProp(new PropItem { Key = key, Value = prop.Value });
        }
    }

    public void Reset(string key, string value)
    {
        PropItem? existing = Props.FirstOrDefault(p => p.Key == key);
        if (existing != null)
        {
            existing.Value = value;
        }
        else
        {
            Props.Add(new PropItem { Key = key, Value = value });
        }
    }
}
