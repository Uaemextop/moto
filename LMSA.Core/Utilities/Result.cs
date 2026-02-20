namespace lenovo.mbg.service.framework.common
{
    /// <summary>
    /// Represents the outcome of a device operation.
    /// </summary>
    public enum Result
    {
        PASSED,
        FAILED,
        QUIT,
        FASTBOOT_FLASH_FAILED,
        FASTBOOT_SHELL_FAILED,
        FASTBOOT_DEGRADE_QUIT
    }
}
