namespace lenovo.mbg.service.framework.services;

/// <summary>
/// Core plugin interface that all LMSA plugins must implement.
/// Defines the lifecycle and UI creation contract for plugins.
/// </summary>
public interface IPlugin
{
    /// <summary>Initializes the plugin. Called once when the plugin is first loaded.</summary>
    void Init();

    /// <summary>
    /// Creates the main UI control for this plugin.
    /// Returns an object that should be a WPF FrameworkElement on Windows.
    /// </summary>
    object CreateControl(IMessageBox iMsg);

    /// <summary>Returns true if the plugin can be closed without interrupting work.</summary>
    bool CanClose();

    /// <summary>Returns true if the plugin is currently performing work.</summary>
    bool IsExecuteWork();

    /// <summary>Called when the plugin's page is selected/navigated to.</summary>
    void OnSelected(string val);

    /// <summary>Called before the plugin's page is selected.</summary>
    void OnSelecting(string val);

    /// <summary>Called to initialize the plugin with optional data.</summary>
    void OnInit(object data);

    /// <summary>Returns true if this plugin represents a non-business (informational) page.</summary>
    bool IsNonBusinessPage();
}
