namespace lenovo.mbg.service.common.webservices;

public static class WebApiUrl
{
    private static readonly string ServiceInterfaceUrl = "https://lsa.lenovo.com/Interface";

    private static readonly string BaseHttpUrl = "https://lsa.lenovo.com/lmsa-web";

    public static string GET_PUBLIC_KEY = ServiceInterfaceUrl + "/common/rsa.jhtml";

    public static string INIT_TOKEN = ServiceInterfaceUrl + "/client/initToken.jhtml";

    public static string GET_DEVICE_INFO = ServiceInterfaceUrl + "/device/getDeviceInfo.jhtml";

    public static string GET_DEVICE_ICON = ServiceInterfaceUrl + "/device/getDeviceIcon.jhtml";

    public static string POST_UPGRADE_FLASH_INFO = ServiceInterfaceUrl + "/dataCollection/UpgradeFlashInfo.jhtml";

    public static string DISPOSE_TOKEN = ServiceInterfaceUrl + "/client/deleteToken.jhtml";

    public static string USER_GUIDE = ServiceInterfaceUrl + "/client/getUserGuide.jhtml";

    public static string HELP_URI = ServiceInterfaceUrl + "/client/clientHelp.jhtml";

    public static string CLIENT_VERSION = ServiceInterfaceUrl + "/client/getNextUpdateClient.jhtml";

    public static string UPDATE_VERSION = ServiceInterfaceUrl + "/client/getPluginCategoryList.jhtml";

    public static string PLUGIN_VERSION = ServiceInterfaceUrl + "/client/getClientPlugins.jhtml";

    public static string SHOW_FEEDBACK = BaseHttpUrl + "/Tips/feedback.html";

    public static string Webwervice_Get_RomResources = ServiceInterfaceUrl + "/priv/getRomList.jhtml";

    public static string SURVEY_URL = ServiceInterfaceUrl + "/survey/getIsNeedTrigger.jhtml";

    public static string UPLOAD_USER_DEVICE = ServiceInterfaceUrl + "/registeredModel/addModels.jhtml";

    public static string DELETE_USER_DEVICE = ServiceInterfaceUrl + "/registeredModel/models.jhtml";

    public static string LENOVOID_LOGIN_CALLBACK = ServiceInterfaceUrl + "/user/lenovoIdLogin.jhtml";

    public static string UPDATE_DOWNLOAD_URL = ServiceInterfaceUrl + "/client/renewFileLink.jhtml";

    public static string NOTICE_URL = ServiceInterfaceUrl + "/notice/getNoticeInfo.jhtml";

    public static string NOTICE_BROADCAST_URL = ServiceInterfaceUrl + "/notice/getBroadcast.jhtml";

    public static string GET_UPGRADEFLASH_MATCH_TYPES = ServiceInterfaceUrl + "/rescueDevice/getParamType.jhtml";

    public static string RESUCE_AUTOMATCH_GETPARAMS_MAPPING = ServiceInterfaceUrl + "/rescueDevice/getRomMatchParams.jhtml";

    public static string RESUCE_AUTOMATCH_GETROM = ServiceInterfaceUrl + "/rescueDevice/getNewResource.jhtml";

    public static string RESUCE_CHECK_SUPPORT_FASTBOOT_MODE = ServiceInterfaceUrl + "/rescueDevice/getMarketSupport.jhtml";

    public static string RESUCE_CHECK_MODEL_NAME_DRIVERS = ServiceInterfaceUrl + "/model/getDriverSpecialConfig.jhtml";

    public static string LOAD_YOUTUBE_INFO = ServiceInterfaceUrl + "/model/getYoutubeVideo.jhtml";

    public static string GET_MOLI_REQUEST_URL = ServiceInterfaceUrl + "/moli/getMoliUrl.jhtml";

    public static string MODEL_READ_CONFIG = ServiceInterfaceUrl + "/rescueDevice/modelReadConfigration.jhtml";

    public static string NETWORK_CONNECT_CHECK = "https://lsa.lenovo.com/lmsa-web/index.jsp";

    public static string TOKEN_URL = string.Empty;

    public static string LOGIN_TOKEN = string.Empty;

    public static string LOAD_SMART_DEVICE = ServiceInterfaceUrl + "/rescueDevice/smartMarketNames.jhtml";

    public static string LOAD_WARRANTY_BANNER = ServiceInterfaceUrl + "/client/motoCare.jhtml";

    public static string LOAD_COUPON = ServiceInterfaceUrl + "/client/discountCoupon.jhtml";

    public static string CALL_API_URL = ServiceInterfaceUrl + "/dictionary/getApiInfo.jhtml";

    public static string CALL_B2B_ORDERS_URL = ServiceInterfaceUrl + "/vip/getB2BInfo.jhtml";

    public static string CALL_B2B_ACTIVE_ORDERS_URL = ServiceInterfaceUrl + "/vip/getActiveB2BInfos.jhtml";

    public static string CALL_B2B_QUERY_ORDER_URL = ServiceInterfaceUrl + "/vip/getEnableB2BOrder.jhtml";

    public static string CALL_B2B_GET_ORDERID_URL = ServiceInterfaceUrl + "/vip/getOrderNum.jhtml";

    public static string CALL_B2B_ORDER_BUY_URL = ServiceInterfaceUrl + "/vip/buy.jhtml";

    public static string CALL_B2B_GET_PRICE_URL = ServiceInterfaceUrl + "/vip/card.jhtml";

    public static string ROMFILE_CHECK_RULES = ServiceInterfaceUrl + "/model/rules.jhtml";

    public static string GET_SUPPORT_FASTBOOT_BY_MODELNAME = ServiceInterfaceUrl + "/model/isReadSupport.jhtml";

    public static string GET_MUTIL_TUTORIALS_QUESTIONS = ServiceInterfaceUrl + "/guide/getGuideQuestion.jhtml";

    public static string USER_LOGOUT = ServiceInterfaceUrl + "/user/logout.jhtml";

    public static string USER_FORGOT_PASSWORD = ServiceInterfaceUrl + "/user/forgotPassword.jhtml";

    public static string USER_CHANGE_PASSWORD = ServiceInterfaceUrl + "/user/changePassword.jhtml";

    public static string USER_LOGIN = ServiceInterfaceUrl + "/user/login.jhtml";

    public static string USER_GUEST_LOGIN = ServiceInterfaceUrl + "/user/guestLogin.jhtml";

    public static string USER_RECORD_LOGIN = ServiceInterfaceUrl + "/user/recordLogin.jhtml";

    public static string PRIV_GET_PRIV_INFO = ServiceInterfaceUrl + "/priv/getPrivInfo.jhtml";

    public static string GET_IS_NEED_TRIGGER_SURVER = ServiceInterfaceUrl + "/survey/getIsNeedTriggerSurvey.jhtml";

    public static string SURVEY_REFRESH = ServiceInterfaceUrl + "/survey/refreshTrigger.jhtml";

    public static string SURVEY_GET_QUESTIONS = ServiceInterfaceUrl + "/survey/getAllQuestions.jhtml";

    public static string SURVEY_RECORD = ServiceInterfaceUrl + "/survey/record.jhtml";

    public static string FEEDBACK_GET_LIST = ServiceInterfaceUrl + "/feedback/getFeedbackList.jhtml";

    public static string FEEDBACK_GET_INFO = ServiceInterfaceUrl + "/feedback/getFeedbackInfo.jhtml";

    public static string FEEDBACK_FILE_SINGNATURE = ServiceInterfaceUrl + "/feedback/fileSignatureUrl.jhtml";

    public static string FEEDBACK_GET_HELPFUL = ServiceInterfaceUrl + "/feedback/replyHelpful.jhtml";

    public static string FEEDBACK_GET_UPLOAD = ServiceInterfaceUrl + "/feedback/postFeedbackInfo.jhtml";

    public static string FEEDBACK_GET_UPLOAD_GUEST = ServiceInterfaceUrl + "/feedback/guestPostFeedbackInfo.jhtml";

    public static string FEEDBACK_GET_ISSUE_INFO = ServiceInterfaceUrl + "/feedback/getFeedbackIssueInfo.jhtml";

    public static string FEEDBACK_BACKUP_RESTORE = ServiceInterfaceUrl + "/dataCollection/addBackupRestore.jhtml";

    public static string GET_FASTBOOTDATA_RECIPE = ServiceInterfaceUrl + "/rescueDevice/getRescueModelRecipe.jhtml";

    public static string UPLOAD_DOWNLOAD_SPEEDINFO = ServiceInterfaceUrl + "/dataCollection/romDownloadInfo.jhtml";

    public static string RESUCE_FAILED_UPLOAD = ServiceInterfaceUrl + "/dataCollection/uploadFile.jhtml";

    public static string FEEDBACK_NO_TRANSLATE = ServiceInterfaceUrl + "/dataCollection/untranslatedSentences.jhtml";

    public static string USER_BEHAVIOR_COLLECTION = ServiceInterfaceUrl + "/dataCollection/addUserBehavior.jhtml";

    public static string UPLOAD_RESCUE_TOOL_LOG = ServiceInterfaceUrl + "/dataCollection/nativeToolLog.jhtml";

    public static string COLLECTION_RESCUE_SUCCESS_LOG_UPLOAD = ServiceInterfaceUrl + "/dataCollection/rescueSuccessLog.jhtml";

    public static string COLLECTION_ASSISTANTAPP = ServiceInterfaceUrl + "/dataCollection/assistantApp.jhtml";

    public static string MOLI_INFO = ServiceInterfaceUrl + "/moli/moliAndLena.jhtml";

    public static string CHECK_MA_VERSION = ServiceInterfaceUrl + "/apk/download.jhtml";

    public static string FORMAT_LENOVOID_ACCOUNT = "https://passport.lenovo.com/interserver/authen/1.2/getaccountid?lpsust={0}&realm=lmsaclient";
}
