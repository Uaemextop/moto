using System;
using System.Threading.Tasks;

namespace lenovo.mbg.service.framework.services;

public interface IMessageBox
{
    void ShowDownloadCenter(bool isShow = true);

    void SelRegistedDevIfExist(string category, Action<string> callBack = null);

    void CallMotoCare(string imei, object wModel);

    Task ShowQuitSurvey();

    void ShowMutilIcon(bool showIcon, bool showList);

    void ChangeIsEnabled(bool isEnabled);

    Task ShowB2BExpired(int _modeType);

    Task ShowB2BRemind(int used, int total);

    void ShowMutilTutorials(bool show);

    void ChangePinStoryboard(bool start);

    void SetDriverButtonStatus(string _code);

    void SetDeviceConnectIconStatus(int _status);

    void LogOut();
}
