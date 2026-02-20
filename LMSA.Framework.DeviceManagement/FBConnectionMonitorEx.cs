using System;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.socket;

namespace lenovo.mbg.service.framework.devicemgt;

public class FBConnectionMonitorEx : AbstractConnectionMonitorEx
{
    public FBConnectionMonitorEx()
    {
    }

    public override void Start()
    {
    }

    public override void Stop()
    {
    }

    public override void Dispose()
    {
        Stop();
    }
}
