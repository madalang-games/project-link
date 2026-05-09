# Data/Generated - Auto-generated C# data model classes

## Files
| file | class | source CSV |
|---|---|---|
| `ingame/IngameStageInfo.cs` | `IngameStageInfo` | `shared/datas/ingame/ingame_stage_info.csv` |
| `ingame/IngameStageNodes.cs` | `IngameStageNodes` | `shared/datas/ingame/ingame_stage_nodes.csv` |
| `ingame/IngameStage.cs` | `IngameStage` | `shared/datas/ingame/ingame_stage.csv` |
| `ingame/IngameNodeColors.cs` | `IngameNodeColors` | `shared/datas/ingame/ingame_node_colors.csv` |
| `ingame/IngameItem.cs` | `IngameItem` | `shared/datas/ingame/ingame_item.csv` |
| `outgame/OutgameAvatar.cs` | `OutgameAvatar` | `shared/datas/outgame/outgame_avatar.csv` |
| `outgame/OutgameDailyChallenge.cs` | `OutgameDailyChallenge` | `shared/datas/outgame/outgame_daily_challenge.csv` |
| `outgame/OutgameDailyReward.cs` | `OutgameDailyReward` | `shared/datas/outgame/outgame_daily_reward.csv` |
| `outgame/OutgameSeasonEvent.cs` | `OutgameSeasonEvent` | `shared/datas/outgame/outgame_season_event.csv` |
| `outgame/OutgameShopCatalog.cs` | `OutgameShopCatalog` | `shared/datas/outgame/outgame_shop_catalog.csv` |
| `outgame/OutgameStaminaConfig.cs` | `OutgameStaminaConfig` | `shared/datas/outgame/outgame_stamina_config.csv` |
| `string/Clientstring.cs` | `Clientstring` | `shared/datas/string/clientstring.csv` |
| `string/ErrorMessages.cs` | `ErrorMessages` | `shared/datas/string/error_messages.csv` |

## Symbols
| symbol | kind | note |
|---|---|---|
| `IngameStageInfo.stageId` | field | int; PK |
| `IngameStageInfo.width` / `height` | fields | int; grid dimensions |
| `IngameStageInfo.timeLimit` | field | int; seconds; 0 = no limit |
| `IngameStageNodes.stageId` | field | int; FK to IngameStageInfo |
| `IngameStageNodes.colorId` | field | int; 1-based color identifier |
| `IngameStageNodes.nodeIndex` | field | int; 1 or 2 (start/end node) |
| `IngameStageNodes.x` / `y` | fields | int; 0-based grid position |
| `IngameStage.stageId` | field | int; PK |
| `IngameStage.width` / `height` | fields | int; grid dimensions |
| `IngameStage.timeLimit` | field | int; seconds; 0 = no limit |
| `IngameStage.nodeMap` | field | string; base36 encoded grid; values 0=empty, 1-20=groupId |
| `IngameStage.cellMap` | field | string; base36 encoded grid; values 0=empty, 1=obstacle, 2+=gimmick |
| `IngameNodeColors.nodeGroupId` | field | int; matches groupId values in IngameStage.nodeMap |
| `IngameNodeColors.hexColor` | field | string; HTML hex color e.g. `#FF3B3B` |
| `IngameNodeColors.displayName` | field | string; human-readable color name |
| `IngameItem.id` | field | int; PK for item display metadata |
| `OutgameShopCatalog.productId` | field | int; PK for shop product |
| `OutgameShopCatalog.category` | field | string; `ITEM` or `COIN` or `BUNDLE` or `NO_ADS` |
| `OutgameShopCatalog.isEnabled` | field | bool; false hides product from catalog |
| `OutgameStaminaConfig.maxStamina` | field | int; max stamina display cap |
| `OutgameStaminaConfig.rechargeSeconds` | field | int; recharge cadence in seconds |
| `OutgameDailyReward.streakDay` | field | int; 1..7 streak day |
| `OutgameSeasonEvent.eventId` | field | int; season event PK |

## Cross-refs
- Gen output: auto-generated from `shared/datas/ingame/`, `shared/datas/outgame/`, and `shared/datas/string/` CSVs
- Consumed by: client `Data.StageLoader` (IngameStage, IngameNodeColors), client `Data.OutgameDataLoader` (outgame tables, IngameItem), client `Core.LocalizationManager` (Clientstring)
- Source: `shared/datas/ingame/AGENTS.md`, `shared/datas/outgame/AGENTS.md`, `shared/datas/string/AGENTS.md`

## Rules
- NEVER edit; regenerate with `npm run gen:data` (or `tools/gen-data.bat`).
- To add/change fields: edit source CSV in `shared/datas/`, re-run gen.
- ResourcePath constant in each class points to the `Resources/Data/` runtime CSV.
