# ADB and Fastboot Commands Analysis

## Analysis Summary

This document contains all ADB and fastboot commands found in the decompiled .NET binaries from the Lenovo Mobile Software Assistant (LMSA) application.

## Source Files Analyzed

- Software Fix.exe (main executable)
- lenovo.mbg.service.framework.smartdevice.dll
- lenovo.mbg.service.framework.devicemgt.dll
- lenovo.mbg.service.lmsa.phoneManager.dll
- lenovo.mbg.service.lmsa.flash.dll
- And multiple other Lenovo service DLLs and plugins

---

## Fastboot Commands

### Basic Fastboot Operations

The application uses the following fastboot commands through `ProcessRunner.ProcessString()` and `EncapsulationFastbootCommand()` wrapper functions:

1. **getvar all** - Get all device variables
   ```
   ProcessRunner.ProcessList(fullpath, EncapsulationFastbootCommand("getvar all"), 12000)
   ```

2. **oem read_sv** - Read secure version information
   ```
   ProcessRunner.ProcessString(fullpath, EncapsulationFastbootCommand("oem read_sv"), 12000)
   ```

3. **oem partition** - Get partition information
   ```
   ProcessRunner.ProcessString(fullpath, EncapsulationFastbootCommand("oem partition"), 12000)
   ```

4. **oem partition dump logfs** - Dump logfs partition
   ```
   ProcessRunner.ProcessString(fullpath, EncapsulationFastbootCommand("oem partition dump logfs"), 20000)
   ```

5. **oem fb_mode_set** - Set fastboot mode (command can be skipped)

6. **oem fb_mode_clear** - Clear fastboot mode (command can be skipped)

### Fastboot Flash Operations

From `FastbootFlash.cs`:

```csharp
Dictionary<string, int> OperationToTimeout = new Dictionary<string, int>
{
    { "flash", 300000 },      // 5 minutes
    { "erase", 60000 },       // 1 minute
    { "oem", 60000 },         // 1 minute
    { "getvar", 20000 },      // 20 seconds
    { "reboot", 20000 },      // 20 seconds
    { "reboot-bootloader", 10000 },  // 10 seconds
    { "format", 60000 },      // 1 minute
    { "flashall", 600000 },   // 10 minutes
    { "continue", 10000 }     // 10 seconds
};
```

### Fastboot Command Patterns

- **flash [partition] [file]** - Flash partition with file
- **erase [partition]** - Erase partition (particularly: "erase userdata", "erase metadata")
- **format [partition]** - Format partition
- **reboot** - Reboot device
- **reboot-bootloader** - Reboot to bootloader
- **continue** - Continue boot
- **flashall** - Flash all partitions from XML configuration

### Error Handling

The application checks for these error patterns in fastboot responses:
- "STATUS_SEC_VIOLATE_ANTI_ROLLBACK" - Anti-rollback violation
- "error" - Generic error
- "fail" - Generic failure
- Exit code -1073741515 with empty response indicates critical error

---

## ADB Commands

### Device Management Commands

From `AdbOperator.cs`:

1. **Install Package**
   ```
   install -r "[apk_path]"
   ```

2. **Uninstall Package**
   ```
   uninstall [package_name]
   uninstall com.lmsa.lmsaappclient
   uninstall com.motorola.lmsaappclient
   ```

3. **Reboot**
   ```csharp
   adb.Reboot(mode, deviceData)  // modes: bootloader, recovery, etc.
   ```
   
   From AndroidShell.cs:
   ```
   reboot bootloader
   reboot edl  // Emergency Download Mode
   ```

### Shell Commands

1. **Get Device Properties**
   ```
   shell getprop
   shell getprop ro.build.version.sdk
   shell getprop ro.build.version.full
   ```

2. **Activity Manager Commands**
   ```
   shell am start -n [package]/[package].ui.FlashActivity
   shell am start -n [package]/[package].main.start.LaunchActivity
   shell am force-stop [package]
   ```

3. **Window Manager Query**
   ```
   shell "dumpsys window | grep mCurrentFocus"
   ```

4. **Check Permissions** (Android 10+)
   - The app checks device permissions through `CheckPermissions()` method

5. **File Operations**
   ```
   pull [source] [destination]
   ```

### ADB Server Management

```csharp
AdbServer.Instance.StartServer("adb.exe", restartServerIfNewer: true)
```

### Device Serial Number Commands

Commands can be device-specific:
```
adb -s [device_serial] [command]
```

---

## Command Execution Infrastructure

### Wrapper Functions

1. **EncapsulationFastbootCommand()** - Wraps fastboot commands with device serial if needed
2. **ProcessRunner.ProcessString()** - Executes command and returns string output
3. **ProcessRunner.ProcessList()** - Executes command and returns list of lines
4. **DeviceOperator.Command()** - Generic command execution
5. **DeviceOperator.Shell()** - ADB shell command execution

### Timeout Configurations

Commands use various timeouts:
- Standard operations: 12,000 - 60,000 ms
- Flash operations: 300,000 ms (5 minutes)
- Flash all operations: 600,000 ms (10 minutes)

### Device Connection Monitoring

The application monitors:
- ADB device connections
- Fastboot device connections  
- Device physical state (Offline, Online, Fastboot, etc.)

---

## Process Management

The application can kill processes:
```csharp
GlobalFun.KillProcess("adb")
GlobalFun.KillProcess("fastboot")
```

---

## Tool Paths

The application uses these tool executables:
- `adb.exe` - Android Debug Bridge
- `fastboot.exe` - Fastboot tool
- `fastbootmonitor.exe` - Fastboot monitoring tool

---

## Additional Commands Found in Source

From searching all decompiled sources:

### Port Forwarding
```csharp
adb.CreateForward(deviceData, "tcp:[localPort]", "tcp:[devicePort]", allowRebind: false)
adb.RemoveForward(deviceData, localPort)
adb.RemoveAllForwards(deviceData)
```

### File Operations
```csharp
// Push file
syncService.Push(stream, deviceFilePath, 777, DateTime.Now, null, CancellationToken.None)

// Pull file  
pull [source] [destination]
```

### Package Manager
```csharp
PackageManager.InstallPackage(apkPath, reinstall: true)
PackageManager.UninstallPackage(apkName)
```

---

## Command Execution Example

From the decompiled code, here's how commands are typically executed:

```csharp
// Fastboot command
string response = ProcessRunner.ProcessString(
    fullpath,  // fastboot.exe path
    EncapsulationFastbootCommand("getvar all"),
    12000      // timeout in milliseconds
);

// ADB command
string response = DeviceOperator.Command(
    "shell getprop",
    -1,        // timeout (-1 = default)
    deviceId   // device serial
);
```

---

## Error Detection Patterns

The application checks responses for these patterns:
- "error" (case insensitive)
- "fail" (case insensitive)
- "STATUS_SEC_VIOLATE_ANTI_ROLLBACK"
- Empty response with specific exit codes

---

## Summary

This application is the Lenovo Mobile Software Assistant (LMSA), which provides:

1. **Device Rescue/Flashing** - Flash firmware to Lenovo/Motorola devices
2. **Phone Management** - Install/uninstall apps, manage files
3. **Hardware Testing** - Test device hardware components
4. **Backup/Restore** - Backup and restore device data
5. **Data Transfer** - Transfer data between devices
6. **Toolbox Functions** - Various utility functions via plugins

The application extensively uses both ADB and fastboot protocols to communicate with Android devices in various states (normal, fastboot, recovery, EDL mode).

