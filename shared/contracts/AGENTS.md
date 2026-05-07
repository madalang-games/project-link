# shared/contracts — Shared C# DTO Contracts

## Overview
Language-agnostic DTO definitions written directly in C#.
Both server (ASP.NET Core) and client (Unity) consume these classes.

## Project
`ProjectLink.Contracts.csproj` — targets `netstandard2.1` (Unity compatible)

## Structure
```
shared/contracts/
  [Domain]/
    [Domain]Requests.cs    e.g. StageRequests.cs
    [Domain]Responses.cs   e.g. StageResponses.cs
```

## Rules
- Target `netstandard2.1` — no net8.0-only APIs
- Only POCOs: public properties, no logic, no dependencies
- Nullable enabled — use `Type?` for optional fields
- Namespace: `ProjectLink.Contracts.[Domain]`
- Server: references via `ProjectReference` in `ProjectLink.API.csproj`
- Unity: copy `.cs` files to `client/project-link/Assets/Scripts/Generated/Contracts/`

## Adding a new domain
1. Create `[Domain]/` subdirectory
2. Add `[Domain]Requests.cs` and/or `[Domain]Responses.cs`
3. Copy updated `.cs` files to Unity Assets (manual or via script)
4. Update `## Files` and `## Symbols` below

## Files
| file | namespace | role |
|------|-----------|------|
| _(none yet)_ | — | — |

## Symbols
| symbol | kind | note |
|--------|------|------|
| _(none yet)_ | — | — |
