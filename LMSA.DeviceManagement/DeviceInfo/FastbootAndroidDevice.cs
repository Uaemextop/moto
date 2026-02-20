using System;
using System.Collections.Generic;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt.DeviceInfo;

public class FastbootAndroidDevice : IAndroidDevice, ILoadDeviceData
{
    private ReadPropertiesInFastboot _ReadPropertiesInFastboot;

    public string AndroidVersion => _ReadPropertiesInFastboot.GetProp("androidVer") ?? string.Empty;

    public double BatteryQuantityPercentage => 0.0;

    public string Brand => string.Empty;

    public string Carrier => _ReadPropertiesInFastboot.GetProp("ro.carrier") ?? string.Empty;

    public string CountryCode => string.Empty;

    public string CustomerVersion => string.Empty;

    public string FingerPrint => _ReadPropertiesInFastboot.GetProp("ro.build.fingerprint") ?? string.Empty;

    public long FreeExternalStorage => 0L;

    public long FreeInternalStorage => 0L;

    public string HWCode => string.Empty;

    public string IMEI1 => _ReadPropertiesInFastboot.GetProp("imei") ?? string.Empty;

    public string IMEI2 => string.Empty;

    public string ModelId => string.Empty;

    public string ModelName
    {
        get
        {
            string? prop = _ReadPropertiesInFastboot.GetProp("sku");
            if (string.IsNullOrEmpty(prop))
            {
                prop = _ReadPropertiesInFastboot.GetProp("ro.boot.hardware.sku");
            }
            return prop ?? string.Empty;
        }
    }

    public string ModelName2
    {
        get
        {
            string? prop = _ReadPropertiesInFastboot.GetProp("sku");
            if (string.IsNullOrEmpty(prop))
            {
                prop = _ReadPropertiesInFastboot.GetProp("ro.boot.hardware.sku");
            }
            return prop ?? string.Empty;
        }
    }

    public string Operator => string.Empty;

    public string OtaModel => string.Empty;

    public Dictionary<string, string> Others => _ReadPropertiesInFastboot.Props;

    public string PN => string.Empty;

    public string RoHardWare => string.Empty;

    public string RomVersion => string.Empty;

    public string SN => GetPropertyValue("serialno") ?? string.Empty;

    public long TotalExternalStorage => 0L;

    public long TotalInternalStorage => 0L;

    public long UsedExternalStorage => 0L;

    public long UsedInternalStorage => 0L;

    public string FsgVersion => _ReadPropertiesInFastboot.GetProp("version-baseband") ?? string.Empty;

    public string Processor => string.Empty;

    public string Uptime => string.Empty;

    public string InternalStoragePath => string.Empty;

    public string ExternalStoragePath => string.Empty;

    public string TotalExternalStorageWithUnit => string.Empty;

    public string UsedExternalStorageWithUnit => string.Empty;

    public string FreeExternalStorageWithUnit => string.Empty;

    public string TotalInternalStorageWithUnit => string.Empty;

    public string UsedInternalStorageWithUnit => string.Empty;

    public string FreeInternalStorageWithUnit => string.Empty;

    public int ApiLevel => -1;

    public string Category => "phone";

    public int SimCount
    {
        get
        {
            string? propertyValue = GetPropertyValue("oem hw dualsim");
            if (!string.IsNullOrEmpty(propertyValue) && propertyValue.Equals("true", StringComparison.CurrentCultureIgnoreCase))
            {
                return 2;
            }
            return 1;
        }
    }

    public FastbootAndroidDevice(DeviceEx device)
    {
        _ReadPropertiesInFastboot = new ReadPropertiesInFastboot(device);
    }

    public void Load()
    {
        _ReadPropertiesInFastboot.Run();
    }

    public string GetPropertyValue(string name)
    {
        return _ReadPropertiesInFastboot.GetProp(name) ?? string.Empty;
    }
}
