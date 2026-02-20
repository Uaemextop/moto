namespace lenovo.mbg.service.lmsa.common.ImportExport;

/// <summary>
/// Helper for backup/restore data transfer operations.
/// </summary>
public class AppDataTransferHelper
{
    public enum BackupRestoreResult
    {
        Success,
        Fail,
        Undo,
        Undo_DisConnected
    }
}
