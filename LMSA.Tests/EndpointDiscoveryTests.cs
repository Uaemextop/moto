using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using lenovo.mbg.service.common.webservices;
using Xunit;

namespace LMSA.Tests
{
    public class EndpointDiscoveryTests
    {
        private class MockHandler : HttpMessageHandler
        {
            private readonly HttpStatusCode _statusCode;
            private readonly string _content;

            public MockHandler(HttpStatusCode statusCode, string content = "")
            {
                _statusCode = statusCode;
                _content = content;
            }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new HttpResponseMessage(_statusCode)
                {
                    Content = new StringContent(_content)
                });
            }
        }

        [Fact]
        public async Task ProbeEndpointAsync_SuccessfulResponse_ReturnsCorrectResult()
        {
            var handler = new MockHandler(HttpStatusCode.OK, "OK");
            var httpClient = new HttpClient(handler);
            using var discovery = new EndpointDiscovery(httpClient);

            var result = await discovery.ProbeEndpointAsync("https://example.com/test");

            Assert.True(result.IsResponding);
            Assert.Equal(200, result.StatusCode);
            Assert.Null(result.Error);
            Assert.True(result.ResponseTimeMs >= 0);
        }

        [Fact]
        public async Task ProbeEndpointAsync_NotFound_ReturnsStatusCode()
        {
            var handler = new MockHandler(HttpStatusCode.NotFound, "Not Found");
            var httpClient = new HttpClient(handler);
            using var discovery = new EndpointDiscovery(httpClient);

            var result = await discovery.ProbeEndpointAsync("https://example.com/missing");

            Assert.True(result.IsResponding);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public async Task ProbeLmsaInterfaceAsync_WithRsaKey_DetectsLmsa()
        {
            var rsaResponse = "{\"code\":\"0000\",\"desc\":\"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQ...\"}";
            var handler = new MockHandler(HttpStatusCode.OK, rsaResponse);
            var httpClient = new HttpClient(handler);
            using var discovery = new EndpointDiscovery(httpClient);

            var result = await discovery.ProbeLmsaInterfaceAsync("lsa.lenovo.com");

            Assert.True(result.IsResponding);
            Assert.True(result.HasLmsaInterface);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task ProbeLmsaInterfaceAsync_WithoutRsaKey_NoLmsa()
        {
            var handler = new MockHandler(HttpStatusCode.NotFound, "Not Found");
            var httpClient = new HttpClient(handler);
            using var discovery = new EndpointDiscovery(httpClient);

            var result = await discovery.ProbeLmsaInterfaceAsync("example.com");

            Assert.True(result.IsResponding);
            Assert.False(result.HasLmsaInterface);
        }

        [Fact]
        public async Task ProbeSubdomainAsync_SetsCorrectUrl()
        {
            var handler = new MockHandler(HttpStatusCode.OK);
            var httpClient = new HttpClient(handler);
            using var discovery = new EndpointDiscovery(httpClient);

            var result = await discovery.ProbeSubdomainAsync("test.lenovo.com");

            Assert.Equal("https://test.lenovo.com", result.Url);
        }

        [Fact]
        public async Task ProbeAllSubdomainsAsync_ReturnsResultsForAllSubdomains()
        {
            var handler = new MockHandler(HttpStatusCode.OK);
            var httpClient = new HttpClient(handler);
            using var discovery = new EndpointDiscovery(httpClient);

            var results = await discovery.ProbeAllSubdomainsAsync();

            Assert.Equal(LenovoEndpoints.AllSubdomains.Length, results.Count);
        }

        [Fact]
        public async Task ProbeAllApiEndpointsAsync_ReturnsResultsForAllEndpoints()
        {
            var handler = new MockHandler(HttpStatusCode.OK, "{\"code\":\"403\",\"desc\":\"Invalid token.\"}");
            var httpClient = new HttpClient(handler);
            using var discovery = new EndpointDiscovery(httpClient);

            var results = await discovery.ProbeAllApiEndpointsAsync(LenovoEndpoints.Production);

            var endpointCount = LenovoEndpoints.GetAllApiEndpoints().Count;
            Assert.Equal(endpointCount, results.Count);
        }

        [Fact]
        public async Task ProbeExternalServicesAsync_ReturnsResults()
        {
            var handler = new MockHandler(HttpStatusCode.OK);
            var httpClient = new HttpClient(handler);
            using var discovery = new EndpointDiscovery(httpClient);

            var results = await discovery.ProbeExternalServicesAsync();

            Assert.True(results.Count >= 10, $"Expected >= 10 external service results, got {results.Count}");
        }

        [Fact]
        public void EndpointDiscovery_DefaultConstructor_DoesNotThrow()
        {
            using var discovery = new EndpointDiscovery();
            Assert.NotNull(discovery);
        }

        [Fact]
        public void EndpointDiscovery_Dispose_DoesNotThrow()
        {
            var discovery = new EndpointDiscovery();
            discovery.Dispose();
        }

        [Fact]
        public void EndpointProbeResult_DefaultValues()
        {
            var result = new EndpointProbeResult();
            Assert.Equal(string.Empty, result.Url);
            Assert.False(result.IsResponding);
            Assert.Null(result.StatusCode);
            Assert.Equal(0, result.ResponseTimeMs);
            Assert.Null(result.Error);
            Assert.False(result.HasLmsaInterface);
        }
    }
}
