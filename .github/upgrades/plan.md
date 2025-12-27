# .NET 9 & .NET 10 Multi-Target Upgrade Plan - MauiMicroMvvm Solution

## Table of Contents

- [Executive Summary](#executive-summary)
- [Migration Strategy](#migration-strategy)
- [Detailed Dependency Analysis](#detailed-dependency-analysis)
- [Implementation Timeline](#implementation-timeline)
- [Detailed Execution Steps](#detailed-execution-steps)
- [Project-by-Project Migration Plans](#project-by-project-migration-plans)
- [Package Update Reference](#package-update-reference)
- [Breaking Changes Catalog](#breaking-changes-catalog)
- [Risk Management](#risk-management)
- [Testing & Validation Strategy](#testing--validation-strategy)
- [Complexity & Effort Assessment](#complexity--effort-assessment)
- [Source Control Strategy](#source-control-strategy)
- [Success Criteria](#success-criteria)

---

## Executive Summary

### Scenario Description
Upgrade the MauiMicroMvvm solution from .NET 8 to **multi-target .NET 9 and .NET 10**, including updating NuGet packages and addressing security vulnerabilities. **All .NET 8 targets will be removed.**

### Scope
**Projects Affected**: 3 of 4 projects
- `src\MauiMicroMvvm\MauiMicroMvvm.csproj` (Core library)
- `src\MauiMicroMvvm.Rx\MauiMicroMvvm.Rx.csproj` (Reactive extensions)
- `sample\MauiMicroSample\MauiMicroSample.csproj` (Sample MAUI application)

**Project Not Requiring Upgrade**:
- `src\MauiMicroMvvm.Templates\MauiMicroMvvm.Templates.csproj` (netstandard2.0 - already compatible)

**MSBuild Files Requiring Updates**:
- `Directory.Build.props` - Update DotNetVersion property
- `Directory.Packages.props` - Update 3 package versions

### Current State
- **Total Projects**: 4 (all SDK-style)
- **Current Target Frameworks**: net8.0, net8.0-android, net8.0-ios, net8.0-maccatalyst, net8.0-windows, netstandard2.0
- **Total LOC**: 1,483 lines across 52 files
- **Dependency Depth**: 2 levels
- **NuGet Packages**: 7 total (4 compatible, 3 need updates)

### Target State
- **Class Libraries**: **net9.0;net10.0** (multi-targeting, .NET 8 removed)
- **MAUI Sample**: **net10.0-android;net10.0-ios;net10.0-maccatalyst;net10.0-windows** (all .NET 10, .NET 8 removed)
- **Package Updates**: 3 packages
  - **Microsoft.Maui.Controls**: 8.0.3 ? 10.0.1 (for packable projects)
  - **Microsoft.Maui.Controls**: $(MauiVersion) ? 10.0.1 (for sample app - hardcoded for consistency)
  - **Microsoft.Extensions.Logging.Console**: 8.0.0 ? 10.0.1
  - **Refit**: 7.0.0 ? 9.0.2 (?? **SECURITY VULNERABILITY**)
- **MSBuild Property Updates**:
  - **DotNetVersion**: net8.0 ? net10.0 (in Directory.Build.props)

### Discovered Metrics
| Metric | Value | Assessment |
|--------|-------|------------|
| Project Count | 4 | Small solution |
| Dependency Depth | 2 levels | Simple structure |
| Security Vulnerabilities | 1 (Refit) | Critical - must fix |
| Package Updates | 3 | Minimal impact |
| MSBuild File Updates | 2 | Low complexity |
| LOC to Modify | 0+ | Estimated 0.0% |
| API Breaking Changes | 0 detected | Low risk |

### Complexity Classification
**Classification: Simple**

**Rationale**:
- ? Small solution (?5 projects)
- ? Low dependency depth (?2 levels)
- ? All SDK-style projects
- ? No API compatibility issues
- ? Minimal code changes expected
- ? Clean MSBuild file structure
- ?? One security vulnerability (manageable)
- ?? Multi-targeting adds slight complexity
- ?? Complete removal of .NET 8 targets

### Selected Strategy
**All-At-Once Strategy** - All projects upgraded simultaneously in a single coordinated operation, completely removing .NET 8 support.

**Rationale**:
- Small solution with clear dependency structure
- All projects currently on modern .NET (net8.0)
- Minimal package complexity (57% already compatible)
- Low risk of breaking changes
- Fastest path to completion
- Clean break from .NET 8, supporting only .NET 9 (STS) and .NET 10 (LTS)
- Multi-targeting provides flexibility for consumers choosing between STS and LTS

### Critical Issues
?? **Security Vulnerability**: Refit package (7.0.0) in MauiMicroSample project contains known security vulnerabilities. Must upgrade to 9.0.2 as part of this migration.

### Recommended Approach
All-at-once atomic upgrade: Update all project target frameworks to net9.0;net10.0 for class libraries and net10.0-* for MAUI sample, update packages simultaneously, update MSBuild properties, fix any compilation errors, then validate through build and testing. **Complete removal of .NET 8 support.**

### Iteration Strategy Used
**Fast Batch Approach** (Simple Solution)
- **Phase 1**: Discovery & Classification (complete)
- **Phase 2**: Foundation - strategy, dependency analysis, project stubs (3 iterations)
- **Phase 3**: Batched project details (1-2 iterations)
- **Total Expected Iterations**: 6-7

---

## Migration Strategy

### Approach Selection

**Selected Strategy: All-At-Once**

All projects will be upgraded simultaneously in a single coordinated operation, with complete removal of .NET 8 support.

### Rationale

**Why All-At-Once is Appropriate**:

? **Small Solution**: Only 4 projects (3 requiring upgrade)
? **Simple Dependencies**: Clean 2-level hierarchy, no circular dependencies
? **Modern Foundation**: All projects already on .NET 8 (easy leap to .NET 10)
? **Low Complexity**: 1,483 LOC, minimal expected code changes
? **Package Compatibility**: 71% of packages already compatible
? **SDK-Style Projects**: All projects use modern SDK format
? **Fast Completion**: No multi-targeting complexity, quickest path to .NET 10

?? **Consideration**: One security vulnerability requires immediate attention (supports all-at-once approach)

### All-At-Once Strategy - Specific Considerations

**Atomic Operation Principles**:
1. All project files updated to target frameworks simultaneously
2. All package references updated in a single pass (centrally managed via Directory.Packages.props)
3. Single dependency restore operation
4. Unified build to identify all compilation errors at once
5. Fix all compilation errors in one session
6. Single comprehensive validation

**No Intermediate States**:
- No projects remain on .NET 8 during migration
- No multi-targeting complexity (except MauiMicroSample which requires platform-specific TFMs)
- Clean cut-over from .NET 8 to .NET 10

### Dependency-Based Ordering

While all projects are updated simultaneously, **validation must respect dependency order**:

1. **Foundation First**: MauiMicroMvvm.csproj (no dependencies)
2. **Extensions Second**: MauiMicroMvvm.Rx.csproj (depends on foundation)
3. **Application Last**: MauiMicroSample.csproj (depends on foundation + extensions)

This ensures that if compilation errors occur, they can be addressed from the bottom up.

### Parallel vs Sequential Execution

**File Updates**: Can be performed in parallel (no interdependencies)

**Build Validation**: Must be sequential following dependency order
- Build MauiMicroMvvm first
- Build MauiMicroMvvm.Rx second (requires MauiMicroMvvm DLL)
- Build MauiMicroSample last (requires both DLLs)

**Alternative**: Full solution build will automatically respect dependency order

### Phase Definitions

#### Phase 0: Preparation
**Scope**: Verify prerequisites
- Confirm .NET 10 SDK installed
- Verify no blocking issues in assessment

#### Phase 1: Atomic Upgrade
**Scope**: All projects upgraded simultaneously
- Update all TargetFramework properties
- Update all package versions
- Update MSBuild properties
- Restore dependencies
- Build solution and fix all compilation errors
- Verify 0 build errors

**Deliverables**: 
- All projects target .NET 9.0 and .NET 10.0 (appropriate multi-target frameworks)
- All packages updated to compatible versions
- MSBuild properties updated
- Solution builds successfully with 0 errors

#### Phase 2: Validation & Testing
**Scope**: Comprehensive validation
- Run all tests (if test projects exist)
- Verify no warnings
- Validate dependency resolution
- Confirm security vulnerability resolved

**Deliverables**:
- All tests pass
- No package vulnerabilities
- Clean build output

---

## Implementation Timeline

### Phase 0: Preparation

**Operations**:
- Verify .NET 10 SDK installed on development machine
- Confirm branch `upgrade-to-NET10` is active
- Review assessment findings

**Deliverables**: 
- Environment ready for upgrade

### Phase 1: Atomic Upgrade

**Operations** (performed as single coordinated batch):

1. **Update MSBuild Properties** (in Directory.Build.props)
   - DotNetVersion: `net8.0` ? `net10.0`

2. **Update Project Target Frameworks**
   - MauiMicroMvvm.csproj: `net8.0` ? `net9.0;net10.0`
   - MauiMicroMvvm.Rx.csproj: `net8.0` ? `net9.0;net10.0`
   - MauiMicroSample.csproj: **Remove all .NET 8 targets, add net10.0-* targets**

3. **Update Package References** (in Directory.Packages.props)
   - Microsoft.Maui.Controls: `8.0.3` ? `10.0.1` (packable projects)
   - Microsoft.Maui.Controls: `$(MauiVersion)` ? `10.0.1` (sample app)
   - Microsoft.Extensions.Logging.Console: `8.0.0` ? `10.0.1`
   - Refit: `7.0.0` ? `9.0.2` (?? Security fix)

4. **Restore Dependencies**
   - `dotnet restore` for entire solution

5. **Build Solution and Fix Compilation Errors**
   - Build entire solution
   - Address any compilation errors discovered
   - Rebuild to verify fixes

6. **Verify Success**
   - Solution builds with 0 errors
   - No dependency conflicts
   - Security vulnerability resolved

**Deliverables**: 
- All projects upgraded to .NET 9.0 and .NET 10.0
- All MSBuild properties updated
- Solution builds successfully
- All packages updated

### Phase 2: Validation & Testing

**Operations**:
- Build in Release configuration
- Run tests (if test projects exist)
- Verify no build warnings
- Confirm package vulnerability scan shows no issues

**Deliverables**: 
- All tests pass
- Clean build output
- No security vulnerabilities

---

## Detailed Execution Steps

### Step 1: Verify Prerequisites

**Action**: Confirm .NET 10 SDK is installed

**Validation**:
```bash
dotnet --list-sdks
```
Expected: .NET 10.x.x SDK listed

**If Missing**: Download from https://dotnet.microsoft.com/download/dotnet/10.0

---

### Step 2: Update MSBuild Properties

**Action**: Update DotNetVersion property in Directory.Build.props

**File**: `Directory.Build.props`

**Change**:
```xml
<!-- BEFORE (Line 26) -->
<PropertyGroup>
  <DotNetVersion>net8.0</DotNetVersion>
</PropertyGroup>

<!-- AFTER -->
<PropertyGroup>
  <DotNetVersion>net10.0</DotNetVersion>
</PropertyGroup>
```

**Note**: While this property is not currently used in project files, updating it maintains consistency and prepares for future use.

---

### Step 3: Update Project Target Frameworks

**Action**: Update TargetFramework properties in project files

**Files to Modify**:
1. `src\MauiMicroMvvm\MauiMicroMvvm.csproj`
2. `src\MauiMicroMvvm.Rx\MauiMicroMvvm.Rx.csproj`
3. `sample\MauiMicroSample\MauiMicroSample.csproj`

See [Project-by-Project Migration Plans](#project-by-project-migration-plans) for specific changes.

---

### Step 4: Update Package References

**Action**: Update Directory.Packages.props with new package versions

**File**: `Directory.Packages.props`

**Changes**:
```xml
<!-- BEFORE -->
<PackageVersion Include="Microsoft.Maui.Controls" Version="8.0.3" Condition="$(IsPackable) == 'true'"/>
<PackageVersion Include="Microsoft.Maui.Controls" Version="$(MauiVersion)" Condition="$(IsPackable) != 'true'"/>
<PackageVersion Include="Microsoft.Extensions.Logging.Console" Version="8.0.0" />
<PackageVersion Include="Refit" Version="7.0.0" />

<!-- AFTER -->
<PackageVersion Include="Microsoft.Maui.Controls" Version="10.0.1" Condition="$(IsPackable) == 'true'"/>
<PackageVersion Include="Microsoft.Maui.Controls" Version="10.0.1" Condition="$(IsPackable) != 'true'"/>
<PackageVersion Include="Microsoft.Extensions.Logging.Console" Version="10.0.1" />
<PackageVersion Include="Refit" Version="9.0.2" />
```

**Rationale**: Hardcoding Microsoft.Maui.Controls to 10.0.1 for both packable and non-packable projects ensures:
- Consistency across all projects
- No version conflicts between .NET 9 and .NET 10 multi-targeting
- Single version (10.0.1) supports both .NET 9 and .NET 10

See [Package Update Reference](#package-update-reference) for complete details.

---

### Step 5: Restore Dependencies

**Action**: Restore NuGet packages for entire solution

**Command**:
```bash
dotnet restore MauiMicroMvvm.sln
```

**Expected Outcome**: All packages restore without conflicts

---

### Step 6: Build Solution and Fix Compilation Errors

**Action**: Build entire solution to identify any compilation errors

**Command**:
```bash
dotnet build MauiMicroMvvm.sln --configuration Debug
```

**Expected Outcome**: Solution builds with 0 errors

**If Errors Occur**: See [Breaking Changes Catalog](#breaking-changes-catalog) for guidance on common issues.

**Validation Order** (if building projects individually):
1. Build `src\MauiMicroMvvm\MauiMicroMvvm.csproj`
2. Build `src\MauiMicroMvvm.Rx\MauiMicroMvvm.Rx.csproj`
3. Build `sample\MauiMicroSample\MauiMicroSample.csproj`

---

### Step 7: Verify Success

**Build Verification**:
```bash
dotnet build MauiMicroMvvm.sln --configuration Release
```

**Checklist**:
- [ ] Solution builds with 0 errors
- [ ] Solution builds with 0 warnings (or only expected warnings)
- [ ] All projects restored successfully
- [ ] No package dependency conflicts

---

### Step 8: Run Tests (if applicable)

**Action**: Execute test projects to validate functionality

**Assessment Notes**: No test projects were identified in the assessment. If test projects exist:

```bash
dotnet test MauiMicroMvvm.sln --configuration Release
```

**Expected Outcome**: All tests pass

---

### Step 9: Verify Security Vulnerability Resolution

**Action**: Confirm Refit package vulnerability is resolved

**Check Package Version**:
```bash
dotnet list package --vulnerable
```

**Expected Outcome**: No vulnerabilities reported

---

## Project-by-Project Migration Plans

### Project: MauiMicroMvvm.csproj

**Location**: `src\MauiMicroMvvm\MauiMicroMvvm.csproj`

**Current State**:
- **Target Framework**: net8.0
- **Project Type**: Class Library (SDK-style)
- **Dependencies**: 0 project dependencies
- **Dependants**: 2 (MauiMicroMvvm.Rx, MauiMicroSample)
- **Packages**: Microsoft.Maui.Controls (8.0.3)
- **LOC**: 906 lines across 23 files
- **Risk Level**: ?? Low

**Target State**:
- **Target Frameworks**: net9.0;net10.0

**Migration Steps**:

1. **Update TargetFramework Property**
   
   **File**: `src\MauiMicroMvvm\MauiMicroMvvm.csproj`
   
   **Change**:
   ```xml
   <!-- BEFORE -->
   <TargetFramework>net8.0</TargetFramework>
   
   <!-- AFTER -->
   <TargetFrameworks>net9.0;net10.0</TargetFrameworks>
   ```

2. **Package Updates**
   
   **Microsoft.Maui.Controls**: 8.0.3 ? 10.0.1 (Updated in Directory.Packages.props)
   
   **Reason**: Latest version supporting both .NET 9 and .NET 10
   
   **Impact**: 
   - Single package version works across both target frameworks
   - Access to latest MAUI features and bug fixes
   - Improved performance and compatibility

3. **Expected Breaking Changes**
   
   ? None identified in assessment
   - No API compatibility issues detected
   - 0% estimated LOC impact
   - MAUI 8.0.3 ? 10.0.1 maintains backward compatibility

4. **Code Modifications**
   
   ? None expected
   - Assessment shows 0 files with incidents requiring code changes
   - No obsolete API usage detected
   - Review for any MAUI-specific deprecated warnings after build

5. **Validation**
   
   **Build Command**:
   ```bash
   dotnet build src\MauiMicroMvvm\MauiMicroMvvm.csproj --configuration Debug
   ```
   
   **Success Criteria**:
   - [ ] Builds without errors
   - [ ] Builds without warnings
   - [ ] Package restore successful
   - [ ] Output DLL targets net9.0 and net10.0

**Notes**: 
- This is the foundation library with no dependencies
- Should be validated first in the dependency chain
- Two projects depend on this, so successful build is critical

---

### Project: MauiMicroMvvm.Rx.csproj

**Location**: `src\MauiMicroMvvm.Rx\MauiMicroMvvm.Rx.csproj`

**Current State**:
- **Target Framework**: net8.0
- **Project Type**: Class Library (SDK-style)
- **Dependencies**: 1 (MauiMicroMvvm)
- **Dependants**: 1 (MauiMicroSample)
- **Packages**: ReactiveUI (20.1.63)
- **LOC**: 103 lines across 3 files
- **Risk Level**: ?? Low

**Target State**:
- **Target Frameworks**: net9.0;net10.0

**Migration Steps**:

1. **Update TargetFramework Property**
   
   **File**: `src\MauiMicroMvvm.Rx\MauiMicroMvvm.Rx.csproj`
   
   **Change**:
   ```xml
   <!-- BEFORE -->
   <TargetFramework>net8.0</TargetFramework>
   
   <!-- AFTER -->
   <TargetFrameworks>net9.0;net10.0</TargetFrameworks>
   ```

2. **Package Updates**
   
   ? ReactiveUI 20.1.63 - Already compatible (no update needed per assessment)

3. **Expected Breaking Changes**
   
   ? None identified in assessment
   - No API compatibility issues detected
   - 0% estimated LOC impact

4. **Code Modifications**
   
   ? None expected
   - Assessment shows 0 files with incidents requiring code changes
   - No obsolete API usage detected

5. **Validation**
   
   **Build Command**:
   ```bash
   dotnet build src\MauiMicroMvvm.Rx\MauiMicroMvvm.Rx.csproj --configuration Debug
   ```
   
   **Success Criteria**:
   - [ ] Builds without errors
   - [ ] Builds without warnings
   - [ ] Package restore successful
   - [ ] Project reference to MauiMicroMvvm resolves correctly
   - [ ] Output DLL targets net9.0 and net10.0

**Notes**: 
- Depends on MauiMicroMvvm (must build after foundation library)
- Extends core library with ReactiveUI support
- Small codebase reduces risk

---

### Project: MauiMicroSample.csproj

**Location**: `sample\MauiMicroSample\MauiMicroSample.csproj`

**Current State**:
- **Target Frameworks**: net8.0-android; net8.0-ios; net8.0-maccatalyst; net8.0-windows10.0.19041.0
- **Project Type**: .NET MAUI Application (SDK-style)
- **Dependencies**: 2 (MauiMicroMvvm, MauiMicroMvvm.Rx)
- **Dependants**: 0 (top-level application)
- **Packages**: 
  - Microsoft.Maui.Controls (version from $(MauiVersion) variable)
  - Microsoft.Extensions.Logging.Console (8.0.0 ? 10.0.1)
  - Refit (7.0.0 ? 9.0.2 ?? SECURITY)
- **LOC**: 474 lines across 26 files
- **Risk Level**: ?? Low (?? Security vulnerability present)

**Target State**:
- **Target Frameworks**: net10.0-android;net10.0-ios;net10.0-maccatalyst;net10.0-windows

**Strategy**: Multi-targeting approach - maintain existing .NET 8 mobile platforms and add .NET 10 Windows support. **All other targets removed.**

**Migration Steps**:

1. **Update TargetFrameworks Property**
   
   **File**: `sample\MauiMicroSample\MauiMicroSample.csproj`
   
   **Change**:
   ```xml
   <!-- BEFORE -->
   <TargetFrameworks>net8.0-android;net8.0-ios;net8.0-maccatalyst;net8.0-windows10.0.19041.0</TargetFrameworks>
   
   <!-- AFTER -->
   <TargetFrameworks>net10.0-android;net10.0-ios;net10.0-maccatalyst;net10.0-windows</TargetFrameworks>
   ```
   
   **Rationale**: 
   - Assessment proposes keeping only net10.0-* targets
   - Removes all .NET 8 targets as part of upgrade
   - Prepares project for .NET 10 only support

2. **Package Updates**
   
   Update in `Directory.Packages.props`:
   
   | Package | Current | Target | Reason |
   |---------|---------|--------|--------|
   | Microsoft.Maui.Controls | $(MauiVersion) | 10.0.1 | .NET 9 & 10 support, consistency (dynamic?hardcoded) |
   | Microsoft.Extensions.Logging.Console | 8.0.0 | 10.0.1 | Framework compatibility |
   | Refit | 7.0.0 | 9.0.2 | ?? **SECURITY VULNERABILITY** |
   
   **Critical Notes**:
   - **Refit 7.0.0** contains known security vulnerabilities. Upgrade to 9.0.2 is mandatory.
   - **Microsoft.Maui.Controls** changed from dynamic `$(MauiVersion)` to hardcoded `10.0.1` for consistency and to prevent version conflicts in multi-targeting scenario.

3. **Expected Breaking Changes**
   
   **Refit 7.0.0 ? 9.0.2**:
   - Review Refit changelog for API changes between versions
   - Potential changes in HTTP client configuration
   - Possible changes in attribute usage or interface definitions
   
   **Microsoft.Extensions.Logging.Console 8.0.0 ? 10.0.1**:
   - Generally backward compatible
   - Possible configuration format changes (review if custom logging configuration exists)

4. **Code Modifications**
   
   **Areas to Review**:
   - Refit interface definitions (check for deprecated attributes)
   - HTTP client setup and configuration
   - Logging configuration (if customized)
   
   **Assessment Notes**: 1 file with incidents reported - review after build for specific guidance

5. **Conditional Compilation** (if needed)
   
   If code needs to differ between .NET versions:
   ```csharp
   #if NET10_0_OR_GREATER
       // .NET 10+ specific code
   #elif NET9_0_OR_GREATER
       // .NET 9+ specific code
   #else
       // .NET 8 code
   #endif
   ```

6. **Validation**
   
   **Build Command**:
   ```bash
   dotnet build sample\MauiMicroSample\MauiMicroSample.csproj --configuration Debug
   ```
   
   **Success Criteria**:
   - [ ] Builds without errors for all target frameworks
   - [ ] Builds without warnings
   - [ ] Package restore successful
   - [ ] Project references to MauiMicroMvvm and MauiMicroMvvm.Rx resolve correctly
   - [ ] Refit interfaces compile successfully
   - [ ] Multi-targeting produces correct outputs for each TFM

**Notes**: 
- This is the top-level application (no dependants)
- Must build after both dependency libraries
- Contains the security vulnerability that must be addressed
- Multi-targeting adds complexity but maintains broad platform support

---

### Project: MauiMicroMvvm.Templates.csproj

**Location**: `src\MauiMicroMvvm.Templates\MauiMicroMvvm.Templates.csproj`

**Current State**:
- **Target Framework**: netstandard2.0
- **Project Type**: Class Library (SDK-style)
- **Dependencies**: 0
- **Dependants**: 0

**Target State**:
- **No changes required** ?

**Migration Steps**:
- ? **None** - netstandard2.0 is compatible with all .NET versions including .NET 10

**Notes**: 
- This project serves as a template package
- netstandard2.0 ensures maximum compatibility
- No action required during upgrade

---

## Package Update Reference

### Overview

The solution uses **Central Package Management** via `Directory.Packages.props`. All package version updates are made in this single file, automatically applying to all projects that reference these packages.

### Package Updates Required

| Package | Current Version | Target Version | Projects Affected | Update Reason | Priority |
|---------|----------------|----------------|-------------------|---------------|----------|
| Microsoft.Maui.Controls | 8.0.3 | 10.0.1 | MauiMicroMvvm (packable) | .NET 9 & 10 support, latest features | High |
| Microsoft.Maui.Controls | $(MauiVersion) | 10.0.1 | MauiMicroSample (dynamic?hardcoded) | Consistency, .NET 9 & 10 support | High |
| Microsoft.Extensions.Logging.Console | 8.0.0 | 10.0.1 | MauiMicroSample | Framework compatibility, latest features | Medium |
| Refit | 7.0.0 | 9.0.2 | MauiMicroSample | ?? **SECURITY VULNERABILITY** | ?? **CRITICAL** |

### Compatible Packages (No Update Needed)

| Package | Version | Projects | Status |
|---------|---------|----------|--------|
| ReactiveUI | 20.1.63 | MauiMicroMvvm.Rx | ? Compatible with .NET 9 & 10 |
| Nerdbank.GitVersioning | 3.6.133 | All projects (global) | ? Compatible |
| Microsoft.SourceLink.GitHub | 8.0.0 | All projects (global) | ? Compatible |

### Update Implementation

**File**: `Directory.Packages.props`

**Complete Change Set**:

```xml
<Project>
  <ItemGroup>
    <!-- UPDATE: 8.0.3 ? 10.0.1 for .NET 9 & 10 support (packable projects) -->
    <PackageVersion Include="Microsoft.Maui.Controls" Version="10.0.1" Condition="$(IsPackable) == 'true'"/>
    
    <!-- UPDATE: $(MauiVersion) ? 10.0.1 for consistency (sample app) -->
    <PackageVersion Include="Microsoft.Maui.Controls" Version="10.0.1" Condition="$(IsPackable) != 'true'"/>
    
    <!-- NO CHANGE - Already compatible -->
    <PackageVersion Include="ReactiveUI" Version="20.1.63" />
    
    <!-- UPDATE: 8.0.0 ? 10.0.1 -->
    <PackageVersion Include="Microsoft.Extensions.Logging.Console" Version="10.0.1" />
    
    <!-- UPDATE: 7.0.0 ? 9.0.2 (SECURITY FIX) -->
    <PackageVersion Include="Refit" Version="9.0.2" />
  </ItemGroup>

  <ItemGroup>
    <GlobalPackageReference Include="Nerdbank.GitVersioning" Version="3.6.133" />
    <GlobalPackageReference Include="Microsoft.SourceLink.GitHub" Version="8.0.0" />
  </ItemGroup>
</Project>
```

### Package-Specific Update Details

#### Microsoft.Maui.Controls (8.0.3 ? 10.0.1 & $(MauiVersion) ? 10.0.1)

**Reason**: Update to latest version supporting .NET 9 and .NET 10

**Key Changes**:
- **Packable projects** (MauiMicroMvvm): 8.0.3 ? 10.0.1
- **Sample app** (MauiMicroSample): $(MauiVersion) ? 10.0.1 (from dynamic to hardcoded)

**Why Hardcode for Sample App**:
- **Consistency**: Single version (10.0.1) across all projects prevents version conflicts
- **Multi-targeting**: 10.0.1 supports both .NET 9 and .NET 10 simultaneously
- **Predictability**: No dependency on MAUI workload version ($(MauiVersion))
- **Simplicity**: Easier troubleshooting and dependency management

**Breaking Changes**: 
- Generally backward compatible from 8.x to 10.x
- Review MAUI 9 and 10 release notes for behavioral changes
- New features and performance improvements

**Validation**:
- Verify all MAUI controls render correctly
- Test navigation and lifecycle events
- Check for deprecated API warnings

**References**:
- [.NET MAUI 9 Release Notes](https://learn.microsoft.com/en-us/dotnet/maui/whats-new/dotnet-9)
- [.NET MAUI 10 Release Notes](https://learn.microsoft.com/en-us/dotnet/maui/whats-new/dotnet-10)

---

#### Microsoft.Extensions.Logging.Console (8.0.0 ? 10.0.1)

**Reason**: Update to match .NET 10 framework version

**Breaking Changes**: 
- Generally backward compatible
- May include new logging features and performance improvements
- Configuration schema likely unchanged

**Validation**:
- Verify logging still works in MauiMicroSample application
- Check console output format (may have minor improvements)

**References**:
- [Release Notes](https://github.com/dotnet/runtime/releases)

---

#### Refit (7.0.0 ? 9.0.2) ?? CRITICAL

**Reason**: **Security vulnerability in version 7.0.0**

**Known Breaking Changes** (7.x ? 9.x):
- Refit 8.0+ requires .NET 6 minimum (compatible with .NET 10)
- Possible changes to attribute behavior
- HTTP client configuration may have new options
- Serialization improvements (System.Text.Json focus)

**Potential Code Impact**:
- Review Refit interface definitions in MauiMicroSample
- Check for deprecated attributes or parameters
- Verify HTTP client factory configuration
- Test API calls after upgrade

**Validation**:
- Compile and verify no errors in Refit interface definitions
- Test HTTP API calls to external services
- Verify error handling still works correctly

**References**:
- [Refit Changelog](https://github.com/reactiveui/refit/releases)
- Security Advisory: (specific CVE to be confirmed via `dotnet list package --vulnerable`)

**Migration Checklist**:
- [ ] Update version in Directory.Packages.props
- [ ] Restore packages
- [ ] Build MauiMicroSample project
- [ ] Review compiler warnings related to Refit
- [ ] Test API endpoints
- [ ] Verify security scan shows no vulnerabilities

---

#### Microsoft.Maui.Controls (8.0.3 ? 10.0.1)

**Reason**: Ensure compatibility with .NET 10 and take advantage of new features

**Breaking Changes**: 
- Generally backward compatible
- May include new controls, properties, or methods
- XAML parser changes could affect resource dictionaries

**Validation**:
- Test all UI controls in MauiMicroSample
- Verify custom renderers or effects still function
- Ensure resources and styles are applied correctly in XAML

**References**:
- [MAUI Migration Guide](https://docs.microsoft.com/en-us/dotnet/maui/migration/)

---

## Breaking Changes Catalog

### Framework Breaking Changes (.NET 8 ? .NET 10)

**Assessment Finding**: 0 API compatibility issues detected

**General Considerations**:
- .NET 9 and .NET 10 maintain strong backward compatibility with .NET 8
- Most breaking changes are opt-in or affect edge cases
- No project-specific breaking changes identified in assessment

**Areas to Monitor**:
1. **ASP.NET Core** (if applicable): Configuration changes, middleware updates
2. **Serialization**: System.Text.Json improvements may affect JSON handling
3. **Performance**: Optimizations may change timing-sensitive code
4. **APIs**: Deprecated APIs may have warnings (but still functional)

**Assessment Confidence**: High - 0 binary/source incompatibilities detected

---

### Package Breaking Changes

#### Refit 7.0.0 ? 9.0.2

**Potential Breaking Changes**:

1. **Attribute Changes**
   - Review usage of `[Get]`, `[Post]`, `[Put]`, `[Delete]` attributes
   - Check for deprecated parameters in attributes
   
2. **HTTP Client Configuration**
   - Verify `RefitSettings` initialization
   - Check custom serializer configuration
   
3. **Return Types**
   - Refit 8+ improved `Task<T>` and `IObservable<T>` support
   - Verify async patterns still compile
   
4. **Serialization**
   - Default serializer may have changed (likely System.Text.Json)
   - Custom JsonSerializerSettings may need updates

**Mitigation**:
- Review Refit interface definitions after upgrade
- Compiler will catch most breaking changes
- Test API calls thoroughly

---

#### Microsoft.Extensions.Logging.Console 8.0.0 ? 10.0.1

**Potential Breaking Changes**:

? **Low Risk** - Generally backward compatible

**Considerations**:
- New logging formatters may be available
- Configuration schema should remain compatible
- Console output format may have minor improvements

**Mitigation**:
- Minimal code changes expected
- Test console logging output
- Review any custom logging configuration

---

#### Microsoft.Maui.Controls 8.0.3 ? 10.0.1

**Potential Breaking Changes**:

- New default XML namespace in XAML: `xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"`
- Changes to control templates or styles may affect appearance
- Obsolete members may generate warnings

**Mitigation**:
- Review and resolve any XAML namespace or control/template changes
- Update any obsolete API usages in code-behind or templates
- Test UI thoroughly

---

### Code Modification Patterns

**If Compilation Errors Occur**:

1. **Obsolete API Usage**
   ```csharp
   // Compiler will show: Warning/Error CS0618: 'X' is obsolete: 'Use Y instead'
   // Action: Replace with suggested alternative
   ```

2. **Namespace Changes**
   ```csharp
   // Compiler will show: Error CS0246: Type or namespace 'X' could not be found
   // Action: Add missing using directive or update namespace
   ```

3. **Type Compatibility**
   ```csharp
   // Compiler will show: Error CS1503: Cannot convert from 'X' to 'Y'
   // Action: Review type changes in package updates
   ```

**Expected Code Changes**: 0+ lines (assessment estimates 0.0% of codebase)

---

## Risk Management

### Risk Assessment

| Risk Category | Level | Description | Mitigation |
|---------------|-------|-------------|------------|
| **Security Vulnerability** | ?? High | Refit 7.0.0 contains known vulnerabilities | Mandatory upgrade to 9.0.2 as part of migration |
| **Breaking Changes** | ?? Low | 0 API compatibility issues detected | Follow breaking changes catalog; compiler will catch issues |
| **Build Failures** | ?? Low | All SDK-style projects, clean dependencies | Dependency-ordered validation; Git rollback available |
| **Package Conflicts** | ?? Low | 57% packages already compatible | Central package management simplifies resolution |
| **Multi-Targeting Complexity** | ?? Medium | MauiMicroSample adds net10.0-windows | Test all target frameworks; conditional compilation if needed |
| **Testing Coverage** | ?? Medium | No test projects identified in assessment | Manual validation required; recommend adding tests |

### High-Risk Changes

| Project | Risk Level | Description | Mitigation Strategy |
|---------|------------|-------------|---------------------|
| MauiMicroSample | ?? Critical | Refit security vulnerability | Upgrade to 9.0.2, test all API calls, verify no vulnerabilities remain |
| MauiMicroSample | ?? Medium | Multi-targeting complexity | Build all TFMs, use conditional compilation if needed |

### Security Vulnerabilities

**Package**: Refit
**Current Version**: 7.0.0
**Vulnerable**: Yes ??
**Remediation**: Upgrade to 9.0.2
**CVE**: (Run `dotnet list package --vulnerable` for specific CVE details)

**Verification After Upgrade**:
```bash
dotnet list package --vulnerable
