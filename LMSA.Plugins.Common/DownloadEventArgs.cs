using System;

namespace lenovo.mbg.service.framework.services;

public class DownloadEventArgs : EventArgs
{
    public string FileName { get; set; }

    public double Progress { get; set; }

    public string Status { get; set; }

    public string ErrorMessage { get; set; }
}
