using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace lenovo.mbg.service.common.webservices
{
    /// <summary>
    /// Result of probing a single endpoint or subdomain.
    /// </summary>
    public class EndpointProbeResult
    {
        public string Url { get; set; } = string.Empty;
        public bool IsResponding { get; set; }
        public int? StatusCode { get; set; }
        public long ResponseTimeMs { get; set; }
        public string? Error { get; set; }
        public string? ServerHeader { get; set; }
        public bool HasLmsaInterface { get; set; }
    }

    /// <summary>
    /// Utility for probing Lenovo subdomains and LMSA API endpoints to determine
    /// which ones are active and responding.
    ///
    /// Usage:
    ///   var discovery = new EndpointDiscovery();
    ///   var results = await discovery.ProbeAllSubdomainsAsync();
    ///   foreach (var r in results.Where(r => r.IsResponding))
    ///       Console.WriteLine($"{r.Url} -> HTTP {r.StatusCode} ({r.ResponseTimeMs}ms)");
    /// </summary>
    public class EndpointDiscovery : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly bool _ownsHttpClient;

        public EndpointDiscovery() : this(null) { }

        public EndpointDiscovery(HttpClient? httpClient)
        {
            if (httpClient != null)
            {
                _httpClient = httpClient;
                _ownsHttpClient = false;
            }
            else
            {
                var handler = new HttpClientHandler
                {
                    AllowAutoRedirect = false,
                    ServerCertificateCustomValidationCallback = (_, _, _, _) => true
                };
                _httpClient = new HttpClient(handler);
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "LMSA-EndpointDiscovery/1.0");
                _ownsHttpClient = true;
            }
        }

        /// <summary>
        /// Probe a single URL to check if it responds.
        /// </summary>
        public async Task<EndpointProbeResult> ProbeEndpointAsync(string url, string method = "GET", int timeoutMs = 5000)
        {
            var result = new EndpointProbeResult { Url = url };
            var sw = Stopwatch.StartNew();

            try
            {
                using var cts = new CancellationTokenSource(timeoutMs);
                var request = new HttpRequestMessage(
                    method.Equals("POST", StringComparison.OrdinalIgnoreCase) ? HttpMethod.Post : HttpMethod.Get,
                    url);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cts.Token);
                sw.Stop();

                result.IsResponding = true;
                result.StatusCode = (int)response.StatusCode;
                result.ResponseTimeMs = sw.ElapsedMilliseconds;
                result.ServerHeader = response.Headers.Server?.ToString();
            }
            catch (TaskCanceledException)
            {
                sw.Stop();
                result.ResponseTimeMs = sw.ElapsedMilliseconds;
                result.Error = "Timeout";
            }
            catch (HttpRequestException ex)
            {
                sw.Stop();
                result.ResponseTimeMs = sw.ElapsedMilliseconds;
                result.Error = ex.Message;
            }
            catch (Exception ex)
            {
                sw.Stop();
                result.ResponseTimeMs = sw.ElapsedMilliseconds;
                result.Error = ex.GetType().Name + ": " + ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Probe a subdomain's base HTTPS URL.
        /// </summary>
        public Task<EndpointProbeResult> ProbeSubdomainAsync(string host, int timeoutMs = 5000)
        {
            return ProbeEndpointAsync($"https://{host}", "GET", timeoutMs);
        }

        /// <summary>
        /// Test if a host has the LMSA Interface by probing the RSA public key endpoint (POST).
        /// A response with {"code":"0000",...} indicates a live LMSA server.
        /// </summary>
        public async Task<EndpointProbeResult> ProbeLmsaInterfaceAsync(string host, int timeoutMs = 5000)
        {
            var url = $"https://{host}/Interface/common/rsa.jhtml";
            var result = new EndpointProbeResult { Url = url };
            var sw = Stopwatch.StartNew();

            try
            {
                using var cts = new CancellationTokenSource(timeoutMs);
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                var response = await _httpClient.SendAsync(request, cts.Token);
                sw.Stop();

                result.IsResponding = true;
                result.StatusCode = (int)response.StatusCode;
                result.ResponseTimeMs = sw.ElapsedMilliseconds;

                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync(cts.Token);
                    result.HasLmsaInterface = body.Contains("\"code\":\"0000\"");
                }
            }
            catch (Exception ex)
            {
                sw.Stop();
                result.ResponseTimeMs = sw.ElapsedMilliseconds;
                result.Error = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Probe all known subdomains for basic HTTPS connectivity.
        /// </summary>
        public async Task<IReadOnlyList<EndpointProbeResult>> ProbeAllSubdomainsAsync(int timeoutMs = 5000)
        {
            var tasks = LenovoEndpoints.AllSubdomains
                .Select(s => ProbeSubdomainAsync(s.Host, timeoutMs));
            return await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Probe all known subdomains to check if they have the LMSA Interface.
        /// </summary>
        public async Task<IReadOnlyList<EndpointProbeResult>> ProbeAllForLmsaInterfaceAsync(int timeoutMs = 5000)
        {
            var tasks = LenovoEndpoints.AllSubdomains
                .Select(s => ProbeLmsaInterfaceAsync(s.Host, timeoutMs));
            return await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Probe all API endpoints on a given server environment.
        /// </summary>
        public async Task<IReadOnlyList<EndpointProbeResult>> ProbeAllApiEndpointsAsync(
            ServerEnvironment env, int timeoutMs = 5000)
        {
            var endpoints = LenovoEndpoints.GetAllApiEndpoints();
            var tasks = endpoints.Select(ep =>
                ProbeEndpointAsync(ep.BuildUrl(env), ep.HttpMethod, timeoutMs));
            return await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Probe all external service URLs.
        /// </summary>
        public async Task<IReadOnlyList<EndpointProbeResult>> ProbeExternalServicesAsync(int timeoutMs = 5000)
        {
            var urls = new[]
            {
                LenovoEndpoints.ExternalServices.NETWORK_CHECK_URL,
                LenovoEndpoints.ExternalServices.QR_CODE_DOWNLOAD_MA,
                LenovoEndpoints.ExternalServices.WARRANTY_SUPPORT_URL,
                LenovoEndpoints.ExternalServices.WARRANTY_SDE_TOKEN,
                LenovoEndpoints.ExternalServices.WARRANTY_SDE_URL,
                LenovoEndpoints.ExternalServices.WARRANTY_IBASE_URL,
                LenovoEndpoints.ExternalServices.WARRANTY_MDS_OAUTH,
                LenovoEndpoints.ExternalServices.WARRANTY_MDS_ORDER,
                LenovoEndpoints.ExternalServices.LENOVOID_PRELOGIN,
                LenovoEndpoints.ExternalServices.MOLI_CALLCENTER,
                LenovoEndpoints.ExternalServices.PRIVACY_POLICY,
            };

            var tasks = urls.Select(url => ProbeEndpointAsync(url, "GET", timeoutMs));
            return await Task.WhenAll(tasks);
        }

        public void Dispose()
        {
            if (_ownsHttpClient)
                _httpClient.Dispose();
        }
    }
}
