using lenovo.mbg.service.common.webservices.WebApiModel;

namespace lenovo.mbg.service.common.webservices.WebApiServices;

public class WarrantyService : ApiBaseService
{
    public ResponseModel<object> LoadWarrantyBanner(object parameters)
    {
        return RequestBase(WebApiUrl.LOAD_WARRANTY_BANNER, parameters);
    }
}
