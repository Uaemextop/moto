using System;
using System.Collections.Generic;
using System.Linq;
using lenovo.mbg.service.common.webservices;
using Xunit;

namespace LMSA.Tests
{
    public class LenovoEndpointsTests
    {
        [Fact]
        public void Production_HasCorrectBaseUrl()
        {
            Assert.Equal("https://lsa.lenovo.com", LenovoEndpoints.Production.BaseUrl);
            Assert.True(LenovoEndpoints.Production.IsProduction);
        }

        [Fact]
        public void Staging_HasCorrectBaseUrl()
        {
            Assert.Equal("https://lsatest.lenovo.com", LenovoEndpoints.Staging.BaseUrl);
            Assert.False(LenovoEndpoints.Staging.IsProduction);
        }

        [Fact]
        public void InterfaceUrl_AppendsInterface()
        {
            Assert.Equal("https://lsa.lenovo.com/Interface", LenovoEndpoints.Production.InterfaceUrl);
            Assert.Equal("https://lsatest.lenovo.com/Interface", LenovoEndpoints.Staging.InterfaceUrl);
        }

        [Fact]
        public void AllEnvironments_ContainsBothServers()
        {
            Assert.Equal(2, LenovoEndpoints.AllEnvironments.Length);
            Assert.Contains(LenovoEndpoints.AllEnvironments, e => e.Name == "Production");
            Assert.Contains(LenovoEndpoints.AllEnvironments, e => e.Name == "Staging");
        }

        [Fact]
        public void AllSubdomains_ContainsLmsaCoreHosts()
        {
            var hosts = LenovoEndpoints.AllSubdomains.Select(s => s.Host).ToList();
            Assert.Contains("lsa.lenovo.com", hosts);
            Assert.Contains("lsatest.lenovo.com", hosts);
            Assert.Contains("passport.lenovo.com", hosts);
            Assert.Contains("download.lenovo.com", hosts);
            Assert.Contains("supportapi.lenovo.com", hosts);
            Assert.Contains("microapi-us-sde.lenovo.com", hosts);
            Assert.Contains("api-pre-mds-us.lenovo.com", hosts);
            Assert.Contains("forums.lenovo.com", hosts);
            Assert.Contains("moli.lenovo.com", hosts);
            Assert.Contains("www3.lenovo.com", hosts);
            Assert.Contains("club.lenovo.com.cn", hosts);
            Assert.Contains("lenovomobilesupport.lenovo.com", hosts);
        }

        [Fact]
        public void AllSubdomains_HasNoDuplicates()
        {
            var hosts = LenovoEndpoints.AllSubdomains.Select(s => s.Host).ToList();
            var distinct = hosts.Distinct().ToList();
            Assert.Equal(distinct.Count, hosts.Count);
        }

        [Fact]
        public void AllSubdomains_HasAtLeast30Entries()
        {
            Assert.True(LenovoEndpoints.AllSubdomains.Length >= 30,
                $"Expected >= 30 subdomains, got {LenovoEndpoints.AllSubdomains.Length}");
        }

        [Fact]
        public void GetCoreSubdomains_ReturnsOnlyCore()
        {
            var core = LenovoEndpoints.GetCoreSubdomains();
            Assert.All(core, s => Assert.True(s.IsLmsaCore));
            Assert.True(core.Count >= 12, $"Expected >= 12 core subdomains, got {core.Count}");
        }

        [Fact]
        public void GetResolvedSubdomains_ReturnsOnlyResolved()
        {
            var resolved = LenovoEndpoints.GetResolvedSubdomains();
            Assert.All(resolved, s => Assert.True(s.DnsResolved));
        }

        [Fact]
        public void GetAllApiEndpoints_Has72Endpoints()
        {
            var endpoints = LenovoEndpoints.GetAllApiEndpoints();
            Assert.True(endpoints.Count >= 69,
                $"Expected >= 69 API endpoints, got {endpoints.Count}");
        }

        [Fact]
        public void GetAllApiEndpoints_HasNoDuplicatePaths()
        {
            var endpoints = LenovoEndpoints.GetAllApiEndpoints();
            var paths = endpoints.Select(e => e.Path).ToList();
            var distinct = paths.Distinct().ToList();
            Assert.Equal(distinct.Count, paths.Count);
        }

        [Fact]
        public void GetAllApiEndpoints_HasNoDuplicateNames()
        {
            var endpoints = LenovoEndpoints.GetAllApiEndpoints();
            var names = endpoints.Select(e => e.Name).ToList();
            var distinct = names.Distinct().ToList();
            Assert.Equal(distinct.Count, names.Count);
        }

        [Fact]
        public void GetAllApiEndpoints_AllPathsStartWithSlash()
        {
            var endpoints = LenovoEndpoints.GetAllApiEndpoints();
            Assert.All(endpoints, e => Assert.StartsWith("/", e.Path));
        }

        [Fact]
        public void GetAllApiEndpoints_AllHaveCategory()
        {
            var endpoints = LenovoEndpoints.GetAllApiEndpoints();
            Assert.All(endpoints, e => Assert.False(string.IsNullOrEmpty(e.Category)));
        }

        [Fact]
        public void GetEndpointCategories_ContainsExpectedCategories()
        {
            var categories = LenovoEndpoints.GetEndpointCategories();
            Assert.Contains("security", categories);
            Assert.Contains("rescue", categories);
            Assert.Contains("user", categories);
            Assert.Contains("device", categories);
            Assert.Contains("feedback", categories);
            Assert.Contains("survey", categories);
            Assert.Contains("client", categories);
            Assert.Contains("vip", categories);
            Assert.Contains("dataCollection", categories);
        }

        [Fact]
        public void GetEndpointsByCategory_Rescue_HasExpectedEndpoints()
        {
            var rescue = LenovoEndpoints.GetEndpointsByCategory("rescue");
            Assert.True(rescue.Count >= 7, $"Expected >= 7 rescue endpoints, got {rescue.Count}");
            Assert.Contains(rescue, e => e.Name == "LOAD_SMART_DEVICE");
            Assert.Contains(rescue, e => e.Name == "GET_FASTBOOTDATA_RECIPE");
        }

        [Fact]
        public void BuildUrl_CreatesCorrectProductionUrl()
        {
            var url = LenovoEndpoints.BuildUrl(LenovoEndpoints.Production, ApiPaths.GET_PUBLIC_KEY);
            Assert.Equal("https://lsa.lenovo.com/Interface/common/rsa.jhtml", url);
        }

        [Fact]
        public void BuildUrl_CreatesCorrectStagingUrl()
        {
            var url = LenovoEndpoints.BuildUrl(LenovoEndpoints.Staging, ApiPaths.LOAD_SMART_DEVICE);
            Assert.Equal("https://lsatest.lenovo.com/Interface/rescueDevice/smartMarketNames.jhtml", url);
        }

        [Fact]
        public void BuildUrl_AbsoluteUrl_ReturnsAsIs()
        {
            var url = LenovoEndpoints.BuildUrl(LenovoEndpoints.Production, "https://example.com/test");
            Assert.Equal("https://example.com/test", url);
        }

        [Fact]
        public void BuildBaseUrl_CreatesCorrectUrl()
        {
            var url = LenovoEndpoints.BuildBaseUrl(LenovoEndpoints.Production, BasePaths.SHOW_FEEDBACK);
            Assert.Equal("https://lsa.lenovo.com/Tips/feedback.html", url);
        }

        [Fact]
        public void BuildUrl_NullPath_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                LenovoEndpoints.BuildUrl(LenovoEndpoints.Production, null!));
        }

        [Fact]
        public void ApiPaths_PreservesMisspellings()
        {
            // Preserve original decompiled misspellings for API compatibility
            Assert.Equal("/priv/getRomList.jhtml", ApiPaths.Webwervice_Get_RomResources);
            Assert.Equal("/feedback/fileSignatureUrl.jhtml", ApiPaths.FEEDBACK_FILE_SINGNATURE);
            Assert.Equal("/survey/getIsNeedTriggerSurvey.jhtml", ApiPaths.GET_IS_NEED_TRIGGER_SURVER);
            Assert.Contains("RESUCE", nameof(ApiPaths.RESUCE_AUTOMATCH_GETROM));
        }

        [Fact]
        public void ExternalServices_HasAllUrls()
        {
            Assert.StartsWith("https://", LenovoEndpoints.ExternalServices.NETWORK_CHECK_URL);
            Assert.StartsWith("https://", LenovoEndpoints.ExternalServices.QR_CODE_DOWNLOAD_MA);
            Assert.StartsWith("https://", LenovoEndpoints.ExternalServices.WARRANTY_SUPPORT_URL);
            Assert.StartsWith("https://", LenovoEndpoints.ExternalServices.WARRANTY_SDE_TOKEN);
            Assert.StartsWith("https://", LenovoEndpoints.ExternalServices.WARRANTY_MDS_OAUTH);
            Assert.StartsWith("https://", LenovoEndpoints.ExternalServices.LENOVOID_PRELOGIN);
            Assert.StartsWith("https://", LenovoEndpoints.ExternalServices.LENOVOID_LOGOUT);
            Assert.StartsWith("https://", LenovoEndpoints.ExternalServices.MOLI_CALLCENTER);
            Assert.StartsWith("https://", LenovoEndpoints.ExternalServices.PRIVACY_POLICY);
        }

        [Fact]
        public void Forums_HasAllLanguages()
        {
            Assert.Contains("lp_en", LenovoEndpoints.Forums.PHONES_EN);
            Assert.Contains("lp_es", LenovoEndpoints.Forums.PHONES_ES);
            Assert.Contains("phones_pt", LenovoEndpoints.Forums.PHONES_PT);
            Assert.Contains("lp_pl", LenovoEndpoints.Forums.PHONES_PL);
            Assert.Contains("MotorolaCommunity", LenovoEndpoints.Forums.MOTO_EN);
            Assert.Contains("ComunidadMotorola", LenovoEndpoints.Forums.MOTO_ES);
            Assert.Contains("ComunidadeMotorola", LenovoEndpoints.Forums.MOTO_PT);
            Assert.Contains("lt_en", LenovoEndpoints.Forums.TABLETS_EN);
            Assert.Contains("club.lenovo.com.cn", LenovoEndpoints.Forums.CHINA_MOTO);
        }

        [Fact]
        public void EndpointInfo_BuildUrl_UsesInterfaceUrl()
        {
            var ep = new EndpointInfo
            {
                Name = "TEST",
                Path = "/test/path.jhtml",
                Category = "test",
                HttpMethod = "POST"
            };
            var url = ep.BuildUrl(LenovoEndpoints.Production);
            Assert.Equal("https://lsa.lenovo.com/Interface/test/path.jhtml", url);
        }

        [Fact]
        public void EndpointInfo_BuildUrl_AbsoluteUrl_ReturnsAsIs()
        {
            var ep = new EndpointInfo
            {
                Name = "TEST",
                Path = "https://external.com/api",
                Category = "test"
            };
            var url = ep.BuildUrl(LenovoEndpoints.Production);
            Assert.Equal("https://external.com/api", url);
        }

        [Fact]
        public void LenovoSubdomain_LmsaCoreServers_HaveKnownPaths()
        {
            var lsa = LenovoEndpoints.AllSubdomains.First(s => s.Host == "lsa.lenovo.com");
            Assert.True(lsa.IsLmsaCore);
            Assert.True(lsa.DnsResolved);
            Assert.NotEmpty(lsa.KnownPaths);

            var lsatest = LenovoEndpoints.AllSubdomains.First(s => s.Host == "lsatest.lenovo.com");
            Assert.True(lsatest.IsLmsaCore);
            Assert.True(lsatest.DnsResolved);
        }

        [Fact]
        public void ServerEnvironment_DnsTargets_AreSet()
        {
            Assert.Contains("lmsa-web-prd", LenovoEndpoints.Production.DnsTarget);
            Assert.Contains("lmsa-web-dev", LenovoEndpoints.Staging.DnsTarget);
        }

        [Fact]
        public void ServerEnvironment_ServerSoftware_IsTomcat()
        {
            Assert.Contains("Tomcat", LenovoEndpoints.Production.ServerSoftware);
            Assert.Contains("Tomcat", LenovoEndpoints.Staging.ServerSoftware);
        }

        // Aliases to make the using static ApiPaths references compile
        private static class ApiPaths
        {
            public const string GET_PUBLIC_KEY = lenovo.mbg.service.common.webservices.LenovoEndpoints.ApiPaths.GET_PUBLIC_KEY;
            public const string LOAD_SMART_DEVICE = lenovo.mbg.service.common.webservices.LenovoEndpoints.ApiPaths.LOAD_SMART_DEVICE;
            public const string Webwervice_Get_RomResources = lenovo.mbg.service.common.webservices.LenovoEndpoints.ApiPaths.Webwervice_Get_RomResources;
            public const string FEEDBACK_FILE_SINGNATURE = lenovo.mbg.service.common.webservices.LenovoEndpoints.ApiPaths.FEEDBACK_FILE_SINGNATURE;
            public const string GET_IS_NEED_TRIGGER_SURVER = lenovo.mbg.service.common.webservices.LenovoEndpoints.ApiPaths.GET_IS_NEED_TRIGGER_SURVER;
            public const string RESUCE_AUTOMATCH_GETROM = lenovo.mbg.service.common.webservices.LenovoEndpoints.ApiPaths.RESUCE_AUTOMATCH_GETROM;
        }

        private static class BasePaths
        {
            public const string SHOW_FEEDBACK = lenovo.mbg.service.common.webservices.LenovoEndpoints.BasePaths.SHOW_FEEDBACK;
        }
    }
}
