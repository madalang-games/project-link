# Project Link — Wireframe Generation Prompt (Claude Design Brief)

> Feed this document as the design brief when generating wireframes via Claude Design or any AI design tool.  
> Language: English | Format: Structured Markdown | Style target: Modern hyper-casual mobile game UI

---

## 1. Product Context

**App name**: Project Link  
**Genre**: Hyper-casual puzzle — path-connection grid game  
**Platform**: Mobile (iOS & Android), portrait orientation  
**Target audience**: Casual mobile gamers, all ages  
**Design goal**: Minimalist, vibrant, and immediately legible UI that keeps cognitive overhead near zero during gameplay. Celebrate every small win with animation and visual feedback.

---

## 2. Design System

### Color Palette (Direction)
```
Background (primary):   Deep navy or dark charcoal  (#1A1A2E or similar)
Background (card):      Slightly lighter surface     (#16213E or similar)
Accent (primary):       Vivid electric blue/purple   (#7B2FBE → #E040FB gradient)
Accent (positive):      Bright teal or lime          (#00E5FF or #76FF03)
Accent (warning):       Warm amber                   (#FFB300)
Accent (danger):        Soft red                     (#FF5252)
Text (primary):         Pure white                   (#FFFFFF)
Text (secondary):       Muted white                  (#B0BEC5)
Text (disabled):        Dark muted                   (#546E7A)
```

### Typography
```
Heading (stage number, popup title):    Rounded Bold, 24–32pt
Subheading (section labels):            Rounded SemiBold, 16–20pt
Body (descriptions, lists):             Rounded Medium, 14pt
Caption (timestamps, small labels):     Rounded Regular, 11–12pt
HUD (timer, counters):                  Monospace Bold or Rounded Bold, 18–22pt
```

### Spacing & Shape
```
Border radius (cards, panels):    16–24 px
Border radius (buttons):          12 px (pill for primary CTA)
Border radius (popups):           24 px
HUD strip height:                 ~80–90 px (two rows, safe-area adjusted)
Tab bar height:                   ~64 px
Minimum touch target:             48 × 48 dp
```

### Component Visual Style
| Component | Visual Style |
|-----------|-------------|
| Primary CTA button | Pill shape, gradient fill (accent primary), subtle drop shadow, white bold label |
| Secondary button | Outlined pill, accent color border, transparent fill |
| Destructive button | Solid red/danger fill |
| Icon buttons | Circular background (semi-transparent white 15%), filled icon |
| Cards | Rounded rectangle, surface background, subtle inner glow border |
| HUD strip | Full-width, dark semi-transparent background (blur effect suggestion) |
| Tab bar | Dark surface, selected tab: accent underline or filled tab indicator |
| Popups | Rounded rectangle, dark surface, drop shadow, semi-transparent dark overlay behind |
| Progress bar | Rounded track, gradient fill, subtle glow |

---

## 3. Global UI Patterns

### Toast Notification (Top of Screen)
```
┌─────────────────────────────────────────┐  ← newest (bottom of stack)
│  ⚠ Network error. Please retry.  ×     │
├─────────────────────────────────────────┤
│  ✓ Stamina refilled successfully.  ×    │
├─────────────────────────────────────────┤  ← oldest (top of stack)
│  ✓ Settings saved.  ×                   │
└─────────────────────────────────────────┘
```
- Max 3 toasts stacked; new toast enters from bottom, pushes others up
- Auto-dismiss after 3 s; ×  for manual dismiss
- Positioned at top, safe-area-aware

### Popup Shell (Standard)
```
╔═══════════════════════════════════════╗
║                                   [×] ║  ← close button, top-right
║  [Popup Title]                        ║
║  ─────────────────────────────────    ║
║                                       ║
║      [Content Area]                   ║
║                                       ║
║  ─────────────────────────────────    ║
║   [Secondary Action] [Primary CTA]   ║
╚═══════════════════════════════════════╝
```
- Dark semi-transparent overlay covers entire screen behind popup
- Tapping overlay closes popup (unless non-dismissible)
- Non-dismissible popups: no × button, overlay tap is inert

---

## 4. Scene Wireframes

---

### 4.1 Bootstrap Scene

**Screen count**: 1 base state + 2 overlay states

#### Base State (Loading)
```
┌──────────────────────────────┐
│                              │
│                              │
│                              │
│       [GAME LOGO / SPINE]    │   ← animated Spine logo, center
│                              │
│                              │
│                              │
│   [══════════░░░░░░░░░░░]    │   ← animated progress bar, bottom area
│                              │
└──────────────────────────────┘
```

**Variants**:
- Network error: add centered [Retry] primary button below progress bar; progress bar hidden
- ForceUpdatePopup overlaid (non-dismissible): see Popup Library

---

### 4.2 Title Scene

**Screen count**: 1 main layout + auth badge state variant

#### Main Layout
```
┌──────────────────────────────┐
│                        [⚙]  │   ← settings icon, top-right
│                              │
│                              │
│     [GAME LOGO / SPINE]      │   ← large animated logo, upper-center
│                              │
│   ┌──────────────────────┐   │
│   │  [G] Continue with   │   │   ← Google OAuth button (UIButtonSkin)
│   │      Google          │   │
│   └──────────────────────┘   │
│   ┌──────────────────────┐   │
│   │  [ ] Sign in with    │   │   ← Apple OAuth button (UIButtonSkin)
│   │      Apple           │   │
│   └──────────────────────┘   │
│                              │
│   ┌──────────────────────┐   │
│   │   ▶  TAP TO START    │   │   ← primary CTA, pill, gradient, pulsing
│   └──────────────────────┘   │
│                              │
│               v1.0.0         │   ← version, bottom caption
└──────────────────────────────┘
```

#### Auth Badge State (after OIDC success)
- Below the authenticated provider button: small chip showing provider icon + email/display name
- Other provider button appears grayed/disabled
```
   ┌─────────────────────────┐
   │  [G] Continue with      │   ← active (normal)
   │      Google             │
   └─────────────────────────┘
   │  ✓ user@gmail.com       │   ← auth badge (success chip)
   
   ┌─────────────────────────┐
   │  [ ] Sign in with       │   ← grayed out (disabled)
   │      Apple          [─] │
   └─────────────────────────┘
```

---

### 4.3 Lobby Scene

**Screen count**: 1 base layout with 3 tab content panels

#### HUD (Always Visible)
```
┌──────────────────────────────┐  ← safe area top
│ [Avatar] Nickname        [⋮] │  ← Row 1: avatar (circle) + name | menu
│ ♥ 5/5  23:45          💎 340 │  ← Row 2: stamina | currency
└──────────────────────────────┘
```

#### Menu Dropdown (⋮ tap, slides down from top-right)
```
                    ┌─────────┐
                    │ ⚙ Settings │
                    │ 🌐 Language │
                    └─────────┘
```

#### Tab Bar (Bottom)
```
┌───────────┬───────────┬───────────┐
│   🛒 Shop  │  🏠 Home  │  🏆 Rank  │
│           │  [●active]│           │
└───────────┴───────────┴───────────┘
```
Active tab: filled accent indicator beneath tab icon+label.

---

#### Home Tab — Stage Carousel
```
┌──────────────────────────────┐
│  [HUD]                       │
│                              │
│  ◀  ┌────┐  ┌──────────┐  ┌────┐  ▶  │
│     │ 01 │  │    02    │  │ 03 │      │
│     │ ★★☆│  │  ★★★     │  │ 🔒 │      │
│     └────┘  │ [SPINE]  │  └────┘      │
│             └──────────┘              │
│             [  PLAY ▶ ]              │  ← tap → StageDetailPopup
│                                      │
│  ┌──────────────────────────────┐    │
│  │  📅 Daily Challenge          │    │  ← card
│  │  Streak: 5 days  ■■■■□ 4/5  │    │
│  └──────────────────────────────┘    │
│  ┌──────────────────────────────┐    │
│  │  🎪 Season Event: Link Fest  │    │  ← card (hidden if no event)
│  │  Ends in 2d 14h              │    │
│  └──────────────────────────────┘    │
│  [TAB BAR]                           │
└──────────────────────────────────────┘
```
Carousel rules:
- Center node: full-size card with Spine idle animation, star fill
- Side nodes: ~70% scale, ~60% opacity, no interaction label
- ◀ / ▶ arrows: tap = move 1; long press = progressive speed navigation

---

#### Shop Tab
```
┌──────────────────────────────┐
│  [HUD]                       │
│                              │
│  ── STAMINA ──────────────   │
│  ┌────────────┐ ┌────────────┐│
│  │ [img]      │ │ [img]      ││   ← product cards (2-col grid or list)
│  │ +5 Stamina │ │ +20 Stamina││
│  │  💎 50     │ │  💎 180    ││
│  │ [Purchase] │ │ [Purchase] ││
│  └────────────┘ └────────────┘│
│  ── ITEMS ─────────────────  │
│  ┌────────────────────────┐   │
│  │ [img] Extra Move       │   │   ← list card
│  │ Grants +5 moves        │   │
│  │ 💎 120      [Purchase] │   │
│  └────────────────────────┘   │
│  [TAB BAR]                    │
└──────────────────────────────┘
```

---

#### Ranking Tab
```
┌──────────────────────────────┐
│  [HUD]                       │
│  [ Stages Cleared | Score ]  │   ← segment control
│                              │
│  🥇 1.  [Av] Username   350  │
│  🥈 2.  [Av] Username   310  │
│  🥉 3.  [Av] Username   290  │
│  ── ── ── ── ── ── ── ──     │
│  4.  [Av] Username     250  │
│  5.  [Av] Username     240  │
│   (scroll)                   │
│ ─────────────────────────── │
│  ★ 12. [Av] YOU        180  │   ← my row, pinned bottom, highlighted
│  [TAB BAR]                   │
└──────────────────────────────┘
```

---

### 4.4 Game Scene

#### InGame HUD + Board
```
┌──────────────────────────────┐
│ [⏸] ── Stage 12 ──    02:30 │   ← pause | stage | timer
│ ■■■■□□□□□□□□□□□□□□□□□□□□    │   ← objective bar (color segments)
│ Moves: 8/20                  │
│                              │
│  ┌─────────────────────┐    │
│  │                     │    │
│  │    [GAME BOARD]     │    │   ← grid cells, path lines
│  │                     │    │
│  │                     │    │
│  └─────────────────────┘    │
│                              │
│  [🔧 2] [⚡ 1] [🎯 3]        │   ← item toolbar (bottom fixed)
└──────────────────────────────┘
```

---

## 5. Popup Library Wireframes

---

### ForceUpdatePopup (Non-dismissible)
```
┌─────────────────────────┐
│                         │   ← NO × button
│  🔄  Update Required    │
│                         │
│  A new version of the   │
│  game is available.     │
│  Please update to       │
│  continue playing.      │
│                         │
│  ┌───────────────────┐  │
│  │  Open App Store   │  │   ← primary CTA → store redirect
│  └───────────────────┘  │
└─────────────────────────┘
```

### MaintenancePopup (Non-dismissible)
```
┌─────────────────────────┐
│                         │   ← NO × button
│  🛠  Under Maintenance  │
│                         │
│  [MaintenanceMessage]   │
│                         │
└─────────────────────────┘
```

### SessionExpiredPopup (Non-dismissible)
```
┌─────────────────────────┐
│                         │
│  ⏰  Session Expired    │
│                         │
│  Your session has       │
│  expired. Please log    │
│  in again.              │
│                         │
│  ┌───────────────────┐  │
│  │      Confirm      │  │   ← → Title scene
│  └───────────────────┘  │
└─────────────────────────┘
```

### SettingPopup
```
┌─────────────────────────┐
│  Settings          [×]  │
│  ─────────────────────  │
│  BGM        [●──────]  │   ← toggle
│  SFX        [●──────]  │
│  Haptics    [──────○]  │
│  Notifications [●────]  │
│  ─────────────────────  │
│  Language    [English ▼]│   ← dropdown
│  ─────────────────────  │
│  [Cancel]      [Save]   │
└─────────────────────────┘
```

### EnergyPopup
```
┌─────────────────────────┐
│  Stamina             [×]│
│  ─────────────────────  │
│      ♥♥♥♥♥ 3/5         │
│   Full in 01:20:00      │
│  ─────────────────────  │
│  ┌─────────────────┐    │
│  │ 📺 Watch Ad +2  │    │   ← primary (teal)
│  └─────────────────┘    │
│  ┌─────────────────┐    │
│  │ 💎 Refill (50)  │    │   ← secondary
│  └─────────────────┘    │
└─────────────────────────┘
```

### StageDetailPopup
```
┌─────────────────────────┐
│  Stage 12           [×] │
│  ─────────────────────  │
│  ★ ★ ★                 │   ← star rating (filled/empty)
│  Best: 8,450 pts        │
│  My Rank: #14           │
│  ─────────────────────  │
│  ┌─────────────────┐    │
│  │   ▶  Play       │    │   ← primary CTA
│  └─────────────────┘    │
└─────────────────────────┘
```

### DailyChallengePopup
```
┌─────────────────────────┐
│  Daily Challenge     [×]│
│  ─────────────────────  │
│  Streak: 5 🔥           │
│  [■][■][■][■][□][□][□] │   ← 7-day streak tiles
│  ─────────────────────  │
│  Today: Play 4/5 stages │
│  [■■■■░]               │   ← progress bar
│  Reward: 💎 ×50         │
│  ─────────────────────  │
│  ┌─────────────────┐    │
│  │    Complete ✓   │    │   ← active when CanComplete
│  └─────────────────┘    │
└─────────────────────────┘
```

### RewardPopup
```
┌─────────────────────────┐
│  Reward!             [×]│
│  ─────────────────────  │
│    [Spine celebrate]    │
│   💎 × 50              │
│  ─────────────────────  │
│  ┌──────────┐ ┌──────┐  │
│  │  Claim   │ │ ×2 📺│  │   ← ×1 claim | ×2 ad-gated
│  └──────────┘ └──────┘  │
└─────────────────────────┘
```

### AccountPopup
```
┌─────────────────────────┐
│  Profile             [×]│
│  ─────────────────────  │
│     [Avatar circle]     │
│      Username           │
│   Joined: 2025-01-10    │
│  ─────────────────────  │
│  Linked Accounts:       │
│  [G] Google  [Linked ✓] │
│  [ ] Apple   [Link]     │
└─────────────────────────┘
```

### ClearPopup (Game Scene)
```
┌─────────────────────────┐
│   STAGE CLEAR!          │   ← full-screen dim behind
│   [Spine celebration]   │
│   ⭐ ⭐ ⭐              │   ← animated star fill sequence
│   Score: 12,800         │
│   🏆 NEW RECORD!        │   ← badge (conditional)
│   +💎 30               │   ← soft reward
│  ─────────────────────  │
│  [Lobby] [Retry] [Next▶]│
└─────────────────────────┘
```

### PausePopup (Game Scene)
```
┌─────────────────────────┐
│   Paused             [×]│   ← full-screen dim, board hidden
│  ─────────────────────  │
│  ┌─────────────────┐    │
│  │    ▶ Resume     │    │   ← primary
│  └─────────────────┘    │
│  ┌─────────────────┐    │
│  │    🔄 Retry     │    │   ← secondary
│  └─────────────────┘    │
│  ┌─────────────────┐    │
│  │    🏠 Lobby     │    │   ← secondary
│  └─────────────────┘    │
└─────────────────────────┘
```

### TimeoutPopup (Game Scene, Non-dismissible)
```
┌─────────────────────────┐
│   Time's Up!            │   ← NO × button
│   [Spine timeout anim]  │
│  ─────────────────────  │
│  ┌──────────┐ ┌──────┐  │
│  │  🔄 Retry │ │🏠 Lobby│  │
│  └──────────┘ └──────┘  │
└─────────────────────────┘
```

---

## 6. Animation & Interaction Notes

| Element | Animation |
|---------|-----------|
| App logo (Bootstrap/Title) | Spine idle — gentle float + glow pulse |
| Stage node (carousel center) | Spine idle — subtle bounce/glow |
| Stage node (side/locked) | Static — scale + opacity reduced |
| Star rating fill (ClearPopup) | Sequential fill 0→1→2→3 with Spine sparkle burst per star |
| Currency/stamina delta | Floating +N text, arc-moves toward HUD icon, fade out |
| Objective bar segments | Fill animation left-to-right; Spine particle burst on completion |
| Daily challenge streak tiles | Spine flip animation on tile completion |
| Season event banner | Spine looping animation |
| Popup entry | Scale from 0.8 + fade in, 0.25 s, cubic-bezier ease-out |
| Popup exit | Scale to 0.8 + fade out, 0.2 s |
| Toast entry | Slide down from top + fade in |
| Toast exit | Slide up + fade out |
| Tab switch | Content fade cross-dissolve 0.15 s |
| Carousel navigation | Spring physics slide + scale interpolation |

---

## 7. Skin System Notes

> Reference: `UIButtonSkin.ScriptableObject` (elementName → Sprite)

All buttons and image slots are named so that UIButtonSkin entries can swap in art assets:

| Element Name (skin key) | Context |
|-------------------------|---------|
| `btn_primary` | Primary CTA pill button |
| `btn_secondary` | Secondary outlined button |
| `btn_icon_pause` | InGame pause button |
| `btn_icon_settings` | Settings icon |
| `btn_icon_menu` | ⋮ menu button |
| `btn_icon_close` | × popup close button |
| `btn_tab_shop` | Shop tab icon |
| `btn_tab_home` | Home tab icon |
| `btn_tab_ranking` | Ranking tab icon |
| `slot_avatar` | Player avatar circle frame |
| `slot_currency_soft` | Soft currency icon |
| `slot_stamina_icon` | Stamina heart/icon |
| `slot_star_filled` | Filled star (rating) |
| `slot_star_empty` | Empty star (rating) |
| `slot_stage_node` | Stage carousel node background card |
| `slot_stage_node_locked` | Locked stage node overlay |
| `slot_daily_card` | Daily challenge card background |
| `slot_event_banner` | Season event banner background |
| `slot_reward_currency` | Reward currency icon in popups |
| `slot_provider_google` | Google logo (OAuth button) |
| `slot_provider_apple` | Apple logo (OAuth button) |

---

## 8. Prompt Instructions for Claude Design

When generating wireframes from this document:

1. **Use dark-theme backgrounds** matching the color palette in section 2.
2. **Generate one frame per scene** plus individual popup frames.
3. **Label all interactive elements** with their action (e.g., "→ StageDetailPopup", "API: POST /api/stage/start").
4. **Show placeholder boxes** for Spine animation areas labeled `[SPINE: description]`.
5. **Include state variants** where noted (e.g., Title with/without auth badge, Home tab with/without season event).
6. **Apply the global popup shell** template to all popup frames.
7. **Annotate navigation flows** with arrows between related frames.
8. Keep wireframes **medium-fidelity**: structure and spacing accurate, placeholder icons acceptable, no final art.
