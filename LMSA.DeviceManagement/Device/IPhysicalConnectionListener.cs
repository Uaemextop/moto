namespace lenovo.mbg.service.framework.services.Device;

public interface IPhysicalConnectionListener
{
    void OnConnect(DeviceEx device, DevicePhysicalStateEx phyState);

    void OnDisconnect(DeviceEx device);
}
