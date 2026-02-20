# LMSA Open-Source Reimplementation Task List

## Overview
This task list tracks the **complete from-scratch reimplementation** of the Lenovo Mobile Software Assistant (LMSA) as open-source C# code. Every class, interface, enum, and method signature is derived from the recursively decompiled .NET binaries in `decompiled/reference-src/`.

**Goal**: Build a REAL, working open-source LMSA application **from zero** that maintains ALL functions of the original and behaves identically. This is NOT a demo — it is production software for managing real Android devices.

**Starting point**: The repository has **no solution file, no project directories, and no source code**. The CI setup workflow only downloads and decompiles the original binaries. The agent must create the entire codebase from scratch.

**Original config files**: The XML and config files from `decompiled/reference/` (`plugins.xml`, `download-config.xml`, `log4net.config`, `log4net.plugin.config`, `nt-log4net.config`) must be preserved as-is in the reimplemented project.

**Implementation Approach**:
1. CI workflow downloads LMSA binaries from Dropbox and recursively decompiles all .NET DLLs/EXEs using `ilspycmd`
2. Agent creates the entire solution and project structure from scratch using `dotnet new` commands
3. **Before implementing any feature**, perform reverse engineering analysis of the relevant decompiled sources in `decompiled/reference-src/`
4. Faithfully reproduce every class, interface, enum, and algorithm in the corresponding `LMSA.*` project
5. Copy original XML/config files from `decompiled/reference/` into the project
6. Write xUnit tests for all features
7. Build and verify each component
8. Mark tasks complete with `[x]` only after verification

---

## Phase 0: Create Project Structure From Scratch

The repository starts empty. All projects must be created from zero.

- [ ] Create solution file: `dotnet new sln -n LMSA`
- [ ] Create all `LMSA.*` project directories using `dotnet new classlib` / `dotnet new wpf` / `dotnet new worker`
- [ ] Add all projects to the solution with `dotnet sln add`
- [ ] Configure NuGet package references for each project
- [ ] Create `LMSA.Tests` xUnit test project with Moq and FluentAssertions
- [ ] Copy original config files from `decompiled/reference/` into the project:
  - `plugins.xml` — Plugin catalog with GUID-to-assembly mapping
  - `download-config.xml` — Download settings
  - `log4net.config` — Main application logging config
  - `log4net.plugin.config` — Plugin-specific logging config
  - `nt-log4net.config` — Service logging config
- [ ] Verify `dotnet build` and `dotnet test` pass on empty projects

### Assembly-to-Project Mapping (create all from scratch)

| Original Assembly | LMSA Project | `dotnet new` Type |
|---|---|---|
| `lenovo.mbg.service.framework.services` | `LMSA.Framework.Services` | classlib |
| `lenovo.mbg.service.framework.devicemgt` | `LMSA.Framework.DeviceManagement` | classlib |
| `lenovo.mbg.service.framework.smartdevice` | `LMSA.Framework.SmartDevice` | classlib |
| `lenovo.mbg.service.framework.download` | `LMSA.Framework.Download` | classlib |
| `lenovo.mbg.service.framework.hostcontroller` | `LMSA.Framework.HostController` | classlib |
| `lenovo.mbg.service.framework.lang` | `LMSA.Framework.Language` | classlib |
| `lenovo.mbg.service.framework.pipes` | `LMSA.Framework.Pipes` | classlib |
| `lenovo.mbg.service.framework.socket` | `LMSA.Framework.Socket` | classlib |
| `lenovo.mbg.service.framework.resources` | `LMSA.Framework.Resources` | classlib |
| `lenovo.mbg.service.framework.smartbase` | `LMSA.Framework.SmartBase` | classlib |
| `lenovo.mbg.service.framework.updateversion` | `LMSA.Framework.UpdateVersion` | classlib |
| `lenovo.mbg.service.common.log` | `LMSA.Common.Log` | classlib |
| `lenovo.mbg.service.common.utilities` | `LMSA.Common.Utilities` | classlib |
| `lenovo.mbg.service.common.webservices` | `LMSA.Common.WebServices` | classlib |
| `lenovo.mbg.service.lmsa.common` | `LMSA.Common` | classlib |
| `lenovo.mbg.service.lmsa.hostproxy` | `LMSA.HostProxy` | classlib |
| `Software Fix.exe` | `LMSA.App` | wpf |
| `LmsaWindowsService.exe` | `LMSA.WindowsService` | worker |
| `lenovo.themes.generic` | `LMSA.Themes` | classlib |
| `lenovo.mbg.service.lmsa.flash` | `LMSA.Plugins.Flash` | classlib |
| `lenovo.mbg.service.lmsa.phoneManager` | `LMSA.Plugins.PhoneManager` | classlib |
| `lenovo.mbg.service.lmsa.backuprestore` | `LMSA.Plugins.BackupRestore` | classlib |
| `lenovo.mbg.service.lmsa.dataTransfer` | `LMSA.Plugins.DataTransfer` | classlib |
| `lenovo.mbg.service.lmsa.hardwaretest` | `LMSA.Plugins.HardwareTest` | classlib |
| `lenovo.mbg.service.lmsa.toolbox` | `LMSA.Plugins.Toolbox` | classlib |
| `lenovo.mbg.service.lmsa.{forum,messenger,support,tips}` | `LMSA.Plugins.Support` | classlib |

---

## Phase 1: Core Service Interfaces (`LMSA.Framework.Services`)

**Source**: `decompiled/reference-src/lenovo.mbg.service.framework.services/` (59 .cs files)

### 1.1 Core Plugin System
- [ ] `IPlugin` — Plugin lifecycle: `Init()`, `CreateControl(IMessageBox)`, `CanClose()`, `IsExecuteWork()`, `OnSelected(string)`, `OnSelecting(string)`, `OnInit(object)`, `IsNonBusinessPage()`
- [ ] `IPluginMetadata` — Plugin metadata: `PluginId` property
- [ ] `PluginExportAttribute` — MEF export attribute with `PluginId`
- [ ] `PluginBase` — Abstract base implementing `IPlugin` with defaults
- [ ] `IHost` — Service locator: `HostMainWindowHandle`, `HostProcessId`, `GetService<T>(string)`, `RegisterService(string, object)`

### 1.2 Service Interfaces
- [ ] `IBase`, `IConfigService`, `IDownloadService`, `IMessageBox`
- [ ] `IGlobalCache`, `IGoogleAnalyticsTracker`, `IRsd`
- [ ] `ILanguage`, `IPermission`, `IUser`, `ICheckVersion`
- [ ] `IHostNavigation`, `IHostOperationService`, `IViewContext`, `IViewModelBase`
- [ ] `IResourcesLoggingService`, `IUserBehaviorService`, `IUserMsgControl`
- [ ] `NotifyEventProxy`, `RuntimeContext`, `ServiceProviderUtil`
- [ ] `UserInfo`, `UserInfoArgs`, `UserMsgWndData`
- [ ] `ViewDescription`, `WindowMessageGeneratedEventArgs`
- [ ] `DownloadEventArgs`, `DownloadStatus` (root namespace versions)

### 1.3 Device Models (`.Device` sub-namespace)
- [ ] `DeviceEx` — Abstract device base class
- [ ] `IAndroidDevice` — 40+ property interface
- [ ] `IDeviceOperator` — Command/Shell/Install/Uninstall/Push/Reboot/FindDevices
- [ ] `AbstractDeviceConnectionManagerEx`
- [ ] `DevicePhysicalStateEx` — Enum: None(-1), Offline(0), Online(2), Unauthorized(7), UsbDebugSwitchClosed(9)
- [ ] `DeviceSoftStateEx`, `DeviceType`, `DeviceWorkType`, `DeviceLockReason`
- [ ] `ConnectType`, `ConnectedAppTypesDefine`, `TcpStatus`
- [ ] `MasterDeviceChangedEventArgs`, `WirelessMornitoringAddressChangedHandler`

### 1.4 Download Models (`.Download` sub-namespace)
- [ ] `DownloadInfo`, `DownloadStatus`, `IFileDownload`, `ISaveDownloadInfo`
- [ ] `RemoteDownloadStatusEventArgs`

### 1.5 Business Models (`.Model` sub-namespace)
- [ ] `BehaviorModel`, `BusinessData`, `BusinessModel`, `BusinessStatus`, `BusinessType`

### 1.6 ADB Service Interface
- [ ] `IAdbService` (in `.Adb` sub-namespace)

---

## Phase 2: Common Libraries

### 2.1 Common Utilities (`LMSA.Common.Utilities`) — 63 .cs files
- [ ] `ProcessRunner` — `ProcessString()`, `ProcessList()` with timeout
- [ ] `Configurations` — Path constants for adb.exe, fastboot.exe, log dirs
- [ ] `FileHelper`, `CustomFile`, `ReadWriteFile`
- [ ] `SevenZipHelper` — 7-Zip archive handling
- [ ] `JsonHelper`, `XmlSerializeHelper`
- [ ] `HardwareHelper`, `HardwareEnum`, `DriversHelper`
- [ ] `NetworkCheckHelper`, `WebView2Helper`, `WebBrowserHelper`
- [ ] `AsyncTaskRunner`, `AsyncTaskContext`, `AsyncTaskResult`
- [ ] `Security`, `ContainerManager`, `SysConfig`, `GlobalFun`
- [ ] All remaining utility classes

### 2.2 Common Logging (`LMSA.Common.Log`) — 12 .cs files
- [ ] `LogHelper`, `BusinessLog`, `LogLevel`
- [ ] `LogEncrypt` / `LogDecrypt` — AES-encrypted logs
- [ ] `LoggingEventFactory`

### 2.3 Common Web Services (`LMSA.Common.WebServices`) — 21 .cs files
- [ ] `WebApiUrl`, `WebApiHttpRequest`, `WebApiContext`
- [ ] `RsaWebClient`, `RsaHelper`, `HttpMethod`
- [ ] API Services: `ApiBaseService`, `ApiService`, `RsaService`, `WarrantyService`
- [ ] API Models: `BaseRequestModel`, `RequestModel`, `ResponseModel`, `RSAKey`, `ToolVersionModel`, `FlashedDevModel`, `OrderItem`, `RespOrders`, `PriceInfo`

### 2.4 LMSA Common (`LMSA.Common`) — 25 .cs files
- [ ] Exception/resource types: `ExceptionResultCodes`, `ResourceExecuteResult`, `ResourceTypeDefine`, `DiskSpaceNotEnoughExcpetion`
- [ ] Form verification: `IFormVerify`, `PasswordVerify`, `EmailAddressVerify`, etc.
- [ ] Form ViewModels, views, and converters
- [ ] Import/Export: `AppDataTransferHelper`, `ImportAndExportWrapper`, `ProgressWindowWrapper`, `WorkTransferWindowWrapper`

---

## Phase 3: Device Management (`LMSA.Framework.DeviceManagement`) — 31 .cs files

- [ ] Connection monitors: `DeviceConnectionManagerEx`, `AdbConnectionMonitorEx`, `FBConnectionMonitorEx`, `WifiConnectionMonitorEx`
- [ ] Device implementations: `AdbDeviceEx`, `FastbootDeviceEx`, `WifiDeviceEx`, `TcpAndroidDevice`
- [ ] Device data: `DeviceDataEx`, `WifiDeviceData`, `DeviceReadConfig`
- [ ] Operators: `AdbOperator`, `FastbootOperator`, `ProcessHelper`
- [ ] Device info: `AndroidDeviceProperty`, `FastbootAndroidDevice`, `ILoadDeviceData`, `PropInfoLoader`, `ReadPropertiesInFastboot`
- [ ] Events: `ICompositListener`, `IPhysicalConnectionListener`, `INetworkAdapterListener`
- [ ] Infrastructure: `FileTransferManager`, `MessageManager`, `ConnectSteps`, `ConnectStepStatus`, `ConnectErrorCode`, `DevicemgtContantClass`
- [ ] Event args: `PermissionsCheckConfirmEventArgs`, `TcpConnectStepChangedEventArgs`

---

## Phase 4: Smart Device / Flashing Engine (`LMSA.Framework.SmartDevice`) — 76 .cs files

### 4.1 Core Recipe Framework
- [ ] `Recipe`, `RecipeInfo`, `RecipeResources`
- [ ] `UseCase`, `UseCaseRunner`, `UseCaseDevice`
- [ ] `Result` — 43-value enum
- [ ] `ResultLogger`, `StepInfo`, `StepHelper`
- [ ] `IRecipeMessage`, `RecipeMessage`, `RecipeMessageType`

### 4.2 Step Classes (40+, all inherit BaseStep)
- [ ] `BaseStep` — Abstract base
- [ ] ADB Steps: `ADBConnect`, `WaitConnectByAdb`
- [ ] Fastboot Steps: `FastbootFlash`, `FastbootFlashSinglepartition`, `FastbootShell`, `FastbootDeviceMatchCheck`, `FastbootMatchFlashFile`, `FastbootModifyFlashFile`, `WaitConnectByFastboot`, `WaitFastbootConnectUntilTimeout`, `ReadPropertiesInFastboot`
- [ ] Shell Steps: `Shell`, `AndroidShell`, `AndroidShellVerify`, `ShellCmdStatus`, `ShellCmdType`, `CommandLine`, `CmdRunner`
- [ ] Shell Response Parsers: `ShellResponse`, `ShellResponseFactory`, `ShellResponseCfcflash`, `ShellResponseCmdDloader`, `ShellResponseFlashtool`, `ShellResponseLXConsoleDownLoadTool`, `ShellResponseQdowloader`, `ShellResponseQfil`, `ShellResponseQsaharaserver`, `ShellResponseSpflashtool`, `ShellResponseUpgradetool`
- [ ] Device Steps: `ReadDeviceMode`, `EraseUserData`, `Clear`, `CleanupFactoryMode`, `PushDirectory`
- [ ] File Steps: `LoadFiles`, `CopyFiles`, `CopyLogs`, `RomFileCheck`, `BatFileVersionCheck`
- [ ] Hardware Steps: `FindComPorts`, `FindLocationPort`, `FindPnpDevice`, `InstallDriver`
- [ ] UI Steps: `InfoPrompt`, `InteractPrompt`, `Manual`
- [ ] Misc Steps: `Sleep`, `ConnectSteps`, `ConnectStepInfo`, `RuntimeCheck`, `RunningTimeCheck`, `RunODMSocketServer`

### 4.3 ODM Socket Server (8 classes)
- [ ] `ODMServerMain`, `WebService`, `RestService`, `DataSigningODMService`
- [ ] `Web`, `WebClientTimeout`, `Login`, `Convert`

---

## Phase 5: Download & Update Frameworks

### 5.1 Download Framework (`LMSA.Framework.Download`) — 24 .cs files
- [ ] Controllers: `AbstractDownloadController`, `GeneralDownloadController`, `ConditionDownloadController`, `ImmediatelyDownloadController`
- [ ] `IDownloadCondition`, `DownloadWorker`, `DownloadTaskProcessor`
- [ ] `SysSleepManagement`
- [ ] All download info, status, and event classes

### 5.2 Update Version (`LMSA.Framework.UpdateVersion`) — 34 .cs files
- [ ] Interfaces (11): `IVersionCheck`, `IVersionCheckV1`, `IVersionData`, `IVersionDataCheck`, `IVersionDataV1`, `IVersionDownload`, `IVersionDownloadV1`, `IVersionInstall`, `IVersionInstallV1`, `IVersionEvent`, `IVersionUnInstall`
- [ ] Models (11): `CheckVersionEventArgs`, `CheckVersionStatus`, `DownloadStatusChangedArgs`, `VersionDownloadStatus`, etc.
- [ ] Implementations: `UpdateVersionAutoPush`, `UpdateWoker`, `UpdateWorkV1`

---

## Phase 6: Supporting Frameworks

### 6.1 Host Controller (`LMSA.Framework.HostController`) — 6 files
- [ ] `PluginController`, `PluginContainer`, `PluginViewOfHost`, `Plugin`, `PluginErrorEventArgs`

### 6.2 Language (`LMSA.Framework.Language`) — 12 files
- [ ] `Lang`, `LangLabel`, `LangButton`, `LangRadioButton`, `LangTextBlock`, `LangToolTip`, `LangTranslation`

### 6.3 Pipes (`LMSA.Framework.Pipes`) — 6 files
- [ ] `BasicPipe`, `ServerPipe`, `ClientPipe`, `PipeMessage`, `PipeEventArgs`

### 6.4 Socket (`LMSA.Framework.Socket`) — 40 files
- [ ] File transfer, message protocol, RSA security, heartbeat, JSON endpoints

### 6.5 Resources (`LMSA.Framework.Resources`) — 8 files
- [ ] `Rsd`, `FileDownloadManagerV6`, `DownloadWorker`, `DownloadInfoToJson`, `SysSleepManagement`

### 6.6 Smart Base (`LMSA.Framework.SmartBase`) — 3 files
- [ ] Base classes for smart device features

### 6.7 Host Proxy (`LMSA.HostProxy`) — 3 files
- [ ] `HostProxy` — Host integration proxy

---

## Phase 7: Main Application (`LMSA.App`) — 170+ .cs files

### 7.1 Application Core
- [ ] `App` / `ApplcationClass`, `AppContext`, `MainWindow`
- [ ] `SingleInstance` / `ISingleInstanceApp`, `SplashScreenWindow`, `ClosingWindow`
- [ ] `NativeMethods`, `WM`

### 7.2 ViewModels
- [ ] `MainWindowViewModel`, `DeviceConnectViewModel`, `DeviceViewModel`
- [ ] `LanguageSelectViewModel`, `NewVersionViewModel`, `PrivacyPopViewModel`
- [ ] `SurveyWindowV2ViewModel`, `ViewModelBase`, `ModelBase`
- [ ] `DownloadControlViewModel`, `CouponWindowModel`
- [ ] All remaining ViewModels

### 7.3 Views (V6 generation)
- [ ] `DevConnectView`, `DeviceConnectView`, `HostUpdateWindowV6`
- [ ] `MessageBoxV6`, `ContentMessageBox`, `PicMessageBox`, `RightPicMessageBox`
- [ ] `LanguageSelectViewV6`, `InstallMADialogV6`, `NoticeManagementViewV6`
- [ ] `DebugPermissionWindow`, `B2BPurchaseOverviewV6`, `RegisterDevView`
- [ ] `CouponWindow`, `ExistsSpacePathView`

### 7.4 Login System
- [ ] Business: `UserService`, `PermissionService`, `LoginHandlerFacory`, `LmsaUserLogin`, `LenovoIdUserLogin`, `GuestLogin`
- [ ] Protocol: All login/register/logout protocol models
- [ ] Views and ViewModels

### 7.5 Feedback, Update Version, Resources Cleanup Systems
- [ ] All business, model, view, and ViewModel classes for each subsystem

### 7.6 Services
- [ ] `ConfigService`, `GoogleAnalyticsTracker`, `PermissionService`
- [ ] `PipeClientService`, `ResourcesLoggingService`, `User`, `UserBehaviorService`

### 7.7 Business Logic
- [ ] `PluginCatalog`, `PluginCatalogInfo`, `PluginCatalogPlugin`, `XmlPluginData`
- [ ] `DeviceDataCollection`, `DeviceModel`, `UserDeviceModel`
- [ ] `MainWindowControl`, `MenuPopupWindowBusiness`
- [ ] All remaining business classes

### 7.8 Custom Controls & Converters
- [ ] All WPF custom controls and value converters

---

## Phase 8: Plugins

### 8.1 Flash/Rescue (`LMSA.Plugins.Flash`) — 263 files, GUID `8ab04aa9...`
- [ ] All 263 classes from decompiled source

### 8.2 Phone Manager (`LMSA.Plugins.PhoneManager`) — 1675 files, GUID `02928af0...`
- [ ] All 1675 classes including `.common` and `.apps` sub-assemblies

### 8.3 Backup & Restore (`LMSA.Plugins.BackupRestore`) — 108 files, GUID `13f79fe4...`
- [ ] All 108 classes from decompiled source

### 8.4 Data Transfer (`LMSA.Plugins.DataTransfer`) — 11 files, GUID `d8042f96...`
- [ ] All 11 classes from decompiled source

### 8.5 Hardware Test (`LMSA.Plugins.HardwareTest`) — 17 files, GUID `985c66ac...`
- [ ] All 17 classes from decompiled source

### 8.6 Toolbox (`LMSA.Plugins.Toolbox`) — 1276 files, GUID `dd537b5c...`
- [ ] All 1276 classes including SuperSocket/SuperWebSocket, NAudio, FFmpeg

### 8.7 Support (`LMSA.Plugins.Support`) — 67 files, GUID `a6099126...`
- [ ] All 67 classes: forum, messenger, support, tips

---

## Phase 9: Windows Service (`LMSA.WindowsService`) — 7 files
- [ ] `LmsaService`, `Program`, `ProjectInstaller`
- [ ] `TaskManager`, `TaskScheduler`, `TaskWorker`, `PipeWorkerFactory`

---

## Phase 10: Themes (`LMSA.Themes`)
- [ ] WPF styles, templates, resource dictionaries from `lenovo.themes.generic`

---

## Phase 11: Testing
- [ ] Unit tests for all framework libraries
- [ ] Unit tests for device management (mock ADB/Fastboot)
- [ ] Unit tests for smart device engine (Recipe, Steps, Result)
- [ ] Unit tests for common utilities
- [ ] Unit tests for web services
- [ ] Integration tests for download and device monitoring

---

## Phase 12: Deployment
- [ ] Reproduce `plugins.xml` with correct GUID mapping (copy from original)
- [ ] Include all config files from original
- [ ] Create MSI installer
- [ ] Include adb.exe, fastboot.exe, fastbootmonitor.exe

---

## Priority Recommendations

### Highest Priority (Foundation — create from scratch)
1. **Phase 0**: Create entire project structure from zero
2. **Phase 1**: Core service interfaces
3. **Phase 2.1**: Common utilities (ProcessRunner, Configurations)
4. **Phase 3**: Device management framework

### High Priority (Core Features)
5. Phase 4: Smart device / flashing engine
6. Phase 2.3: Web services
7. Phase 5: Download & update frameworks
8. Phase 8.1: Flash/Rescue plugin

### Medium Priority (User Features)
9. Phase 7: Main WPF application
10. Phase 8.2: Phone Manager plugin
11. Phase 8.3: Backup/Restore plugin
12. Phase 6: Supporting frameworks

### Lower Priority
13. Phase 8.4-8.7: Remaining plugins
14. Phase 9-12: Service, themes, testing, deployment

---

## Estimated Complexity

- **Total LMSA Projects to create from scratch**: 26
- **Total .cs Files to reimplement**: ~8,120
- **Lenovo Custom Code**: ~3,200+ .cs files
- **Plugin Code**: ~3,400+ .cs files
- **Main App Code**: ~170+ .cs files
- **Config files to preserve**: 5 XML/config files from original

---

**Last Updated**: February 20, 2026
**Source**: Recursively decompiled from LMSA Software Fix.exe v7.4.3.4 and all associated DLLs
