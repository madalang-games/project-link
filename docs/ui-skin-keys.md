# UI Skin Keys

`UISpriteSkin` maps these keys to sprites. The UI builder calls `ApplySkin(image, key)` with the keys below. All Image components in the builder-generated UI must have a corresponding entry.

## Button Keys
| key | used by |
|---|---|
| `btn_primary` | Primary CTA buttons: start, play, confirm, claim, retry, update |
| `btn_secondary` | Secondary/cancel buttons and account link state |
| `btn_icon_settings` | Title settings icon |
| `btn_icon_menu` | Lobby HUD menu icon |
| `btn_icon_close` | Popup close icon |
| `btn_icon_pause` | Game pause icon |
| `btn_tab_shop` | Bottom Shop tab icon |
| `btn_tab_home` | Bottom Home tab icon |
| `btn_tab_ranking` | Bottom Ranking tab icon |
| `btn_carousel_prev` | Stage carousel previous arrow |
| `btn_carousel_next` | Stage carousel next arrow |

## Image Slot Keys
| key | used by |
|---|---|
| `slot_logo` | Bootstrap and Title logo slots |
| `slot_progress_track` | Bootstrap progress track |
| `slot_provider_google` | Google provider icon |
| `slot_provider_apple` | Apple provider icon |
| `slot_hud_bg` | Lobby HUD strip background |
| `slot_avatar` | Lobby/account avatar slot |
| `slot_popup_bg` | Popup panel background (Squared shape, default) |
| `slot_popup_bg_horizontal` | Popup panel background (Horizontal shape: wider than tall) |
| `slot_popup_bg_vertical` | Popup panel background (Vertical shape: taller than wide) |
| `slot_popup_overlay` | Popup dim overlay |
| `slot_stamina_icon` | Lobby stamina icon |
| `slot_currency_soft` | Lobby soft currency icon |
| `slot_stage_node` | Home stage carousel node |
| `slot_daily_card` | Home daily challenge card |
| `slot_event_banner` | Home season event banner |
| `slot_board_bg` | Game board background |
| `slot_toggle_off` | Settings toggle — off state image (replaces slot_toggle_track + slot_toggle_handle) |
| `slot_toggle_on` | Settings toggle — on state image |
| `slot_star_filled` | Filled (earned) star in ClearPopup and StageDetailPopup |
| `slot_star_empty` | Empty (unearned) star in ClearPopup and StageDetailPopup |
| `slot_streak_badge` | StreakChallenge badge in stage carousel |

## Convention
- Every `Image` component in builder-generated scenes and popup prefabs must participate in skin. Run `Tools/Project Link/UI Build/Create UI Sprite Skin` to sync the asset; assign sprites in Inspector.
- Builder caches keys via source scan of `ApplySkin(...)` calls — no manual registration needed.

## Source
| source | note |
|---|---|
| `client/project-link/Assets/Scripts/Editor/ProjectLinkUIBuilder.cs` | Calls `ApplySkin(image, key)` for all builder-generated Image components. |
| `client/project-link/Assets/Scripts/Editor/UISpriteSkin.cs` | ScriptableObject lookup implementation. |
| `client/project-link/Assets/Editor/UISpriteSkin.asset` | Editor-only asset; assign sprites per key in Inspector. |
