namespace lenovo.mbg.service.framework.common
{
    /// <summary>
    /// Represents the result of a device operation.
    /// Matches the Result enum from the decompiled LMSA sources.
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
