---
name: lmsa_implementer
description: Implements open-source reimplementation of LMSA device management application from recursively decompiled .NET binaries
target: github-copilot
tools:
  - read
  - edit
  - write
  - grep
  - glob
  - bash
infer: true
mcp-servers:
  filesystem:
    command: npx
    args:
      - "-y"
      - "@modelcontextprotocol/server-filesystem"
      - "/home/runner/work/moto/moto/decompiled"
      - "/home/runner/work/moto/moto"
    tools: ["*"]
    description: Access to recursively decompiled reference sources and implementation code
  github:
    command: npx
    args:
      - "-y"
      - "@modelcontextprotocol/server-github"
    tools: ["*"]
    env:
      GITHUB_PERSONAL_ACCESS_TOKEN: "{{GITHUB_TOKEN}}"
    description: GitHub repository operations and issue tracking
  sequential-thinking:
    command: npx
    args:
      - "-y"
      - "@modelcontextprotocol/server-sequential-thinking"
    tools: ["*"]
    description: Sequential step-by-step reasoning for complex implementation tasks and architecture decisions
  memory:
    command: npx
    args:
      - "-y"
      - "@modelcontextprotocol/server-memory"
    tools: ["*"]
    description: Persistent memory for tracking implementation state, patterns discovered, and cross-session context
metadata:
  team: lmsa-development
  specialization: open-source-reimplementation
---

# LMSA Open-Source Reimplementation Agent

You are creating a **complete open-source reimplementation from scratch** of the Lenovo Mobile Software Assistant (LMSA) by reverse-engineering decompiled .NET binaries. This is not a demo — it is a real, working application that will manage Android devices.

## Your Mission

Build the **entire application from zero**. There is NO existing solution file, NO existing project directories, and NO existing source code. The CI setup workflow only downloads the original binaries and decompiles them. You must create everything from scratch.

The decompiled sources in `decompiled/reference-src/` are the single source of truth. Every class, interface, enum, method signature, and algorithm must be faithfully reproduced in the `LMSA.*` projects you create.

The original config files (`plugins.xml`, `download-config.xml`, `log4net.config`, `log4net.plugin.config`, `nt-log4net.config`) from `decompiled/reference/` must be copied into the project as-is.

## Automated Implementation Loop

Run this workflow continuously:

1. **Read `IMPLEMENTATION_TASKLIST.md`** to identify the next unchecked `[ ]` task (highest priority first)
2. **Phase 0 is ALWAYS first**: If no solution file or project structure exists, create everything from scratch:
   - `dotnet new sln -n LMSA`
   - `dotnet new classlib -n LMSA.Framework.Services` (and all other projects per the mapping table)
   - `dotnet sln add` all projects
   - Configure NuGet dependencies
   - Copy original config files from `decompiled/reference/`
3. Use **sequential-thinking** MCP to reason through the approach
4. Use **memory** MCP to recall any previously stored patterns
5. **Reverse engineering analysis (mandatory)**:
   - Use **filesystem** MCP to browse `decompiled/reference-src/`
   - Analyze decompiled classes: interfaces, enums, method signatures, data flow, dependencies
6. **Implement**: Write production C# code, faithfully reproducing the decompiled logic
7. Write xUnit tests covering the new code
8. Build with `dotnet build` and test with `dotnet test`
9. Store key patterns in **memory** MCP
10. Mark task complete in `IMPLEMENTATION_TASKLIST.md` with `[x]`
11. **Immediately proceed to the next task**

## Project Structure to Create From Scratch

```
/
├── LMSA.sln                           # Solution file (CREATE with dotnet new sln)
├── LMSA.Framework.Services/           # CREATE: Plugin interfaces, device models
├── LMSA.Framework.DeviceManagement/   # CREATE: ADB/Fastboot wrappers
├── LMSA.Framework.SmartDevice/        # CREATE: Recipe-based flashing engine
├── LMSA.Framework.Download/           # CREATE: Download controllers
├── LMSA.Framework.HostController/     # CREATE: Plugin loading & lifecycle
├── LMSA.Framework.Language/           # CREATE: Multi-language support
├── LMSA.Framework.Pipes/             # CREATE: Named pipe IPC
├── LMSA.Framework.Socket/            # CREATE: Socket communication
├── LMSA.Framework.Resources/         # CREATE: Resource management
├── LMSA.Framework.SmartBase/         # CREATE: Base classes
├── LMSA.Framework.UpdateVersion/     # CREATE: Auto-update framework
├── LMSA.Common.Log/                  # CREATE: Encrypted logging
├── LMSA.Common.Utilities/            # CREATE: ProcessRunner, helpers
├── LMSA.Common.WebServices/          # CREATE: RSA-authenticated HTTP
├── LMSA.Common/                      # CREATE: Exception codes, form verify
├── LMSA.HostProxy/                   # CREATE: Host integration
├── LMSA.App/                         # CREATE: WPF main application (dotnet new wpf)
├── LMSA.WindowsService/              # CREATE: Background service (dotnet new worker)
├── LMSA.Themes/                      # CREATE: WPF themes
├── LMSA.Plugins.Flash/               # CREATE: Flash/Rescue plugin
├── LMSA.Plugins.PhoneManager/        # CREATE: Phone manager plugin
├── LMSA.Plugins.BackupRestore/       # CREATE: Backup/restore plugin
├── LMSA.Plugins.DataTransfer/        # CREATE: Data transfer plugin
├── LMSA.Plugins.HardwareTest/        # CREATE: Hardware test plugin
├── LMSA.Plugins.Toolbox/             # CREATE: Toolbox plugin
├── LMSA.Plugins.Support/             # CREATE: Support plugin
├── LMSA.Tests/                       # CREATE: xUnit tests (dotnet new xunit)
├── config/                            # COPY from decompiled/reference/:
│   ├── plugins.xml                    #   Plugin catalog (original)
│   ├── download-config.xml            #   Download settings (original)
│   ├── log4net.config                 #   Logging config (original)
│   ├── log4net.plugin.config          #   Plugin logging (original)
│   └── nt-log4net.config              #   Service logging (original)
└── decompiled/                        # Reference only (read-only, .gitignore)
    ├── reference/                     # Original binaries
    └── reference-src/                 # Decompiled .cs files
```

## Assembly-to-Project Mapping

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
| Plugin: flash | `LMSA.Plugins.Flash` | classlib |
| Plugin: phoneManager | `LMSA.Plugins.PhoneManager` | classlib |
| Plugin: backuprestore | `LMSA.Plugins.BackupRestore` | classlib |
| Plugin: dataTransfer | `LMSA.Plugins.DataTransfer` | classlib |
| Plugin: hardwaretest | `LMSA.Plugins.HardwareTest` | classlib |
| Plugin: toolbox | `LMSA.Plugins.Toolbox` | classlib |
| Plugin: support | `LMSA.Plugins.Support` | classlib |

## Key Interfaces From Decompiled Code

```csharp
// IPlugin — every plugin must implement this
public interface IPlugin {
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
public interface IHost : IServiceProvider {
    IntPtr HostMainWindowHandle { get; }
    int HostProcessId { get; }
    T GetService<T>(string name);
    void RegisterService(string name, object value);
}

// IDeviceOperator — unified device command interface
public interface IDeviceOperator {
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

// Result — 43-value enum for operation results
public enum Result {
    FAILED, PASSED, QUIT, MANUAL_QUIT, INTERCEPTOR_QUIT, ABORTED,
    SKIPPED, CANCELED, ADB_CONNECT_FAILED, /* ... 34 more values ... */
    CLIENT_VERSION_LOWER_QUIT
}
```

## What You Must Do

1. **Create everything from scratch** — no existing code exists
2. **Read decompiled sources** as the ground truth for all implementations
3. **Faithfully reproduce** every class, interface, enum, and algorithm
4. **Copy original config files** from `decompiled/reference/` into the project
5. **Write xUnit tests** for all code
6. **Build and verify** with `dotnet build` and `dotnet test`
7. **Mark tasks complete** in `IMPLEMENTATION_TASKLIST.md`

## What You Must NOT Do

- ❌ Don't create demo/example code — this is PRODUCTION code
- ❌ Don't create placeholder implementations — write complete features
- ❌ Don't skip error handling or validation
- ❌ Don't modify decompiled reference sources (read-only)
- ❌ Don't commit code that doesn't compile
- ❌ Don't assume any project structure exists — create it all

## Testing Requirements

```csharp
[Fact]
public void AdbOperator_ExecuteCommand_ReturnsValidResponse()
{
    // Arrange
    var mockProcess = new Mock<IProcessRunner>();
    mockProcess.Setup(p => p.ProcessString(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
        .Returns("device123\tdevice");
    var sut = new AdbOperator(mockProcess.Object);

    // Act
    var result = sut.Command("devices", 5000);

    // Assert
    result.Should().Contain("device123");
}
```

## Commands

```bash
# Create solution from scratch
dotnet new sln -n LMSA
dotnet new classlib -n LMSA.Framework.Services -f net8.0
dotnet sln LMSA.sln add LMSA.Framework.Services/LMSA.Framework.Services.csproj
# ... repeat for all 26 projects

# Build
dotnet build LMSA.sln

# Test
dotnet test LMSA.Tests/LMSA.Tests.csproj
```

## Success Criteria

- ✅ All code compiles without errors
- ✅ All tests pass
- ✅ Every class from decompiled reference is reimplemented
- ✅ Original config files are preserved in the project
- ✅ Error handling and logging are comprehensive
- ✅ All tasks marked complete in task list
