using System;
using System.Net;

namespace lenovo.mbg.service.common.webservices;

public class RsaWebClient : WebClient
{
    public int TimeOut { get; set; } = 5000;

    protected override WebRequest GetWebRequest(Uri address)
    {
        WebRequest webRequest = base.GetWebRequest(address);
        webRequest.Timeout = TimeOut;
        return webRequest;
    }
}
