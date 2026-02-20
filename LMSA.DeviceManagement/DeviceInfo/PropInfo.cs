using System;
using System.Collections.Generic;
using System.Linq;

namespace lenovo.mbg.service.framework.devicemgt.DeviceInfo;

public class PropInfo
{
    public List<PropItem> Props { get; private set; }

    public PropInfo()
    {
        Props = new List<PropItem>();
    }

    public bool AddOrUpdateProp(List<PropItem> props)
    {
        foreach (PropItem prop in props)
        {
            AddOrUpdateProp(prop);
        }
        return true;
    }

    public bool AddOrUpdateProp(PropItem prop)
    {
        PropItem propItem = Props.FirstOrDefault(n => n.Key.Equals(prop.Key));
        if (propItem != null)
        {
            propItem.Value = prop.Value;
        }
        else
        {
            Props.Add(prop);
        }
        return true;
    }

    public bool AddOrUpdateProp(List<PropItem> items, Dictionary<string, string> replaceKey)
    {
        if (items != null)
        {
            foreach (KeyValuePair<string, string> item in replaceKey)
            {
                foreach (PropItem item2 in items.Where(m => m.Key == item.Key))
                {
                    item2.Key = item.Value;
                }
            }
        }
        AddOrUpdateProp(items);
        return false;
    }

    public void Reset(string key, string value)
    {
        PropItem propItem = Props.Where(m => m.Key.Equals(key)).FirstOrDefault();
        if (propItem != null)
        {
            propItem.Value = value;
        }
    }

    public string GetProp(string property)
    {
        if (Props.Count == 0)
        {
            return string.Empty;
        }
        return (from m in Props
                where m.Key.Equals(property)
                select m.Value).FirstOrDefault();
    }

    public long GetLongProp(string property)
    {
        long result = 0L;
        string prop = GetProp(property);
        if (!string.IsNullOrEmpty(prop))
        {
            long.TryParse(prop, out result);
        }
        return result;
    }

    public int GetIntProp(string property)
    {
        int result = 0;
        string prop = GetProp(property);
        if (!string.IsNullOrEmpty(prop))
        {
            int.TryParse(prop, out result);
        }
        return result;
    }

    public DateTime GetDateTimeProp(string property)
    {
        DateTime result = DateTime.MinValue;
        string prop = GetProp(property);
        if (!string.IsNullOrEmpty(prop))
        {
            DateTime.TryParse(prop, out result);
        }
        return result;
    }
}
