using System;
using System.Collections.Generic;
using System.Linq;

namespace lenovo.mbg.service.common.webservices
{
    /// <summary>
    /// Represents a known LMSA server environment (production, staging, etc.).
    /// Discovered by analyzing the decompiled app.config BaseHttpUrl setting
    /// and DNS/HTTP probing of Lenovo subdomains.
    /// </summary>
    public class ServerEnvironment
    {
        public string Name { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string InterfaceUrl => BaseUrl + "/Interface";
        public string Description { get; set; } = string.Empty;
        public bool IsProduction { get; set; }
        public string DnsTarget { get; set; } = string.Empty;
        public string ServerSoftware { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents a discovered Lenovo subdomain with its purpose and known paths.
    /// </summary>
    public class LenovoSubdomain
    {
        public string Host { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public string[] KnownPaths { get; set; } = Array.Empty<string>();
        public bool IsLmsaCore { get; set; }
        public bool DnsResolved { get; set; }
        public string DnsTarget { get; set; } = string.Empty;
    }

    /// <summary>
    /// Describes an individual API endpoint with its metadata.
    /// </summary>
    public class EndpointInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string HttpMethod { get; set; } = "POST";

        public string BuildUrl(ServerEnvironment env)
        {
            if (Path.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return Path;
            return env.InterfaceUrl + Path;
        }
    }

    /// <summary>
    /// Comprehensive catalog of ALL Lenovo subdomains and API endpoints used by LMSA
    /// (Lenovo Mobile Software Assistant / Software Fix / Rescue and Smart Assistant).
    ///
    /// Sources:
    ///   - Decompiled WebApiUrl.cs (72 API endpoint paths)
    ///   - Decompiled Configurations.cs (BaseHttpUrl, IsReleaseVersion)
    ///   - Decompiled WarrantyService.cs (external service URLs)
    ///   - Decompiled ForumFrame.cs, SearchViewExModel.cs, LenovoIdWindow.cs, etc.
    ///   - DNS resolution and HTTP probing of discovered subdomains
    ///   - Certificate Transparency logs (crt.sh)
    ///   - Trickest/inventory GitHub dataset (3661 Lenovo hostnames)
    /// </summary>
    public static class LenovoEndpoints
    {
        // ====================================================================
        // Server Environments
        // ====================================================================

        /// <summary>
        /// Production LMSA server. DNS: lmsa-web-prd-30521416.us-east-1.elb.amazonaws.com
        /// Running Apache Tomcat/11.0.12. This is the default BaseHttpUrl in app.config.
        /// Configurations.IsReleaseVersion returns true when ServiceBaseUrl == "https://lsa.lenovo.com".
        /// </summary>
        public static readonly ServerEnvironment Production = new()
        {
            Name = "Production",
            BaseUrl = "https://lsa.lenovo.com",
            Description = "Primary LMSA production server (app.config default BaseHttpUrl)",
            IsProduction = true,
            DnsTarget = "lmsa-web-prd-30521416.us-east-1.elb.amazonaws.com",
            ServerSoftware = "Apache Tomcat/11.0.12"
        };

        /// <summary>
        /// Staging/test LMSA server. DNS: lmsa-web-dev-ext-1322476325.us-east-1.elb.amazonaws.com
        /// Running Apache Tomcat/11.0.12. Same API endpoints and RSA public key as production.
        /// Can be used by setting BaseHttpUrl to "https://lsatest.lenovo.com" in app.config.
        /// </summary>
        public static readonly ServerEnvironment Staging = new()
        {
            Name = "Staging",
            BaseUrl = "https://lsatest.lenovo.com",
            Description = "Staging/test LMSA server (same API surface as production)",
            IsProduction = false,
            DnsTarget = "lmsa-web-dev-ext-1322476325.us-east-1.elb.amazonaws.com",
            ServerSoftware = "Apache Tomcat/11.0.12"
        };

        public static readonly ServerEnvironment[] AllEnvironments = { Production, Staging };

        // ====================================================================
        // All Discovered Lenovo Subdomains (from decompiled source + research)
        // ====================================================================

        public static readonly LenovoSubdomain[] AllSubdomains =
        {
            // --- Core LMSA servers (have /Interface API) ---
            new LenovoSubdomain
            {
                Host = "lsa.lenovo.com",
                Purpose = "Primary LMSA production server - all API endpoints",
                KnownPaths = new[] { "/Interface/common/rsa.jhtml", "/Interface/client/initToken.jhtml", "/lmsa-web/index.jsp", "/Tips/feedback.html" },
                IsLmsaCore = true,
                DnsResolved = true,
                DnsTarget = "lmsa-web-prd-30521416.us-east-1.elb.amazonaws.com"
            },
            new LenovoSubdomain
            {
                Host = "lsatest.lenovo.com",
                Purpose = "LMSA staging/test server - identical API surface to production",
                KnownPaths = new[] { "/Interface/common/rsa.jhtml", "/Interface/client/initToken.jhtml", "/lmsa-web/index.jsp", "/Tips/feedback.html" },
                IsLmsaCore = true,
                DnsResolved = true,
                DnsTarget = "lmsa-web-dev-ext-1322476325.us-east-1.elb.amazonaws.com"
            },

            // --- Authentication ---
            new LenovoSubdomain
            {
                Host = "passport.lenovo.com",
                Purpose = "Lenovo ID SSO authentication (login, logout, OAuth)",
                KnownPaths = new[] { "/glbwebauthnv6/preLogin", "/wauthen5/gateway", "/interserver/authen/1.2/getaccountid" },
                IsLmsaCore = true,
                DnsResolved = true,
                DnsTarget = "passport-lenovo.lenovomm.com"
            },
            new LenovoSubdomain
            {
                Host = "ipgpassport.lenovo.com",
                Purpose = "IPG passport service",
                KnownPaths = Array.Empty<string>(),
                IsLmsaCore = false,
                DnsResolved = true,
                DnsTarget = "58.247.171.36"
            },
            new LenovoSubdomain
            {
                Host = "ipgpassportuat.lenovo.com",
                Purpose = "IPG passport UAT environment",
                KnownPaths = Array.Empty<string>(),
                IsLmsaCore = false,
                DnsResolved = true,
                DnsTarget = "58.247.171.38"
            },
            new LenovoSubdomain
            {
                Host = "passport.motorola.com.cn",
                Purpose = "Motorola China passport service",
                KnownPaths = Array.Empty<string>(),
                IsLmsaCore = false,
                DnsResolved = true,
                DnsTarget = "passport-motorola.lenovomm.com"
            },

            // --- Downloads ---
            new LenovoSubdomain
            {
                Host = "download.lenovo.com",
                Purpose = "Primary download server (APKs, PDFs, firmware, drivers)",
                KnownPaths = new[] { "/lsa/ma.apk", "/lenovo/lla/l505-0009-06-{0}.pdf" },
                IsLmsaCore = true,
                DnsResolved = true,
                DnsTarget = "download.lenovo.com.akadns.net"
            },
            new LenovoSubdomain
            {
                Host = "cn.download.lenovo.com",
                Purpose = "China download mirror",
                KnownPaths = Array.Empty<string>(),
                IsLmsaCore = false,
                DnsResolved = true,
                DnsTarget = "cn.download.lenovo.com.edgesuite.net"
            },
            new LenovoSubdomain
            {
                Host = "us.download.lenovo.com",
                Purpose = "US download mirror",
                KnownPaths = Array.Empty<string>(),
                IsLmsaCore = false,
                DnsResolved = true,
                DnsTarget = "us.download.lenovo.com.edgekey.net"
            },
            new LenovoSubdomain
            {
                Host = "filedownload.lenovo.com",
                Purpose = "Alternative file download server",
                KnownPaths = Array.Empty<string>(),
                IsLmsaCore = false,
                DnsResolved = true,
                DnsTarget = "filedownload.lenovo.com.akadns.net"
            },

            // --- Warranty & Support APIs ---
            new LenovoSubdomain
            {
                Host = "supportapi.lenovo.com",
                Purpose = "Warranty API v3 (WarrantyService.SupportUrl)",
                KnownPaths = new[] { "/v3/warranty/" },
                IsLmsaCore = true,
                DnsResolved = true,
                DnsTarget = "supportapi.lenovo.com.edgekey.net"
            },
            new LenovoSubdomain
            {
                Host = "microapi-us-sde.lenovo.com",
                Purpose = "SDE microservice (token exchange, POI requests)",
                KnownPaths = new[] { "/token", "/v1.0/service/poi_request" },
                IsLmsaCore = true,
                DnsResolved = true,
                DnsTarget = "microapi-us.gtm.lenovo.com"
            },
            new LenovoSubdomain
            {
                Host = "ibase.lenovo.com",
                Purpose = "iBase warranty/product/parts lookup",
                KnownPaths = new[] { "/POIRequest.aspx" },
                IsLmsaCore = true,
                DnsResolved = false,
                DnsTarget = ""
            },
            new LenovoSubdomain
            {
                Host = "ibase.gtm.lenovo.com",
                Purpose = "iBase GTM endpoint (alternative)",
                KnownPaths = new[] { "/POIRequest.aspx" },
                IsLmsaCore = true,
                DnsResolved = true,
                DnsTarget = "43.255.226.151"
            },
            new LenovoSubdomain
            {
                Host = "api-pre-mds-us.lenovo.com",
                Purpose = "MDS OAuth token exchange and warranty order queries",
                KnownPaths = new[] { "/auth/oauth/token", "/order/order/rnt/getUnit" },
                IsLmsaCore = true,
                DnsResolved = true,
                DnsTarget = "103.30.232.248"
            },

            // --- Support & Mobile Support ---
            new LenovoSubdomain
            {
                Host = "support.lenovo.com",
                Purpose = "Lenovo support portal",
                KnownPaths = Array.Empty<string>(),
                IsLmsaCore = false,
                DnsResolved = true,
                DnsTarget = "support.lenovo.com.edgekey.net"
            },
            new LenovoSubdomain
            {
                Host = "smartsupport.lenovo.com",
                Purpose = "Smart support portal",
                KnownPaths = Array.Empty<string>(),
                IsLmsaCore = false,
                DnsResolved = true,
                DnsTarget = "smartsupport.lenovo.com.edgekey.net"
            },
            new LenovoSubdomain
            {
                Host = "lenovomobilesupport.lenovo.com",
                Purpose = "Mobile support portal (device-specific support pages)",
                KnownPaths = new[] { "/{region}/{lang}/solutions/find-product-name" },
                IsLmsaCore = true,
                DnsResolved = true,
                DnsTarget = "lenovomobilesupport.lenovo.com.edgekey.net"
            },

            // --- Forums ---
            new LenovoSubdomain
            {
                Host = "forums.lenovo.com",
                Purpose = "Community forums (multi-language: Lenovo phones, tablets, Motorola)",
                KnownPaths = new[]
                {
                    "/t5/Lenovo-Phones/ct-p/lp_en",
                    "/t5/Smartphones-Lenovo/ct-p/lp_es",
                    "/t5/Telefones-Lenovo/ct-p/phones_pt",
                    "/t5/Smartfony-Lenovo/ct-p/lp_pl",
                    "/t5/Motorola-Community/ct-p/MotorolaCommunity",
                    "/t5/Lenovo-Tablets/ct-p/lt_en"
                },
                IsLmsaCore = true,
                DnsResolved = true,
                DnsTarget = "d24zn61bgpeyxm.cloudfront.net"
            },
            new LenovoSubdomain
            {
                Host = "forumscdn.lenovo.com",
                Purpose = "Forums CDN",
                KnownPaths = Array.Empty<string>(),
                IsLmsaCore = false,
                DnsResolved = true,
                DnsTarget = "d1kwop82rlvmed.cloudfront.net"
            },

            // --- Moli (Call Center / Chat) ---
            new LenovoSubdomain
            {
                Host = "moli.lenovo.com",
                Purpose = "Call center / Moli chat support (US region)",
                KnownPaths = new[] { "/callcenter/moli" },
                IsLmsaCore = true,
                DnsResolved = true,
                DnsTarget = "moli-us-llm-1734483785.us-east-1.elb.amazonaws.com"
            },
            new LenovoSubdomain
            {
                Host = "anz.moli.lenovo.com",
                Purpose = "Moli Australia/New Zealand region",
                KnownPaths = new[] { "/callcenter/moli" },
                IsLmsaCore = false,
                DnsResolved = true,
                DnsTarget = "moli-anz-llm-584616787.ap-south-1.elb.amazonaws.com"
            },
            new LenovoSubdomain
            {
                Host = "br.moli.lenovo.com",
                Purpose = "Moli Brazil region",
                KnownPaths = new[] { "/callcenter/moli" },
                IsLmsaCore = false,
                DnsResolved = true,
                DnsTarget = "moli-br-llm-1842339664.us-east-1.elb.amazonaws.com"
            },
            new LenovoSubdomain
            {
                Host = "de.moli.lenovo.com",
                Purpose = "Moli Germany region",
                KnownPaths = new[] { "/callcenter/moli" },
                IsLmsaCore = false,
                DnsResolved = true,
                DnsTarget = "moli-de-llm-1647080319.eu-west-3.elb.amazonaws.com"
            },
            new LenovoSubdomain
            {
                Host = "india.moli.lenovo.com",
                Purpose = "Moli India region",
                KnownPaths = new[] { "/callcenter/moli" },
                IsLmsaCore = false,
                DnsResolved = true,
                DnsTarget = "moli-in-llm-551440711.ap-south-1.elb.amazonaws.com"
            },
            new LenovoSubdomain
            {
                Host = "it.moli.lenovo.com",
                Purpose = "Moli Italy region",
                KnownPaths = new[] { "/callcenter/moli" },
                IsLmsaCore = false,
                DnsResolved = true,
                DnsTarget = "moli-it-llm-579101806.eu-west-3.elb.amazonaws.com"
            },
            new LenovoSubdomain
            {
                Host = "jp.moli.lenovo.com",
                Purpose = "Moli Japan region",
                KnownPaths = new[] { "/callcenter/moli" },
                IsLmsaCore = false,
                DnsResolved = true,
                DnsTarget = "moli-jp-llm-821088279.ap-south-1.elb.amazonaws.com"
            },
            new LenovoSubdomain
            {
                Host = "las.moli.lenovo.com",
                Purpose = "Moli Latin America South region",
                KnownPaths = new[] { "/callcenter/moli" },
                IsLmsaCore = false,
                DnsResolved = true,
                DnsTarget = "moli-las-llm-581246812.us-east-1.elb.amazonaws.com"
            },
            new LenovoSubdomain
            {
                Host = "pl.moli.lenovo.com",
                Purpose = "Moli Poland region",
                KnownPaths = new[] { "/callcenter/moli" },
                IsLmsaCore = false,
                DnsResolved = true,
                DnsTarget = "moli-pl-llm-115217512.eu-west-3.elb.amazonaws.com"
            },
            new LenovoSubdomain
            {
                Host = "uk.moli.lenovo.com",
                Purpose = "Moli United Kingdom region",
                KnownPaths = new[] { "/callcenter/moli" },
                IsLmsaCore = false,
                DnsResolved = true,
                DnsTarget = "moli-uk-llm-681019518.eu-west-3.elb.amazonaws.com"
            },
            new LenovoSubdomain
            {
                Host = "idn.moli.lenovo.com",
                Purpose = "Moli Indonesia region",
                KnownPaths = new[] { "/callcenter/moli" },
                IsLmsaCore = false,
                DnsResolved = true,
                DnsTarget = "moli-in-llm-551440711.ap-south-1.elb.amazonaws.com"
            },

            // --- Other ---
            new LenovoSubdomain
            {
                Host = "www3.lenovo.com",
                Purpose = "Lenovo website (privacy policy links)",
                KnownPaths = new[] { "/us/en/privacy" },
                IsLmsaCore = true,
                DnsResolved = true,
                DnsTarget = "www3.lenovo.com.edgekey.net"
            },
            new LenovoSubdomain
            {
                Host = "club.lenovo.com.cn",
                Purpose = "China community forums (Motorola, phones, tablets)",
                KnownPaths = new[] { "/moto/", "/phone/", "/forum-1349-1.html" },
                IsLmsaCore = true,
                DnsResolved = true,
                DnsTarget = "club.lenovo.com.cn.wswebcdn.com"
            },
            new LenovoSubdomain
            {
                Host = "rom.lenovo.com",
                Purpose = "ROM download server",
                KnownPaths = Array.Empty<string>(),
                IsLmsaCore = false,
                DnsResolved = true,
                DnsTarget = "rom-lenovo.lenovomm.com"
            },
            new LenovoSubdomain
            {
                Host = "ota.lenovo.com",
                Purpose = "OTA update server",
                KnownPaths = Array.Empty<string>(),
                IsLmsaCore = false,
                DnsResolved = true,
                DnsTarget = "ota-new.lenovo.gtm.skycdn.com.cn"
            },
            new LenovoSubdomain
            {
                Host = "smart.lenovo.com",
                Purpose = "Lenovo smart services",
                KnownPaths = Array.Empty<string>(),
                IsLmsaCore = false,
                DnsResolved = true,
                DnsTarget = "43.255.224.102"
            },
        };

        // ====================================================================
        // API Endpoint Paths (from decompiled WebApiUrl.cs)
        // All paths are relative to {BaseUrl}/Interface unless noted otherwise.
        // Original field names preserved (including intentional misspellings).
        // ====================================================================

        public static class ApiPaths
        {
            // --- Security / Token ---
            public const string GET_PUBLIC_KEY = "/common/rsa.jhtml";
            public const string INIT_TOKEN = "/client/initToken.jhtml";
            public const string DISPOSE_TOKEN = "/client/deleteToken.jhtml";

            // --- Client / Updates ---
            public const string CLIENT_VERSION = "/client/getNextUpdateClient.jhtml";
            public const string UPDATE_VERSION = "/client/getPluginCategoryList.jhtml";
            public const string PLUGIN_VERSION = "/client/getClientPlugins.jhtml";
            public const string UPDATE_DOWNLOAD_URL = "/client/renewFileLink.jhtml";
            public const string USER_GUIDE = "/client/getUserGuide.jhtml";
            public const string HELP_URI = "/client/clientHelp.jhtml";
            public const string LOAD_WARRANTY_BANNER = "/client/motoCare.jhtml";
            public const string LOAD_COUPON = "/client/discountCoupon.jhtml";
            public const string CHECK_MA_VERSION = "/apk/download.jhtml";

            // --- Device ---
            public const string GET_DEVICE_INFO = "/device/getDeviceInfo.jhtml";
            public const string GET_DEVICE_ICON = "/device/getDeviceIcon.jhtml";

            // --- User / Authentication ---
            public const string USER_LOGIN = "/user/login.jhtml";
            public const string USER_GUEST_LOGIN = "/user/guestLogin.jhtml";
            public const string USER_LOGOUT = "/user/logout.jhtml";
            public const string USER_FORGOT_PASSWORD = "/user/forgotPassword.jhtml";
            public const string USER_CHANGE_PASSWORD = "/user/changePassword.jhtml";
            public const string USER_RECORD_LOGIN = "/user/recordLogin.jhtml";
            public const string LENOVOID_LOGIN_CALLBACK = "/user/lenovoIdLogin.jhtml";

            // --- Rescue / Flash ---
            public const string LOAD_SMART_DEVICE = "/rescueDevice/smartMarketNames.jhtml";
            public const string GET_UPGRADEFLASH_MATCH_TYPES = "/rescueDevice/getParamType.jhtml";
            public const string RESUCE_AUTOMATCH_GETPARAMS_MAPPING = "/rescueDevice/getRomMatchParams.jhtml";
            public const string RESUCE_AUTOMATCH_GETROM = "/rescueDevice/getNewResource.jhtml";
            public const string RESUCE_CHECK_SUPPORT_FASTBOOT_MODE = "/rescueDevice/getMarketSupport.jhtml";
            public const string MODEL_READ_CONFIG = "/rescueDevice/modelReadConfigration.jhtml";
            public const string GET_FASTBOOTDATA_RECIPE = "/rescueDevice/getRescueModelRecipe.jhtml";

            // --- Model ---
            public const string RESUCE_CHECK_MODEL_NAME_DRIVERS = "/model/getDriverSpecialConfig.jhtml";
            public const string LOAD_YOUTUBE_INFO = "/model/getYoutubeVideo.jhtml";
            public const string ROMFILE_CHECK_RULES = "/model/rules.jhtml";
            public const string GET_SUPPORT_FASTBOOT_BY_MODELNAME = "/model/isReadSupport.jhtml";

            // --- Notice / Notifications ---
            public const string NOTICE_URL = "/notice/getNoticeInfo.jhtml";
            public const string NOTICE_BROADCAST_URL = "/notice/getBroadcast.jhtml";

            // --- Survey ---
            public const string SURVEY_URL = "/survey/getIsNeedTrigger.jhtml";
            public const string GET_IS_NEED_TRIGGER_SURVER = "/survey/getIsNeedTriggerSurvey.jhtml";
            public const string SURVEY_REFRESH = "/survey/refreshTrigger.jhtml";
            public const string SURVEY_GET_QUESTIONS = "/survey/getAllQuestions.jhtml";
            public const string SURVEY_RECORD = "/survey/record.jhtml";

            // --- Feedback ---
            public const string FEEDBACK_GET_LIST = "/feedback/getFeedbackList.jhtml";
            public const string FEEDBACK_GET_INFO = "/feedback/getFeedbackInfo.jhtml";
            public const string FEEDBACK_FILE_SINGNATURE = "/feedback/fileSignatureUrl.jhtml";
            public const string FEEDBACK_GET_HELPFUL = "/feedback/replyHelpful.jhtml";
            public const string FEEDBACK_GET_UPLOAD = "/feedback/postFeedbackInfo.jhtml";
            public const string FEEDBACK_GET_UPLOAD_GUEST = "/feedback/guestPostFeedbackInfo.jhtml";
            public const string FEEDBACK_GET_ISSUE_INFO = "/feedback/getFeedbackIssueInfo.jhtml";

            // --- Data Collection / Telemetry ---
            public const string POST_UPGRADE_FLASH_INFO = "/dataCollection/UpgradeFlashInfo.jhtml";
            public const string UPLOAD_DOWNLOAD_SPEEDINFO = "/dataCollection/romDownloadInfo.jhtml";
            public const string RESUCE_FAILED_UPLOAD = "/dataCollection/uploadFile.jhtml";
            public const string FEEDBACK_BACKUP_RESTORE = "/dataCollection/addBackupRestore.jhtml";
            public const string FEEDBACK_NO_TRANSLATE = "/dataCollection/untranslatedSentences.jhtml";
            public const string USER_BEHAVIOR_COLLECTION = "/dataCollection/addUserBehavior.jhtml";
            public const string UPLOAD_RESCUE_TOOL_LOG = "/dataCollection/nativeToolLog.jhtml";
            public const string COLLECTION_RESCUE_SUCCESS_LOG_UPLOAD = "/dataCollection/rescueSuccessLog.jhtml";
            public const string COLLECTION_ASSISTANTAPP = "/dataCollection/assistantApp.jhtml";

            // --- Registered Models ---
            public const string UPLOAD_USER_DEVICE = "/registeredModel/addModels.jhtml";
            public const string DELETE_USER_DEVICE = "/registeredModel/models.jhtml";

            // --- Privileges / ROM ---
            public const string PRIV_GET_PRIV_INFO = "/priv/getPrivInfo.jhtml";
            public const string Webwervice_Get_RomResources = "/priv/getRomList.jhtml";

            // --- Dictionary / API Info ---
            public const string CALL_API_URL = "/dictionary/getApiInfo.jhtml";

            // --- VIP / B2B ---
            public const string CALL_B2B_ORDERS_URL = "/vip/getB2BInfo.jhtml";
            public const string CALL_B2B_ACTIVE_ORDERS_URL = "/vip/getActiveB2BInfos.jhtml";
            public const string CALL_B2B_QUERY_ORDER_URL = "/vip/getEnableB2BOrder.jhtml";
            public const string CALL_B2B_GET_ORDERID_URL = "/vip/getOrderNum.jhtml";
            public const string CALL_B2B_ORDER_BUY_URL = "/vip/buy.jhtml";
            public const string CALL_B2B_GET_PRICE_URL = "/vip/card.jhtml";

            // --- Moli ---
            public const string GET_MOLI_REQUEST_URL = "/moli/getMoliUrl.jhtml";
            public const string MOLI_INFO = "/moli/moliAndLena.jhtml";

            // --- Guide ---
            public const string GET_MUTIL_TUTORIALS_QUESTIONS = "/guide/getGuideQuestion.jhtml";
        }

        // ====================================================================
        // Non-Interface Paths (relative to BaseUrl, not InterfaceUrl)
        // ====================================================================

        public static class BasePaths
        {
            public const string SHOW_FEEDBACK = "/Tips/feedback.html";
            public const string NETWORK_CONNECT_CHECK = "/lmsa-web/index.jsp";
        }

        // ====================================================================
        // External Service URLs (hardcoded in decompiled source)
        // ====================================================================

        public static class ExternalServices
        {
            // Network connectivity check (from WebApiUrl.NETWORK_CONNECT_CHECK)
            public const string NETWORK_CHECK_URL = "https://lsa.lenovo.com/lmsa-web/index.jsp";

            // QR code download (from Configurations.QrCodeDownloadMaUrl)
            public const string QR_CODE_DOWNLOAD_MA = "https://download.lenovo.com/lsa/ma.apk";

            // PDF document pattern (from AboutOperationItemViewModel)
            public const string PDF_DOCUMENT_PATTERN = "https://download.lenovo.com/lenovo/lla/l505-0009-06-{0}.pdf";

            // Warranty (from WarrantyService)
            public const string WARRANTY_SUPPORT_URL = "https://supportapi.lenovo.com/v3/warranty/";
            public const string WARRANTY_SDE_TOKEN = "https://microapi-us-sde.lenovo.com/token";
            public const string WARRANTY_SDE_URL = "https://microapi-us-sde.lenovo.com/v1.0/service/poi_request";
            public const string WARRANTY_IBASE_URL = "https://ibase.lenovo.com/POIRequest.aspx";
            public const string WARRANTY_MDS_OAUTH = "https://api-pre-mds-us.lenovo.com/auth/oauth/token";
            public const string WARRANTY_MDS_ORDER = "https://api-pre-mds-us.lenovo.com/order/order/rnt/getUnit";

            // Lenovo ID (from LenovoIdWindow, LenovoIdUserLogin)
            public const string LENOVOID_PRELOGIN = "https://passport.lenovo.com/glbwebauthnv6/preLogin";
            public const string LENOVOID_LOGOUT = "https://passport.lenovo.com/wauthen5/gateway?lenovoid.action=uilogout&lenovoid.realm=lmsaclient";
            public const string LENOVOID_ACCOUNT_FORMAT = "https://passport.lenovo.com/interserver/authen/1.2/getaccountid?lpsust={0}&realm=lmsaclient";

            // Moli call center (from RescueFailedSubmitView)
            public const string MOLI_CALLCENTER = "https://moli.lenovo.com/callcenter/moli";

            // Privacy (from MessengerFrame)
            public const string PRIVACY_POLICY = "https://www3.lenovo.com/us/en/privacy";

            // Mobile support (from SearchViewExModel)
            public const string MOBILE_SUPPORT_FORMAT = "https://lenovomobilesupport.lenovo.com/{0}/{1}/solutions/find-product-name";
        }

        // ====================================================================
        // Forum URLs by Language/Region (from decompiled ForumFrame.cs)
        // ====================================================================

        public static class Forums
        {
            // Lenovo Phones
            public const string PHONES_EN = "https://forums.lenovo.com/t5/Lenovo-Phones/ct-p/lp_en";
            public const string PHONES_ES = "https://forums.lenovo.com/t5/Smartphones-Lenovo/ct-p/lp_es";
            public const string PHONES_PT = "https://forums.lenovo.com/t5/Telefones-Lenovo/ct-p/phones_pt";
            public const string PHONES_PL = "https://forums.lenovo.com/t5/Smartfony-Lenovo/ct-p/lp_pl";

            // Motorola Community
            public const string MOTO_EN = "https://forums.lenovo.com/t5/Motorola-Community/ct-p/MotorolaCommunity?profile.language=en";
            public const string MOTO_ES = "https://forums.lenovo.com/t5/Comunidad-Motorola/ct-p/ComunidadMotorola?profile.language=es";
            public const string MOTO_PT = "https://forums.lenovo.com/t5/Comunidade-Motorola/ct-p/ComunidadeMotorola";

            // Lenovo Tablets
            public const string TABLETS_EN = "https://forums.lenovo.com/t5/Lenovo-Tablets/ct-p/lt_en";
            public const string TABLETS_ES = "https://forums.lenovo.com/t5/Tablets-Lenovo-IdeaPad-IdeaTab/ct-p/lt_es";
            public const string TABLETS_PT = "https://forums.lenovo.com/t5/Tablets-Lenovo/ct-p/lt_pt";
            public const string TABLETS_PL = "https://forums.lenovo.com/t5/Tablety-Lenovo/ct-p/lt_pl";

            // China Community
            public const string CHINA_MOTO = "https://club.lenovo.com.cn/moto/";
            public const string CHINA_PHONES = "https://club.lenovo.com.cn/phone/";
            public const string CHINA_FORUM = "https://club.lenovo.com.cn/forum-1349-1.html";
        }

        // ====================================================================
        // Helper Methods
        // ====================================================================

        /// <summary>
        /// Build a full URL from a server environment and an API path.
        /// </summary>
        public static string BuildUrl(ServerEnvironment env, string apiPath)
        {
            if (string.IsNullOrEmpty(apiPath))
                throw new ArgumentNullException(nameof(apiPath));
            if (apiPath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return apiPath;
            return env.InterfaceUrl + apiPath;
        }

        /// <summary>
        /// Build a base-relative URL (not Interface-relative).
        /// </summary>
        public static string BuildBaseUrl(ServerEnvironment env, string basePath)
        {
            if (string.IsNullOrEmpty(basePath))
                throw new ArgumentNullException(nameof(basePath));
            if (basePath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return basePath;
            return env.BaseUrl + basePath;
        }

        /// <summary>
        /// Get all API endpoints as EndpointInfo objects for a given environment.
        /// </summary>
        public static IReadOnlyList<EndpointInfo> GetAllApiEndpoints()
        {
            return new List<EndpointInfo>
            {
                // Security / Token
                new() { Name = "GET_PUBLIC_KEY", Path = ApiPaths.GET_PUBLIC_KEY, Category = "security", HttpMethod = "POST" },
                new() { Name = "INIT_TOKEN", Path = ApiPaths.INIT_TOKEN, Category = "security", HttpMethod = "POST" },
                new() { Name = "DISPOSE_TOKEN", Path = ApiPaths.DISPOSE_TOKEN, Category = "security", HttpMethod = "POST" },

                // Client / Updates
                new() { Name = "CLIENT_VERSION", Path = ApiPaths.CLIENT_VERSION, Category = "client", HttpMethod = "POST" },
                new() { Name = "UPDATE_VERSION", Path = ApiPaths.UPDATE_VERSION, Category = "client", HttpMethod = "POST" },
                new() { Name = "PLUGIN_VERSION", Path = ApiPaths.PLUGIN_VERSION, Category = "client", HttpMethod = "POST" },
                new() { Name = "UPDATE_DOWNLOAD_URL", Path = ApiPaths.UPDATE_DOWNLOAD_URL, Category = "client", HttpMethod = "POST" },
                new() { Name = "USER_GUIDE", Path = ApiPaths.USER_GUIDE, Category = "client", HttpMethod = "POST" },
                new() { Name = "HELP_URI", Path = ApiPaths.HELP_URI, Category = "client", HttpMethod = "POST" },
                new() { Name = "LOAD_WARRANTY_BANNER", Path = ApiPaths.LOAD_WARRANTY_BANNER, Category = "client", HttpMethod = "POST" },
                new() { Name = "LOAD_COUPON", Path = ApiPaths.LOAD_COUPON, Category = "client", HttpMethod = "POST" },
                new() { Name = "CHECK_MA_VERSION", Path = ApiPaths.CHECK_MA_VERSION, Category = "client", HttpMethod = "POST" },

                // Device
                new() { Name = "GET_DEVICE_INFO", Path = ApiPaths.GET_DEVICE_INFO, Category = "device", HttpMethod = "POST" },
                new() { Name = "GET_DEVICE_ICON", Path = ApiPaths.GET_DEVICE_ICON, Category = "device", HttpMethod = "POST" },

                // User / Authentication
                new() { Name = "USER_LOGIN", Path = ApiPaths.USER_LOGIN, Category = "user", HttpMethod = "POST" },
                new() { Name = "USER_GUEST_LOGIN", Path = ApiPaths.USER_GUEST_LOGIN, Category = "user", HttpMethod = "POST" },
                new() { Name = "USER_LOGOUT", Path = ApiPaths.USER_LOGOUT, Category = "user", HttpMethod = "POST" },
                new() { Name = "USER_FORGOT_PASSWORD", Path = ApiPaths.USER_FORGOT_PASSWORD, Category = "user", HttpMethod = "POST" },
                new() { Name = "USER_CHANGE_PASSWORD", Path = ApiPaths.USER_CHANGE_PASSWORD, Category = "user", HttpMethod = "POST" },
                new() { Name = "USER_RECORD_LOGIN", Path = ApiPaths.USER_RECORD_LOGIN, Category = "user", HttpMethod = "POST" },
                new() { Name = "LENOVOID_LOGIN_CALLBACK", Path = ApiPaths.LENOVOID_LOGIN_CALLBACK, Category = "user", HttpMethod = "POST" },

                // Rescue / Flash
                new() { Name = "LOAD_SMART_DEVICE", Path = ApiPaths.LOAD_SMART_DEVICE, Category = "rescue", HttpMethod = "POST" },
                new() { Name = "GET_UPGRADEFLASH_MATCH_TYPES", Path = ApiPaths.GET_UPGRADEFLASH_MATCH_TYPES, Category = "rescue", HttpMethod = "POST" },
                new() { Name = "RESUCE_AUTOMATCH_GETPARAMS_MAPPING", Path = ApiPaths.RESUCE_AUTOMATCH_GETPARAMS_MAPPING, Category = "rescue", HttpMethod = "POST" },
                new() { Name = "RESUCE_AUTOMATCH_GETROM", Path = ApiPaths.RESUCE_AUTOMATCH_GETROM, Category = "rescue", HttpMethod = "POST" },
                new() { Name = "RESUCE_CHECK_SUPPORT_FASTBOOT_MODE", Path = ApiPaths.RESUCE_CHECK_SUPPORT_FASTBOOT_MODE, Category = "rescue", HttpMethod = "POST" },
                new() { Name = "MODEL_READ_CONFIG", Path = ApiPaths.MODEL_READ_CONFIG, Category = "rescue", HttpMethod = "POST" },
                new() { Name = "GET_FASTBOOTDATA_RECIPE", Path = ApiPaths.GET_FASTBOOTDATA_RECIPE, Category = "rescue", HttpMethod = "POST" },

                // Model
                new() { Name = "RESUCE_CHECK_MODEL_NAME_DRIVERS", Path = ApiPaths.RESUCE_CHECK_MODEL_NAME_DRIVERS, Category = "model", HttpMethod = "POST" },
                new() { Name = "LOAD_YOUTUBE_INFO", Path = ApiPaths.LOAD_YOUTUBE_INFO, Category = "model", HttpMethod = "POST" },
                new() { Name = "ROMFILE_CHECK_RULES", Path = ApiPaths.ROMFILE_CHECK_RULES, Category = "model", HttpMethod = "POST" },
                new() { Name = "GET_SUPPORT_FASTBOOT_BY_MODELNAME", Path = ApiPaths.GET_SUPPORT_FASTBOOT_BY_MODELNAME, Category = "model", HttpMethod = "POST" },

                // Notice
                new() { Name = "NOTICE_URL", Path = ApiPaths.NOTICE_URL, Category = "notice", HttpMethod = "POST" },
                new() { Name = "NOTICE_BROADCAST_URL", Path = ApiPaths.NOTICE_BROADCAST_URL, Category = "notice", HttpMethod = "POST" },

                // Survey
                new() { Name = "SURVEY_URL", Path = ApiPaths.SURVEY_URL, Category = "survey", HttpMethod = "POST" },
                new() { Name = "GET_IS_NEED_TRIGGER_SURVER", Path = ApiPaths.GET_IS_NEED_TRIGGER_SURVER, Category = "survey", HttpMethod = "POST" },
                new() { Name = "SURVEY_REFRESH", Path = ApiPaths.SURVEY_REFRESH, Category = "survey", HttpMethod = "POST" },
                new() { Name = "SURVEY_GET_QUESTIONS", Path = ApiPaths.SURVEY_GET_QUESTIONS, Category = "survey", HttpMethod = "POST" },
                new() { Name = "SURVEY_RECORD", Path = ApiPaths.SURVEY_RECORD, Category = "survey", HttpMethod = "POST" },

                // Feedback
                new() { Name = "FEEDBACK_GET_LIST", Path = ApiPaths.FEEDBACK_GET_LIST, Category = "feedback", HttpMethod = "POST" },
                new() { Name = "FEEDBACK_GET_INFO", Path = ApiPaths.FEEDBACK_GET_INFO, Category = "feedback", HttpMethod = "POST" },
                new() { Name = "FEEDBACK_FILE_SINGNATURE", Path = ApiPaths.FEEDBACK_FILE_SINGNATURE, Category = "feedback", HttpMethod = "POST" },
                new() { Name = "FEEDBACK_GET_HELPFUL", Path = ApiPaths.FEEDBACK_GET_HELPFUL, Category = "feedback", HttpMethod = "POST" },
                new() { Name = "FEEDBACK_GET_UPLOAD", Path = ApiPaths.FEEDBACK_GET_UPLOAD, Category = "feedback", HttpMethod = "POST" },
                new() { Name = "FEEDBACK_GET_UPLOAD_GUEST", Path = ApiPaths.FEEDBACK_GET_UPLOAD_GUEST, Category = "feedback", HttpMethod = "POST" },
                new() { Name = "FEEDBACK_GET_ISSUE_INFO", Path = ApiPaths.FEEDBACK_GET_ISSUE_INFO, Category = "feedback", HttpMethod = "POST" },

                // Data Collection
                new() { Name = "POST_UPGRADE_FLASH_INFO", Path = ApiPaths.POST_UPGRADE_FLASH_INFO, Category = "dataCollection", HttpMethod = "POST" },
                new() { Name = "UPLOAD_DOWNLOAD_SPEEDINFO", Path = ApiPaths.UPLOAD_DOWNLOAD_SPEEDINFO, Category = "dataCollection", HttpMethod = "POST" },
                new() { Name = "RESUCE_FAILED_UPLOAD", Path = ApiPaths.RESUCE_FAILED_UPLOAD, Category = "dataCollection", HttpMethod = "POST" },
                new() { Name = "FEEDBACK_BACKUP_RESTORE", Path = ApiPaths.FEEDBACK_BACKUP_RESTORE, Category = "dataCollection", HttpMethod = "POST" },
                new() { Name = "FEEDBACK_NO_TRANSLATE", Path = ApiPaths.FEEDBACK_NO_TRANSLATE, Category = "dataCollection", HttpMethod = "POST" },
                new() { Name = "USER_BEHAVIOR_COLLECTION", Path = ApiPaths.USER_BEHAVIOR_COLLECTION, Category = "dataCollection", HttpMethod = "POST" },
                new() { Name = "UPLOAD_RESCUE_TOOL_LOG", Path = ApiPaths.UPLOAD_RESCUE_TOOL_LOG, Category = "dataCollection", HttpMethod = "POST" },
                new() { Name = "COLLECTION_RESCUE_SUCCESS_LOG_UPLOAD", Path = ApiPaths.COLLECTION_RESCUE_SUCCESS_LOG_UPLOAD, Category = "dataCollection", HttpMethod = "POST" },
                new() { Name = "COLLECTION_ASSISTANTAPP", Path = ApiPaths.COLLECTION_ASSISTANTAPP, Category = "dataCollection", HttpMethod = "POST" },

                // Registered Models
                new() { Name = "UPLOAD_USER_DEVICE", Path = ApiPaths.UPLOAD_USER_DEVICE, Category = "registeredModel", HttpMethod = "POST" },
                new() { Name = "DELETE_USER_DEVICE", Path = ApiPaths.DELETE_USER_DEVICE, Category = "registeredModel", HttpMethod = "POST" },

                // Privileges / ROM
                new() { Name = "PRIV_GET_PRIV_INFO", Path = ApiPaths.PRIV_GET_PRIV_INFO, Category = "priv", HttpMethod = "POST" },
                new() { Name = "Webwervice_Get_RomResources", Path = ApiPaths.Webwervice_Get_RomResources, Category = "priv", HttpMethod = "POST" },

                // Dictionary
                new() { Name = "CALL_API_URL", Path = ApiPaths.CALL_API_URL, Category = "dictionary", HttpMethod = "POST" },

                // VIP / B2B
                new() { Name = "CALL_B2B_ORDERS_URL", Path = ApiPaths.CALL_B2B_ORDERS_URL, Category = "vip", HttpMethod = "POST" },
                new() { Name = "CALL_B2B_ACTIVE_ORDERS_URL", Path = ApiPaths.CALL_B2B_ACTIVE_ORDERS_URL, Category = "vip", HttpMethod = "POST" },
                new() { Name = "CALL_B2B_QUERY_ORDER_URL", Path = ApiPaths.CALL_B2B_QUERY_ORDER_URL, Category = "vip", HttpMethod = "POST" },
                new() { Name = "CALL_B2B_GET_ORDERID_URL", Path = ApiPaths.CALL_B2B_GET_ORDERID_URL, Category = "vip", HttpMethod = "POST" },
                new() { Name = "CALL_B2B_ORDER_BUY_URL", Path = ApiPaths.CALL_B2B_ORDER_BUY_URL, Category = "vip", HttpMethod = "POST" },
                new() { Name = "CALL_B2B_GET_PRICE_URL", Path = ApiPaths.CALL_B2B_GET_PRICE_URL, Category = "vip", HttpMethod = "POST" },

                // Moli
                new() { Name = "GET_MOLI_REQUEST_URL", Path = ApiPaths.GET_MOLI_REQUEST_URL, Category = "moli", HttpMethod = "POST" },
                new() { Name = "MOLI_INFO", Path = ApiPaths.MOLI_INFO, Category = "moli", HttpMethod = "POST" },

                // Guide
                new() { Name = "GET_MUTIL_TUTORIALS_QUESTIONS", Path = ApiPaths.GET_MUTIL_TUTORIALS_QUESTIONS, Category = "guide", HttpMethod = "POST" },
            };
        }

        /// <summary>
        /// Get only the LMSA-core subdomains (those directly used by the app).
        /// </summary>
        public static IReadOnlyList<LenovoSubdomain> GetCoreSubdomains()
        {
            return AllSubdomains.Where(s => s.IsLmsaCore).ToArray();
        }

        /// <summary>
        /// Get only the subdomains that resolved via DNS.
        /// </summary>
        public static IReadOnlyList<LenovoSubdomain> GetResolvedSubdomains()
        {
            return AllSubdomains.Where(s => s.DnsResolved).ToArray();
        }

        /// <summary>
        /// Get all unique categories of API endpoints.
        /// </summary>
        public static IReadOnlyList<string> GetEndpointCategories()
        {
            return GetAllApiEndpoints().Select(e => e.Category).Distinct().OrderBy(c => c).ToArray();
        }

        /// <summary>
        /// Get API endpoints filtered by category.
        /// </summary>
        public static IReadOnlyList<EndpointInfo> GetEndpointsByCategory(string category)
        {
            return GetAllApiEndpoints().Where(e => e.Category == category).ToArray();
        }
    }
}
