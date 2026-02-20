using Newtonsoft.Json;

namespace lenovo.mbg.service.common.webservices.WebApiModel;

public class BaseRequestModel
{
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
