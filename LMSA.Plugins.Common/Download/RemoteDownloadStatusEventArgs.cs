using System;

namespace lenovo.mbg.service.framework.services.Download;

/// <summary>
/// Event args for remote download status change notifications.
/// </summary>
[Serializable]
public class RemoteDownloadStatusEventArgs : EventArgs
{
    public string FileUrl { get; }
    public DownloadStatus Status { get; }
    public DownloadInfo Info { get; }

    public RemoteDownloadStatusEventArgs(string fileUrl, DownloadStatus status, DownloadInfo info)
    {
        FileUrl = fileUrl;
        Status = status;
        Info = info;
    }
}
