using System;

namespace lenovo.mbg.service.framework.services;

/// <summary>
/// Interface for host application operations available to plugins.
/// </summary>
public interface IHostOperationService
{
    IntPtr ShowMaskLayer(string uid, int state = 0);
    void CloseMaskLayer(string uid);
    string GetAppVersion();
    void ShowGuideTips();
    void ShowFeedBack();
    void ShowBannerAsync(object data);
}
