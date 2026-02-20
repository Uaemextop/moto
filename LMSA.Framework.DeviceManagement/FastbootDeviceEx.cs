using System.Threading.Tasks;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt.DeviceInfo;
using lenovo.mbg.service.framework.devicemgt.DeviceOperator;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt;

public class FastbootDeviceEx : DeviceEx
{
    public override IAndroidDevice Property { get; set; }

    public FastbootDeviceEx()
    {
        Property = new FastbootAndroidDevice(this);
        base.DeviceOperator = new FastbootOperator();
    }

    public override void Load()
    {
        ((ILoadDeviceData)Property).Load();
    }

    protected override void OnPhysicalStatusChanged(DevicePhysicalStateEx prev, DevicePhysicalStateEx current)
    {
        if (current != DevicePhysicalStateEx.Online)
        {
            return;
        }
        base.SoftStatus = DeviceSoftStateEx.Connecting;
        Task.Run(delegate
        {
            Load();
        }).ContinueWith(delegate
        {
            if (base.PhysicalStatus == DevicePhysicalStateEx.Online)
            {
                LogHelper.LogInstance.Info("TestLog-->device softStatus Changed ModelName:" + Property.ModelName + ",Imei1:" + Property.IMEI1);
                if (!string.IsNullOrEmpty(Property.ModelName) || !string.IsNullOrEmpty(Property.IMEI1))
                {
                    base.SoftStatus = DeviceSoftStateEx.Online;
                }
                else
                {
                    base.SoftStatus = DeviceSoftStateEx.Offline;
                    if (base.WorkType == DeviceWorkType.None)
                    {
                        GlobalCmdHelper.Instance.Execute(new
                        {
                            type = GlobalCmdType.READ_DEVICEINFO_CALLBACK,
                            data = (Property.Others.Count == 0)
                        });
                    }
                }
            }
            else
            {
                base.SoftStatus = DeviceSoftStateEx.Offline;
            }
        });
    }
}
