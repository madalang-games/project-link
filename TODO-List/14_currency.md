# 14 — Currency System

## Spec
| key | value |
|-----|-------|
| Types | Soft (인게임 획득) only — Hard currency: TODO |
| Uses | Item purchase, Stamina extension (retry without fail deduction) |
| Sources | Stage clear reward, Ad reward, Event reward |

---

## DB

### Table: `user_currency`
| column | type | constraints |
|--------|------|-------------|
| user_id | string(36) | PK NN |
| soft_amount | int64 | NN |

Single-row per user. Updated atomically via `WHERE soft_amount >= @cost` pattern.

### Table: `currency_logs` (already in schema.json)
Append-only audit. Fields: `currency_type = "soft"`.

---

## Atomic Deduction Pattern

```sql
UPDATE user_currency
SET soft_amount = soft_amount - @cost
WHERE user_id = @userId AND soft_amount >= @cost
```
Rows affected = 0 → `InsufficientFundsException` (never go negative).  
On success → append to `currency_logs`.

---

## API Endpoints

| method | path | auth | description |
|--------|------|------|-------------|
| GET | `/api/currency` | JWT | Returns current soft balance |

No direct earn/spend endpoints exposed to client — currency changes are side effects of other actions (stage clear, stamina extend, item purchase, etc.).

---

## Earn Sources (triggered by other endpoints)

| trigger | amount | location |
|---------|--------|----------|
| Stage clear | configurable per stage (from stage data) | `POST /api/stage/{id}/end` |
| Ad reward | server config `currency.ad_reward_soft` | `POST /api/currency/ad-reward` |

### POST /api/currency/ad-reward
- Body: `{ "ad_token": "<platform token>", "reward_type": "soft" }`
- Idempotent by `ad_token`

---

## Config Keys (template.ini / .env)

```
[Currency]
ad_reward_soft=50
```

Stage-specific rewards live in `ingame_stage.csv` (`soft_reward` column — add when implementing).

---

## Tasks

- [x] Add `user_currency` table to `server/db/schema.json` → run `npm run gen:orm`
- [x] `CurrencyService.Deduct(userId, amount, reason, txId)` — SELECT FOR UPDATE, logs to `currency_logs`
- [x] `CurrencyService.Grant(userId, amount, reason, txId)` — SELECT FOR UPDATE, logs to `currency_logs`
- [x] `GET /api/currency` controller
- [x] `POST /api/currency/ad-reward` controller (Redis SETNX idempotency)
- [x] Wire `CurrencyService.Grant` into stage clear flow
- [x] Add `soft_reward` column to `ingame_stage.csv`
- [x] `CurrencyRequests.cs` / `CurrencyResponses.cs` in `shared/contracts/Currency/`

<!-- changed: soft currency only — hard currency deferred, sources via stage/ad -->
