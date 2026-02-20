namespace lenovo.mbg.service.framework.devicemgt
{
    /// <summary>
    /// Common interface for device operators (ADB and Fastboot).
    /// </summary>
    public interface IDeviceOperator
    {
        /// <summary>
        /// Executes a command on the device.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="timeout">Timeout in milliseconds.</param>
        /// <param name="deviceID">Optional device serial number.</param>
        /// <returns>Command output.</returns>
        string Command(string command, int timeout = -1, string deviceID = "");
    }
}
