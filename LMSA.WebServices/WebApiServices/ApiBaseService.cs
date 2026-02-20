using System;
using System.Collections.Generic;
using System.Threading;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.webservices.WebApiModel;

namespace lenovo.mbg.service.common.webservices.WebApiServices;

public class ApiBaseService
{
    public virtual ResponseModel<object> RequestBase(string url, object aparams, int tryCount = 3, Dictionary<string, string> headers = null, HttpMethod method = HttpMethod.POST, string contentType = "application/json", bool author = true, bool failedSave = false)
    {
        ResponseModel<object> responseModel;
        do
        {
            responseModel = func(url, aparams);
            if (responseModel.success)
            {
                break;
            }
            Thread.Sleep(new Random().Next(1000));
        }
        while (--tryCount > 0);
        return responseModel;

        ResponseModel<object> func(string uri, object data)
        {
            return WebApiHttpRequest.Request(uri, (method == HttpMethod.POST) ? new RequestModel(data).ToString() : null, headers, method, contentType, author);
        }
    }

    public object RequestContent(string url, object aparams, int tryCount = 3, Dictionary<string, string> headers = null, HttpMethod method = HttpMethod.POST, string contentType = "application/json", bool author = true)
    {
        ResponseModel<object> result = RequestBase(url, aparams, tryCount, headers, method, contentType, author);
        if (result != null && result.success && result.code == "0000")
        {
            return result.content;
        }
        return null;
    }
}
