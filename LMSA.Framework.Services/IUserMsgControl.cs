using System;

namespace lenovo.mbg.service.framework.services;

public interface IUserMsgControl
{
    bool? Result { get; set; }

    Action<bool?> CloseAction { get; set; }

    Action<bool?> CallBackAction { get; set; }

    object GetMsgUi();
}
