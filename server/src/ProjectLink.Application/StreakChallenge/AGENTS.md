# ProjectLink.Application/StreakChallenge

## Files
| file | class | role |
|------|-------|------|
| `StreakChallengeService.cs` | `StreakChallengeService` | 24H streak challenge: state query, activation, level start, stage result processing, reward claim |

## Symbols
| symbol | kind | note |
|--------|------|------|
| `StreakChallengeService.GetStateAsync` | method | lazy-resets expired cycle; returns INACTIVE placeholder with level config when no active cycle |
| `StreakChallengeService.ActivateAsync` | method | creates new 24H cycle; idempotent (returns existing state if already ACTIVE/COMPLETED) |
| `StreakChallengeService.StartLevelAsync` | method | transitions READY level to STARTED; idempotent on re-call |
| `StreakChallengeService.ProcessStageResultAsync` | method | called by `StageService.EndAsync`; counts first-clears only; returns `NavigationDirective` for client routing |
| `StreakChallengeService.ClaimRewardAsync` | method | marks level reward as CLAIMED; deduplicates via `CorrelationId`; grants soft currency/items |
| `StreakChallengeService.ClaimRewardWithAdAsync` | method | same as ClaimReward but applies `AdMultiplier`; one-per-level guard via ad_reward_history |

## Cross-refs
- Consumed by: `API.Controllers.StreakChallengeController`, `Application.Stage.StageService` (ProcessStageResultAsync)
- Depends on: `IStreakChallengeRepository`, `IStreakChallengeTransaction`, `IStaticDataService`, `Domain.Utilities.IdHelper`

## Rules
- Lazy Reset: check expiry at every public entry point; expired → mark EXPIRED, return INACTIVE
- Idempotency: ActivateAsync, StartLevelAsync, ClaimRewardAsync all safe to call twice
- `ProcessStageResultAsync`: only first-clear main stages count; duplicate stageId in same cycle excluded via `LastCountedStageId`
- All errors expressed as `ExclusionReason` string (not thrown exceptions); use error codes from `error_messages.csv`
