using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.devicemgt.DeviceInfo;
using lenovo.mbg.service.framework.devicemgt.DeviceOperator;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt;

public class FastbootDeviceEx : DeviceEx
{
    public override IAndroidDevice? Property { get; set; }

    public FastbootDeviceEx()
    {
        Property = new FastbootAndroidDevice(this);
        DeviceOperator = new FastbootOperator();
    }

    public override void Load()
    {
        ((ILoadDeviceData)Property!).Load();
    }

    protected override void OnPhysicalStatusChanged(DevicePhysicalStateEx prev, DevicePhysicalStateEx current)
    {
        if (current != DevicePhysicalStateEx.Online)
        {
            return;
        }
        SoftStatus = DeviceSoftStateEx.Connecting;
        System.Threading.Tasks.Task.Run(delegate
        {
            Load();
        }).ContinueWith(delegate
        {
            if (PhysicalStatus == DevicePhysicalStateEx.Online)
            {
                LogHelper.LogInstance.Info("TestLog-->device softStatus Changed ModelName:" + Property?.ModelName + ",Imei1:" + Property?.IMEI1);
                if (!string.IsNullOrEmpty(Property?.ModelName) || !string.IsNullOrEmpty(Property?.IMEI1))
                {
                    SoftStatus = DeviceSoftStateEx.Online;
                }
                else
                {
                    SoftStatus = DeviceSoftStateEx.Offline;
                }
            }
            else
            {
                SoftStatus = DeviceSoftStateEx.Offline;
            }
        });
    }
}
