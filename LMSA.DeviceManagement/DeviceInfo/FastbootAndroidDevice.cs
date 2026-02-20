using System.Collections.Generic;
using Newtonsoft.Json;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt.DeviceInfo;

/// <summary>
/// Represents device properties retrieved from a device in fastboot mode.
/// </summary>
public class FastbootAndroidDevice : IAndroidDevice, ILoadDeviceData
{
    private readonly ReadPropertiesInFastboot _readPropertiesInFastboot;

    public string AndroidVersion => _readPropertiesInFastboot.GetProp("androidVer");

    public double BatteryQuantityPercentage => 0.0;

    public string Brand => string.Empty;

    public string Carrier => _readPropertiesInFastboot.GetProp("ro.carrier");

    public string Category => string.Empty;

    public string CountryCode => string.Empty;

    public string CustomerVersion => string.Empty;

    public string FingerPrint => _readPropertiesInFastboot.GetProp("ro.build.fingerprint");

    public long FreeExternalStorage => 0L;

    public long FreeInternalStorage => 0L;

    public string FreeExternalStorageWithUnit => string.Empty;

    public string FreeInternalStorageWithUnit => string.Empty;

    public string FsgVersion => _readPropertiesInFastboot.GetProp("version-baseband");

    public string HWCode => string.Empty;

    public string IMEI1 => _readPropertiesInFastboot.GetProp("imei");

    public string IMEI2 => string.Empty;

    public string InternalStoragePath => string.Empty;

    public string ExternalStoragePath => string.Empty;

    public int ApiLevel => 0;

    public string ModelId => string.Empty;

    public string ModelName
    {
        get
        {
            string prop = _readPropertiesInFastboot.GetProp("sku");
            if (string.IsNullOrEmpty(prop))
            {
                prop = _readPropertiesInFastboot.GetProp("ro.boot.hardware.sku");
            }
            return prop;
        }
    }

    public string ModelName2 => ModelName;

    public string Operator => string.Empty;

    public string OtaModel => string.Empty;

    [JsonIgnore]
    public Dictionary<string, string> Others => _readPropertiesInFastboot.Props;

    public string PN => string.Empty;

    public string RoHardWare => string.Empty;

    public string RomVersion => string.Empty;

    public string SN => GetPropertyValue("serialno");

    public int SimCount => _readPropertiesInFastboot.GetProp("oem hw dualsim") == "Dual" ? 2 : 1;

    public long TotalExternalStorage => 0L;

    public long TotalInternalStorage => 0L;

    public string TotalExternalStorageWithUnit => string.Empty;

    public string TotalInternalStorageWithUnit => string.Empty;

    public string Uptime => string.Empty;

    public long UsedExternalStorage => 0L;

    public long UsedInternalStorage => 0L;

    public string UsedExternalStorageWithUnit => string.Empty;

    public string UsedInternalStorageWithUnit => string.Empty;

    public string Processor => string.Empty;

    public FastbootAndroidDevice(lenovo.mbg.service.framework.services.Device.DeviceEx device)
    {
        _readPropertiesInFastboot = new ReadPropertiesInFastboot(device);
    }

    public void Load()
    {
        _readPropertiesInFastboot.Run();
    }

    public string GetPropertyValue(string name)
    {
        return _readPropertiesInFastboot.GetProp(name);
    }
}
