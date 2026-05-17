# 24H Streak Challenge Specification

## 1. Document Purpose

This document defines the implementation-ready specification for a 24-hour streak-based challenge system for a stage-based board/puzzle game similar to Flow Free.

The document is structured to help an AI Agent or development team plan and execute the work in implementation order:

1. Product Rules
2. Data Table Design
3. Packet and API Design
4. Server Implementation
5. Client Implementation
6. UI/UX Implementation
7. Reward and Ad Integration
8. Analytics and KPI Logging
9. Edge Cases
10. Test Plan
11. Rollout Checklist

This document intentionally excludes pseudocode and sample data payloads. All concrete values must be driven by metadata and server-side configuration, not hardcoded client or server logic.

---

## 2. Feature Summary

The 24H Streak Challenge is a user-activated challenge event. Once activated, the user has 24 hours to complete consecutive main stages and earn level-based rewards.

The challenge contains multiple levels. Each level requires a configured number of consecutive first-clear main stage completions. If the user fails during a level, only the current level's progress count is reset. Previously completed levels remain completed.

The feature is designed for a board/puzzle game where stages have time limits and users may use items or currency to extend time or revive/continue. A stage clear after using time extension or revive is still considered a valid clear.

---

## 3. Core Product Rules

### 3.1 Event Activation

- The event starts only when the user explicitly activates it.
- The 24-hour duration begins immediately at activation.
- The event does not reset at a fixed daily server time.
- The event uses server time only.
- Device time and timezone changes must not affect event timing.
- The event is reset using a Lazy Reset model.

### 3.2 Event Duration

- The event remains active for the configured duration, initially 24 hours.
- The duration must be controlled by metadata.
- When the event expires, progress is reset during the next relevant server request.
- Relevant requests include:
  - Event popup open
  - Event activation attempt
  - Level start attempt
  - Stage result processing
  - Reward claim attempt

### 3.3 Level Structure

Initial level rules:

| Level | Requirement | Reward |
|---|---:|---|
| Level 1 | 3 consecutive valid stage clears | Metadata-driven |
| Level 2 | 5 consecutive valid stage clears | Metadata-driven |
| Level 3 | 7 consecutive valid stage clears | Metadata-driven |

Requirements must be configurable through metadata.

### 3.4 Level Start Rule

- A level must not begin counting automatically unless the level is explicitly started by user interaction.
- After a level is completed, the next level becomes available but must not start automatically.
- The next level starts only when the user interacts with the start action.
- If a user clears a stage while the next level is available but not started, that clear must not count toward the next level.

### 3.5 Stage Counting Rule

A stage clear counts toward streak progress only when all conditions are met:

- The event is active.
- The current level is started.
- The stage is a normal main stage.
- The stage is cleared for the first time.
- The stage was not already cleared before the result was processed.
- The stage result is verified by the server.
- The event has not expired at the result processing time.

The following must not count:

- Already cleared stage replays
- Tutorial stages
- Bonus stages
- Event-only stages
- Non-main stages
- Local-only unverified results
- Results received after event expiration
- Results submitted when the level is not started

### 3.6 Failure Rule

A failure resets only the current level's progress count.

Failure is applied only when the stage result is confirmed as fail. Valid failure cases include:

- Time limit exceeded
- User gives up
- App forced close if the server determines the stage result as fail
- Any server-confirmed fail result

The following must not immediately cause streak failure unless the server confirms the stage result as fail:

- Temporary network error
- App backgrounding
- Reward claim error
- Ad playback error
- Result submission retry

### 3.7 Time Extension and Revive Rule

- If the user uses an item or currency to extend the stage timer and then clears the stage, the clear is valid.
- If the user uses revive or continue and then clears the stage, the clear is valid.
- If the user fails after using time extension or revive, the result is still fail.
- These policies must be metadata-driven so they can be changed by level or event version.

### 3.8 Level Completion Rule

- When a level reaches its required consecutive clear count, the level becomes completed.
- The level's reward enters reward pending state.
- Progress count for the next level starts at zero.
- The next level becomes available only after the previous level is completed.
- The next level does not start until the user interacts with the level start action.
- Once completed, a level must not be rolled back by later failures.

### 3.9 Event Completion Rule

- When the final level is completed, the event becomes completed after all relevant reward flow requirements are satisfied.
- Once completed, the user cannot restart the event before the current 24-hour cycle expires.
- After expiration, the user may activate a new cycle.

### 3.10 Reward Rule

- Rewards are granted by metadata-defined reward groups.
- Rewards are not hardcoded.
- On level completion, the reward enters pending state.
- The user must claim the reward manually.
- Reward claim must be idempotent.
- Duplicate claim requests must not duplicate rewards.
- Already claimed rewards must not be granted again.
- If reward claim fails because of a network or server issue, the user must be able to retry while the reward remains valid.
- Unclaimed rewards are deleted when the event expires.

### 3.11 Ad Reward Multiplier Rule

- Reward doubling is supported through rewarded ads.
- The reward multiplier applies to the level reward being claimed.
- The multiplier must be metadata-driven.
- The number of multiplier uses must be limited by metadata.
- Ad success and failure must be logged separately.
- If ad playback fails, the user must still be able to claim the base reward if the reward remains valid.

### 3.12 Stage End Navigation Rule

The existing direct next-stage flow must be changed when the streak event requires user attention.

The client must return the user to the lobby when:

- A level completion condition is met
- A reward pending popup must be shown
- A level start popup must be shown
- A streak failure must be explained
- The event expires
- Any event state transition requires explicit user interaction

If no event transition occurs, the existing next-stage flow may remain unchanged.

---

## 4. Metadata Requirements

All feature behavior must be configurable through metadata.

### 4.1 Event Metadata

Event metadata must support:

- Event ID
- Version
- Enabled state
- Duration
- Reset type
- Reset mode
- Time source
- Expiration check policy
- Stage counting policy
- Level start policy
- Stage end navigation policy
- Reward policy
- Ad policy
- Minimum user eligibility if needed
- A/B test group support if needed

### 4.2 Level Metadata

Level metadata must support:

- Event ID
- Event version
- Level index
- Required consecutive clear count
- Reward group ID
- Whether time extension clear is allowed
- Whether revive clear is allowed
- Whether the level is enabled
- Display order
- UI label or localization key

### 4.3 Reward Metadata

Reward metadata must support:

- Reward group ID
- Reward group version
- Reward type
- Item type
- Item ID
- Item amount
- Optional weighted reward settings
- Optional random amount settings
- Reward display metadata
- Localization keys

### 4.4 Metadata Versioning

- The event metadata version must be fixed at event activation.
- A user's active cycle must continue using the metadata version captured at activation.
- Later metadata changes must not affect active cycles.
- New metadata versions apply only to newly activated cycles.

---

## 5. Data Table Design

The exact naming convention should follow the project's existing database standards. The following logical tables are required.

### 5.1 Event Master Table

Purpose: stores event-level configuration.

Required fields:

- Event identifier
- Version
- Enabled state
- Duration
- Reset policy
- Expiration policy
- Stage count policy reference
- Reward policy reference
- Ad policy reference
- Created time
- Updated time

### 5.2 Event Level Master Table

Purpose: stores level-specific configuration.

Required fields:

- Event identifier
- Version
- Level index
- Required consecutive clear count
- Reward group identifier
- Time extension allowed flag
- Revive allowed flag
- Enabled flag
- Display order
- Localization key

### 5.3 Reward Group Master Table

Purpose: defines reward groups used by the event.

Required fields:

- Reward group identifier
- Reward group version
- Reward type
- Enabled state
- Display metadata reference
- Created time
- Updated time

### 5.4 Reward Item Master Table

Purpose: defines the item grants inside each reward group.

Required fields:

- Reward group identifier
- Reward group version
- Item type
- Item identifier
- Amount
- Optional weight
- Optional minimum amount
- Optional maximum amount
- Display order

### 5.5 User Event State Table

Purpose: stores each user's current event cycle.

Required fields:

- User identifier
- Event identifier
- Cycle identifier
- Event metadata version
- Event status
- Activated time
- Expiration time
- Current level
- Last processed play session identifier
- Last counted stage identifier
- Created time
- Updated time

### 5.6 User Event Level State Table

Purpose: stores per-level progress for the user's active cycle.

Required fields:

- User identifier
- Event identifier
- Cycle identifier
- Level index
- Level status
- Required count
- Current count
- Started time
- Completed time
- Reward state
- Reward claimed time
- Updated time

### 5.7 User Reward Claim History Table

Purpose: ensures reward claim idempotency and auditability.

Required fields:

- Claim identifier
- User identifier
- Event identifier
- Cycle identifier
- Level index
- Reward group identifier
- Reward group version
- Reward multiplier
- Claim status
- Failure reason if applicable
- Created time
- Updated time

### 5.8 User Ad Reward History Table

Purpose: tracks rewarded ad multiplier usage.

Required fields:

- User identifier
- Event identifier
- Cycle identifier
- Level index
- Ad placement identifier
- Ad result
- Reward multiplier applied flag
- Created time

### 5.9 Analytics Event Table or Pipeline

Purpose: stores feature analytics logs.

The analytics system must support:

- Event impression logs
- User interaction logs
- Progress logs
- Failure logs
- Reward logs
- Ad logs
- Expiration logs
- Exclusion reason logs

---

## 6. Packet and API Design

The packet and API layer should be designed after data tables are finalized.

### 6.1 Client to Server Requests

Required request types:

1. Get event state
2. Activate event
3. Start level
4. Process stage result
5. Claim reward
6. Claim reward with ad multiplier
7. Refresh event state after lobby entry

### 6.2 Server to Client Responses

Required response information:

- Event status
- Remaining time
- Current cycle identifier
- Current level
- Per-level status
- Per-level progress count
- Per-level required count
- Reward pending status
- Reward claimed status
- Available actions
- Navigation directive
- Popup directive
- Failure reason when relevant
- Exclusion reason when relevant
- Metadata version

### 6.3 Available Actions

The server response should tell the client which actions are currently valid.

Possible actions:

- Activate event
- Start level
- Continue current level
- Claim reward
- Watch ad for multiplied reward
- View completed state
- Start new cycle after expiration

### 6.4 Navigation Directives

The server should be able to instruct the client to:

- Stay in stage result flow
- Return to lobby
- Open event popup
- Open reward claim popup
- Open level start popup
- Open failure explanation popup
- Open expiration popup

### 6.5 Error and Exclusion Reasons

The API must return structured reasons for non-counted results.

Required reason categories:

- Event inactive
- Event expired
- Level not started
- Not a main stage
- Stage already cleared
- Duplicate result
- Server validation failed
- Metadata unavailable
- Reward already claimed
- Reward expired
- Claim duplicate
- Invalid level state
- Invalid cycle state

### 6.6 Idempotency Requirements

Idempotency is required for:

- Stage result processing
- Reward claim
- Ad multiplier reward claim
- Event activation
- Level start

Idempotency must prevent:

- Duplicate progress increments
- Duplicate failure resets
- Duplicate reward grants
- Duplicate ad multiplier application
- Duplicate event cycles from rapid repeated requests

---

## 7. Server Implementation Plan

### 7.1 Metadata Loading

Implement metadata loading first.

Server must:

- Load event metadata by event ID and version
- Load level metadata by event ID and version
- Load reward metadata by reward group ID and version
- Validate metadata consistency at startup or deployment
- Reject invalid event metadata before it reaches production

### 7.2 Event State Management

Implement event state transitions:

- Inactive to active
- Active to expired
- Active to completed
- Level locked to ready
- Level ready to started
- Level started to completed
- Reward none to pending
- Reward pending to claimed
- Reward pending to expired

### 7.3 Lazy Reset

Implement Lazy Reset on all relevant server entry points.

Lazy Reset must:

- Check server time against expiration time
- Expire the current cycle if needed
- Delete or invalidate unclaimed rewards after expiration
- Clear active progress
- Return the correct client state for a new cycle

### 7.4 Stage Result Processing

Implement stage result handling.

Server must verify:

- Event is active
- Current level is started
- Stage is countable
- Stage is first clear
- Result is not duplicated
- Result is processed before expiration
- Stage result is server-valid

On clear:

- Increment current level count
- Complete level if requirement is met
- Set reward pending
- Unlock next level as ready if applicable
- Mark final level completion if applicable
- Return appropriate navigation and popup directives

On fail:

- Reset only current level count
- Preserve completed levels
- Return failure explanation directive

### 7.5 Reward Claim

Implement manual reward claim.

Server must:

- Verify reward pending state
- Verify event is not expired
- Resolve reward from metadata
- Apply ad multiplier if valid
- Grant inventory items
- Write claim history
- Mark reward as claimed
- Handle duplicate claim safely
- Return updated event state

### 7.6 Ad Multiplier

Implement rewarded ad multiplier integration.

Server must:

- Validate ad success callback or trusted client proof according to existing ad policy
- Verify multiplier availability
- Apply multiplier only once per allowed unit
- Log ad usage
- Prevent duplicate multiplier claims

### 7.7 Server Validation and Anti-Abuse

Server must defend against:

- Device time manipulation
- Duplicate packet submission
- Replay stage farming
- Claim replay
- Stage result manipulation
- Metadata version mismatch
- Offline result abuse

---

## 8. Client Implementation Plan

### 8.1 Lobby HUD

Implement the Home HUD entry point.

HUD must support:

- Inactive state
- Active state
- Current level progress
- Reward pending state
- Completed state
- Expired or new cycle available state
- Remaining time display

### 8.2 Event Popup

Implement the main event popup.

Popup must show:

- Remaining time
- Vertical level progress
- Level status
- Current count and required count
- Reward state
- Claim button when reward is pending
- Start button when a level is ready
- Completed state when all levels are complete

### 8.3 Level Start Interaction

Client must require user interaction to start each level.

Client must not locally start a level without server confirmation.

### 8.4 Stage Result Flow Integration

Client must integrate event checks into the stage result flow.

After each stage result:

- Send result to server or wait for existing result processing response
- Read event-related response
- Apply server navigation directive
- Return to lobby when required
- Show event popup when required
- Do not auto-enter next stage when event transition requires attention

### 8.5 Failure UX

Client must clearly communicate that only the current level progress was reset.

Failure UX must show:

- Current level
- Previous count
- Reset count
- Completed levels preserved
- Retry path

### 8.6 Reward UX

Client must support:

- Reward pending popup
- Manual claim
- Reward claim retry
- Reward double ad option
- Reward claim success screen
- Transition to next level start popup

### 8.7 Expiration UX

When the event has expired, the client should avoid negative messaging about deleted unclaimed rewards.

Recommended UX direction:

- Inform the user that the previous challenge ended
- Offer a new challenge start path
- Do not emphasize lost unclaimed rewards

### 8.8 Offline and Network Handling

Client must:

- Show local stage result normally
- Avoid finalizing event progress locally without server confirmation
- Retry event state requests when network returns
- Allow reward claim retry if claim fails
- Show safe messaging when server validation is pending

---

## 9. UI/UX Flow Requirements

### 9.1 Home HUD Flow

1. User sees challenge HUD on Lobby Home.
2. User taps HUD.
3. Client requests latest event state.
4. Event popup opens based on server state.

### 9.2 First Activation Flow

1. User opens event popup.
2. User taps activate.
3. Server creates a new cycle.
4. Level 1 becomes ready.
5. User taps start Level 1.
6. Level 1 becomes started.
7. Future valid first-clear main stages count toward Level 1.

### 9.3 Level Completion Flow

1. User clears the final required stage for the current level.
2. Server marks the level complete.
3. Server creates reward pending state.
4. Client returns to lobby.
5. Reward popup opens.
6. User claims reward or watches ad for doubled reward.
7. Next level unlock popup appears if another level exists.
8. User may start the next level through interaction.

### 9.4 Failure Flow

1. User fails a stage while a level is started.
2. Server resets only the current level progress.
3. Client returns to lobby if required by event transition policy.
4. Failure popup explains the current level reset.
5. Completed levels remain displayed as completed.

### 9.5 Completion Flow

1. User completes the final level.
2. Final reward enters pending state.
3. User claims reward.
4. Event becomes completed.
5. HUD and popup show completed state until expiration.
6. User cannot start another cycle until expiration.

### 9.6 Expiration Flow

1. User enters lobby, popup, stage result, or claim flow after expiration.
2. Server performs Lazy Reset.
3. Unclaimed reward is expired or deleted according to policy.
4. Client shows new challenge availability.
5. User may activate a new cycle.

---

## 10. Analytics and KPI Logging

### 10.1 Required Logs

Implement logs for:

- HUD impression
- HUD click
- Event popup open
- Event activation
- Level ready
- Level start
- Stage result counted
- Stage result excluded
- Progress increment
- Progress reset
- Level complete
- Reward popup open
- Reward claim click
- Reward claim success
- Reward claim failure
- Reward double ad click
- Reward double ad success
- Reward double ad failure
- Event complete
- Event expire
- Lazy Reset execution

### 10.2 Exclusion Reason Logging

Every excluded stage result must include a reason.

Required exclusion reasons:

- Event inactive
- Level not started
- Event expired
- Not main stage
- Already cleared stage
- Duplicate result
- Server validation failed
- Invalid event state
- Invalid metadata version

### 10.3 Recommended KPIs

Track:

- HUD impression count
- HUD click-through rate
- Event activation rate
- Level 1 start rate
- Level 1 completion rate
- Level 2 completion rate
- Level 3 completion rate
- Level failure rate
- Failure recovery rate
- Reward claim rate
- Reward double ad usage rate
- Event completion rate
- Expiration rate
- Average progress before expiration
- Distribution of last reached level
- Time extension usage during streak attempts
- Clear rate after time extension
- Fail rate without time extension

---

## 11. Edge Case Requirements

### 11.1 Time and Expiration

- Use server time only.
- Ignore device time changes.
- Ignore timezone changes.
- If the stage result is processed after expiration, it must not count.
- If the user reconnects after expiration, Lazy Reset must occur.

### 11.2 Stage Eligibility

- Count only first-clear main stages.
- Do not count previously cleared stage replays.
- Do not count tutorial, bonus, or special stages.
- Verify first-clear state on the server.

### 11.3 Level State

- Do not count progress if the level is not started.
- Do not auto-start the next level after completion.
- Do not reset completed levels on later failure.
- Reset only the current level count on failure.
- Set next level progress to zero when the next level becomes ready.

### 11.4 Reward State

- Pending rewards are claimable only before expiration.
- Pending rewards expire or are deleted after event expiration.
- Reward claim must be retryable after temporary failure.
- Duplicate reward claim must be prevented.
- Reward metadata must use the version captured at activation.

### 11.5 App Lifecycle

- App forced close should only cause streak failure if the stage result becomes server-confirmed fail.
- App backgrounding must not automatically reset streak.
- Network issues must not automatically reset streak.
- Offline clears must not finalize event progress until server validation.

### 11.6 Rapid Requests

Server must safely handle:

- Rapid event activation taps
- Rapid level start taps
- Duplicate stage result submissions
- Duplicate claim taps
- Repeated ad reward callbacks
- Concurrent requests from multiple devices

---

## 12. Test Plan

### 12.1 Metadata Tests

Verify:

- Event metadata loads correctly.
- Level metadata loads correctly.
- Reward metadata loads correctly.
- Invalid metadata is rejected.
- Metadata version is fixed at event activation.
- New metadata does not affect active cycles.

### 12.2 Data State Tests

Verify:

- New user event state is created correctly.
- Level states initialize correctly.
- Current level updates correctly.
- Completed levels remain completed.
- Current level count resets correctly on failure.
- Event becomes completed after final level completion.
- Event becomes expired after duration.

### 12.3 API Tests

Verify:

- Get event state returns correct available actions.
- Activate event is idempotent.
- Start level is idempotent.
- Stage result processing is idempotent.
- Reward claim is idempotent.
- Ad multiplier claim is idempotent.
- Expired event requests trigger Lazy Reset.

### 12.4 Stage Counting Tests

Verify valid counting for:

- Active event
- Started level
- Main stage
- First clear
- Server-validated clear
- Clear after time extension
- Clear after revive

Verify excluded counting for:

- Event inactive
- Level not started
- Already cleared stage replay
- Non-main stage
- Duplicate result
- Expired event
- Server validation failure

### 12.5 Failure Tests

Verify:

- Time limit fail resets current level count.
- User give-up resets current level count.
- Server-confirmed forced close fail resets current level count.
- Completed previous levels are preserved.
- Failure popup shows correct reset scope.
- Network error does not reset progress by itself.

### 12.6 Reward Tests

Verify:

- Reward enters pending state after level completion.
- Claim grants correct metadata reward.
- Duplicate claim does not duplicate reward.
- Claim failure allows retry.
- Claimed reward cannot be claimed again.
- Pending reward expires after event expiration.
- Reward multiplier applies correctly after ad success.
- Reward multiplier does not apply after ad failure.

### 12.7 UI Flow Tests

Verify:

- HUD displays correct state.
- Event popup displays vertical progress correctly.
- Level start requires user interaction.
- Level completion returns user to lobby.
- Claim popup appears after level completion.
- Next level popup appears after claim.
- Failure UX explains current level reset only.
- Completed event cannot be restarted before expiration.
- Expired event shows new challenge path.

### 12.8 Navigation Tests

Verify:

- Existing next-stage flow remains when no event transition occurs.
- Client returns to lobby on level completion.
- Client returns to lobby on required failure flow.
- Client returns to lobby on expiration.
- Client does not auto-enter next stage when event popup is required.

### 12.9 Analytics Tests

Verify all required logs fire with correct parameters.

Special attention:

- Stage result counted
- Stage result excluded with reason
- Progress reset
- Level complete
- Reward claim success
- Reward claim failure
- Ad success
- Ad failure
- Lazy Reset

### 12.10 Concurrency and Abuse Tests

Verify:

- Duplicate result packets do not increase count twice.
- Duplicate fail packets do not repeatedly reset incorrectly.
- Duplicate claim packets do not duplicate rewards.
- Multiple devices do not create conflicting event cycles.
- Device time changes do not affect remaining time.
- Offline result abuse is rejected or safely delayed until validation.

---

## 13. Implementation Order

### Phase 1: Specification and Metadata

1. Confirm final product rules.
2. Confirm metadata ownership and deployment pipeline.
3. Define event metadata.
4. Define level metadata.
5. Define reward metadata.
6. Define localization keys.
7. Define metadata validation rules.

### Phase 2: Data Tables

1. Create event master table.
2. Create event level master table.
3. Create reward group and reward item tables if not already available.
4. Create user event state table.
5. Create user event level state table.
6. Create reward claim history table.
7. Create ad reward history table.
8. Add required indexes and uniqueness constraints.
9. Add migration and rollback scripts.

### Phase 3: Packet and API

1. Define request and response contracts.
2. Define available action model.
3. Define navigation directive model.
4. Define popup directive model.
5. Define error and exclusion reason model.
6. Define idempotency keys.
7. Review packet compatibility with existing stage result flow.

### Phase 4: Server Core

1. Implement metadata loader.
2. Implement event state service.
3. Implement Lazy Reset.
4. Implement event activation.
5. Implement level start.
6. Implement stage result processing.
7. Implement level completion.
8. Implement reward pending state.
9. Implement event completion.
10. Implement expiration handling.

### Phase 5: Reward and Ad Server Integration

1. Implement reward claim validation.
2. Integrate inventory grant.
3. Implement claim idempotency.
4. Implement ad multiplier validation.
5. Implement ad reward history.
6. Implement reward retry behavior.
7. Implement expired reward handling.

### Phase 6: Client Core

1. Implement event state fetching.
2. Implement Home HUD state display.
3. Implement event popup.
4. Implement vertical level progress UI.
5. Implement level start interaction.
6. Integrate event response into stage result flow.
7. Implement server-driven navigation handling.
8. Remove or bypass direct next-stage flow when event transition requires attention.

### Phase 7: Client Reward and Ad UX

1. Implement reward pending popup.
2. Implement base reward claim.
3. Implement rewarded ad multiplier option.
4. Implement reward success UI.
5. Implement claim retry UI.
6. Implement next level start popup.
7. Implement final completion UI.

### Phase 8: Failure and Expiration UX

1. Implement failure popup.
2. Show current level reset only.
3. Preserve completed level display.
4. Implement expiration popup.
5. Implement new cycle start path.
6. Avoid negative messaging about expired unclaimed rewards.

### Phase 9: Analytics

1. Add HUD logs.
2. Add activation logs.
3. Add level start logs.
4. Add progress logs.
5. Add exclusion reason logs.
6. Add failure logs.
7. Add reward logs.
8. Add ad logs.
9. Add completion and expiration logs.
10. Validate analytics schema.

### Phase 10: QA and Balancing

1. Run metadata tests.
2. Run API tests.
3. Run server state tests.
4. Run client flow tests.
5. Run reward tests.
6. Run ad tests.
7. Run concurrency tests.
8. Run abuse tests.
9. Verify analytics.
10. Tune level requirements and reward values through metadata.

### Phase 11: Release

1. Prepare feature flag.
2. Prepare metadata for initial release.
3. Prepare A/B test if needed.
4. Enable internal QA.
5. Enable limited live cohort.
6. Monitor logs and KPIs.
7. Adjust metadata if needed.
8. Expand rollout.

---

## 14. Acceptance Criteria

The feature is ready for release when:

- All event behavior is metadata-driven.
- Event activation starts a server-timed 24-hour cycle.
- Lazy Reset works on all relevant entry points.
- Only first-clear main stages count.
- Already cleared stage replays do not count.
- Level start requires explicit user interaction.
- Failure resets only the current level count.
- Completed levels remain completed.
- Level completion creates reward pending state.
- Reward claim is manual and idempotent.
- Unclaimed rewards expire after event expiration.
- Reward doubling through ads works once according to metadata.
- Level completion and required event transitions return the user to lobby.
- The client does not auto-enter the next stage when an event popup is required.
- UI clearly communicates progress, reward, failure, completion, and expiration states.
- All required analytics logs are implemented.
- All critical edge cases pass QA.

---

## 15. Open Decisions

The following decisions should be confirmed before implementation begins:

1. Exact database table naming convention.
2. Exact packet naming convention.
3. Whether reward metadata already exists or new reward tables are required.
4. Whether the final level reward claim is required before marking the event as completed.
5. Whether failure always returns to lobby or only when the design requires explanation.
6. Whether expired unclaimed rewards are physically deleted or marked expired for audit.
7. Whether event eligibility has a minimum user level or stage requirement.
8. Whether the feature will be A/B tested at launch.
9. Whether there is any multi-device support requirement.
10. Whether offline stage results are allowed to be submitted later for event progress.