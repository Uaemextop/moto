# Lenovo Mobile Software Assistant (LMSA) Decompilation Analysis

## Overview

This repository contains the analysis and documentation of the Lenovo Mobile Software Assistant (LMSA), also known as "Software Fix". The application was decompiled to extract and document all ADB and fastboot commands used by the program.

## Analysis Completed

✅ Downloaded and extracted ZIP file from Dropbox
✅ Decompiled all .NET DLLs and EXEs using ILSpy
✅ Decompiled DLLs in the plugins folder
✅ Extracted and documented all fastboot commands
✅ Extracted and documented all ADB commands
✅ Created comprehensive analysis reports

## Documentation Files

- **[ADB_FASTBOOT_COMMANDS.md](ADB_FASTBOOT_COMMANDS.md)** - Detailed reference of all ADB and fastboot commands found in the application
- **[DECOMPILATION_SUMMARY.md](DECOMPILATION_SUMMARY.md)** - Complete analysis summary including architecture, security observations, and technical details

## Key Findings

### Application Purpose
Lenovo Mobile Software Assistant is legitimate software for:
- Rescuing/flashing firmware to Lenovo/Motorola devices
- Managing phone content (apps, files, backups)
- Hardware testing
- Data transfer between devices

### Commands Found

**Fastboot Commands:**
- getvar all, oem read_sv, oem partition
- flash, erase, format, flashall
- reboot, reboot-bootloader, continue
- oem partition dump logfs

**ADB Commands:**
- install, uninstall
- shell getprop, shell am (activity manager)
- pull, push (file operations)
- reboot bootloader, reboot edl
- Port forwarding operations

### Decompilation Details

- **Tool Used**: ILSpy v9.1.0.7988
- **Files Analyzed**: 97 DLLs and EXEs
- **Programming Language**: C# (.NET Framework/WPF)
- **Architecture**: Modular plugin system

## Source Download

Original files downloaded from:
```
https://www.dropbox.com/scl/fi/3kuolvy11282n0q0pw99b/examples.zip?rlkey=1rldjq71q8igq0ban9lg9homn&st=07i1mewd&dl=1
```

## Analysis Date

February 18, 2026
