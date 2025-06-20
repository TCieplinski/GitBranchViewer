# 📘 Git Branch Viewer - Documentation (v1.0.0)

## 🧩 Project Structure Overview

- `/Models` - Configuration and domain models
- `/Services` - Git CLI interface, branch comparison logic, configuration loader
- `/Utils` - Helpers like path expansion
- `/Pages` - Planned Blazor UI
- `/Shared` - Optional Blazor layout/components
- `/Data` - Reserved for data services or local caching
- `/wwwroot` - Static files (JS, CSS)
- `/docs` - Project documentation (this file, project structure)

## 🔧 Core Components

### `GitService.cs`
- Runs `git` commands via `ProcessStartInfo`
- Supports: listing branches, diffing, commit log

### `BranchComparisonService.cs`
- Wraps `GitService` for high-level diff operations
- Provides changed files, commit differences, file diff text

### `ConfigLoader.cs`
- Loads `GitViewerSettings` from `appsettings.json`
- Expands `~` to home using `FileHelper`

### `GitRepositoryConfig.cs`
- Maps repo name → local path → branch types
- Defines diff display behavior (`DiffOptions`)

### `FileHelper.cs`
- Expands `~` to user home across OS platforms

### `Program.cs`
- Console UI for selecting repo and branches
- Runs comparisons with summary output

## ⚙️ Configuration File

### `appsettings.json` example

    {
      "GitRepositories": [
        {
          "Name": "GitBranchViewer",
          "Path": "~/git_sources/TestRepo",
          "BranchTypes": {
            "main": "Documentation",
            "env/dev": "Development"
          }
        }
      ],
      "DefaultDiffOptions": {
        "IgnoreWhitespace": true,
        "ShowOnlyChangedFiles": true
      }
    }

## 🚀 Status

- ✅ CLI version operational
- 🔜 Blazor UI in planning
- 🔐 Compliant with LawBook v6.2.0

## 🛣 Roadmap

- Blazor-based branch and diff viewer
- Commit details with optional syntax highlight
- Repo filtering, pinning, and snapshot saving

