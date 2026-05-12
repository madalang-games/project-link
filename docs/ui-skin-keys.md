# UI Skin Keys

`UIButtonSkin` maps these keys to sprites. The UI builder reads button keys from `UIButtonSkin.buttons` and image slot keys from `UIButtonSkin.imageSlots`.

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
| `slot_popup_bg` | Popup panels and lobby dropdown panel |
| `slot_popup_overlay` | Popup dim overlay |
| `slot_stamina_icon` | Lobby stamina icon |
| `slot_currency_soft` | Lobby soft currency icon |
| `slot_stage_node` | Home stage carousel node |
| `slot_daily_card` | Home daily challenge card |
| `slot_event_banner` | Home season event banner |
| `slot_tab_indicator` | Selected tab indicator |
| `slot_board_bg` | Game board background |
| `slot_toggle_track` | Settings toggle track |
| `slot_toggle_handle` | Settings toggle handle |

## Source
| source | note |
|---|---|
| `client/project-link/Assets/Scripts/Editor/ProjectLinkUIBuilder.cs` | Calls `ApplyButtonSkin` and `ApplySlotSkin` with the keys above. |
| `client/project-link/Assets/Scripts/Editor/UIButtonSkin.cs` | ScriptableObject lookup implementation. |
| `client/project-link/Assets/Editor/UIButtonSkin.asset` | Editor-only asset that should contain the sprite mappings. |
