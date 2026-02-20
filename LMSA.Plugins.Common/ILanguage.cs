namespace lenovo.mbg.service.framework.services;

/// <summary>
/// Provides language/localization support to plugins.
/// </summary>
public interface ILanguage
{
    string GetString(string key);

    string GetString(string key, params object[] args);
}
