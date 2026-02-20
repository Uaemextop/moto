using System.Collections.Generic;

namespace lenovo.mbg.service.framework.services.Download;

/// <summary>
/// Interface for persisting download information to storage.
/// </summary>
public interface ISaveDownloadInfo
{
    string DownloadingPath { get; }
    string DownloadedPath { get; }

    bool Save(string path, object data);
    List<T> Get<T>(string path);
}
