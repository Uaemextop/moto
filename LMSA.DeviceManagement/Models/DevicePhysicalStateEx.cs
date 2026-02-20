namespace lenovo.mbg.service.framework.devicemgt
{
    /// <summary>
    /// Represents the physical state of an Android device.
    /// Matches the DevicePhysicalStateEx enum from the decompiled LMSA sources.
    /// </summary>
    public enum DevicePhysicalStateEx
    {
        Offline,
        Online,
        Fastboot,
        Recovery,
        EDL,
        Unknown
    }
}
