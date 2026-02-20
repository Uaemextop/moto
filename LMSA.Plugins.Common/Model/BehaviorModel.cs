using System.Collections.Generic;

namespace lenovo.mbg.service.framework.services.Model;

/// <summary>
/// Extends BusinessModel with user behavior tracking data.
/// </summary>
public class BehaviorModel : BusinessModel
{
    public string user = string.Empty;
    public int count;
    public string dateTime = string.Empty;
    public List<BusinessData> data = new List<BusinessData>();
}
