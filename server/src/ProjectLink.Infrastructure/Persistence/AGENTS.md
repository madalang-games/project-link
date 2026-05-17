# ProjectLink.Infrastructure/Persistence

## Files
| file | class | role |
|------|-------|------|
| `AppDbContext.cs` | `AppDbContext` | EF Core DbContext — entity mapping via `OnModelCreating` |
| `SessionRepository.cs` | `SessionRepository` | Session CRUD |
| `UserProfileRepository.cs` | `UserProfileRepository` | User profile upsert/read |
| `ProgressRepository.cs` | `ProgressRepository` | Stage progress batch read/write |
| `CurrencyRepository.cs` | `CurrencyRepository` | Soft currency grant/deduct with FOR UPDATE + audit log |
| `StaminaRepository.cs` | `StaminaRepository` | Stamina read/deduct/add with lazy recharge and FOR UPDATE |
| `InventoryRepository.cs` | `InventoryRepository` | Inventory grant/deduct |
| `RankingRepository.cs` | `RankingRepository` | Best records + ranking cache CRUD |
| `PlayerSettingsRepository.cs` | `PlayerSettingsRepository` | Player settings get-or-default + upsert |
| `StageEndTransactionRepository.cs` | `StageEndTransactionRepository` | Atomic stage-end TX (progress, best, stamina, currency, ranking); exposes `IsFirstClear` |
| `StaminaRefillTransactionRepository.cs` | `StaminaRefillTransactionRepository` | Atomic full refill TX (stamina+currency+log) |
| `StreakChallengeRepository.cs` | `StreakChallengeRepository` | Streak challenge user state + level state CRUD |
| `StreakChallengeTransactionRepository.cs` | `StreakChallengeTransactionRepository` | Atomic streak TX (activate, startLevel, claimReward, expiry soft-delete) |
| `ShopPurchaseTransactionRepository.cs` | `ShopPurchaseTransactionRepository` | Atomic shop purchase TX (currency+inventory) |

## Symbols
| symbol | kind | note |
|--------|------|------|
| `StageEndTransactionRepository.ExecuteAsync` | method | First-clear soft reward, clear stamina refund, ranking cache update; FOR UPDATE NOWAIT on stage rows; returns `IsFirstClear` |
| `StaminaRefillTransactionRepository.ExecuteAsync` | method | FOR UPDATE on stamina_state + user_currency; throws if full or insufficient funds |
| `StreakChallengeRepository.GetUserStateAsync` | method | reads streak_challenge_user_state + level states for a user |
| `StreakChallengeRepository.GetLevelStateAsync` | method | reads single streak_challenge_user_level_state by (userId, eventId, levelIndex) |
| `StreakChallengeTransactionRepository.ActivateAsync` | method | FOR UPDATE; idempotent cycle creation with correlation_id |
| `StreakChallengeTransactionRepository.StartLevelAsync` | method | FOR UPDATE; idempotent level start; throws if wrong state |
| `StreakChallengeTransactionRepository.ClaimRewardAsync` | method | FOR UPDATE; marks reward CLAIMED, grants items via inventory repo |
| `StreakChallengeTransactionRepository.ExpireAsync` | method | soft-delete: marks PENDING rewards EXPIRED, sets event_status=EXPIRED |

## Cross-refs
- Depends on: `Domain.Entities.*`, `Domain.Interfaces.*`
- Consumed by: `Application.*Service` (via injected interfaces)

## Rules
- EF Core is ORM only — never run migrations; schema managed via `npm run gen:orm`
- Transaction repos use `BeginTransactionAsync` → `SaveChangesAsync` → `CommitAsync`
- Lock order in StageEndTransaction: stage_progress → stage_best_records → stamina_state → user_currency → user_ranking_cache (prevents deadlocks)
- `ChangeTracker.Clear()` before reads that follow `ExecuteSqlInterpolatedAsync` (avoids stale cache)
- PostgreSQL exception 55P03 (lock_not_available) bubbles up from FOR UPDATE NOWAIT — GlobalExceptionMiddleware maps it to 409
