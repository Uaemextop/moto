# GitHub Copilot Instructions for LMSA Project

## Project Overview

This project is a C# .NET Framework implementation of the Lenovo Mobile Software Assistant (LMSA), a Windows desktop application for managing, rescuing, and flashing firmware to Lenovo and Motorola Android devices.

**Project Goal**: Implement a complete, production-ready version of LMSA based on decompiled source code analysis.

**IMPORTANT**: This is a REAL, production application - not an example, demo, or reference implementation. You are writing actual working code that will manage real Android devices.

---

## Technology Stack

### Primary Technologies
- **Language**: C# 8.0+ with .NET Framework 4.7.2 or .NET 6+
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Markup**: XAML for UI definitions

### Required Libraries
- **SharpAdbClient** (v1.3.0+) - ADB device communication
- **Newtonsoft.Json** (v13.0+) - JSON serialization
- **log4net** (v2.0.15+) - Logging framework
- **BouncyCastle.Crypto** (v2.0+) - Cryptographic operations
- **protobuf-net** (v3.0+) - Protocol Buffers
- **SevenZipSharp** - Archive handling
- **Microsoft.Web.WebView2** - Embedded web browser
- **Microsoft.Xaml.Behaviors** - WPF behaviors
- **SharpVectors** - SVG rendering

---

## Architectural Requirements

### 1. Plugin Architecture

All major features must be implemented as plugins:

```csharp
// Plugin interface
public interface IPlugin
{
    string PluginID { get; }  // Unique GUID
    string Name { get; }
    void Initialize();
    void Shutdown();
}

// Plugin structure
plugins/
├── [guid]/
│   ├── lenovo.mbg.service.lmsa.[feature].dll
│   └── dependencies...
```

**Plugin Categories**:
- Phone Manager (file/app management)
- Flash/Rescue (firmware operations)
- Backup/Restore (data backup)
- Data Transfer (device-to-device)
- Hardware Test (component testing)
- Toolbox (utilities)

### 2. Service Framework

Use a layered service architecture:

```csharp
namespace lenovo.mbg.service.framework.[layer];

// Layers:
// - devicemgt     : Device management and communication
// - smartdevice   : Device-specific operations
// - services      : Application services
// - common        : Shared utilities and logging
```

### 3. Device Communication

#### ADB Operations
```csharp
// Use IAdbClient from SharpAdbClient
IAdbClient adbClient = AdbConnectionMonitorEx.m_AdbClient;

// Command execution pattern
public string Command(string command, int timeout = -1, string deviceID = "")
{
    string fullCommand = !string.IsNullOrEmpty(deviceID)
        ? $"-s {deviceID} {command}"
        : command;
    return ProcessRunner.ProcessString(adbPath, fullCommand, timeout);
}
```

#### Fastboot Operations
```csharp
// Direct process execution
string response = ProcessRunner.ProcessString(
    fastbootPath,
    EncapsulationFastbootCommand(command),
    timeout
);
```

---

## Coding Standards

### General Principles
- Write clear, maintainable code
- Follow SOLID principles
- Prefer composition over inheritance
- Use dependency injection where appropriate

### Naming Conventions
- **Classes**: PascalCase (e.g., `DeviceOperator`, `FlashManager`)
- **Methods**: PascalCase (e.g., `ExecuteCommand`, `GetDeviceInfo`)
- **Properties**: PascalCase (e.g., `DeviceId`, `IsConnected`)
- **Fields (private)**: camelCase with underscore prefix (e.g., `_adbClient`, `_isRunning`)
- **Constants**: PascalCase or ALL_CAPS (e.g., `MaxRetryCount`, `DEFAULT_TIMEOUT`)
- **Namespaces**: lowercase with dots (e.g., `lenovo.mbg.service.framework.devicemgt`)

### Code Formatting
- **Indentation**: 4 spaces (never tabs)
- **Braces**: Opening brace on same line for methods, next line for control structures
- **Line length**: Maximum 120 characters
- **Using statements**: Outside namespace, sorted alphabetically

### Comments
- Only add comments for complex logic or non-obvious behavior
- Don't comment what the code does; comment why it does it
- Use XML documentation comments for public APIs:

```csharp
/// <summary>
/// Executes a fastboot command on the specified device.
/// </summary>
/// <param name="command">The fastboot command to execute</param>
/// <param name="timeout">Timeout in milliseconds</param>
/// <returns>Command output as string</returns>
public string ExecuteFastbootCommand(string command, int timeout)
```

---

## Error Handling Requirements

### Result Pattern
```csharp
public enum Result
{
    PASSED,
    FAILED,
    QUIT,
    FASTBOOT_FLASH_FAILED,
    FASTBOOT_SHELL_FAILED,
    FASTBOOT_DEGRADE_QUIT
}
```

### Error Detection
```csharp
// Check for error patterns in responses
bool hasError = response.ToLower().Contains("error")
             || response.ToLower().Contains("fail");

// Handle specific errors
if (response.Contains("STATUS_SEC_VIOLATE_ANTI_ROLLBACK"))
{
    return Result.FASTBOOT_DEGRADE_QUIT;
}
```

### Retry Logic
```csharp
int retries = 3;
string response;
do
{
    response = ExecuteCommand(command);
    if (IsSuccess(response)) break;
} while (--retries > 0);
```

---

## Logging Requirements

### Logging Pattern
```csharp
// Use log4net
private static readonly ILog log = LogManager.GetLogger(typeof(ClassName));

// Log levels
log.Info($"Starting operation: {operationName}");
log.Debug($"Command executed: {command}");
log.Error($"Operation failed: {error}", exception);

// Framework logging pattern
Log.AddLog($"adb command: {command}, response: {response}", upload: true);
Log.AddResult(this, Result.PASSED, null);
Log.AddResult(this, Result.FAILED, "Error description");
```

### What to Log
- ✅ Command execution (command + response)
- ✅ Device state changes
- ✅ Operation results (PASSED/FAILED)
- ✅ Error conditions and exceptions
- ❌ Don't log sensitive data (passwords, tokens)
- ❌ Don't log excessive debug info in production

---

## WPF/XAML UI Guidelines

### MVVM Pattern
```csharp
// ViewModel
public class FeatureViewModel : INotifyPropertyChanged
{
    private string _status;
    public string Status
    {
        get => _status;
        set { _status = value; OnPropertyChanged(); }
    }

    public ICommand ExecuteCommand { get; }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
```

### XAML Bindings
```xml
<TextBlock Text="{Binding Status}" />
<Button Command="{Binding ExecuteCommand}" Content="Execute" />
```

### Multi-Language Support
- Use resource files for all UI strings
- Support language pack system (lang folder)
- Never hardcode user-facing strings

---

## Security Requirements

### Critical Security Rules

1. **Anti-Rollback Protection**
   - Always check for anti-rollback violations
   - Abort operations on STATUS_SEC_VIOLATE_ANTI_ROLLBACK
   - Verify secure version before flashing

2. **Permission Validation**
   - Check Android permissions before operations (Android 10+)
   - Validate user authorization for destructive operations
   - Confirm before erasing userdata or metadata

3. **Data Protection**
   - Use BouncyCastle for cryptographic operations
   - Never log sensitive information
   - Secure temporary file handling

4. **Process Security**
   - Validate all file paths before execution
   - Sanitize command arguments
   - Prevent command injection

---

## Testing Requirements

### Unit Test Pattern
```csharp
[TestClass]
public class AdbOperatorTests
{
    [TestMethod]
    public void Command_WithValidCommand_ReturnsResponse()
    {
        // Arrange
        var mockAdb = new MockAdbClient();
        var operator = new AdbOperator(mockAdb);

        // Act
        var result = operator.Command("shell getprop", -1, "device123");

        // Assert
        Assert.IsNotNull(result);
        Assert.IsFalse(result.Contains("error"));
    }
}
```

### Test Coverage Requirements
- Unit tests for all core operations
- Mock device connections for testing
- Test error conditions and edge cases
- Test timeout handling
- Integration tests for critical paths

---

## Configuration Management

### Configuration Files
- `app.config` - Application configuration
- `log4net.config` - Logging configuration
- `plugins.xml` - Plugin manifest
- `download-config.xml` - Download settings

### Configuration Pattern
```csharp
public static class Configurations
{
    public static string AdbPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "adb.exe");
    public static string FastbootPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fastboot.exe");
    public static string RescueFailedLogPath => Path.Combine(LogDirectory, "rescue_failed.log");
}
```

---

## Performance Considerations

- Use async/await for I/O operations
- Implement proper cancellation tokens
- Dispose of resources properly (IDisposable pattern)
- Cache device information to reduce queries
- Use background workers for long operations
- Update UI on dispatcher thread

---

## Deployment Considerations

- Target Windows 10+ (x64)
- Include all necessary DLLs in deployment
- Sign executables for Windows SmartScreen
- Create MSI installer
- Support silent installation
- Include uninstaller

---

## Common Patterns to Follow

### Device State Machine
```csharp
public enum DevicePhysicalStateEx
{
    Offline,
    Online,
    Fastboot,
    Recovery,
    EDL,
    Unknown
}
```

### Command Timeout Map
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

## Reference Documentation

- **Task List**: See `IMPLEMENTATION_TASKLIST.md` for complete feature list
- **Command Reference**: See `ADB_FASTBOOT_COMMANDS.md` for all commands
- **Architecture**: See `DECOMPILATION_SUMMARY.md` for system architecture
- **Decompiled Sources**: See `decompiled/` directory for implementation patterns

---

## Questions or Issues?

- Refer to decompiled sources for implementation patterns
- Check existing documentation files for specifications
- Follow the patterns already established in the codebase
- When in doubt, prioritize safety and error handling

---

**Last Updated**: February 20, 2026
**Applies To**: All LMSA implementation files
