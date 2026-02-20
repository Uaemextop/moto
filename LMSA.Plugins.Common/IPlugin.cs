namespace lenovo.mbg.service.framework.services;

/// <summary>
/// Interface for LMSA plugin lifecycle and UI management.
/// </summary>
public interface IPlugin
{
    void Init();
    object? CreateControl(object? messageBox);
    bool CanClose();
    bool IsExecuteWork();
    void OnSelected(string val);
    void OnSelecting(string val);
    void OnInit(object data);
    bool IsNonBusinessPage();
}
