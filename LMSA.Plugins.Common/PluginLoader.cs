using lenovo.mbg.service.framework.common;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace lenovo.mbg.service.lmsa.plugin
{
    /// <summary>
    /// Loads and manages LMSA plugins from the plugins directory.
    /// Each plugin resides in a GUID-named subdirectory and implements IPlugin.
    /// </summary>
    public class PluginLoader
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(PluginLoader));

        private readonly Dictionary<string, IPlugin> _plugins
            = new Dictionary<string, IPlugin>(StringComparer.OrdinalIgnoreCase);

        private readonly string _pluginsDirectory;

        /// <summary>
        /// Initializes a new PluginLoader using the default plugins directory.
        /// </summary>
        public PluginLoader() : this(Configurations.PluginsDirectory) { }

        /// <summary>
        /// Initializes a new PluginLoader with a custom plugins directory path.
        /// </summary>
        public PluginLoader(string pluginsDirectory)
        {
            _pluginsDirectory = pluginsDirectory
                ?? throw new ArgumentNullException(nameof(pluginsDirectory));
        }

        /// <summary>
        /// Gets all currently loaded plugins.
        /// </summary>
        public IReadOnlyDictionary<string, IPlugin> LoadedPlugins => _plugins;

        /// <summary>
        /// Discovers and loads all plugins from the plugins directory.
        /// </summary>
        /// <returns>Number of plugins successfully loaded.</returns>
        public int LoadAll()
        {
            if (!Directory.Exists(_pluginsDirectory))
            {
                _log.Warn($"Plugins directory not found: {_pluginsDirectory}");
                return 0;
            }

            int loaded = 0;
            foreach (string pluginDir in Directory.GetDirectories(_pluginsDirectory))
            {
                loaded += LoadFromDirectory(pluginDir);
            }

            _log.Info($"Loaded {loaded} plugin(s) from {_pluginsDirectory}");
            return loaded;
        }

        /// <summary>
        /// Loads plugins from a specific directory.
        /// </summary>
        private int LoadFromDirectory(string directory)
        {
            int loaded = 0;
            foreach (string dllPath in Directory.GetFiles(directory, "lenovo.mbg.service.lmsa.*.dll"))
            {
                try
                {
                    var assembly = Assembly.LoadFrom(dllPath);
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (!typeof(IPlugin).IsAssignableFrom(type) || type.IsInterface || type.IsAbstract)
                            continue;

                        var plugin = Activator.CreateInstance(type) as IPlugin;
                        if (plugin == null)
                            continue;
                        plugin.Initialize();
                        _plugins[plugin.PluginId] = plugin;
                        _log.Info($"Loaded plugin: {plugin.Name} ({plugin.PluginId}) v{plugin.Version}");
                        loaded++;
                    }
                }
                catch (Exception ex)
                {
                    _log.Error($"Failed to load plugin from {dllPath}", ex);
                }
            }
            return loaded;
        }

        /// <summary>
        /// Shuts down and unloads all loaded plugins.
        /// </summary>
        public void UnloadAll()
        {
            foreach (var kvp in _plugins)
            {
                try
                {
                    kvp.Value.Shutdown();
                    _log.Info($"Shutdown plugin: {kvp.Value.Name}");
                }
                catch (Exception ex)
                {
                    _log.Error($"Error shutting down plugin {kvp.Key}", ex);
                }
            }
            _plugins.Clear();
        }

        /// <summary>
        /// Gets a plugin by its unique GUID identifier.
        /// </summary>
        /// <param name="pluginId">Plugin GUID string.</param>
        /// <returns>Plugin instance or null if not found.</returns>
        public IPlugin? GetPlugin(string pluginId)
        {
            _plugins.TryGetValue(pluginId, out var plugin);
            return plugin;
        }
    }
}
