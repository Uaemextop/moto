using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices.WebApiModel;

namespace lenovo.mbg.service.common.webservices;

public class WebApiHttpRequest
{
    private static long interlocker;

    public static Func<string, object, object> WebApiCallback { get; set; }

    public static ResponseModel<string> RequestBase(string url, string body, Dictionary<string, string> headers = null, HttpMethod method = HttpMethod.POST, string contentType = "application/json", bool addAuthorizationHeader = false)
    {
        HttpWebRequest httpWebRequest = null;
        try
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            ServicePointManager.DefaultConnectionLimit = 200;
            LogHelper.LogInstance.Debug("Request " + url + ", params: " + body);
            httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
            httpWebRequest.UseDefaultCredentials = true;
            httpWebRequest.ServicePoint.ConnectionLimit = 200;
            httpWebRequest.ProtocolVersion = HttpVersion.Version11;
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.79 Safari/537.36";
            httpWebRequest.Method = method.ToString();
            httpWebRequest.ContentType = contentType;
            httpWebRequest.KeepAlive = false;
            httpWebRequest.Timeout = 120000;
            httpWebRequest.ReadWriteTimeout = 600000;
            httpWebRequest.ContentLength = 0L;
            httpWebRequest.Headers.Add("Cache-Control", "no-cache");
            httpWebRequest.Headers.Add("Request-Tag", "lmsa");
            if (addAuthorizationHeader)
            {
                foreach (KeyValuePair<string, string> header in WebApiContext.REQUEST_AUTHOR_HEADERS)
                {
                    httpWebRequest.Headers.Add(header.Key, header.Value);
                }
            }
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    httpWebRequest.Headers.Add(header.Key, header.Value);
                }
            }
            if (!string.IsNullOrEmpty(body))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(body);
                httpWebRequest.ContentLength = bytes.Length;
                using Stream stream = httpWebRequest.GetRequestStream();
                stream.Write(bytes, 0, bytes.Length);
            }
            string text = null;
            using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    string responseHeader = httpWebResponse.GetResponseHeader("Guid");
                    string responseHeader2 = httpWebResponse.GetResponseHeader("Authorization");
                    if (responseHeader == WebApiContext.GUID && !string.IsNullOrEmpty(responseHeader2))
                    {
                        WebApiContext.JWT_TOKEN = responseHeader2;
                    }
                    using Stream stream2 = httpWebResponse.GetResponseStream();
                    using MemoryStream memoryStream = new MemoryStream();
                    byte[] array = new byte[10240];
                    for (int num = stream2.Read(array, 0, array.Length); num > 0; num = stream2.Read(array, 0, array.Length))
                    {
                        memoryStream.Write(array, 0, num);
                    }
                    memoryStream.Seek(0L, SeekOrigin.Begin);
                    using StreamReader streamReader = new StreamReader(memoryStream);
                    text = streamReader.ReadToEnd();
                }
            }
            if (url == WebApiUrl.LENOVOID_LOGIN_CALLBACK)
            {
                LogHelper.LogInstance.AnalyzeUnsafeText(text);
            }
            LogHelper.LogInstance.Debug("Response " + url + ", result: " + text);
            return new ResponseModel<string>
            {
                code = "0000",
                success = true,
                content = text
            };
        }
        catch (WebException ex)
        {
            if (url.Contains(Configurations.BaseHttpUrl))
            {
                LogHelper.LogInstance.Error($"Get Context From Url: [{url}] WebException:{ex}");
                Task.Delay(new Random().Next(100)).ContinueWith(delegate
                {
                    if (Interlocked.Read(ref interlocker) == 0L)
                    {
                        Interlocked.Exchange(ref interlocker, 1L);
                        WebApiCallback?.Invoke("NONETWORK", ex.Status == WebExceptionStatus.NameResolutionFailure);
                        Interlocked.Exchange(ref interlocker, 0L);
                    }
                });
            }
            else
            {
                LogHelper.LogInstance.Error("Get Context From Third Url: [" + url + "] WebException:" + ex.Message);
            }
            return new ResponseModel<string>
            {
                code = "ERROR",
                desc = ex.Message
            };
        }
        catch (Exception ex)
        {
            LogHelper.LogInstance.Error("Get Context From Url: [" + url + "] Exception:" + ex.Message);
            return new ResponseModel<string>
            {
                code = "ERROR",
                desc = ex.Message
            };
        }
        finally
        {
            try
            {
                httpWebRequest?.Abort();
            }
            catch (Exception)
            {
            }
        }
    }

    public static ResponseModel<object> Request(string url, string body, Dictionary<string, string> headers = null, HttpMethod method = HttpMethod.POST, string contentType = "application/json", bool addAuthorizationHeader = false)
    {
        ResponseModel<string> responseModel = RequestBase(url, body, headers, method, contentType, addAuthorizationHeader);
        if (responseModel.success)
        {
            ResponseModel<object> responseModel2 = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseModel<object>>(responseModel.content);
            if (responseModel2 != null && (responseModel2.code == "402" || responseModel2.code == "406"))
            {
                Task.Delay(new Random().Next(100)).ContinueWith(delegate
                {
                    if (Interlocked.Read(ref interlocker) == 0L)
                    {
                        Interlocked.Exchange(ref interlocker, 1L);
                        WebApiCallback?.Invoke("TOKEN_EXPRIED", null);
                        Interlocked.Exchange(ref interlocker, 0L);
                    }
                });
            }
            if (responseModel2 != null)
            {
                responseModel2.success = responseModel.success;
                return responseModel2;
            }
        }
        return new ResponseModel<object>
        {
            code = responseModel.code,
            content = responseModel.content,
            desc = responseModel.desc,
            success = responseModel.success
        };
    }
}
