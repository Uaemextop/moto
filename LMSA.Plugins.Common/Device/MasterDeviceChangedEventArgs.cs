using System;

namespace lenovo.mbg.service.framework.services.Device;

/// <summary>
/// Event args raised when the master device changes.
/// </summary>
public class MasterDeviceChangedEventArgs : EventArgs
{
    public DeviceEx Previous { get; private set; }
    public DeviceEx Current { get; private set; }

    public MasterDeviceChangedEventArgs(DeviceEx previous, DeviceEx current)
    {
        Previous = previous;
        Current = current;
    }
}
