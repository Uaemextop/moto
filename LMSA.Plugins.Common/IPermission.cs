using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace lenovo.mbg.service.framework.services;

public interface IPermission
{
    bool? AppModuleIsReady(DeviceEx device, List<string> permissionModuleNames);

    Task BeginCheckAppIsReady(DeviceEx device, List<string> permissionModuleNames, Action<bool?> finalResultRepoter);

    void BeginRollPollingAppModuleIsReady(DeviceEx device, List<string> permissionModuleNames, CancellationTokenSource cancelToken, Action<bool?> finalResultRepoter, int periodTime = 1000);

    Task BeginConfirmAppIsReady(DeviceEx device, string permissionModule, CancellationTokenSource cancelToken, Action<bool?> finalResultRepoter);
}
