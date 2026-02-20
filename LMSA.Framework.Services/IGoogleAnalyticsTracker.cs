using System.Collections.Generic;

namespace lenovo.mbg.service.framework.services;

public interface IGoogleAnalyticsTracker
{
    object Tracker { get; }

    void Send(IDictionary<string, string> data);
}
