namespace lenovo.mbg.service.framework.services;

public interface IPlugin
{
    void Init();

    bool CanClose();

    bool IsExecuteWork();

    void OnSelected(string val);

    void OnSelecting(string val);

    void OnInit(object data);

    bool IsNonBusinessPage();
}
