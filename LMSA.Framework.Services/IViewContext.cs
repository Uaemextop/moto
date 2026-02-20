using System;

namespace lenovo.mbg.service.framework.services;

public interface IViewContext
{
    object SwitchView(ViewDescription view);

    object SwitchView(ViewDescription view, object initilizeData);

    object SwitchView(ViewDescription view, object initilizeData, bool reload, bool reloadData = false);

    TViewModel FindViewModel<TViewModel>(Type viewType, string uiid = null) where TViewModel : IViewModelBase;

    object FindView(Type viewType, string uiid = null);
}
