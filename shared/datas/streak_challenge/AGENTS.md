# shared/datas/streak_challenge — 24H Streak Challenge Configuration Data

## Tables
| file | rows | key |
|------|------|-----|
| `streak_challenge_event.csv` | Event master config (1 row per version) | `eventId,version` (PK) |
| `streak_challenge_level.csv` | Per-level config (3 rows per version) | `eventId,version,levelIndex` (PK) |
| `streak_challenge_reward_group.csv` | Reward group definitions | `rewardGroupId,rewardGroupVersion` (PK) |
| `streak_challenge_reward_item.csv` | Reward item grants per group | `rewardGroupId,rewardGroupVersion` (composite PK with displayOrder) |

## Schema

**streak_challenge_event**
- `eventId` int32 PK — always 1 for current event
- `version` int32 PK — metadata version; fixed at activation time per user cycle
- `isEnabled` bool NN — false = event hidden
- `durationSeconds` int32 NN — cycle duration (86400 = 24h)
- `resetType` string(32) NN — `LAZY` = reset on next relevant request
- `stageCountPolicy` string(64) NN — `FIRST_CLEAR_MAIN` = count only first-clear main stages
- `rewardPolicy` string(64) NN — `MANUAL_CLAIM` = user must manually claim
- `adPolicy` string(64) NN — `MULTIPLIER_ONCE_PER_LEVEL` = one ad multiplier per level per cycle

**streak_challenge_level**
- `eventId` int32 PK
- `version` int32 PK
- `levelIndex` int32 PK — 0-based (0,1,2)
- `requiredClearCount` int32 NN — consecutive clears needed to complete this level
- `rewardGroupId` int32 NN — FK → streak_challenge_reward_group.rewardGroupId
- `allowTimeExtension` bool NN — true = time-extended clears count
- `allowRevive` bool NN — true = revive clears count
- `isEnabled` bool NN
- `displayOrder` int32 NN — ascending UI display order
- `localizationKey` string(64) NN — client localization key for level label

**streak_challenge_reward_group**
- `rewardGroupId` int32 PK
- `rewardGroupVersion` int32 PK
- `rewardType` string(32) NN — `FIXED` | `WEIGHTED`
- `isEnabled` bool NN

**streak_challenge_reward_item**
- `rewardGroupId` int32 PK
- `rewardGroupVersion` int32 PK
- `itemType` string(32) NN — `SOFT_CURRENCY` | `ITEM`
- `itemId` int32 NN — `ingame_item.id` for ITEM; 0 for SOFT_CURRENCY
- `amount` int32 NN
- `weight` int32 NN — weight for WEIGHTED groups; 100 for FIXED
- `displayOrder` int32 NN

## Cross-refs
- Gen output: `client/generated/data/streak_challenge/` and `server/generated/data/streak_challenge/`
- Consumed by: server `Infrastructure.Data.StaticDataService`
- Consumed by: server `Application.StreakChallenge.StreakChallengeService`
- Consumed by: client `Data.Generated.streak_challenge.*`

## Rules
- `streak_challenge_event` version must be fixed at user cycle activation (metadata snapshot)
- Level rewards use `rewardGroupId` captured at activation — later group changes do NOT affect active cycles
- Level 0 = first level (UI shows as Level 1)
- `requiredClearCount` counts CONSECUTIVE first-clear main stage clears within one active level
