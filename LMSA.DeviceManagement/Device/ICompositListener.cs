using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.devicemgt;

public interface ICompositListener : IPhysicalConnectionListener, INetworkAdapterListener
{
}
