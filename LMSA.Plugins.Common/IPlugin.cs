namespace lenovo.mbg.service.lmsa.plugin
{
    /// <summary>
    /// Core interface that all LMSA plugins must implement.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>Gets the unique GUID identifier for this plugin.</summary>
        string PluginId { get; }

        /// <summary>Gets the human-readable name of the plugin.</summary>
        string Name { get; }

        /// <summary>Gets the plugin version string.</summary>
        string Version { get; }

        /// <summary>
        /// Called once when the plugin is loaded. Perform initialization here.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Called when the plugin is being unloaded. Release resources here.
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Gets whether this plugin is currently active/enabled.
        /// </summary>
        bool IsEnabled { get; }
    }
}
