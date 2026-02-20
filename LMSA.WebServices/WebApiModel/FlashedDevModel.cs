using System;

namespace lenovo.mbg.service.common.webservices.WebApiModel;

public class FlashedDevModel
{
    public string Imei { get; set; }

    public string ModelName { get; set; }

    public string Category { get; set; }

    public long? createDate { get; set; }

    public DateTime FlashDate => new DateTime(1970, 1, 1).AddMilliseconds(createDate.Value);
}
