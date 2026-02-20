# GitHub Copilot Instructions for LMSA Open-Source Reimplementation

## Project Overview

This project is an **open-source reimplementation from scratch** of the Lenovo Mobile Software Assistant (LMSA), also known as "Software Fix" — a Windows desktop application for managing, rescuing, and flashing firmware to Lenovo and Motorola Android devices.

**Project Goal**: Build the entire application **from zero** by reverse-engineering the recursively decompiled .NET binaries (DLLs and EXEs) of the original LMSA application. There is **no existing project structure** — the agent must create the solution file, all project directories, all `.csproj` files, and all source code from scratch. The decompiled sources in `decompiled/reference-src/` are the single source of truth.

**Decompilation workflow**: The CI setup workflow **only** downloads the original LMSA binaries from Dropbox and decompiles all .NET DLLs/EXEs using `ilspycmd`. It does NOT create, restore, or build any project. The agent is responsible for creating the entire open-source codebase.

**IMPORTANT**: This is a REAL, production application — not an example, demo, or reference implementation. The reimplemented app must maintain ALL functions of the original and behave identically. Every class, method signature, enum, interface, and algorithm must be faithfully reproduced.

**IMPORTANT**: The repository starts empty (no solution, no projects). You must create everything using `dotnet new`, `dotnet sln`, and manual file creation.

---

## Verified Assembly Inventory (from decompilation)

The original LMSA application consists of the following assemblies. Each must be reimplemented as an `LMSA.*` project:

### Lenovo Framework Assemblies (16 assemblies)
| Original Assembly | CS Files | LMSA Project Target | Purpose |
|---|---|---|---|
| `lenovo.mbg.service.framework.services` | 59 | `LMSA.Framework.Services` | Plugin interfaces (IPlugin, IHost), device models, service contracts |
| `lenovo.mbg.service.framework.devicemgt` | 31 | `LMSA.Framework.DeviceManagement` | ADB/Fastboot wrappers, device connection monitoring |
| `lenovo.mbg.service.framework.smartdevice` | 76 | `LMSA.Framework.SmartDevice` | Recipe-based flashing engine, step framework, ODM server |
| `lenovo.mbg.service.framework.download` | 24 | `LMSA.Framework.Download` | Download controllers, multi-threaded download management |
| `lenovo.mbg.service.framework.hostcontroller` | 6 | `LMSA.Framework.HostController` | Plugin container, loader, and lifecycle management |
| `lenovo.mbg.service.framework.lang` | 12 | `LMSA.Framework.Language` | Multi-language support (LangLabel, LangButton, LangTranslation) |
| `lenovo.mbg.service.framework.pipes` | 6 | `LMSA.Framework.Pipes` | IPC via named pipes (ServerPipe, ClientPipe) |
| `lenovo.mbg.service.framework.socket` | 40 | `LMSA.Framework.Socket` | Socket communication, file transfer, RSA data security |
| `lenovo.mbg.service.framework.resources` | 8 | `LMSA.Framework.Resources` | Download info persistence, ROM resource management |
| `lenovo.mbg.service.framework.smartbase` | 3 | `LMSA.Framework.SmartBase` | Base classes for smart features |
| `lenovo.mbg.service.framework.updateversion` | 34 | `LMSA.Framework.UpdateVersion` | Version checking, auto-update, installation |
| `lenovo.mbg.service.common.log` | 12 | `LMSA.Common.Log` | AES-encrypted logging, structured log methods |
| `lenovo.mbg.service.common.utilities` | 63 | `LMSA.Common.Utilities` | ProcessRunner, file/hardware/crypto helpers, async tasks |
| `lenovo.mbg.service.common.webservices` | 21 | `LMSA.Common.WebServices` | RSA-authenticated HTTP client, WebAPI context, API services |
| `lenovo.mbg.service.lmsa.common` | 25 | `LMSA.Common` | Exception codes, resource types, form verification |
| `lenovo.mbg.service.lmsa.hostproxy` | 3 | `LMSA.HostProxy` | Host integration proxy |

### Main Application
| Original Assembly | CS Files | LMSA Project Target | Purpose |
|---|---|---|---|
| `Software Fix.exe` | 170+ | `LMSA.App` (WPF) | Main WPF application: MainWindow, ViewModels, Login, Feedback, etc. |
| `LmsaWindowsService.exe` | 7 | `LMSA.WindowsService` | Background Windows Service with named pipe IPC |
| `lenovo.themes.generic` | — | `LMSA.Themes` | WPF themes, styles, and resources (XAML/BAML) |

### Plugin Assemblies (7 plugins, each in a GUID directory)
| Plugin GUID | Plugin Name | Assembly | CS Files | LMSA Project Target |
|---|---|---|---|---|
| `8ab04aa9...` | **Rescue/Flash** | `lenovo.mbg.service.lmsa.flash` | 263 | `LMSA.Plugins.Flash` |
| `02928af0...` | **My Device / Phone Manager** | `lenovo.mbg.service.lmsa.phoneManager` (+`.common`, +`.apps`) | 1675 | `LMSA.Plugins.PhoneManager` |
| `13f79fe4...` | **Backup & Restore** | `lenovo.mbg.service.lmsa.backuprestore` | 108 | `LMSA.Plugins.BackupRestore` |
| `d8042f96...` | **Data Transfer** | `lenovo.mbg.service.lmsa.dataTransfer` | 11 | `LMSA.Plugins.DataTransfer` |
| `985c66ac...` | **Hardware Test** | `lenovo.mbg.service.lmsa.hardwaretest` | 17 | `LMSA.Plugins.HardwareTest` |
| `dd537b5c...` | **Toolbox** | `lenovo.mbg.service.lmsa.toolbox` | 1276 | `LMSA.Plugins.Toolbox` |
| `a6099126...` | **Support** | `lenovo.mbg.service.lmsa.{forum,messenger,support,tips}` | 67 | `LMSA.Plugins.Support` |

---

## Technology Stack (verified from decompiled binaries)

### Primary Technologies
- **Language**: C# (.NET Framework 4.7.2 / .NET 8 for reimplementation)
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Markup**: XAML for UI definitions (compiled as BAML in original)

### Required Libraries (verified from reference DLLs)
- **SharpAdbClient** — ADB device communication (IAdbClient, DeviceData, SyncService)
- **Newtonsoft.Json** — JSON serialization throughout the codebase
- **log4net** — Logging framework (with AES-encrypted log support)
- **BouncyCastle.Crypto** — RSA encryption for API communication
- **protobuf-net** — Protocol Buffers serialization
- **SevenZipSharp** — 7-Zip archive handling for ROM packages
- **Microsoft.Web.WebView2** — Embedded web browser (Core, WinForms, Wpf)
- **Microsoft.Xaml.Behaviors** — WPF interaction behaviors
- **SharpVectors** — SVG rendering (Converters, Core, Css, Dom, Model, Rendering)
- **XamlAnimatedGif** — GIF animation in WPF (used in Flash plugin tutorials)
- **SuperSocket/SuperWebSocket** — WebSocket server (used in Toolbox plugin)
- **NAudio** — Audio processing (used in Toolbox plugin)
- **FFmpeg.AutoGen** — FFmpeg interop (used in Toolbox plugin)
- **Gma.QrCodeNet.Encoding** — QR code generation
- **GoogleAnalytics** — Usage analytics tracking (Core + WPF.Managed)
- **Common.Logging** — Logging abstraction layer

---

## Architecture (verified from decompiled source)

### 1. Plugin System (MEF-based)

The plugin system uses MEF (Managed Extensibility Framework) with custom attributes:

```csharp
// From decompiled PluginExportAttribute.cs
[MetadataAttribute]
[AttributeUsage(AttributeTargets.Class)]
public class PluginExportAttribute : ExportAttribute
{
    public string PluginId { get; private set; }
    public PluginExportAttribute(Type contractType, string pluginId)
        : base(contractType) { PluginId = pluginId; }
}

// From decompiled IPluginMetadata.cs
public interface IPluginMetadata
{
    string PluginId { get; }
}
```

Plugin lifecycle is managed by `PluginController` and `PluginContainer` from `framework.hostcontroller`. Plugins are discovered from `plugins.xml` which maps GUIDs to assemblies.

### 2. Core Interfaces (from decompiled `framework.services`)

```csharp
// IPlugin — every plugin must implement this
public interface IPlugin
{
    void Init();
    FrameworkElement CreateControl(IMessageBox iMsg);
    bool CanClose();
    bool IsExecuteWork();
    void OnSelected(string val);
    void OnSelecting(string val);
    void OnInit(object data);
    bool IsNonBusinessPage();
}

// IHost — service locator for plugins
public interface IHost : IServiceProvider
{
    IntPtr HostMainWindowHandle { get; }
    int HostProcessId { get; }
    T GetService<T>(string name);
    void RegisterService(string name, object value);
}

// PluginBase — abstract base class all plugins extend
public abstract class PluginBase : IPlugin
{
    public static ILanguage LangHelper;
    // ... default implementations for IPlugin methods
}
```

### 3. Device Communication Layer

```csharp
// IDeviceOperator — unified device command interface
public interface IDeviceOperator
{
    string Command(string command, int timeout = -1, string deviceID = "");
    string Shell(string deviceID, string command);
    void Install(string deviceID, string apkPath);
    void Uninstall(string deviceID, string apkName);
    void ForwardPort(string deviceID, int devicePort, int localPort);
    void RemoveForward(string deviceID, int localPort);
    void RemoveAllForward(string deviceID);
    void PushFile(string deviceID, string localFilePath, string deviceFilePath);
    void Reboot(string deviceID, string mode);
    List<string> FindDevices();
}

// IAndroidDevice — 40+ properties describing a connected device
public interface IAndroidDevice
{
    int ApiLevel { get; }
    string IMEI1 { get; }
    string ModelId { get; }
    string SN { get; }
    string Brand { get; }
    string AndroidVersion { get; }
    double BatteryQuantityPercentage { get; }
    // ... 35+ more properties
    string GetPropertyValue(string name);
}
```

### 4. Device State Machine

```csharp
// DevicePhysicalStateEx — actual values from decompiled code
public enum DevicePhysicalStateEx
{
    None = -1,
    Offline = 0,
    Online = 2,
    Unauthorized = 7,
    UsbDebugSwitchClosed = 9
}
```

### 5. Recipe-Based Flashing Engine

The smart device framework uses a Recipe→Steps→Result execution model:

```csharp
// Recipe — orchestrates a flashing operation
public class Recipe
{
    public RecipeInfo Info { get; }
    public List<BaseStep> Steps { get; }
    public SortedList<string, dynamic> Cache { get; }
    public RecipeResources Resources { get; }
    public DeviceEx Device { get; }
    public UseCaseDevice UcDevice { get; set; }
}

// BaseStep — all 40+ step types inherit from this
public abstract class BaseStep : IDisposable
{
    public string Name => Info.Name;
    public Recipe Recipe { get; }
    public StepInfo Info { get; }
    protected ResultLogger Log { get; }
    protected SortedList<string, dynamic> Cache { get; }
    protected RecipeResources Resources { get; }
}
```

### 6. Result Enum (complete from decompiled code)

```csharp
public enum Result
{
    FAILED, PASSED, QUIT, MANUAL_QUIT, INTERCEPTOR_QUIT, ABORTED,
    SKIPPED, CANCELED, ADB_CONNECT_FAILED, CLEAR_FACTORYMODE_FAILED,
    FASTBOOT_FLASH_SINGLEPARTITION_FAILED, FASTBOOT_SHELL_FAILED,
    FASTBOOT_FLASH_FAILED, FASTBOOT_FLASH_ERASEDATE_FAILED,
    FASTBOOT_FLASH_FILE_MATCH_FAILED, FASTBOOT_SLOT_SET_FAILED,
    FASTBOOT_CONNECT_FAILED, FASTBOOT_DEGRADE_QUIT,
    FASTBOOT_CID_CHECKE_QUIT, FASTBOOT_ERROR_RULES_QUIT,
    FIND_COMPORT_FAILED, FIND_LOCATIONPORT_FAILED,
    LOAD_RESOURCE_FAILED, LOAD_RESOURCE_FAILED_REPLACE,
    LOAD_RESOURCE_FAILED_COUNTRYCODE, SHELL_CONNECTED_FAILED,
    SHELL_RESCUE_FAILED, SHELL_EXE_TERMINATED_EXIT,
    SHELL_EXE_START_FAILED, ROM_UNMATCH_FAILED,
    PROCESS_FORCED_TEREMINATION, FIND_PNPDEVICE_FAILED,
    DEVICE_CONNECT_FAILED, PROGRESS, AUTRORIZED_FAILED,
    INSTALL_VC_RUNNINGTIME_FAILED, MODELNAME_CHECK_FAILED_QUIT,
    CHECK_ROM_FILE_FAILED, COPYFILES_FAILED, COPYLOGS_FAILED,
    DRIVER_INSTALL_FAILED, ROM_DIRECTORY_NOT_EXISTS,
    CLIENT_VERSION_LOWER_QUIT
}
```

---

## Coding Standards

### General Principles
- Write clear, maintainable code
- Follow SOLID principles
- Prefer composition over inheritance
- Use dependency injection where appropriate

### Naming Conventions (matching original decompiled code)
- **Classes**: PascalCase (e.g., `DeviceConnectionManagerEx`, `FastbootFlash`)
- **Methods**: PascalCase (e.g., `ExecuteCommand`, `GetPropertyValue`)
- **Properties**: PascalCase (e.g., `PluginId`, `HostMainWindowHandle`)
- **Fields (private)**: camelCase (e.g., `device`, `objContainer`) — some use underscore prefix
- **Constants**: PascalCase or ALL_CAPS (e.g., `MaxRetryCount`, `JWT_TOKEN`)
- **Namespaces**: lowercase with dots (e.g., `lenovo.mbg.service.framework.devicemgt`)
- **Enums**: ALL_CAPS values (e.g., `PASSED`, `FAILED`, `FASTBOOT_FLASH_FAILED`)

### Code Formatting
- **Indentation**: 4 spaces (never tabs)
- **Braces**: Allman style (next line) for classes and methods
- **Line length**: Maximum 120 characters
- **Using statements**: Outside namespace, sorted alphabetically

---

## Error Handling Requirements

### Error Detection (from decompiled FastbootFlash.cs)
```csharp
bool hasError = response.ToLower().Contains("error")
             || response.ToLower().Contains("fail");

if (response.Contains("STATUS_SEC_VIOLATE_ANTI_ROLLBACK"))
    return Result.FASTBOOT_DEGRADE_QUIT;
```

### Command Timeout Map (exact values from decompiled FastbootFlash.cs)
```csharp
private static Dictionary<string, int> OperationToTimeout = new Dictionary<string, int>
{
    { "flash", 300000 },          // 5 minutes
    { "erase", 60000 },           // 1 minute
    { "oem", 60000 },             // 1 minute
    { "getvar", 20000 },          // 20 seconds
    { "reboot", 20000 },          // 20 seconds
    { "reboot-bootloader", 10000 }, // 10 seconds
    { "format", 60000 },          // 1 minute
    { "flashall", 600000 },       // 10 minutes
    { "continue", 10000 }         // 10 seconds
};
```

---

## Logging Requirements

### Logging Pattern (from decompiled code)
```csharp
// ResultLogger from framework.smartdevice
Log.AddLog($"adb command: {command}, response: {response}", upload: true);
Log.AddResult(this, Result.PASSED, null);
Log.AddResult(this, Result.FAILED, "Error description");
```

### Encrypted Logging (from common.log)
The original uses AES-encrypted log files. The `LogHelper` and `LogEncrypt`/`LogDecrypt` classes handle this.

---

## WPF/XAML UI Guidelines

### MVVM Pattern
- ViewModels in `lenovo.mbg.service.lmsa.ViewModels` namespace
- Views in `lenovo.mbg.service.lmsa.ViewV6` namespace
- Custom controls in `lenovo.mbg.service.lmsa.UserControls`
- Converters in `lenovo.mbg.service.lmsa.Converters`

### Multi-Language Support
- `LangLabel`, `LangButton`, `LangTextBlock` controls from `framework.lang`
- Language packs in `lang/` folder
- `LangTranslation` for runtime translation
- Never hardcode user-facing strings

---

## Security Requirements

1. **Anti-Rollback Protection**: Check for `STATUS_SEC_VIOLATE_ANTI_ROLLBACK` in fastboot responses
2. **RSA-Encrypted API**: `RsaWebClient` and `RsaHelper` sign API requests and verify responses
3. **AES-Encrypted Logs**: `LogEncrypt`/`LogDecrypt` in `common.log`
4. **Permission Validation**: Check Android permissions (Android 10+) via `CheckPermissions()`
5. **Process Security**: Validate file paths, sanitize command arguments, prevent command injection

---

## Testing Requirements

### Unit Test Pattern
```csharp
[Fact]
public void Command_WithValidCommand_ReturnsResponse()
{
    // Arrange
    var mockAdb = new Mock<IAdbClient>();
    var sut = new AdbOperator(mockAdb.Object);

    // Act
    var result = sut.Command("shell getprop", -1, "device123");

    // Assert
    result.Should().NotBeNull();
    result.Should().NotContain("error");
}
```

### Test Framework
- **xUnit** with `[Fact]` attributes
- **Moq** for mocking
- **FluentAssertions** for assertions

---

## Configuration Files (from original)
- `plugins.xml` — Plugin catalog with GUID mapping
- `download-config.xml` — Download settings
- `log4net.config` — Logging configuration
- `log4net.plugin.config` — Plugin-specific logging
- `nt-log4net.config` — Service logging

---

## Reference Documentation

- **Task List**: See `IMPLEMENTATION_TASKLIST.md` for complete feature list with class-level detail
- **Command Reference**: See `ADB_FASTBOOT_COMMANDS.md` for all ADB/fastboot commands
- **Architecture**: See `DECOMPILATION_SUMMARY.md` for system architecture
- **Decompiled Sources**: See `decompiled/reference-src/` for the ground truth of all implementations

---

## Implementation Workflow (Mandatory)

Before implementing any feature from the task list, follow this mandatory workflow:

1. **Create project structure from scratch**: There is no existing solution or project. Use `dotnet new sln`, `dotnet new classlib`, `dotnet new wpf`, `dotnet sln add` to create the solution and all projects from zero. Match the assembly-to-project mapping in the task list.
2. **Reverse engineering analysis**: Browse `decompiled/reference-src/` and analyze the relevant decompiled DLLs/EXEs for the feature — identify all classes, interfaces, enums, method signatures, data flow, dependencies, and algorithms
3. **Copy original config files**: Copy `plugins.xml`, `download-config.xml`, `log4net.config`, `log4net.plugin.config`, and `nt-log4net.config` from `decompiled/reference/` into the appropriate project output directories. These files must be kept as-is from the original.
4. **Implement**: Write clean C# code in the appropriate `LMSA.*` project directory, faithfully reproducing the decompiled logic. Match every class, interface, enum, and method signature from the decompiled reference
5. **Test**: Write xUnit tests to verify correctness
6. **Build and verify**: Run `dotnet build` and `dotnet test` to confirm everything compiles and passes

---

**Last Updated**: February 20, 2026
**Applies To**: All LMSA implementation files
