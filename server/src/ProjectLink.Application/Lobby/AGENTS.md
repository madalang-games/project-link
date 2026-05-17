# ProjectLink.Application/Lobby

## Files
| file | class | role |
|------|-------|------|
| `LobbyService.cs` | `LobbyService` | Aggregates profile+stamina+currency+progress+streak+event into one response |

## Symbols
| symbol | kind | note |
|--------|------|------|
| `LobbyService.GetAsync` | method | sequential DB reads (profile+stamina+currency+progress+streak state+event); `HighestStageId` derived from `UserProfile.MaxClearedStageId` (not stage_progress.Max) |

## Cross-refs
- Consumed by: `API.Controllers.LobbyController` → `GET /api/lobby`
- Depends on: `IUserProfileRepository`, `IStaminaRepository`, `ICurrencyRepository`, `IProgressRepository`, `IStreakChallengeRepository`, `IStaticDataService`

## Rules
- Streak challenge compact HUD state (EventStatus, RemainingTimeIso, CurrentLevel, HasPendingReward) derived from `IStreakChallengeRepository.GetActiveStateAsync`
- Season event IsActive checked against current UTC time vs event StartAt/EndAt
- `HighestStageId` uses `UserProfile.MaxClearedStageId` — NOT `stage_progress.Max()`
