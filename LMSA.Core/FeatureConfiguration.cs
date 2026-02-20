namespace lenovo.mbg.service.lmsa.core;

/// <summary>
/// Controls feature availability in the open-source reimplementation.
/// All features are available by default for offline/standalone operation.
/// </summary>
public static class FeatureConfiguration
{
    /// <summary>
    /// When true, all features operate in offline mode with full functionality.
    /// Default: true.
    /// </summary>
    public static bool OfflineMode { get; set; } = true;

    /// <summary>
    /// When true, developer options are visible in the UI.
    /// Default: true.
    /// </summary>
    public static bool ShowDeveloperOptions { get; set; } = true;

    /// <summary>
    /// When true, B2B informational popups are suppressed in offline mode.
    /// Default: true.
    /// </summary>
    public static bool SuppressB2BPopups { get; set; } = true;

    /// <summary>
    /// When true, multi-device support is enabled.
    /// Default: true.
    /// </summary>
    public static bool MultiDeviceSupport { get; set; } = true;

    /// <summary>
    /// When true, all order items default to enabled and displayed.
    /// Default: true.
    /// </summary>
    public static bool DefaultOrdersEnabled { get; set; } = true;
}
