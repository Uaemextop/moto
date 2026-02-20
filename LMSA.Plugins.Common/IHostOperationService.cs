using System;

namespace lenovo.mbg.service.framework.services;

public interface IHostOperationService
{
    IntPtr ShowMaskLayer(string uid);

    void CloseMaskLayer(string uid);

    string GetAppVersion();

    void ShowGuideTips();

    void ShowFeedBack();

    void ShowBannerAsync(object data);
}
