using System;
using System.Collections.Generic;

namespace lenovo.mbg.service.framework.services.Device;

public interface INetworkAdapterListener
{
    void OnWifiMonitoringEndPointChanged(List<Tuple<string, string>> endpoints);
}
