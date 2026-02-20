using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt.DeviceInfo;

[Serializable]
public class AndroidDeviceProperty : IAndroidDevice, ILoadDeviceData
{
    private PropInfo _PropInfo;

    protected List<string> countryElements = new List<string>
    {
        "ro.lenovo.easyimage.code",
        "persist.sys.withsim.country",
        "gsm.operator.iso-country"
    };

    public static readonly string[] SN_PROP_FIELDS = new string[]
    {
        "sys.customsn.showcode", "ro.lenovosn2", "persist.radio.factory_phone_sn",
        "gsm.lenovosn2", "persist.sys.snvalue", "ro.serialno", "ro.odm.lenovo.sn"
    };

    public static readonly string[] SN_PROP_INVALID_VALUES = new string[] { "UNKNOWN" };

    protected static Dictionary<string, List<string>> propsMapping = new Dictionary<string, List<string>>
    {
        { "imei1", new List<string> { "device.imei1", "gsm.imei1" } },
        { "imei2", new List<string> { "device.imei2", "gsm.imei2" } },
        { "sn", new List<string> { "sys.customsn.showcode", "ro.lenovosn2", "persist.radio.factory_phone_sn", "gsm.lenovosn2", "persist.sys.snvalue", "ro.serialno", "ro.odm.lenovo.sn" } },
        { "pn", new List<string> { "gsm.serial", "gsm.sn1", "ro.psnno", "sys.pn", "ro.pcbasn", "persist.sys.cit.sn", "gsm.sn", "persist.sys.pnvalue", "ro.odm.lenovo.psn", "sys.pcba.serialno" } }
    };

    private string _pn = string.Empty;
    private string _sn = string.Empty;

    public string AndroidVersion => _PropInfo.GetProp("ro.build.version.release");

    public string Brand => _PropInfo.GetProp("ro.product.brand");

    public string Carrier => _PropInfo.GetProp("ro.carrier");

    public string CountryCode
    {
        get
        {
            string text = null;
            foreach (string countryElement in countryElements)
            {
                text = _PropInfo.GetProp(countryElement);
                if (text != null)
                {
                    break;
                }
            }
            return text;
        }
    }

    public string FingerPrint => _PropInfo.GetProp("ro.build.fingerprint");

    public string HWCode => GetHwCode();

    public string IMEI1
    {
        get
        {
            List<string> list = propsMapping["imei1"];
            string text2 = null;
            foreach (string item in list)
            {
                text2 = _PropInfo.GetProp(item);
                if (!string.IsNullOrEmpty(text2))
                {
                    break;
                }
            }
            return text2;
        }
    }

    public string IMEI2
    {
        get
        {
            List<string> list = propsMapping["imei2"];
            string text2 = null;
            foreach (string item in list)
            {
                text2 = _PropInfo.GetProp(item);
                if (!string.IsNullOrEmpty(text2))
                {
                    break;
                }
            }
            return text2;
        }
    }

    public string ModelName
    {
        get
        {
            string result = _PropInfo.GetProp("ro.product.model");
            if (!string.IsNullOrEmpty(Brand) && Regex.IsMatch(Brand, "motorola", RegexOptions.IgnoreCase))
            {
                string prop = _PropInfo.GetProp("ro.boot.hardware.sku");
                if (!string.IsNullOrEmpty(prop) && !prop.StartsWith("CMIT_ID"))
                {
                    result = prop;
                }
            }
            return result;
        }
    }

    public string ModelName2 => _PropInfo.GetProp("ro.product.model");

    public string OtaModel => _PropInfo.GetProp("ro.product.ota.model");

    public string ModelId => string.Empty;

    public Dictionary<string, string> Others
    {
        get
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (_PropInfo.Props != null)
            {
                _PropInfo.Props.ForEach(n => dic[n.Key] = n.Value);
            }
            return dic;
        }
    }

    public string PN
    {
        get
        {
            if (string.IsNullOrEmpty(_pn))
            {
                _pn = GetPN();
            }
            return _pn;
        }
    }

    public string RomVersion => _PropInfo.GetProp("ro.build.version.incremental");

    public string CustomerVersion => _PropInfo.GetProp("ro.build.customer-version");

    public string RoHardWare => _PropInfo.GetProp("ro.hardware");

    public string SN
    {
        get
        {
            if (string.IsNullOrEmpty(_sn))
            {
                _sn = GetSN();
            }
            return _sn;
        }
    }

    public double BatteryQuantityPercentage
    {
        get
        {
            double.TryParse(_PropInfo.GetProp("battery.quantity"), out double result);
            return result;
        }
    }

    public string InternalStoragePath => _PropInfo.GetProp("Internal.Storage.Path");

    public string ExternalStoragePath => _PropInfo.GetProp("External.Storage.Path");

    public string TotalExternalStorageWithUnit => _PropInfo.GetProp("External.Storage.TotalWithUnit");

    public string UsedExternalStorageWithUnit => _PropInfo.GetProp("External.Storage.UsedWithUnit");

    public string TotalInternalStorageWithUnit => _PropInfo.GetProp("Internal.Storage.TotalWithUnit");

    public string UsedInternalStorageWithUnit => _PropInfo.GetProp("Internal.Storage.UsedWithUnit");

    public string FreeExternalStorageWithUnit => _PropInfo.GetProp("External.Storage.FreeWithUnit");

    public string FreeInternalStorageWithUnit => _PropInfo.GetProp("Internal.Storage.FreeWithUnit");

    public long TotalExternalStorage => _PropInfo.GetLongProp("External.Storage.Total");

    public long UsedExternalStorage => _PropInfo.GetLongProp("External.Storage.Used");

    public long FreeExternalStorage => _PropInfo.GetLongProp("External.Storage.Free");

    public long TotalInternalStorage => _PropInfo.GetLongProp("Internal.Storage.Total");

    public long UsedInternalStorage => _PropInfo.GetLongProp("Internal.Storage.Used");

    public long FreeInternalStorage => _PropInfo.GetLongProp("Internal.Storage.Free");

    public string Operator => _PropInfo.GetProp("phone.type");

    public string FsgVersion
    {
        get
        {
            string prop = _PropInfo.GetProp("ril.baseband.config.version");
            if (string.IsNullOrEmpty(prop))
            {
                prop = _PropInfo.GetProp("gsm.version.baseband");
            }
            if (string.IsNullOrEmpty(prop))
            {
                prop = _PropInfo.GetProp("vendor.ril.baseband.config.version");
            }
            return prop;
        }
    }

    public string Processor => _PropInfo.GetProp("processor");

    public string Uptime => _PropInfo.GetProp("upTime");

    public int ApiLevel => _PropInfo.GetIntProp("ro.build.version.sdk");

    public string Category
    {
        get
        {
            string modelName = ModelName;
            if ("Lenovo PB2-690Y".Equals(modelName, StringComparison.OrdinalIgnoreCase) ||
                "Lenovo PB2-690M".Equals(modelName, StringComparison.OrdinalIgnoreCase))
            {
                return "tablet";
            }
            List<string> source = new List<string> { "phone", "tablet" };
            string _category = _PropInfo.GetProp("ro.lenovo.device");
            if (string.IsNullOrEmpty(_category) || !source.Contains(_category, StringComparer.CurrentCultureIgnoreCase))
            {
                _category = _PropInfo.GetProp("ro.build.characteristics");
            }
            if (string.IsNullOrEmpty(_category) || !source.Contains(_category, StringComparer.CurrentCultureIgnoreCase))
            {
                _category = _PropInfo.GetProp("ro.odm.lenovo.device");
            }
            return source.FirstOrDefault(n => n.Equals(_category, StringComparison.CurrentCultureIgnoreCase));
        }
    }

    public int SimCount
    {
        get
        {
            string prop = _PropInfo.GetProp("persist.radio.multisim.config");
            if (string.IsNullOrEmpty(prop) ||
                "SS".Equals(prop?.Trim(), StringComparison.CurrentCultureIgnoreCase) ||
                "ssss".Equals(prop?.Trim(), StringComparison.CurrentCultureIgnoreCase))
            {
                return 1;
            }
            return 2;
        }
    }

    public AndroidDeviceProperty()
    {
        _PropInfo = new PropInfo();
    }

    public AndroidDeviceProperty(PropInfo propInfo)
    {
        _PropInfo = propInfo;
    }

    public void Load()
    {
    }

    public void Load(PropInfo prop)
    {
        if (prop != null)
        {
            _PropInfo.AddOrUpdateProp(prop.Props);
        }
    }

    public void AddOrUpdate(PropItem prop)
    {
        _PropInfo.AddOrUpdateProp(prop);
    }

    private string GetPN()
    {
        List<string> list = propsMapping["pn"];
        foreach (string item in list)
        {
            string text2 = _PropInfo.GetProp(item);
            if (string.IsNullOrEmpty(text2))
            {
                continue;
            }
            if (text2.Contains(" "))
            {
                string[] array = text2.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (array.Length != 0)
                {
                    text2 = array[0];
                }
            }
            if (text2.Length == 18 || text2.Length == 23 || text2.Length == 25)
            {
                return text2.Trim();
            }
        }
        return string.Empty;
    }

    private string GetSN()
    {
        List<string> list = propsMapping["sn"];
        foreach (string item in list)
        {
            string empty = _PropInfo.GetProp(item);
            if (!string.IsNullOrEmpty(empty) && !SN_PROP_INVALID_VALUES.Contains(empty.ToUpper()))
            {
                return empty.Trim();
            }
        }
        return string.Empty;
    }

    private string GetHwCode()
    {
        if (string.IsNullOrEmpty(PN))
        {
            return string.Empty;
        }
        return PN.Length switch
        {
            18 => PN.Substring(3, 2),
            23 => PN.Substring(14, 2),
            25 => (ModelName == null || !ModelName.Replace(" ", "").Equals("LenovoA2020a40", StringComparison.CurrentCultureIgnoreCase))
                ? PN.Substring(23, 2)
                : PN.Substring(14, 2),
            _ => string.Empty,
        };
    }

    public string GetPropertyValue(string name)
    {
        return _PropInfo.GetProp(name);
    }
}
