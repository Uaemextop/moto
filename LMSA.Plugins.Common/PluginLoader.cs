using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;

namespace lenovo.mbg.service.lmsa.plugins
{
    /// <summary>
    /// Loads and manages LMSA plugins from the plugins directory.
    /// </summary>
    public class PluginLoader
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(PluginLoader));
        private readonly string _pluginsDirectory;
        private readonly Dictionary<string, IPlugin> _loadedPlugins = new Dictionary<string, IPlugin>();

        public IReadOnlyDictionary<string, IPlugin> LoadedPlugins => _loadedPlugins;

        public PluginLoader(string pluginsDirectory)
        {
            _pluginsDirectory = pluginsDirectory ?? throw new ArgumentNullException(nameof(pluginsDirectory));
        }

        /// <summary>
        /// Discovers and loads all plugins from the plugins directory.
        /// Each plugin resides in a GUID-named subdirectory.
        /// </summary>
        public void LoadAll()
        {
            if (!Directory.Exists(_pluginsDirectory))
            {
                _log.Warn($"Plugins directory not found: {_pluginsDirectory}");
                return;
            }

            foreach (var dir in Directory.GetDirectories(_pluginsDirectory))
            {
                LoadPluginsFromDirectory(dir);
            }

            _log.Info($"Loaded {_loadedPlugins.Count} plugins");
        }

        /// <summary>
        /// Loads a single plugin from a DLL path.
        /// </summary>
        public IPlugin? LoadPlugin(string dllPath)
        {
            if (!File.Exists(dllPath))
            {
                _log.Error($"Plugin DLL not found: {dllPath}");
                return null;
            }

            try
            {
                var assembly = Assembly.LoadFrom(dllPath);
                var pluginTypes = assembly.GetTypes()
                    .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                foreach (var pluginType in pluginTypes)
                {
                    var plugin = (IPlugin?)Activator.CreateInstance(pluginType);
                    if (plugin == null) continue;
                    if (!_loadedPlugins.ContainsKey(plugin.PluginID))
                    {
                        _loadedPlugins[plugin.PluginID] = plugin;
                        _log.Info($"Loaded plugin: {plugin.Name} ({plugin.PluginID})");
                        return plugin;
                    }
                    else
                    {
                        _log.Warn($"Duplicate plugin ID: {plugin.PluginID} ({plugin.Name})");
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Failed to load plugin from {dllPath}", ex);
            }

            return null;
        }

        /// <summary>
        /// Initializes all loaded plugins.
        /// </summary>
        public void InitializeAll()
        {
            foreach (var kvp in _loadedPlugins)
            {
                try
                {
                    if (!kvp.Value.IsInitialized)
                    {
                        kvp.Value.Initialize();
                        _log.Info($"Initialized plugin: {kvp.Value.Name}");
                    }
                }
                catch (Exception ex)
                {
                    _log.Error($"Failed to initialize plugin: {kvp.Value.Name}", ex);
                }
            }
        }

        /// <summary>
        /// Shuts down all loaded plugins.
        /// </summary>
        public void ShutdownAll()
        {
            foreach (var kvp in _loadedPlugins)
            {
                try
                {
                    if (kvp.Value.IsInitialized)
                    {
                        kvp.Value.Shutdown();
                        _log.Info($"Shut down plugin: {kvp.Value.Name}");
                    }
                }
                catch (Exception ex)
                {
                    _log.Error($"Failed to shut down plugin: {kvp.Value.Name}", ex);
                }
            }
        }

        /// <summary>
        /// Gets a plugin by its ID.
        /// </summary>
        public IPlugin? GetPlugin(string pluginId)
        {
            return _loadedPlugins.TryGetValue(pluginId, out var plugin) ? plugin : null;
        }

        /// <summary>
        /// Unloads a plugin by its ID.
        /// </summary>
        public bool UnloadPlugin(string pluginId)
        {
            if (_loadedPlugins.TryGetValue(pluginId, out var plugin))
            {
                try
                {
                    if (plugin.IsInitialized)
                        plugin.Shutdown();
                }
                catch (Exception ex)
                {
                    _log.Warn($"Error shutting down plugin {plugin.Name}: {ex.Message}");
                }

                _loadedPlugins.Remove(pluginId);
                _log.Info($"Unloaded plugin: {plugin.Name}");
                return true;
            }
            return false;
        }

        private void LoadPluginsFromDirectory(string directory)
        {
            var dllFiles = Directory.GetFiles(directory, "lenovo.mbg.service.lmsa.*.dll");
            foreach (var dll in dllFiles)
            {
                LoadPlugin(dll);
            }
        }
    }
}
