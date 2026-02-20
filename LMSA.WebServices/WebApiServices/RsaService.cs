using System;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.webservices.WebApiModel;
using Newtonsoft.Json;

namespace lenovo.mbg.service.common.webservices.WebApiServices;

public class RsaService
{
    public static string InitPublicKey()
    {
        try
        {
            ResponseModel<string> response = WebApiHttpRequest.RequestBase(WebApiUrl.GET_PUBLIC_KEY, null, method: HttpMethod.GET, addAuthorizationHeader: false);
            if (response.success && !string.IsNullOrEmpty(response.content))
            {
                var result = JsonConvert.DeserializeObject<ResponseModel<object>>(response.content);
                if (result != null && result.code == "0000" && result.content != null)
                {
                    string publicKeyJava = result.content.ToString();
                    if (RsaHelper.RSAPublicKeyJava2DotNet(publicKeyJava, out string dotNetKey))
                    {
                        return dotNetKey;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LogHelper.LogInstance.Error("RsaService.InitPublicKey error: " + ex.Message);
        }
        return null;
    }
}
