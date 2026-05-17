# 11 — 24H Streak Challenge

## Scope
Replace daily_challenge system with 24H Streak Challenge (server-driven, 3-level, lazy reset).

## Status: DONE

### P0 — Data Source
- [x] Delete outgame_daily_challenge.csv, outgame_daily_reward.csv
- [x] Create shared/datas/streak_challenge/ with 4 CSV files
- [x] Update AGENTS.md

### P1 — Shared Contracts
- [x] Delete Daily/ contracts directory
- [x] Create StreakChallenge/ contracts (Requests + Responses)
- [x] Update StageResponses.cs (add StreakChallenge? field)
- [x] Update LobbyResponses.cs (LobbyDailyChallenge → LobbyStreakChallenge)

### P2 — DB Schema
- [x] Remove daily_challenge_progress table
- [x] Add streak_challenge_user_state, streak_challenge_user_level_state, streak_challenge_reward_claim_history, streak_challenge_ad_reward_history

### P3 — Server Domain
- [x] Delete daily challenge domain files
- [x] Create StreakChallenge static data, entities, interfaces
- [x] Update IStaticDataService, IStageEndTransaction

### P4 — Server Application
- [x] Delete DailyChallengeService
- [x] Implement StreakChallengeService (activate, startLevel, processResult, claimReward, claimWithAd, lobbySnapshot)
- [x] Update StageService.EndAsync (processStageResult hook, streak directive)
- [x] Update LobbyService (streak HUD snapshot)

### P5 — Server Infrastructure
- [x] Delete DailyChallengeRepository, DailyChallengeCompleteTransactionRepository
- [x] Implement StreakChallengeRepository, StreakChallengeTransactionRepository
- [x] Update AppDbContext (remove daily mapping, add 4 streak entities)
- [x] Update StageEndTransactionRepository (remove daily block, expose IsFirstClear)
- [x] Update StaticDataService (remove daily loaders, add streak CSV loaders)

### P6 — Server API
- [x] Delete DailyChallengeController
- [x] Create StreakChallengeController (GET, activate, startLevel, claimReward, claimRewardWithAd)
- [x] Update Program.cs DI (remove daily, add streak repos + service)

### P7 — Client
- [x] Delete generated Daily contract files
- [x] Create StreakChallengeRequests.cs, StreakChallengeResponses.cs in Generated/Contracts
- [x] Update LobbyResponses.cs (LobbyStreakChallenge)
- [x] Update StageResponses.cs (StreakChallenge? field)
- [x] Update PopupId (DailyChallenge → StreakChallenge)
- [x] Delete DailyChallengePopup, create StreakChallengePopup
- [x] Update RuntimeNavigationButtons (OpenStreakChallengePopup)
- [x] Update GameContext (remove daily challenge state, add IsStreakChallengeActive)
- [x] Update InGameController (remove daily routing, add streak directive handling)
- [x] Update IUiDataService / HttpUiDataService / UiDataRoutes (streak endpoints)
- [x] Update UiViewModels / UiViewModelMapper (StreakChallengeModel)
- [x] Update StaticCatalogService, OutgameDataLoader (remove daily refs)
- [x] Update LobbyWireframeController (streak HUD display)

### P8 — Documentation
- [x] Create TODO-List/11_streak_challenge.md (this file)
- [x] Update affected AGENTS.md files

<!-- changed: daily_challenge fully replaced with 24H Streak Challenge system -->
