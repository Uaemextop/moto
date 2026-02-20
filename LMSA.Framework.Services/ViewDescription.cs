using System;

namespace lenovo.mbg.service.framework.services;

public class ViewDescription
{
    public string UIID { get; private set; }

    public Type ViewType { get; private set; }

    public Type ViewModelType { get; private set; }

    public object View { get; set; }

    public IViewModelBase ViewModel { get; set; }

    public ViewDescription(Type viewType, Type viewModelType, string uiid = null)
    {
        ViewType = viewType;
        ViewModelType = viewModelType;
        UIID = ViewType.FullName + uiid;
    }
}
