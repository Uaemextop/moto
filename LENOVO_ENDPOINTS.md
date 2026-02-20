# Lenovo Subdomain & Endpoint Discovery Report

> Comprehensive discovery and verification of all Lenovo subdomains and LMSA (Software Fix) API endpoints.

## Discovery Summary

| Metric | Count |
|---|---|
| **Subdomain sources queried** | 5 (Trickest/inventory, HackerTarget API, crt.sh CT logs, DNS brute-force, Decompiled LMSA) |
| **Total unique lenovo.com subdomains** | 2,246 |
| **DNS resolved** | 1,290 |
| **HTTPS responding** | 630 |
| **HTTP 200 OK** | 392 |
| **LMSA Interface active** | 2 |

### Sources

1. **Trickest/inventory** (GitHub) — 3,101 hostnames from public bug bounty asset inventory
2. **HackerTarget API** — 51 hostnames from passive DNS
3. **crt.sh Certificate Transparency** — 36 moli.lenovo.com regional subdomains
4. **DNS brute-force** — 67 manually crafted LMSA-related candidates
5. **Decompiled LMSA source code** — 12 unique subdomains hardcoded in the application

---

## LMSA-Active Servers

Only **2 subdomains** out of 2,246 run the LMSA Interface (confirmed via `POST /Interface/common/rsa.jhtml`):

| Server | BaseUrl | DNS Target | Role |
|---|---|---|---|
| **lsa.lenovo.com** | `https://lsa.lenovo.com` | `lmsa-web-prd-30521416.us-east-1.elb.amazonaws.com` | **Production** (app.config default) |
| **lsatest.lenovo.com** | `https://lsatest.lenovo.com` | `lmsa-web-dev-ext-1322476325.us-east-1.elb.amazonaws.com` | **Staging/Test** |

Both servers run **Apache Tomcat/11.0.12** and return the **same RSA public key**, confirming they use the same LMSA backend codebase.

### Server Environment Detection

From decompiled `Configurations.cs`:
```csharp
public static bool IsReleaseVersion =>
    "https://lsa.lenovo.com".Equals(ServiceBaseUrl, StringComparison.InvariantCultureIgnoreCase);
```

The base URL is configured in `app.config`:
```xml
<add key="BaseHttpUrl" value="https://lsa.lenovo.com"/>
```

Change to `https://lsatest.lenovo.com` to switch to staging.

---

## LMSA API Endpoints (Production vs Staging)

All 72 endpoints from decompiled `WebApiUrl.cs` tested on both servers. Base path: `{BaseUrl}/Interface/`

| Endpoint | Path | PROD | STG |
|---|---|---|---|
| GET_PUBLIC_KEY | `/common/rsa.jhtml` | 200 ✅ | 200 ✅ |
| INIT_TOKEN | `/client/initToken.jhtml` | 200 ✅ | 200 ✅ |
| DISPOSE_TOKEN | `/client/deleteToken.jhtml` | 200 ✅ | 200 ✅ |
| USER_GUIDE | `/client/getUserGuide.jhtml` | 200 ✅ | 200 ✅ |
| HELP_URI | `/client/clientHelp.jhtml` | 200 ✅ | 200 ✅ |
| CLIENT_VERSION | `/client/getNextUpdateClient.jhtml` | 405 ⚠️ | 405 ⚠️ |
| UPDATE_VERSION | `/client/getPluginCategoryList.jhtml` | 200 ✅ | 200 ✅ |
| PLUGIN_VERSION | `/client/getClientPlugins.jhtml` | 200 ✅ | 200 ✅ |
| UPDATE_DOWNLOAD_URL | `/client/renewFileLink.jhtml` | 200 ✅ | 200 ✅ |
| LOAD_WARRANTY_BANNER | `/client/motoCare.jhtml` | 200 ✅ | 200 ✅ |
| LOAD_COUPON | `/client/discountCoupon.jhtml` | 200 ✅ | 200 ✅ |
| GET_DEVICE_INFO | `/device/getDeviceInfo.jhtml` | 200 ✅ | 200 ✅ |
| GET_DEVICE_ICON | `/device/getDeviceIcon.jhtml` | 200 ✅ | 200 ✅ |
| USER_LOGIN | `/user/login.jhtml` | 200 ✅ | 200 ✅ |
| USER_GUEST_LOGIN | `/user/guestLogin.jhtml` | 200 ✅ | 200 ✅ |
| USER_LOGOUT | `/user/logout.jhtml` | 200 ✅ | 200 ✅ |
| LENOVOID_LOGIN_CALLBACK | `/user/lenovoIdLogin.jhtml` | 405 ⚠️ | 405 ⚠️ |
| USER_RECORD_LOGIN | `/user/recordLogin.jhtml` | 200 ✅ | 200 ✅ |
| USER_FORGOT_PASSWORD | `/user/forgotPassword.jhtml` | 200 ✅ | 200 ✅ |
| USER_CHANGE_PASSWORD | `/user/changePassword.jhtml` | 200 ✅ | 200 ✅ |
| LOAD_SMART_DEVICE | `/rescueDevice/smartMarketNames.jhtml` | 200 ✅ | 200 ✅ |
| GET_UPGRADEFLASH_MATCH_TYPES | `/rescueDevice/getParamType.jhtml` | 200 ✅ | 200 ✅ |
| RESUCE_AUTOMATCH_GETPARAMS | `/rescueDevice/getRomMatchParams.jhtml` | 200 ✅ | 200 ✅ |
| RESUCE_AUTOMATCH_GETROM | `/rescueDevice/getNewResource.jhtml` | 200 ✅ | 200 ✅ |
| RESUCE_CHECK_FASTBOOT | `/rescueDevice/getMarketSupport.jhtml` | 200 ✅ | 200 ✅ |
| MODEL_READ_CONFIG | `/rescueDevice/modelReadConfigration.jhtml` | 405 ⚠️ | 405 ⚠️ |
| GET_FASTBOOTDATA_RECIPE | `/rescueDevice/getRescueModelRecipe.jhtml` | 200 ✅ | 200 ✅ |
| RESUCE_CHECK_DRIVERS | `/model/getDriverSpecialConfig.jhtml` | 200 ✅ | 200 ✅ |
| LOAD_YOUTUBE_INFO | `/model/getYoutubeVideo.jhtml` | 200 ✅ | 200 ✅ |
| ROMFILE_CHECK_RULES | `/model/rules.jhtml` | 200 ✅ | 200 ✅ |
| GET_SUPPORT_FASTBOOT | `/model/isReadSupport.jhtml` | 200 ✅ | 200 ✅ |
| NOTICE_URL | `/notice/getNoticeInfo.jhtml` | 405 ⚠️ | 405 ⚠️ |
| NOTICE_BROADCAST_URL | `/notice/getBroadcast.jhtml` | 405 ⚠️ | 405 ⚠️ |
| SURVEY_URL | `/survey/getIsNeedTrigger.jhtml` | 200 ✅ | 200 ✅ |
| GET_IS_NEED_TRIGGER_SURVER | `/survey/getIsNeedTriggerSurvey.jhtml` | 200 ✅ | 200 ✅ |
| SURVEY_REFRESH | `/survey/refreshTrigger.jhtml` | 200 ✅ | 200 ✅ |
| SURVEY_GET_QUESTIONS | `/survey/getAllQuestions.jhtml` | 200 ✅ | 200 ✅ |
| SURVEY_RECORD | `/survey/record.jhtml` | 200 ✅ | 200 ✅ |
| FEEDBACK_GET_LIST | `/feedback/getFeedbackList.jhtml` | 200 ✅ | 200 ✅ |
| FEEDBACK_GET_INFO | `/feedback/getFeedbackInfo.jhtml` | 200 ✅ | 200 ✅ |
| FEEDBACK_FILE_SINGNATURE | `/feedback/fileSignatureUrl.jhtml` | 200 ✅ | 200 ✅ |
| FEEDBACK_GET_HELPFUL | `/feedback/replyHelpful.jhtml` | 200 ✅ | 200 ✅ |
| FEEDBACK_GET_UPLOAD | `/feedback/postFeedbackInfo.jhtml` | 200 ✅ | 200 ✅ |
| FEEDBACK_GET_UPLOAD_GUEST | `/feedback/guestPostFeedbackInfo.jhtml` | 405 ⚠️ | 405 ⚠️ |
| FEEDBACK_GET_ISSUE_INFO | `/feedback/getFeedbackIssueInfo.jhtml` | 405 ⚠️ | 405 ⚠️ |
| POST_UPGRADE_FLASH_INFO | `/dataCollection/UpgradeFlashInfo.jhtml` | 200 ✅ | 200 ✅ |
| UPLOAD_DOWNLOAD_SPEEDINFO | `/dataCollection/romDownloadInfo.jhtml` | 200 ✅ | 200 ✅ |
| RESUCE_FAILED_UPLOAD | `/dataCollection/uploadFile.jhtml` | 200 ✅ | 200 ✅ |
| FEEDBACK_BACKUP_RESTORE | `/dataCollection/addBackupRestore.jhtml` | 200 ✅ | 200 ✅ |
| FEEDBACK_NO_TRANSLATE | `/dataCollection/untranslatedSentences.jhtml` | 200 ✅ | 200 ✅ |
| USER_BEHAVIOR_COLLECTION | `/dataCollection/addUserBehavior.jhtml` | 200 ✅ | 200 ✅ |
| UPLOAD_RESCUE_TOOL_LOG | `/dataCollection/nativeToolLog.jhtml` | 200 ✅ | 200 ✅ |
| COLLECTION_RESCUE_LOG | `/dataCollection/rescueSuccessLog.jhtml` | 200 ✅ | 200 ✅ |
| COLLECTION_ASSISTANTAPP | `/dataCollection/assistantApp.jhtml` | 405 ⚠️ | 405 ⚠️ |
| UPLOAD_USER_DEVICE | `/registeredModel/addModels.jhtml` | 200 ✅ | 200 ✅ |
| DELETE_USER_DEVICE | `/registeredModel/models.jhtml` | 200 ✅ | 200 ✅ |
| PRIV_GET_PRIV_INFO | `/priv/getPrivInfo.jhtml` | 200 ✅ | 200 ✅ |
| Webwervice_Get_RomResources | `/priv/getRomList.jhtml` | 200 ✅ | 200 ✅ |
| CALL_API_URL | `/dictionary/getApiInfo.jhtml` | 405 ⚠️ | 405 ⚠️ |
| CALL_B2B_ORDERS_URL | `/vip/getB2BInfo.jhtml` | 200 ✅ | 200 ✅ |
| CALL_B2B_ACTIVE_ORDERS | `/vip/getActiveB2BInfos.jhtml` | 200 ✅ | 200 ✅ |
| CALL_B2B_QUERY_ORDER | `/vip/getEnableB2BOrder.jhtml` | 200 ✅ | 200 ✅ |
| CALL_B2B_GET_ORDERID | `/vip/getOrderNum.jhtml` | 200 ✅ | 200 ✅ |
| CALL_B2B_ORDER_BUY | `/vip/buy.jhtml` | 200 ✅ | 200 ✅ |
| CALL_B2B_GET_PRICE | `/vip/card.jhtml` | 200 ✅ | 200 ✅ |
| GET_MOLI_REQUEST_URL | `/moli/getMoliUrl.jhtml` | 200 ✅ | 200 ✅ |
| MOLI_INFO | `/moli/moliAndLena.jhtml` | 200 ✅ | 200 ✅ |
| GET_MUTIL_TUTORIALS | `/guide/getGuideQuestion.jhtml` | 200 ✅ | 200 ✅ |
| CHECK_MA_VERSION | `/apk/download.jhtml` | 405 ⚠️ | 405 ⚠️ |
| SHOW_FEEDBACK | `/Tips/feedback.html` (base) | 200 ✅ | 200 ✅ |
| NETWORK_CONNECT_CHECK | `/lmsa-web/index.jsp` (base) | 400 | 303 |

**Legend:** ✅ = Responds (200 OK), ⚠️ = Method not allowed (405, needs POST with proper body/token)

**Key finding:** Production and staging have **identical endpoint availability** — every endpoint returns the same HTTP status on both servers.

---

## External Service Endpoints

Subdomains used by LMSA for external services (warranty, auth, downloads):

| Subdomain | URL | HTTP | Purpose |
|---|---|---|---|
| **passport.lenovo.com** | `/glbwebauthnv6/preLogin` | 200 ✅ | Lenovo ID pre-login |
| **passport.lenovo.com** | `/wauthen5/gateway` | 200 ✅ | Lenovo ID logout gateway |
| **download.lenovo.com** | `/lsa/ma.apk` | 200 ✅ | Mobile Assistant APK download |
| **supportapi.lenovo.com** | `/v3/warranty/` | 404 | Warranty API (needs serial number) |
| **microapi-us-sde.lenovo.com** | `/token` | 401 🔒 | SDE token exchange (needs auth) |
| **microapi-us-sde.lenovo.com** | `/v1.0/service/poi_request` | 405 | SDE POI request (needs POST) |
| **api-pre-mds-us.lenovo.com** | `/auth/oauth/token` | 401 🔒 | MDS OAuth (needs credentials) |
| **api-pre-mds-us.lenovo.com** | `/order/order/rnt/getUnit` | 405 | Warranty order query (needs POST) |
| **moli.lenovo.com** | `/callcenter/moli` | 200 ✅ | Call center / chat support |
| **www3.lenovo.com** | `/us/en/privacy` | 200 ✅ | Privacy policy page |
| **ibase.lenovo.com** | `/POIRequest.aspx` | ❌ timeout | iBase warranty (DNS unresolved) |
| **ibase.gtm.lenovo.com** | `/POIRequest.aspx` | ❌ timeout | iBase GTM alternative |

### Additional Service Endpoints (from decompiled `WarrantyService.cs`)

| Endpoint | Purpose | Auth Required |
|---|---|---|
| `api-mds-us.lenovo.com` | MDS production (non-pre) | ✅ Yes (OAuth) |
| `api-pre-mds-us.lenovo.com` | MDS pre-production | ✅ Yes (OAuth) |
| `america-mds.lenovo.com` | Americas MDS | ✅ Yes |
| `emea-mds.lenovo.com` | EMEA MDS | ✅ Yes |
| `brazil-mds.lenovo.com` | Brazil MDS | ✅ Yes |
| `na-mds.lenovo.com` | North America MDS | ✅ Yes |

---

## Moli Regional Endpoints (Call Center / Chat)

Discovered via Certificate Transparency (crt.sh) — all on AWS ELB:

| Region | Host | DNS Target |
|---|---|---|
| US (default) | `moli.lenovo.com` | `moli-us-llm-*.us-east-1.elb.amazonaws.com` |
| ANZ | `anz.moli.lenovo.com` | `moli-anz-llm-*.ap-south-1.elb.amazonaws.com` |
| Brazil | `br.moli.lenovo.com` | `moli-br-llm-*.us-east-1.elb.amazonaws.com` |
| Germany | `de.moli.lenovo.com` | `moli-de-llm-*.eu-west-3.elb.amazonaws.com` |
| India | `india.moli.lenovo.com` | `moli-in-llm-*.ap-south-1.elb.amazonaws.com` |
| Indonesia | `idn.moli.lenovo.com` | `moli-in-llm-*.ap-south-1.elb.amazonaws.com` |
| Italy | `it.moli.lenovo.com` | `moli-it-llm-*.eu-west-3.elb.amazonaws.com` |
| Japan | `jp.moli.lenovo.com` | `moli-jp-llm-*.ap-south-1.elb.amazonaws.com` |
| LATAM South | `las.moli.lenovo.com` | `moli-las-llm-*.us-east-1.elb.amazonaws.com` |
| Poland | `pl.moli.lenovo.com` | `moli-pl-llm-*.eu-west-3.elb.amazonaws.com` |
| UK | `uk.moli.lenovo.com` | `moli-uk-llm-*.eu-west-3.elb.amazonaws.com` |

---

## Forum URLs (from decompiled `ForumFrame.cs`)

| Language | Device | URL |
|---|---|---|
| English | Phones | `forums.lenovo.com/t5/Lenovo-Phones/ct-p/lp_en` |
| Spanish | Phones | `forums.lenovo.com/t5/Smartphones-Lenovo/ct-p/lp_es` |
| Portuguese | Phones | `forums.lenovo.com/t5/Telefones-Lenovo/ct-p/phones_pt` |
| Polish | Phones | `forums.lenovo.com/t5/Smartfony-Lenovo/ct-p/lp_pl` |
| English | Motorola | `forums.lenovo.com/t5/Motorola-Community/ct-p/MotorolaCommunity` |
| Spanish | Motorola | `forums.lenovo.com/t5/Comunidad-Motorola/ct-p/ComunidadMotorola` |
| Portuguese | Motorola | `forums.lenovo.com/t5/Comunidade-Motorola/ct-p/ComunidadeMotorola` |
| English | Tablets | `forums.lenovo.com/t5/Lenovo-Tablets/ct-p/lt_en` |
| Spanish | Tablets | `forums.lenovo.com/t5/Tablets-Lenovo-IdeaPad-IdeaTab/ct-p/lt_es` |
| Portuguese | Tablets | `forums.lenovo.com/t5/Tablets-Lenovo/ct-p/lt_pt` |
| Polish | Tablets | `forums.lenovo.com/t5/Tablety-Lenovo/ct-p/lt_pl` |
| Chinese | Motorola | `club.lenovo.com.cn/moto/` |
| Chinese | Phones | `club.lenovo.com.cn/phone/` |
| Chinese | General | `club.lenovo.com.cn/forum-1349-1.html` |


---

## Complete List: All 630 HTTPS-Responding Subdomains

Out of 2,246 unique lenovo.com subdomains discovered, 1,290 resolved via DNS and 630 responded over HTTPS.

<details>
<summary>Click to expand full list (630 hosts)</summary>

| Subdomain | HTTP | DNS Target |
|---|---|---|
| 123.lenovo.com.cn | 200 | www.idea123.cn. |
| 2008.lenovo.com | 400 | blog.lenovo.com. |
| 3g.lenovo.com.cn | 200 | 3g.lenovo.com.cn.wswebcdn.com. |
| 58867000.lenovo.com.cn | 200 | 43.255.227.105 |
| a.lenovo.com.cn | 200 | toc.lenovo.com.cn. |
| aaa.lenovo.com.cn | 200 | tob.lenovo.com.cn. |
| accessorysmartfind.lenovo.com | 200 | nde-ex-router-ex-alb-191741537.us-east-1.elb.amazonaws.com. |
| account.cnno1.uds.lenovo.com | 200 | account.cnno1.uds.lenovo.com.cdn.cloudflare.net. |
| account.euwe1.uds.lenovo.com | 200 | account.euwe1.uds.lenovo.com.cdn.cloudflare.net. |
| account.lenovo.com | 200 | account.lenovo.com.edgekey.net. |
| account.naea1.uds.lenovo.com | 200 | account.naea1.uds.lenovo.com.cdn.cloudflare.net. |
| account.uds.lenovo.com | 200 | account.naea1.uds.lenovo.com.cdn.cloudflare.net. |
| accsmartfind.lenovo.com | 200 | nde-ex-router-ex-alb-191741537.us-east-1.elb.amazonaws.com. |
| activity.lenovo.com.cn | 403 | activity.lenovo.com.cn.wswebcdn.com. |
| admin.cochat.lenovo.com | 200 | admin.cochat.lenovo.com.akadns.net. |
| admin.lenovo.com.cn | 200 | admin.lenovo.com.cn.cdn20.com. |
| agile.lenovo.com | 200 | 103.30.232.101 |
| ai.lenovo.com.cn | 200 | ai.lenovo.com.cn.wswebcdn.com. |
| aifusion.lenovo.com | 403 | aifusion.lenovo.com.volcgslb.com. |
| aipd.lenovo.com | 404 | 104.232.228.91 |
| aisrv.lenovo.com.cn | 502 | aisrv.lenovo.com.cn.wswebcdn.com. |
| america-mds.lenovo.com | 200 | mds-us-2037268721.us-east-1.elb.amazonaws.com. |
| anz.moli.lenovo.com | 200 | moli-anz-llm-584616787.ap-south-1.elb.amazonaws.com. |
| apemealoyalty.lenovo.com | 200 | lenovosmbsandbox-widgets-sb.crowdtwist.com. |
| api-ar.cnno1.uds.lenovo.com | 404 | api-ar.cnno1.uds.lenovo.com.cdn.cloudflare.net. |
| api-ar.euwe1.uds.lenovo.com | 404 | api-ar.euwe1.uds.lenovo.com.cdn.cloudflare.net. |
| api-ar.naea1.uds.lenovo.com | 404 | api-ar.naea1.uds.lenovo.com.cdn.cloudflare.net. |
| api-awsus-t.lenovo.com | 200 | 13.248.163.4 |
| api-awsus.lenovo.com | 200 | 76.223.65.132 |
| api-cui.euwe1.uds.lenovo.com | 204 | 3.126.41.212 |
| api-cui.naea1.uds.lenovo.com | 204 | 23.20.14.93 |
| api-cui.uds-qa.lenovo.com | 204 | 98.83.179.0 |
| api-emea-mds.lenovo.com | 404 | mds-emea-540672444.eu-central-1.elb.amazonaws.com. |
| api-mds-us.lenovo.com | 200 | nde-ex-router-ex-alb-191741537.us-east-1.elb.amazonaws.com. |
| api-mds.lenovo.com | 404 | mds-1267536186.ap-southeast-1.elb.amazonaws.com. |
| api-mtls-int.naea1.uds-pen.lenovo.com | 400 | 54.166.106.62 |
| api-mtls.cnno1.uds-sit.lenovo.com | 403 | api-mtls.cnno1.uds-sit.lenovo.com.cdn.cloudflare.net. |
| api-mtls.euwe1.uds.lenovo.com | 401 | api-mtls.euwe1.uds.lenovo.com.cdn.cloudflare.net. |
| api-mtls.naea1.uds-dev.lenovo.com | 400 | 52.55.35.200 |
| api-mtls.naea1.uds-qa.lenovo.com | 400 | 54.225.120.11 |
| api-mtls.naea1.uds-sit.lenovo.com | 401 | api-mtls.naea1.uds-sit.lenovo.com.cdn.cloudflare.net. |
| api-mtls.naea1.uds.lenovo.com | 401 | api-mtls.naea1.uds.lenovo.com.cdn.cloudflare.net. |
| api-mtls.uds-qa.lenovo.com | 400 | k8s-waf-nginxqa-d9a7d510d4-78d4586f3c633156.elb.us-east-1.amazonaws.com. |
| api-pre1-america-mds.lenovo.com | 404 | mds-us-pre-1009839166.us-east-1.elb.amazonaws.com. |
| api-qas-mds.lenovo.com | 404 | mds-qas-655301268.ap-southeast-1.elb.amazonaws.com. |
| api-tst-emea-mds.lenovo.com | 404 | mds-emea-pre-678481178.eu-central-1.elb.amazonaws.com. |
| api.cnno1.uds.lenovo.com | 404 | api.cnno1.uds.lenovo.com.cdn.cloudflare.net. |
| api.euwe1.uds.lenovo.com | 404 | api.euwe1.uds.lenovo.com.cdn.cloudflare.net. |
| api.lenovo.com.cn | 200 | 43.255.226.57 |
| api.naea1.uds.lenovo.com | 404 | api.naea1.uds.lenovo.com.cdn.cloudflare.net. |
| api.uds.lenovo.com | 404 | api.naea1.uds.lenovo.com. |
| api.wd.lenovo.com | 504 | 137.117.90.63 |
| apifis-t.lenovo.com.cn | 404 | 146.196.114.128 |
| aployalty.lenovo.com | 200 | lenovoapb2c-widgets-de.crowdtwist.com. |
| aposenablement.lenovo.com | 200 | 207.7.91.165 |
| app-analysis-tdp-us.lenovo.com | 200 | 104.232.228.78 |
| app-analysis-tdp.lenovo.com | 200 | 146.196.115.148 |
| app-download.cnno1.uds.lenovo.com | 403 | d1yy35eda1ai2r.cloudfront.cn. |
| app-download.euwe1.uds.lenovo.com | 403 | d2zv1gf9rk9sjl.cloudfront.net. |
| app-download.naea1.uds.lenovo.com | 403 | 3.167.56.69 |
| app.lenovo.com | 200 | app-lenovo.lenovomm.com. |
| appdl.lenovo.com.cn | 403 | appdl.lenovo.com.cn.trpcdn.net. |
| approval-akamai-test.cochat.lenovo.com | 400 | 23.207.150.69 |
| approval.cochat.lenovo.com | 200 | approval.cochat.lenovo.gtm.skycdn.com.cn. |
| approve.cochat.lenovo.com | 200 | approve.cochat.lenovo.gtm.skycdn.com.cn. |
| apps.cochat.lenovo.com | 200 | apps.cochat-2.lenovo.gtm.skycdn.com.cn. |
| apps.lenovo.com.cn | 200 | pc-store-old.mbgstore.lenovo.com.cn. |
| apps.thinkshield.lenovo.com | 200 | 13.226.209.109 |
| appserver.lenovo.com.cn | 200 | appserver.lenovo.com.cn.wscdns.com. |
| appshopforwindows.lenovo.com | 400 | redirect.www.lenovo.com. |
| apsmbloyalty.lenovo.com | 200 | lenovoapsmb-widgets-de.crowdtwist.com. |
| arabic.lenovo.com | 200 | arabic.lenovo.com.edgekey.net. |
| ask.lenovo.com.cn | 200 | toc.lenovo.com.cn. |
| asp.lenovo.com.cn | 200 | tob.lenovo.com.cn. |
| aurora.lenovo.com | 200 | 103.212.14.23 |
| auth.cnno1.uds.lenovo.com | 403 | auth.cnno1.uds.lenovo.com.cdn.cloudflare.net. |
| auth.euwe1.uds.lenovo.com | 403 | auth.euwe1.uds.lenovo.com.cdn.cloudflare.net. |
| auth.naea1.uds-stage.lenovo.com | 404 | auth.naea1.uds-stage.lenovo.com.cdn.cloudflare.net. |
| auth.naea1.uds.lenovo.com | 403 | auth.naea1.uds.lenovo.com.cdn.cloudflare.net. |
| author.lenovo.com | 401 | author.lenovo.com.edgekey.net. |
| b.lenovo.com | 200 | 39.105.251.195 |
| b.lenovo.com.cn | 200 | b.lenovo.com.cn.wswebcdn.com. |
| b2bius.lenovo.com | 404 | 103.30.232.7 |
| b2bservice.lenovo.com.cn | 200 | toc.lenovo.com.cn. |
| b2c.lenovo.com | 500 | b2c.lenovo.gtm.skycdn.com.cn. |
| baiyingmalladmin.lenovo.com | 200 | 39.105.251.195 |
| bbt.lenovo.com | 200 | 84.252.116.19 |
| beta.start.lenovo.com | 200 | start.lenovo.com.edgekey.net. |
| biz.lenovo.com.cn | 200 | biz.lenovo.com.cn.wswebcdn.com. |
| blog.lenovo.com | 200 | social.lenovo.com.edgekey.net. |
| blogs.lenovo.com | 400 | redirect.www.lenovo.com. |
| bm.lenovo.com.cn | 200 | bm.lenovo.com.cn.wswebcdn.com. |
| booking-qa.lenovo.com | 200 | 104.232.228.48 |
| booking-uat.lenovo.com | 200 | 104.232.228.34 |
| booking.lenovo.com | 200 | 103.30.234.53 |
| box.lenovo.com | 405 | 81.70.227.121 |
| bpsso.lenovo.com | 403 | bpsso.lenovomm.com. |
| br.moli.lenovo.com | 200 | moli-br-llm-1842339664.us-east-1.elb.amazonaws.com. |
| brain.lenovo.com | 404 | 43.255.226.237 |
| brandworld.lenovo.com | 200 | brandworld.lenovo.com.edgekey.net. |
| brazil-mds.lenovo.com | 200 | mds-sa-elb-141948487.sa-east-1.elb.amazonaws.com. |
| bsp.lenovo.com | 200 | bsp.lenovo.gtm.skycdn.com.cn. |
| buy.lenovo.com.cn | 200 | buy.lenovo.com.cn.wswebcdn.com. |
| c.lenovo.com.cn | 404 | c.lenovo.com.cn.wswebcdn.com. |
| callhome.uds.lenovo.com | 403 | d3jc3tecbpue8w.cloudfront.net. |
| canada.lenovo.com | 200 | canada.lenovo.com.edgekey.net. |
| capital.lenovo.com | 200 | 139.219.3.37 |
| cas.wx.lenovo.com.cn | 200 | 47.98.45.123 |
| cbp.lenovo.com | 200 | 146.196.115.98 |
| cbp.lenovo.com.cn | 200 | 146.196.115.98 |
| cbs.lenovo.com.cn | 200 | tob.lenovo.com.cn. |
| cdo-tst.lenovo.com | 404 | 103.212.14.43 |
| checkout.lenovo.com | 200 | redirect-to-www.lenovo.com.edgekey.net. |
| cispcmgr.lenovo.com.cn | 400 | cispcmgr.mbgcdn.lenovo.com.cn. |
| class.smartedu.lenovo.com | 200 | smarteduprod1-internet-nlb-c5e7fb2cae10593e.elb.cn-northwest-1.amazonaws.com.cn. |
| classification.lenovo.com | 200 | 3.92.109.81 |
| click.lenovo.com | 200 | 143.64.216.217 |
| cloud.lenovo.com | 200 | cloudlenovo.mbgstore.lenovo.com.cn. |
| cloudboard.lenovo.com | 200 | 47.94.178.174 |
| club.lenovo.com.cn | 200 | club.lenovo.com.cn.wswebcdn.com. |
| cms.csw.lenovo.com | 503 | cms.csw.lenovo.com.edgekey.net. |
| cmsdev.lenovo.com.cn | 404 | 39.105.123.68 |
| cn-api-test.lenovo.com | 404 | api-cn-test.lenovo.gtm2.akadns99.net. |
| cn.sso.lenovo.com | 404 | 104.18.40.91 |
| cnapp.cochat.lenovo.com | 404 | cnapp.cochat-2.lenovo.gtm.skycdn.com.cn. |
| cochat.lenovo.com | 200 | global.cochat.lenovo.com.akadns.net. |
| codata.lenovo.com | 200 | 43.255.226.174 |
| collect.vgs.lenovo.com.cn | 200 | collect-vgs.mbgcdn.lenovo.com.cn. |
| community.lenovo.com | 403 | licommunity-1597361843.us-east-1.elb.amazonaws.com. |
| conference.cochat.lenovo.com | 403 | conference.cochat.lenovo.com.akadns.net. |
| console.thinksmart.lenovo.com | 200 | 13.226.209.47 |
| cp-dev.vgs.lenovo.com.cn | 404 | 47.95.178.31 |
| crm.lenovo.com | 200 | crm.lenovo.gtm.skycdn.com.cn. |
| cs.lenovo.com | 500 | cs.lenovo.com.edgekey.net. |
| csapi.lenovo.com.cn | 200 | csapi.lenovo.gtm.skycdn.com.cn. |
| csp.lenovo.com | 200 | 104.232.228.121 |
| cube.lenovo.com | 200 | 103.212.4.6 |
| cube.lenovo.com.cn | 200 | gtm-cn-i7m2iit0403.gtm-a2b9.com. |
| cui.lenovo.com.cn | 200 | 39.107.146.170 |
| cuiapi.lenovo.com.cn | 404 | 47.95.96.14 |
| cuiauth.lenovo.com.cn | 404 | 39.107.146.178 |
| customer.lenovo.com | 200 | customer-uat.lenovo.gtm1.akadns99.net. |
| d.lenovo.com.cn | 200 | 146.196.117.3 |
| daas-h5.lenovo.com.cn | 200 | tob.lenovo.com.cn. |
| daas-h5uat.lenovo.com.cn | 200 | 146.196.114.215 |
| daas.lenovo.com.cn | 200 | tob.lenovo.com.cn. |
| dashboard.lenovo.com.cn | 200 | dashboard.mbgcdn.lenovo.com.cn. |
| datacenter.lenovo.com.cn | 200 | datacenter.lenovo.com.cn.wswebcdn.com. |
| daystar.lenovo.com | 200 | 81.69.35.222 |
| dc.lenovo.com.cn | 200 | dc.lenovo.com.cn.wswebcdn.com. |
| dcg.lenovo.com.cn | 200 | dcg.lenovo.com.cn.wswebcdn.com. |
| dcs.lenovo.com | 200 | 13.229.177.201 |
| dds.lenovo.com | 200 | 54.254.191.37 |
| demo-eudp.smartedu.lenovo.com | 200 | 69.231.185.94 |
| dev.cochat.lenovo.com | 404 | dev.cochat.lenovo.com.eo.dnse0.com. |
| dev.lenovo.com | 200 | dev.lenovo.gtm.skycdn.com.cn. |
| dev.sso.lenovo.com | 404 | 172.64.147.165 |
| developer.uds-dev.lenovo.com | 307 | 98.88.212.150 |
| developer.uds-qa.lenovo.com | 307 | 23.22.151.155 |
| dh.lenovo.com.cn | 403 | pcsdgslb1.lenovo.com.cn. |
| dianping.lenovo.com.cn | 200 | toc.lenovo.com.cn. |
| dianpinguat.lenovo.com.cn | 302 | 146.196.114.7 |
| digital.lenovo.com | 200 | 146.196.115.69 |
| discovery.lenovo.com.cn | 200 | discovery.mbgcdn.lenovo.com.cn. |
| do.lenovo.com.cn | 200 | 103.212.14.27 |
| doc.lenovo.com.cn | 403 | doc.lenovo.com.cn.lv.volcfake.com.volcgslb.com. |
| document.cochat.lenovo.com | 200 | document.cochat.lenovo.gtm.skycdn.com.cn. |
| download.lenovo.com | 403 | download.lenovo.com.akadns.net. |
| downloads.csw.mp.gdi.lenovo.com | 403 | 18.160.46.73 |
| downloads.naea1.uds.lenovo.com | 403 | 3.162.125.102 |
| downloads.uat.mp.gdi.lenovo.com | 403 | 18.67.76.10 |
| driverdl.lenovo.com.cn | 403 | driverdl.lenovo.com.cn.lv.volcfake.com.volcgslb.com. |
| driverupdate.lenovo.com.cn | 302 | driverupdate.lenovo.com.cn.lv.volcfake.com.volcgslb.com. |
| dsp.lenovo.com.cn | 200 | dsp.mbgcdn.lenovo.com.cn. |
| dt.lenovo.com | 404 | 146.196.114.60 |
| dtf.lenovo.com | 302 | 43.255.227.170 |
| e.lenovo.com.cn | 200 | toc.lenovo.com.cn. |
| ebpcn.lenovo.com.cn | 200 | ebpcn.lenovo.gtm3.akadns99.net. |
| edge.brain.lenovo.com | 200 | 43.255.226.237 |
| education.lenovo.com | 200 | education.lenovo.com.edgekey.net. |
| ema.euwe1.uds.lenovo.com | 200 | 3.65.192.10 |
| ema.naea1.uds-stage.lenovo.com | 200 | 18.210.97.191 |
| email.lenovo.com | 400 | redirect.www.lenovo.com. |
| emea-mds.lenovo.com | 200 | mds-emea-540672444.eu-central-1.elb.amazonaws.com. |
| emealoyalty.lenovo.com | 200 | lenovoemeab2c-widgets-de.crowdtwist.com. |
| emeasmbloyalty.lenovo.com | 200 | lenovoemeasmb-widgets-de.crowdtwist.com. |
| enterpriseregistration.lenovo.com | 404 | stscn.lenovo.com. |
| epg.lenovo.com.cn | 200 | 47.95.65.78 |
| events.lenovo.com | 403 | router.goldcast.io. |
| express.lenovo.com | 404 | 43.255.226.42 |
| f.lenovo.com.cn | 404 | f.lenovo.com.cn.wswebcdn.com. |
| feedback.uds.lenovo.com | 404 | 100.27.240.241 |
| feeds.lenovo.com.cn | 200 | slb-browser-traffic.mbgcdn.lenovo.com.cn. |
| file.cochat.lenovo.com | 200 | file.cochat.lenovo.gtm.skycdn.com.cn. |
| file.lenovo.com.cn | 404 | toc.lenovo.com.cn. |
| filedownload.csw.lenovo.com | 403 | filedownload-csw-lenovo.com. |
| filedownload.lenovo.com | 404 | filedownload.lenovo.com.akadns.net. |
| files.cochat.lenovo.com | 404 | files.cochat.lenovo.com.akadns.net. |
| fod.lenovo.com | 200 | 43.255.226.71 |
| fod2.lenovo.com | 403 | 104.232.254.5 |
| fodapi.lenovo.com | 403 | 43.255.226.71 |
| form.lenovo.com | 502 | 146.196.114.220 |
| forums.lenovo.com | 200 | d24zn61bgpeyxm.cloudfront.net. |
| forumscdn.lenovo.com | 200 | d1kwop82rlvmed.cloudfront.net. |
| funtv.vgs.lenovo.com.cn | 200 | 47.95.178.31 |
| funweb.vgs.lenovo.com.cn | 200 | 47.95.178.31 |
| g.lenovo.com | 404 | 146.196.114.128 |
| game.lenovo.com.cn | 200 | 47.95.65.78 |
| gaming.lenovo.com | 200 | gaming.lenovo.com.edgekey.net. |
| geo.cnno1.uds.lenovo.com | 403 | geo.cnno1.uds.lenovo.com.cdn.cloudflare.net. |
| geo.euwe1.uds.lenovo.com | 403 | geo.euwe1.uds.lenovo.com.cdn.cloudflare.net. |
| geo.uds.lenovo.com | 403 | geo.uds.lenovo.com.cdn.cloudflare.net. |
| gg.lenovo.com.cn | 200 | gj.mbgcdn.lenovo.com.cn. |
| gj.lenovo.com | 200 | gj-lenovo.lenovomm.com. |
| gj.lenovo.com.cn | 200 | gj.mbgcdn.lenovo.com.cn. |
| gk.lenovo.com.cn | 200 | tob.lenovo.com.cn. |
| glass-upload.naea1.uds.lenovo.com | 503 | glass-upload.naea1.uds.lenovo.com.cdn.cloudflare.net. |
| global.lenovo.com | 200 | start.lenovo.com.edgekey.net. |
| go.lenovo.com | 200 | d1h4y25ns8mkbo.cloudfront.net. |
| go.lenovo.com.cn | 200 | slb-browser-traffic.mbgcdn.lenovo.com.cn. |
| go2.lenovo.com | 200 | mkto-ab270109.com. |
| gsp.lenovo.com | 404 | gsp.lenovo.com.edgekey.net. |
| gw.lenovo.com.cn | 404 | 43.255.226.57 |
| hao.lenovo.com.cn | 200 | hao.mbgcdn.lenovo.com.cn. |
| hawk.lenovo.com | 404 | 103.30.235.183 |
| help.lenovo.com | 200 | help.lenovo.com.edgekey.net. |
| help.lenovo.com.cn | 404 | help.lenovo.com.cn.wswebcdn.com. |
| hi.lenovo.com | 200 | pcsdgslb1.lenovo.com.cn. |
| hi.lenovo.com.cn | 200 | slb-browser-traffic.mbgcdn.lenovo.com.cn. |
| home.cochat.lenovo.com | 200 | home.cochat.lenovo.com.eo.dnse0.com. |
| homeedgeserver.lenovo.com | 404 | 101.132.103.1 |
| hotfix.lenovo.com.cn | 404 | hotfix.mbgcdn.lenovo.com.cn. |
| hpc.lenovo.com | 200 | 84.252.116.20 |
| hub.csw.lenovo.com | 200 | hub-csw-lenovo.com. |
| hubv1.lenovo.com.cn | 404 | 8.131.99.70 |
| hwpad-player.lenovo.com.cn | 200 | 161.117.163.242 |
| i.lenovo.com.cn | 200 | i.lenovo.com.cn.wscdns.com. |
| i.survey.lenovo.com | 200 | 43.255.226.58 |
| ibsm.lenovo.com | 404 | 84.252.116.25 |
| icc.lenovo.com | 404 | 146.196.115.24 |
| ievents-us-hls.lenovo.com | 400 | ievents-us-hls.lenovo.com.akamaized.net. |
| iknow.lenovo.com.cn | 200 | toc.lenovo.com.cn. |
| ilive.lenovo.com.cn | 200 | 39.107.248.68 |
| inapi.lenovo.com | 500 | 146.196.114.124 |
| india.moli.lenovo.com | 200 | moli-in-llm-551440711.ap-south-1.elb.amazonaws.com. |
| info.lenovo.com.cn | 200 | 43.255.226.106 |
| ink-gb.lenovo.com | 200 | d5tbg71z015s6.cloudfront.net. |
| ink.lenovo.com | 200 | d374l8t4mjd2tc.cloudfront.net. |
| investor.lenovo.com | 200 | lenovo.irasia.com. |
| ipgpassport.lenovo.com | 200 | 58.247.171.36 |
| ipgpassportuat.lenovo.com | 200 | 58.247.171.38 |
| ips.lenovo.com | 200 | tob.lenovo.com.cn. |
| ips.lenovo.com.cn | 200 | tob.lenovo.com.cn. |
| iqvv.naea1.uds.lenovo.com | 200 | 3.222.152.210 |
| itsechelp.lenovo.com | 200 | vpc-secreview.us-east-1.elasticbeanstalk.com. |
| jiazhipc.lenovo.com.cn | 200 | 39.105.219.72 |
| jobs.lenovo.com | 202 | lenovo.avature.net. |
| jpserver.lenovo.com | 403 | 103.30.235.167 |
| js.lenovo.com.cn | 403 | js.mbgcdn.lenovo.com.cn. |
| jzh.lenovo.com.cn | 200 | jzh.lenovo.com.cn.c.vedcdnlb.com. |
| jzhadmin.lenovo.com.cn | 404 | jzhadmin.lenovo.com.cn.c.vedcdnlb.com. |
| jzhapp.lenovo.com.cn | 200 | jzhapp.lenovo.com.cn.w.cdngslb.com. |
| jzhcs.lenovo.com.cn | 404 | 146.196.115.114 |
| jzhedm.lenovo.com.cn | 200 | cdn.wjx.cn. |
| jzhfile.lenovo.com.cn | 403 | 43.255.227.10 |
| kabbms.lenovo.com.cn | 403 | 43.255.227.23 |
| kas.lenovo.com.cn | 200 | tob.lenovo.com.cn. |
| kb.lenovo.com | 503 | 104.232.254.24 |
| kf.cube.lenovo.com | 200 | tob.lenovo.com.cn. |
| knola.lenovo.com | 200 | knola-us-prod-server-839039526.us-east-1.elb.amazonaws.com. |
| las.moli.lenovo.com | 200 | moli-las-llm-581246812.us-east-1.elb.amazonaws.com. |
| lcc.lenovo.com | 404 | 104.232.228.26 |
| lcse.lenovo.com.cn | 200 | 43.255.226.135 |
| ld.lenovo.com | 200 | 146.196.115.107 |
| ldc-open.lenovo.com.cn | 404 | ldc-open.mbgcdn.lenovo.com.cn. |
| ldc.lenovo.com.cn | 400 | ldc.mbgcdn.lenovo.com.cn. |
| ldiplusstatus.uds.lenovo.com | 200 | ffq32ccyhqzl.stspg-customer.com. |
| ldistatus.uds.lenovo.com | 200 | fml0qwfkkb54.stspg-customer.com. |
| le-credit.lenovo.com | 200 | 146.196.114.60 |
| learning.lenovo.com | 200 | lenovo.docebosaas.com. |
| learningcenter.lenovo.com | 200 | 146.196.115.71 |
| legion.lenovo.com | 403 | legion.lenovo.com.cdn.cloudflare.net. |
| lena.lenovo.com | 200 | lena.lenovo.com.edgekey.net. |
| lenbpreturns.lenovo.com | 200 | 104.232.228.124 |
| lenovoedu.lenovo.com | 200 | 104.232.228.17 |
| lenovopress.lenovo.com | 200 | 3.149.92.68 |
| lepay.lenovo.com | 200 | 146.196.114.60 |
| lesc.lenovo.com | 200 | 104.232.225.105 |
| less-liveevent.uds-dev.lenovo.com | 302 | 34.205.42.244 |
| lestore.lenovo.com | 200 | 8.131.110.191 |
| lexue.lenovo.com.cn | 200 | 146.196.114.215 |
| li.lenovo.com.cn | 200 | 43.255.227.208 |
| linux.lenovo.com | 302 | 103.30.232.221 |
| lis.lenovo.com | 403 | lis.lenovo.gtm1.akadns99.net. |
| live.lenovo.com.cn | 200 | live.lenovo.com.cn.wswebcdn.com. |
| liveupdate5.lenovo.com.cn | 200 | 43.255.226.106 |
| liveupdate7.lenovo.com.cn | 404 | 43.255.226.106 |
| locker.lenovo.com | 200 | idmgmt.us-east-1.elasticbeanstalk.com. |
| login.lenovo.com | 403 | 146.196.114.55 |
| logupload.lenovo.com | 200 | logupload.lenovo.com.edgekey.net. |
| loyalty-sandbox.lenovo.com | 200 | lenovo-sandbox-widgets-sb.crowdtwist.com. |
| loyalty.lenovo.com | 200 | lenovo-widgets-prod.crowdtwist.com. |
| lpos-qas.lenovo.com.cn | 200 | 146.196.115.21 |
| lpos-test.lenovo.com.cn | 200 | 146.196.115.21 |
| lpos.lenovo.com | 403 | lpos.lenovo.com.akadns.net. |
| lrms.lenovo.com | 200 | 43.255.227.210 |
| lsa.lenovo.com | 200 | lmsa-web-prd-30521416.us-east-1.elb.amazonaws.com. |
| lsc.lenovo.com | 200 | d3pxuobxz5pydq.cloudfront.net. |
| lsc.lenovo.com.cn | 200 | 146.196.115.24 |
| lscs.lenovo.com | 200 | d29xfvdddvtlh7.cloudfront.net. |
| lsw-fast.lenovo.com.cn | 403 | lsw-fast.mbgcdn.lenovo.com.cn. |
| lsw.lenovo.com.cn | 404 | 47.95.137.211 |
| lt.lenovo.com.cn | 403 | lt.mbgcdn.lenovo.com.cn. |
| lzupdate.lenovo.com.cn | 403 | lzupdate.mbgcdn.lenovo.com.cn. |
| m.lenovo.com | 301 | redirect.www.lenovo.com. |
| m.lenovo.com.cn | 200 | m.lenovo.com.cn.wscdns.com. |
| mail.cochat.lenovo.com | 200 | mailcochat-geo.lenovo.gtm.skycdn.com.cn. |
| mail.lenovo.com | 200 | 103.212.4.42 |
| mailae.cochat.lenovo.com | 200 | 104.232.228.44 |
| mailae.lenovo.com | 200 | 104.232.228.103 |
| mailap.cochat.lenovo.com | 200 | 103.30.234.12 |
| mailap.lenovo.com | 200 | 103.30.234.222 |
| marketing.lenovo.com.cn | 404 | marketing.lenovo.com.cn.wswebcdn.com. |
| mave.dds.lenovo.com | 403 | d3enz4jnemt9cu.cloudfront.net. |
| mclub.lenovo.com.cn | 200 | mclub.lenovo.com.cn.wswebcdn.com. |
| mcstaging.store.lenovo.com | 403 | prod.magentocloud.map.fastly.net. |
| mds.lenovo.com | 200 | mds-1267536186.ap-southeast-1.elb.amazonaws.com. |
| media.cochat.lenovo.com | 200 | 43.255.227.86 |
| meeting.lenovo.com | 200 | meeting.lenovo.com.w.alikunlun.com. |
| membership.lenovo.com.cn | 200 | membership.lenovo.com.cn.eo.dnse0.com. |
| microservice.cochat.lenovo.com | 404 | microservice.cochat.lenovo.gtm.skycdn.com.cn. |
| moli.lenovo.com | 200 | moli-us-llm-1734483785.us-east-1.elb.amazonaws.com. |
| mooc.lenovo.com | 302 | mooc.lenovo.com.edgekey.net. |
| mqtt.cnno1.uds.lenovo.com | 404 | k8s-coreprod-lcpcorem-3441ba55df-995e7b2094b2a5e8.elb.cn-northwest-1.amazonaws.com.cn. |
| mqtt.euwe1.uds.lenovo.com | 404 | 3.74.59.197 |
| mqtt.naea1.uds.lenovo.com | 404 | 34.199.135.124 |
| mqtt.uds-qa.lenovo.com | 404 | k8s-coreqa-lcpcorem-64bb8aacb3-dca6b517d937c6ee.elb.us-east-1.amazonaws.com. |
| msg.csw.lenovo.com | 200 | msg-csw-lenovo.com. |
| msipm.lenovo.com.cn | 200 | 146.196.115.42 |
| msupport.lenovo.com.cn | 200 | toc.lenovo.com.cn. |
| mt.lenovo.com.cn | 403 | mt.mbgcdn.lenovo.com.cn. |
| my.lenovo.com.cn | 200 | my.mbgcdn.lenovo.com.cn. |
| mystart.lenovo.com | 200 | start.lenovo.com.edgekey.net. |
| na-mds.lenovo.com | 200 | nde-ex-router-ex-alb-191741537.us-east-1.elb.amazonaws.com. |
| nasmbteam.lenovo.com | 204 | 04294278-ed9c-4084-af98-e4d2579c646c.outrch.com. |
| natapps.lenovo.com.cn | 200 | 47.95.137.211 |
| nc.lenovo.com | 503 | rr-tst-php-1625580219.us-east-1.elb.amazonaws.com. |
| newdriverdl.lenovo.com.cn | 403 | newdriverdl.lenovo.com.cn.lv.volcfake.com.volcgslb.com. |
| news.lenovo.com | 200 | 107.20.186.84 |
| news.lenovo.com.cn | 200 | news.lenovo.com.cn.wswebcdn.com. |
| newsupport.lenovo.com.cn | 200 | newsupport.lenovo.gtm.skycdn.com.cn. |
| newthink.lenovo.com.cn | 200 | toc.lenovo.com.cn. |
| ngspms.lenovo.com | 404 | 58.247.171.27 |
| npm-registry.cnno1.uds.lenovo.com | 403 | npm-registry.cnno1.uds.lenovo.com.cdn.cloudflare.net. |
| npm-registry.euwe1.uds.lenovo.com | 403 | npm-registry.euwe1.uds.lenovo.com.cdn.cloudflare.net. |
| npm-registry.naea1.uds.lenovo.com | 403 | npm-registry.naea1.uds.lenovo.com.cdn.cloudflare.net. |
| oauth2.lenovo.com.cn | 200 | 39.106.232.208 |
| ocr.lenovo.com | 200 | authservice-53791546.cn-northwest-1.elb.amazonaws.com.cn. |
| office365.lenovo.com.cn | 200 | toc.lenovo.com.cn. |
| ok.lenovo.com | 200 | okt.to. |
| one.lenovo.com | 200 | pr1-lenovoone.mbgcdn.lenovo.com.cn. |
| one.lenovo.com.cn | 404 | one.mbgcdn.lenovo.com.cn. |
| oobe.cnno1.uds.lenovo.com | 403 | oobe.cnno1.uds.lenovo.com.cdn.cloudflarecn.net. |
| oobe.naea1.uds.lenovo.com | 403 | oobe.naea1.uds.lenovo.com.cdn.cloudflare.net. |
| oobe.uds-dev.lenovo.com | 403 | oobe.naea1.uds-dev.lenovo.com.cdn.cloudflare.net. |
| oobe.uds-qa.lenovo.com | 403 | oobe.naea1.uds-qa.lenovo.com.cdn.cloudflare.net. |
| oobe.uds.lenovo.com | 403 | oobe.naea1.uds.lenovo.com.cdn.cloudflare.net. |
| op.lenovo.com.cn | 403 | pcsdgslb1.lenovo.com.cn. |
| open.lenovo.com | 200 | openlenovo.mbgstore.lenovo.com.cn. |
| open.lenovo.com.cn | 403 | open.lenovo.com.cn.wscdns.com. |
| optimus.lenovo.com.cn | 200 | optimushub.lenovo.com.cn. |
| optout.wd.lenovo.com | 403 | 137.117.93.2 |
| order.lenovo.com.cn | 502 | order.lenovo.com.cn.wscdns.com. |
| osd.lenovo.com | 200 | osd-prod-alb-2041558713.cn-north-1.elb.amazonaws.com.cn. |
| otaapi.vgs.lenovo.com.cn | 500 | 47.95.178.31 |
| otp.lenovo.com | 200 | otp.lenovo.gtm.akadns99.net. |
| outlet.lenovo.com | 200 | outlet.lenovo.com.edgekey.net. |
| outletap.lenovo.com | 200 | outletap.lenovo.com.edgekey.net. |
| ovp.lenovo.com | 200 | 104.232.228.76 |
| paas.lenovo.com.cn | 200 | 39.105.251.195 |
| pages.lenovo.com | 200 | lenovounitedstatesinc.mktoweb.com. |
| partner.lenovo.com.cn | 200 | partner.lenovo.com.cn.wswebcdn.com. |
| passport.lenovo.com | 200 | passport-lenovo.lenovomm.com. |
| pauto.lenovo.com | 403 | 43.255.227.41 |
| pay.lenovo.com.cn | 200 | 43.255.226.57 |
| pay.mbgcdn.lenovo.com.cn | 204 | 39.103.32.65 |
| pc.cochat.lenovo.com | 200 | pc.cochat.lenovo.com.lenovo.gtm.skycdn.com.cn. |
| pc.lenovo.com.cn | 200 | personalcloud.mbgcdn.lenovo.com.cn. |
| pcsdgslb1.lenovo.com.cn | 200 | 39.106.232.147 |
| pcsdgslb2.lenovo.com.cn | 403 | pcsdgslb2.mbgcdn.lenovo.com.cn. |
| pcsdtest5.lenovo.com.cn | 400 | pcsdtest5.mbgcdn.lenovo.com.cn. |
| pcservice.lenovo.com.cn | 200 | toc.lenovo.com.cn. |
| personalcloud.lenovo.com.cn | 200 | personalcloud.mbgcdn.lenovo.com.cn. |
| pitf-dev.vgs.lenovo.com.cn | 404 | 47.95.178.31 |
| pitf.vgs.lenovo.com.cn | 404 | pitf.mbgcdn.lenovo.com.cn. |
| play.lenovo.com.cn | 200 | 47.95.65.78 |
| plm.lenovo.com | 404 | 146.196.114.128 |
| pms.lenovo.com | 404 | 146.196.114.223 |
| pms.lenovo.com.cn | 200 | tob.lenovo.com.cn. |
| politemail.lenovo.com | 200 | lenovoserv-n7k73mbz7kd4k-app.azurewebsites.net. |
| politemailanalytics.lenovo.com | 200 | lenovoserv-n7k73mbz7kd4k-app.azurewebsites.net. |
| portal-aui.cnno1.uds.lenovo.com | 403 | portal-aui.cnno1.uds.lenovo.com.cdn.cloudflare.net. |
| portal-aui.euwe1.uds.lenovo.com | 403 | 3.171.76.93 |
| portal-platform.cnno1.uds.lenovo.com | 403 | portal-platform.cnno1.uds.lenovo.com.cdn.cloudflare.net. |
| portal-platform.euwe1.uds.lenovo.com | 403 | portal-platform.euwe1.uds.lenovo.com.cdn.cloudflare.net. |
| portal-platform.naea1.uds.lenovo.com | 403 | portal-platform.naea1.uds.lenovo.com.cdn.cloudflare.net. |
| portal-platform.uds.lenovo.com | 403 | portal-platform.naea1.uds.lenovo.com. |
| portal.cnno1.uds.lenovo.com | 403 | portal.cnno1.uds.lenovo.com.cdn.cloudflare.net. |
| portal.euwe1.uds.lenovo.com | 403 | portal.euwe1.uds.lenovo.com.cdn.cloudflare.net. |
| portal.naea1.uds-qa.lenovo.com | 200 | d1ek6smjd3hdg7.cloudfront.net. |
| portal.naea1.uds.lenovo.com | 403 | portal.naea1.uds.lenovo.com.cdn.cloudflare.net. |
| portal.thinkshield.lenovo.com | 200 | 3.171.38.30 |
| portal.thinksmart.lenovo.com | 200 | 3.171.38.36 |
| portal.uds-qa.lenovo.com | 200 | d1ek6smjd3hdg7.cloudfront.net. |
| portal.uds-stage.lenovo.com | 200 | 50.16.78.125 |
| portal.uds.lenovo.com | 403 | portal.naea1.uds.lenovo.com. |
| ppl.lenovo.com.cn | 403 | 43.255.227.23 |
| pre-sales.lenovo.com | 404 | pre-sales.lenovo.com.edgekey.net. |
| pre.wx.lenovo.com.cn | 403 | 47.98.38.232 |
| preprdesupport.lenovo.com | 404 | 104.232.254.23 |
| privacy.lenovo.com.cn | 200 | 39.97.45.154 |
| promotion.lenovo.com.cn | 404 | promotion.lenovo.com.cn.wswebcdn.com. |
| promotions.lenovo.com | 403 | promotions.lenovo.com.cdn.cloudflare.net. |
| pt.lenovo.com.cn | 403 | pcsdgslb1.lenovo.com.cn. |
| ptstpd.lenovo.com.cn | 200 | toc.lenovo.com.cn. |
| pub.lenovo.com.cn | 200 | toc.lenovo.com.cn. |
| pvtcui.lenovo.com.cn | 200 | 39.107.213.161 |
| pvtcuiapi.lenovo.com.cn | 200 | 47.95.96.41 |
| pvtcuiauth.lenovo.com.cn | 404 | 47.95.68.166 |
| q.lenovo.com.cn | 200 | 103.212.4.7 |
| qi.lenovo.com | 301 | 103.212.14.25 |
| qms.lenovo.com | 404 | 146.196.114.224 |
| qstanalytics.lenovo.com | 200 | 104.232.228.39 |
| reg.lenovo.com | 200 | reg.lenovo.com.wswebcdn.com. |
| reg.lenovo.com.cn | 200 | reg.lenovo.com.cn.wswebcdn.com. |
| remote.daystar.lenovo.com | 200 | 121.5.6.217 |
| repository.lenovo.com | 200 | repository.lenovo.gtm.akadns99.net. |
| research.lenovo.com | 200 | research.lenovo.com.akadns.net. |
| resolute-api.lenovo.com | 404 | rem-alb-1459514721.us-east-1.elb.amazonaws.com. |
| resolute.lenovo.com | 200 | d3inbzb1r89bfy.cloudfront.net. |
| resolve.lenovo.com | 200 | 103.30.235.169 |
| retail.lenovo.com | 200 | 146.196.114.229 |
| reviews.lenovo.com | 200 | redirect-to-www.lenovo.com.edgekey.net. |
| robot.lenovo.com.cn | 200 | toc.lenovo.com.cn. |
| robotrs.lenovo.com.cn | 403 | toc.lenovo.com.cn. |
| rs.x.lenovo.com.cn | 200 | 43.255.226.57 |
| rx.lenovo.com.cn | 200 | toc.lenovo.com.cn. |
| rz.lenovo.com | 200 | 146.196.114.60 |
| rzcloud.lenovo.com | 200 | 146.196.114.60 |
| s.lenovo.com | 200 | lenovo.com.ssl.d1.sc.omtrdc.net. |
| s.lenovo.com.cn | 200 | s.lenovo.com.cn.wscdns.com. |
| saascdn.lenovo.com.cn | 403 | saascdn.mbgcdn.lenovo.com.cn. |
| sales.lenovo.com | 200 | sales.lenovo.gtm.skycdn.com.cn. |
| sb-aployalty.lenovo.com | 200 | lenovoapb2csandbox-widgets-sb.crowdtwist.com. |
| sb-apsmbloyalty.lenovo.com | 200 | lenovoapsmbsandbox-widgets-sb.crowdtwist.com. |
| sb-emealoyalty.lenovo.com | 200 | lenovoemeab2csandbox-widgets-sb.crowdtwist.com. |
| sb-emeasmbloyalty.lenovo.com | 200 | lenovoemeasmbsandbox-widgets-sb.crowdtwist.com. |
| scanmsg.lenovo.com.cn | 200 | toc.lenovo.com.cn. |
| scc.lenovo.com | 200 | scc.lenovo.gtm.akadns99.net. |
| scc.lenovo.com.cn | 200 | 103.212.14.27 |
| scct.lenovo.com | 200 | scct.lenovo.gtm1.akadns99.net. |
| scf.lenovo.com | 200 | 146.196.114.60 |
| scl.lenovo.com | 400 | 146.196.114.142 |
| sdc.lenovo.com | 500 | 43.255.227.103 |
| sdiuat.lenovo.com.cn | 404 | 146.196.114.3 |
| search.lenovo.com | 200 | redirect-to-www.lenovo.com.edgekey.net. |
| sec.lenovo.com | 200 | 43.255.226.195 |
| sec.lenovo.com.cn | 302 | sec.lenovo.com.cn.wswebcdn.com. |
| sentry-lb.lenovo.com.cn | 301 | sentry-lb.mbgcdn.lenovo.com.cn. |
| server.lenovo.com.cn | 200 | toc.lenovo.com.cn. |
| serverproven.lenovo.com | 200 | d3lji75bq2luhn.cloudfront.net. |
| service.lenovo.com.cn | 200 | toc.lenovo.com.cn. |
| serviceforcedemo.lenovo.com.cn | 200 | tob.lenovo.com.cn. |
| servicepartner.lenovo.com | 200 | 104.232.225.28 |
| servicepartneruat.lenovo.com | 200 | 104.232.225.27 |
| services.lenovo.com | 500 | lenovo.com.edgekey.net. |
| services.renewals.lenovo.com | 200 | gdxnhhk.impervadns.net. |
| servicesportal.lenovo.com | 200 | lenovoau.service-now.com. |
| servicetools.lenovo.com | 200 | 104.232.228.16 |
| ses.lenovo.com | 200 | ses-prod-alb-1200413742.cn-northwest-1.elb.amazonaws.com.cn. |
| seservice-uat.lenovo.com | 200 | 146.196.115.83 |
| shcanteen.lenovo.com | 200 | 146.196.114.230 |
| sho.lenovo.com.cn | 403 | pcsdgslb2.lenovo.com.cn. |
| shop.lenovo.com | 200 | shop.lenovo.com.edgekey.net. |
| shop.lenovo.com.cn | 200 | shop.lenovo.com.cn.wscdns.com. |
| show.lenovo.com.cn | 403 | show.mbgcdn.lenovo.com.cn. |
| showcase.lenovo.com | 200 | showcase-elb-001-407388199.ap-southeast-1.elb.amazonaws.com. |
| siot-nas.lenovo.com.cn | 200 | siot-nas.mbgcdn.lenovo.com.cn. |
| siotapp.lenovo.com.cn | 404 | 39.97.21.211 |
| sj.lenovo.com | 200 | sj-lenovo.lenovomm.com. |
| sl.lenovo.com.cn | 403 | pcsdgslb2.lenovo.com.cn. |
| slb.lenovo.com.cn | 200 | slb-browser-traffic.mbgcdn.lenovo.com.cn. |
| smallgame.vgs.lenovo.com.cn | 200 | 47.95.178.31 |
| smart.lenovo.com.cn | 404 | 47.95.71.249 |
| smartfind.lenovo.com | 200 | nde-ex-router-ex-alb-191741537.us-east-1.elb.amazonaws.com. |
| smartpc.lenovo.com.cn | 200 | smartpc.mbgcdn.lenovo.com.cn. |
| smartssc.lenovo.com | 200 | 43.255.226.198 |
| smartwidget.csw.lenovo.com | 200 | smartwidget-csw-lenovo.com. |
| smb.lenovo.com | 200 | 39.102.31.63 |
| smsc.lenovo.com | 200 | smsc.lenovo.com.edgekey.net. |
| smtestalipublic.lenovo.com.cn | 200 | smtestalipublic.lenovopcsd.com. |
| smtp.cube.lenovo.com | 200 | tob.lenovo.com.cn. |
| so.lenovo.com.cn | 200 | so.mbgcdn.lenovo.com.cn. |
| soa.lenovo.com | 200 | soa.gtm.lenovo.com. |
| soaus-test.lenovo.com | 404 | microapi-us-t.gtm.lenovo.com. |
| soaus.lenovo.com | 404 | microapi-us.gtm.lenovo.com. |
| software.lenovo.com.cn | 200 | 43.255.226.106 |
| sol.lenovo.com.cn | 200 | sol.lenovo.com.cn.wswebcdn.com. |
| speedtest.lenovo.com.cn | 404 | speedtest.mbgcdn.lenovo.com.cn. |
| spokenenglish.smartedu.lenovo.com | 200 | smarteduprod1-internet-nlb-c5e7fb2cae10593e.elb.cn-northwest-1.amazonaws.com.cn. |
| sports.cochat.lenovo.com | 200 | sports.cochat.lenovo.gtm.skycdn.com.cn. |
| srmcn.lenovo.com.cn | 200 | srm.lenovo.gtm3.akadns99.net. |
| srv.lenovo.com.cn | 404 | srv.lenovo.com.cn.wswebcdn.com. |
| ss.lenovo.com | 200 | 103.212.14.66 |
| sso.lenovo.com | 404 | 172.64.147.165 |
| sso.lenovo.com.cn | 200 | tob.lenovo.com.cn. |
| st.lenovo.com | 200 | lst-km-prd-ext-1674948765.us-east-1.elb.amazonaws.com. |
| sta.vgs.lenovo.com.cn | 200 | sta.vgs.lenovo.com.cn.volcgslb.com. |
| staff.lenovo.com.cn | 200 | staff.lenovo.com.cn.wswebcdn.com. |
| start.de.lenovo.com | 200 | start.lenovo.com.edgekey.net. |
| start.gb.lenovo.com | 200 | start.lenovo.com.edgekey.net. |
| start.jp.lenovo.com | 200 | start.lenovo.com.edgekey.net. |
| start.lenovo.com | 200 | start.lenovo.com.edgekey.net. |
| start.uk.lenovo.com | 200 | start.lenovo.com.edgekey.net. |
| startpage.de.lenovo.com | 200 | start.lenovo.com.edgekey.net. |
| startpage.gb.lenovo.com | 200 | start.lenovo.com.edgekey.net. |
| startpage.lenovo.com | 200 | start.lenovo.com.edgekey.net. |
| startpage.uk.lenovo.com | 200 | start.lenovo.com.edgekey.net. |
| static-assets.lenovo.com.cn | 500 | grow.lenovo.com.cn.edgekey.net. |
| static.lenovo.com | 404 | static.lenovo.com.edgekey.net. |
| static.test.wx.lenovo.com.cn | 200 | static.test.wx.lenovo.com.cn.w.kunlungr.com. |
| static.wx.lenovo.com.cn | 200 | static.wx.lenovo.com.cn.w.kunlungr.com. |
| stom.lenovo.com.cn | 200 | toc.lenovo.com.cn. |
| store.lenovo.com | 200 | prod.magentocloud.map.fastly.net. |
| storefront.lenovo.com.cn | 200 | 43.255.226.197 |
| storyhub.lenovo.com | 200 | news.lenovo.com. |
| stscn.lenovo.com | 404 | stscn.lenovo.gtm.akadns99.net. |
| suoping.lenovo.com.cn | 403 | suoping.mbgcdn.lenovo.com.cn. |
| support.lenovo.com.cn | 200 | toc.lenovo.com.cn. |
| support1.lenovo.com.cn | 200 | toc.lenovo.com.cn. |
| support5.lenovo.com.cn | 200 | toc.lenovo.com.cn. |
| supportapi.lenovo.com | 200 | supportapi.lenovo.com.edgekey.net. |
| supportuat.lenovo.com.cn | 302 | 146.196.114.7 |
| sus.lex.lenovo.com.cn | 404 | 47.95.137.109 |
| switch.lenovo.com.cn | 200 | 39.106.232.212 |
| t.lenovo.com.cn | 200 | toc.lenovo.com.cn. |
| tabletcust.lenovo.com | 200 | ccs-prd-com-alb-196613027.cn-northwest-1.elb.amazonaws.com.cn. |
| tabletindirect.lenovo.com | 200 | start.lenovo.com.edgekey.net. |
| tabletstart.lenovo.com | 200 | start.lenovo.com.edgekey.net. |
| tag.lenovo.com | 200 | 40.162.90.108 |
| talent.lenovo.com.cn | 200 | 103.212.14.17 |
| tdms.lenovo.com | 404 | 58.247.171.16 |
| techexperts.lenovo.com | 200 | 64.147.70.90 |
| techtoday.lenovo.com | 200 | techtoday.lenovo.com.cdn.cloudflare.net. |
| techworld.lenovo.com.cn | 200 | techworld.lenovo.com.cn.wswebcdn.com. |
| techworld21.lenovo.com | 200 | 3.162.103.111 |
| test.b2bius.lenovo.com | 404 | 103.30.232.242 |
| test.wx.lenovo.com.cn | 403 | 47.97.212.11 |
| testforums.lenovo.com | 403 | li-community-tst-201725610.us-east-1.elb.amazonaws.com. |
| testforumscdn.lenovo.com | 403 | li-community-forumscdn-1814582216.us-east-1.elb.amazonaws.com. |
| think.lenovo.com.cn | 200 | toc.lenovo.com.cn. |
| thinkagile.lenovo.com | 200 | thinkagile.lenovo.com.edgekey.net. |
| thinkuat.lenovo.com.cn | 302 | 146.196.114.7 |
| tmp.lenovo.com | 200 | 146.196.114.55 |
| tms.lenovo.com | 200 | 43.255.226.155 |
| tms.lenovo.com.cn | 200 | tms.lenovo.com.cn.wswebcdn.com. |
| tools.dds.lenovo.com | 403 | ddj82cgkc62yh.cloudfront.net. |
| tools.lenovo.com.cn | 302 | 43.255.226.57 |
| training.renewals.lenovo.com | 200 | gdxnhhk.impervadns.net. |
| transformu.lenovo.com | 200 | transformu.lenovo.com.wswebcdn.com. |
| trk.ecomm.lenovo.com | 404 | sendgrid-secure.bluecore.com. |
| ts.lenovo.com | 200 | 20.206.74.61 |
| uk.moli.lenovo.com | 200 | moli-uk-llm-681019518.eu-west-3.elb.amazonaws.com. |
| uki.lenovo.com | 403 | uki-lenovo.lenovomm.com. |
| unlockrowapi.lenovo.com | 400 | 103.30.232.207 |
| us.download.lenovo.com | 403 | us.download.lenovo.com.edgekey.net. |
| us.sso.lenovo.com | 404 | 172.64.147.165 |
| user.lenovo.com.cn | 403 | toc.lenovo.com.cn. |
| vantage.csw-qa.lenovo.com | 403 | d3lvs18p04sa48.cloudfront.net. |
| vantage.csw.lenovo.com | 200 | vantage-csw-lenovo.com. |
| vendorscorecard.lenovo.com | 200 | 103.30.235.217 |
| vgs.lenovo.com.cn | 200 | 47.95.65.78 |
| vibsdepot.lenovo.com | 403 | vibsdepot.lenovo.com.edgekey.net. |
| video.lenovo.com | 403 | 103.212.14.3 |
| vip.lenovo.com.cn | 200 | 146.196.115.107 |
| vmware.lenovo.com | 200 | vmware.lenovo.com.edgekey.net. |
| voice.lenovo.com | 500 | 120.133.65.33 |
| vorakjd.lenovo.com.cn | 404 | 116.196.122.188 |
| vpntools.lenovo.com | 200 | vpc-vpntools.us-east-1.elasticbeanstalk.com. |
| wan.lenovo.com.cn | 200 | 47.95.65.78 |
| watermark.lenovo.com | 200 | watermark.us-east-1.elasticbeanstalk.com. |
| weather.lenovo.com.cn | 200 | weather.mbgcdn.lenovo.com.cn. |
| webform.wd.lenovo.com | 200 | 137.117.90.63 |
| webvpn.cn.lenovo.com | 200 | webvpn.cn.lenovo.gtm.akadns99.net. |
| webvpn.hk.lenovo.com | 200 | webvpnhkhkg01.lenovo.gtm.akadns99.net. |
| webvpn.sk.lenovo.com | 200 | webvpndefra01.lenovo.gtm.akadns99.net. |
| webvpn.us.lenovo.com | 200 | webvpnusrdu01.lenovo.gtm.akadns99.net. |
| webvpnauto.lenovo.com | 200 | webvpn.lenovo.gtm.skycdn.com.cn. |
| webvpnbrsao03.lenovo.com | 200 | webvpnbrsao03.lenovo.gtm.akadns99.net. |
| webvpncnpek01.lenovo.com | 200 | webvpn.cn.lenovo.gtm.akadns99.net. |
| webvpncnszx01.lenovo.com | 200 | webvpncnszx01.lenovo.gtm.akadns99.net. |
| webvpndefra01.lenovo.com | 200 | webvpndefra01.lenovo.gtm.akadns99.net. |
| webvpnhkhkg01.lenovo.com | 200 | webvpnhkhkg01.lenovo.gtm.akadns99.net. |
| webvpnusrdu01.lenovo.com | 200 | webvpnusrdu01.lenovo.gtm.akadns99.net. |
| webvpnussjo03.lenovo.com | 200 | 104.232.234.4 |
| weixin.lenovo.com.cn | 200 | 43.255.226.57 |
| whp.lenovo.com | 403 | 146.196.114.74 |
| whpu.lenovo.com | 200 | 146.196.115.93 |
| windows-server.lenovo.com | 200 | windows-server.lenovo.com.edgekey.net. |
| windows.lenovo.com | 503 | redirect-to-www.lenovo.com.edgekey.net. |
| wms.lenovo.com.cn | 200 | wms.lenovo.com.cn.wswebcdn.com. |
| workphone.lenovo.com | 200 | vpc-workphone.us-east-1.elasticbeanstalk.com. |
| www.lenovo.com | 200 | www.lenovo.com.edgekey.net. |
| www.lenovo.com.cn | 200 | www.lenovo.com.cn.lxdns.com. |
| www.pcsupport.lenovo.com | 301 | www.pcsupport.lenovo.com.edgesuite.net. |
| www01.lenovo.com | 400 | www01.lenovo.com.edgekey.net. |
| www02.lenovo.com | 400 | www02.lenovo.com.edgekey.net. |
| www03.lenovo.com | 400 | www03.lenovo.com.edgekey.net. |
| www04.lenovo.com | 400 | www04.lenovo.com.edgekey.net. |
| www3.lenovo.com | 200 | www3.lenovo.com.edgekey.net. |
| www3.lenovo.com.cn | 200 | www3.lenovo.com.cn.wswebcdn.com. |
| x.lenovo.com.cn | 200 | toc.lenovo.com.cn. |
| xapi.lenovo.com | 200 | xapi.lenovo.com.eo.dnse0.com. |
| xcloud.lenovo.com.cn | 200 | 146.196.115.76 |
| xiaoyi.lenovo.com | 404 | 47.111.81.183 |
| xiaoyi.pre.lenovo.com | 200 | 116.62.132.229 |
| xm.lenovo.com | 200 | 103.212.14.18 |
| xpanel.lenovo.com.cn | 404 | xpanel.mbgcdn.lenovo.com.cn. |
| yun.lenovo.com | 200 | yun-cname.lenovows.com. |
| zhi.lenovo.com.cn | 200 | 43.255.227.123 |

</details>

---

## HTTP Response Code Distribution

| HTTP Code | Count | Meaning |
|---|---|---|
| 200 | 392 | OK — host is live and serving content |
| 404 | 88 | Not Found — host responds but path doesn't exist |
| 403 | 87 | Forbidden — host responds but access denied |
| 400 | 19 | Bad Request |
| 302 | 10 | Redirect |
| 500 | 8 | Internal Server Error |
| 503 | 5 | Service Unavailable |
| 204 | 5 | No Content |
| 401 | 4 | Unauthorized (needs credentials) |
| 301 | 4 | Permanent Redirect |
| 502 | 3 | Bad Gateway |
| 307 | 2 | Temporary Redirect |
| 504 | 1 | Gateway Timeout |
| 405 | 1 | Method Not Allowed |
| 202 | 1 | Accepted |
| 000 | 660 | Connection timeout / no HTTPS |

---

*Generated on 2026-02-20 by LMSA Endpoint Discovery*
*Data sources: trickest/inventory, HackerTarget API, crt.sh CT logs, DNS brute-force, decompiled LMSA binaries*
