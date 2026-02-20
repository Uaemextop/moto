namespace lenovo.mbg.service.framework.services;

/// <summary>
/// Provides message dialog functionality to plugins.
/// </summary>
public interface IMessageBox
{
    void Show(string message);

    bool Confirm(string message);
}
