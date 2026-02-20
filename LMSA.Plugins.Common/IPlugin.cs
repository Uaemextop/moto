namespace lenovo.mbg.service.lmsa.plugins
{
    /// <summary>
    /// Interface that all LMSA plugins must implement.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Unique identifier for the plugin.
        /// </summary>
        string PluginID { get; }

        /// <summary>
        /// Human-readable name of the plugin.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Plugin description.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Plugin version.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Initializes the plugin and its resources.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Shuts down the plugin and releases resources.
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Gets whether the plugin is currently initialized.
        /// </summary>
        bool IsInitialized { get; }
    }
}
