using System;

namespace lenovo.mbg.service.framework.devicemgt.DeviceInfo;

[Serializable]
public class PropItem
{
    public string Key { get; set; }

    public string Value { get; set; }

    public override string ToString()
    {
        return "Key:" + Key + ",Value:" + Value;
    }
}
