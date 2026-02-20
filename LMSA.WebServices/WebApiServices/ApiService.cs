using System;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.webservices.WebApiModel;

namespace lenovo.mbg.service.common.webservices.WebApiServices;

public class ApiService : ApiBaseService
{
    public ResponseModel<object> GetDeviceInfo(object parameters)
    {
        return RequestBase(WebApiUrl.GET_DEVICE_INFO, parameters);
    }

    public ResponseModel<object> GetDeviceIcon(object parameters)
    {
        return RequestBase(WebApiUrl.GET_DEVICE_ICON, parameters);
    }

    public ResponseModel<object> PostUpgradeFlashInfo(object parameters)
    {
        return RequestBase(WebApiUrl.POST_UPGRADE_FLASH_INFO, parameters, failedSave: true);
    }

    public ResponseModel<object> GetClientVersion(object parameters)
    {
        return RequestBase(WebApiUrl.CLIENT_VERSION, parameters);
    }

    public ResponseModel<object> GetPluginVersion(object parameters)
    {
        return RequestBase(WebApiUrl.PLUGIN_VERSION, parameters);
    }

    public ResponseModel<object> GetRomResources(object parameters)
    {
        return RequestBase(WebApiUrl.Webwervice_Get_RomResources, parameters);
    }

    public ResponseModel<object> GetUpgradeFlashMatchTypes(object parameters)
    {
        return RequestBase(WebApiUrl.GET_UPGRADEFLASH_MATCH_TYPES, parameters);
    }

    public ResponseModel<object> GetAutoMatchParamsMapping(object parameters)
    {
        return RequestBase(WebApiUrl.RESUCE_AUTOMATCH_GETPARAMS_MAPPING, parameters);
    }

    public ResponseModel<object> GetAutoMatchRom(object parameters)
    {
        return RequestBase(WebApiUrl.RESUCE_AUTOMATCH_GETROM, parameters);
    }

    public ResponseModel<object> CheckSupportFastbootMode(object parameters)
    {
        return RequestBase(WebApiUrl.RESUCE_CHECK_SUPPORT_FASTBOOT_MODE, parameters);
    }

    public ResponseModel<object> GetSupportFastbootByModelName(object parameters)
    {
        return RequestBase(WebApiUrl.GET_SUPPORT_FASTBOOT_BY_MODELNAME, parameters);
    }

    public ResponseModel<object> UserLogin(object parameters)
    {
        return RequestBase(WebApiUrl.USER_LOGIN, parameters);
    }

    public ResponseModel<object> UserGuestLogin(object parameters)
    {
        return RequestBase(WebApiUrl.USER_GUEST_LOGIN, parameters);
    }

    public ResponseModel<object> UserLogout(object parameters)
    {
        return RequestBase(WebApiUrl.USER_LOGOUT, parameters);
    }
}
