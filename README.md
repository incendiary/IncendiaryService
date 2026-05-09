# IncendiaryService

A Windows service written in C# that provisions a local user account and adds it to specified local groups. Built as a lightweight persistence technique for use during authorised red team engagements.

> **Legal Notice:** This tool is intended solely for use by security professionals during authorised penetration tests and red team engagements. Deploying this against systems without explicit written authorisation is illegal. The author assumes no liability for misuse.

---

## Overview

IncendiaryService installs as a Windows service via [Topshelf](https://github.com/Topshelf/Topshelf). On start, it:

1. Checks whether the configured local user account exists.
2. Creates the account with a randomly generated password if it does not.
3. Adds the account to the local Administrators group.
4. Optionally adds the account to additional groups specified in the configuration file.

All actions are logged to `C:\log.log`.

The service reads its configuration from `C:\config.cfg` at startup, falling back to compiled defaults if the file is absent. This allows the configuration to be staged on the target independently of the binary.

---

## Requirements

- Windows (tested on Windows 10 / Server 2019+)
- .NET Framework 4.7.2
- Local Administrator privileges for installation
- Visual Studio 2022 (for building from source)

---

## Building

1. Open `IncendiaryService.sln` in Visual Studio 2022.
2. Restore NuGet packages.
3. Build in **Release** configuration.
4. The output binary is located at `IncendiaryService\bin\Release\IncendiaryService.exe`.

---

## Installation

Use the Topshelf command-line interface to install and manage the service.

```powershell
# Install the service
.\IncendiaryService.exe install

# Start the service
.\IncendiaryService.exe start

# Stop and uninstall the service
.\IncendiaryService.exe stop
.\IncendiaryService.exe uninstall
```

---

## Configuration

The service reads `C:\config.cfg` on startup. If the file does not exist, compiled defaults are used.

| Key | Default | Description |
|---|---|---|
| `ServiceName` | `IncendiaryUserService` | Windows service name |
| `SamAccountName` | `Incendiary` | SAM account name for the local user |
| `Name` | `Incendiary User` | Display name for the local user |
| `RandomPassword` | *(auto-generated)* | Password for the account; omit to generate a random 16-character password |
| `AdditionalGroups` | `Administrators` | Comma-separated list of local groups to add the user to |
| `CleanupOnStop` | `false` | When `true`, removes the user account when the service is stopped |
| `UseEventLog` | `false` | When `true`, writes all log messages to the Windows Application event log under source `IncendiaryUserService` |

Example `C:\config.cfg`:

```
ServiceName = IncendiaryUserService
SamAccountName = Incendiary
Name = Incendiary User
AdditionalGroups = Administrators, Remote Desktop Users
```

> Omitting `RandomPassword` is recommended. The generated password is logged to `C:\log.log`.

---

## Roadmap

| # | Status | Description |
|---|---|---|
| [#4](https://github.com/incendiary/IncendiaryService/issues/4) | ✅ Done | Secret sanitisation — git history and source audited, no credentials found |
| [#5](https://github.com/incendiary/IncendiaryService/issues/5) | ✅ Done | Professional README with legal disclaimer and structured docs |
| [#6](https://github.com/incendiary/IncendiaryService/issues/6) | ✅ Done | `.editorconfig` — Microsoft C# naming and formatting conventions |
| [#7](https://github.com/incendiary/IncendiaryService/issues/7) | ✅ Done | `.pre-commit-config.yaml` — GitLeaks secret scanning + dotnet format |
| [#11](https://github.com/incendiary/IncendiaryService/issues/11) | 🔲 Open | Dependency audit — check NuGet packages for CVEs |
| [#12](https://github.com/incendiary/IncendiaryService/issues/12) | 🔲 Open | Code quality — remove duplication and dead code in `Program.cs` |
| [#13](https://github.com/incendiary/IncendiaryService/issues/13) | 🔲 Open | Add integration tests for service provisioning logic |
| [#8](https://github.com/incendiary/IncendiaryService/issues/8) | 🔲 Open | Migrate to SDK-style `.csproj` targeting modern .NET |
| [#9](https://github.com/incendiary/IncendiaryService/issues/9) | 🔲 Open | Configurable account cleanup on service stop |
| [#10](https://github.com/incendiary/IncendiaryService/issues/10) | 🔲 Open | Optional Windows Event Log output alongside file logging |

---

> **Note on AI assistance:** Claude Code was used heavily during the preparation of this repository for public release — for the security audit, tooling setup, and documentation. Things should work, but some elements (particularly the pre-commit hooks and dotnet format configuration) haven't been fully end-to-end tested in a live Windows environment. PRs and fixes are very welcome.
