using System;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.socket;

namespace lenovo.mbg.service.framework.devicemgt;

// Stub implementation - TcpAndroidDevice will be fully implemented in a subsequent step
public abstract class TcpAndroidDevice : DeviceEx
{
    public int RetryConnect = 1;
    public IMessageManager MessageManager { get; set; }
    public IFileTransferManager FileTransferManager { get; set; }
    public override IAndroidDevice Property { get; set; }
    public bool? IsReady { get; protected set; }
    
    public event EventHandler<TcpConnectStepChangedEventArgs> TcpConnectStepChanged;
    public event EventHandler<PermissionsCheckConfirmEventArgs> PermissionsCheckConfirmEvent;

    protected virtual ConnectErrorCode CheckMotoAppVersion() { return ConnectErrorCode.OK; }
    public virtual ConnectErrorCode StartMotoApp() { return ConnectErrorCode.OK; }
    protected virtual ConnectErrorCode CheckMaAppVersion() { return ConnectErrorCode.OK; }
    public virtual ConnectErrorCode StartMaApp() { return ConnectErrorCode.OK; }
    protected virtual ConnectErrorCode InstallApp() { return ConnectErrorCode.OK; }
    protected virtual ConnectErrorCode UninstallApp() { return ConnectErrorCode.OK; }
    protected virtual void UninstallDebugOrOldPackageNameApp() { }
    public virtual void CallAppToFrontstage() { }
    protected virtual WifiDeviceData GetServiceHostEndPointInfo() { return null; }
    public virtual ConnectErrorCode LoadProperty() { return ConnectErrorCode.OK; }
    public virtual bool FocuseApp() { return true; }
    protected virtual bool CheckPermissionsPerpare() { return true; }
    
    public bool CheckPermissions(System.Collections.Generic.List<string> permissionModules)
    {
        return false;
    }

    public override void Load() { }
}
