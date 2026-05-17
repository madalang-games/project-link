# ProjectLink.Domain/Entities

## Files
| file | class | table |
|------|-------|-------|
| `Session.cs` | `Session` | `sessions` |
| `StageProgress.cs` | `StageProgress` | `stage_progress` (PK: user_id+stage_id) |
| `StageBestRecord.cs` | `StageBestRecord` | `stage_best_records` (PK: user_id+stage_id) |
| `ClientMeta.cs` | `ClientMeta` | `client_meta` |
| `UserCurrency.cs` | `UserCurrency` | `user_currency` |
| `CurrencyLog.cs` | `CurrencyLog` | `currency_logs` (append-only) |
| `StaminaState.cs` | `StaminaState` | `stamina_state` |
| `Inventory.cs` | `Inventory` | `inventory` (PK: user_id+item_id) |
| `UserProfile.cs` | `UserProfile` | `user_profiles` |
| `UserRankingCache.cs` | `UserRankingCache` | `user_ranking_cache` |
| `ActionLog.cs` | `ActionLog` | `action_logs` (append-only) |
| `PlayerSettings.cs` | `PlayerSettings` | `player_settings` |
| `StreakChallengeUserState.cs` | `StreakChallengeUserState` | `streak_challenge_user_state` (PK: user_id+event_id+cycle_id) |
| `StreakChallengeUserLevelState.cs` | `StreakChallengeUserLevelState` | `streak_challenge_user_level_state` (PK: user_id+event_id+cycle_id+level_index) |
| `StreakChallengeRewardClaimHistory.cs` | `StreakChallengeRewardClaimHistory` | `streak_challenge_reward_claim_history` (correlation-id dedup) |
| `StreakChallengeAdRewardHistory.cs` | `StreakChallengeAdRewardHistory` | `streak_challenge_ad_reward_history` (one-per-level ad claim) |

## Symbols
| symbol | kind | note |
|--------|------|------|
| `UserProfile.AvatarId` | property | default=1; shown in ranking entries and lobby |
| `UserProfile.MaxClearedStageId` | property | highest sequentially-cleared stage (0 = none); updated by atomic conditional UPDATE in `StageEndTransactionRepository` |
| `StreakChallengeUserState.EventStatus` | property | `ACTIVE` / `COMPLETED` / `EXPIRED`; mutated by lazy reset |
| `StreakChallengeUserState.ExpiresAt` | property | UTC expiry for the 24H cycle |
| `StreakChallengeUserLevelState.LevelStatus` | property | `LOCKED` / `READY` / `STARTED` / `COMPLETED` |
| `StreakChallengeUserLevelState.CurrentCount` | property | stage clears counted toward required threshold |
| `PlayerSettings.Language` | property | default="EN"; ISO 639-1 code |

## Cross-refs
- Mapped by: server `Infrastructure.Persistence.AppDbContext.OnModelCreating`
- Used by: all `Infrastructure.Persistence.*Repository` classes

## Rules
- Plain C# POCOs — no validation logic, no domain methods
- `StreakChallengeUserState.EventStatus` is lazily reset on read: expired cycles are marked EXPIRED and a fresh INACTIVE state is returned
- Append-only tables (`currency_logs`, `action_logs`): never UPDATE or DELETE rows
