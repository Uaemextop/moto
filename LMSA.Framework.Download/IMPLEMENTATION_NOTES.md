# LMSA.Framework.Download Implementation Notes

## Overview
This project implements the download framework from the decompiled `lenovo.mbg.service.framework.download` assembly.

## Implemented Components

### 1. ICondition (1 file)
- `IDownloadCondition.cs` - Interface for download conditions

### 2. DownloadUnity (10 files)
- `DownloadStatus.cs` - Enum with 13 download states (WAITTING, DOWNLOADING, PAUSE, etc.)
- `DownloadEventArgs.cs` - Event arguments for download status changes
- `AbstractDownloadInfo.cs` - Abstract base class for download information
- `DownloadTask.cs` - Represents a download task with lifecycle management
- `CompareDictionary.cs` - Custom comparer for AbstractDownloadInfo
- `CompareObject.cs` - Generic object comparer (fixed from invalid decompiled IL)
- `DictionaryExtensionMethodClass.cs` - Extension method TryAddEx for ConcurrentDictionary
- `DownloadThreadManager.cs` - Singleton thread manager for download tasks
- `ThreadHandle.cs` - Handle for cancellation tokens

### 3. DownloadMode (4 files)
- `IDownload.cs` - Interface for download implementations
- `AbstractDownloadMode.cs` - Base class with common download logic (MD5 check, disk space, etc.)
- `HttpDownload.cs` - HTTP/HTTPS download implementation
- `FtpDownload.cs` - FTP download implementation

### 4. DownloadSave (2 files)
- `ISaveDownloadInfoMode.cs` - Interface for persisting download state
- `SaveDownloadInfo2Json.cs` - JSON-based persistence implementation

### 5. DownloadControllerImpl (4 files)
- `AbstractDownloadController.cs` - Base controller with task management
- `GeneralDownloadController.cs` - Standard download controller
- `ConditionDownloadController.cs` - Controller with download conditions
- `ImmediatelyDownloadController.cs` - Immediate execution controller

### 6. Root Namespace (3 files)
- `DownloadTaskProcessor.cs` - Processes individual download tasks
- `DownloadWorker.cs` - Singleton worker managing all downloads
- `SysSleepManagement.cs` - Prevents system sleep during downloads (P/Invoke)

## Key Features

1. **Multi-threaded Downloads**: Uses Task Parallel Library with cancellation tokens
2. **Resume Support**: Checks existing files and resumes from offset
3. **MD5 Validation**: Optional MD5 checksum verification
4. **Priority System**: Downloads prioritized by level and controller level
5. **Disk Space Check**: Validates available space before download
6. **Speed Calculation**: Real-time download speed tracking
7. **Multiple Protocols**: HTTP, HTTPS, and FTP support
8. **Persistence**: JSON-based state saving/loading
9. **Sleep Prevention**: Prevents Windows sleep during active downloads

## Build Notes

- Uses Newtonsoft.Json 13.0.4 for JSON serialization
- Targets .NET 8.0
- Nullable reference types disabled to match decompiled code
- One decompiled class (CompareObject<T>.GetHashCode) had invalid IL and was fixed

## Namespace Structure

All classes use the lowercase namespace `lenovo.mbg.service.framework.download` to match the original assembly.

## Dependencies

- LMSA.Framework.Services
- LMSA.Common.Utilities
- LMSA.Common.Log
- Newtonsoft.Json (13.0.4)

## Status

✅ All 23 classes implemented
✅ Builds successfully with 0 errors
✅ Exact method signatures preserved from decompiled code
✅ All misspellings preserved (e.g., "existsed", "WAITTING")
