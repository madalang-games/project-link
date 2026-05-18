using Microsoft.EntityFrameworkCore;
using ProjectLink.Domain.Entities;
using ProjectLink.Domain.Interfaces;
using ProjectLink.Domain.StaticData;

namespace ProjectLink.Infrastructure.Persistence;

public class StreakChallengeTransactionRepository : IStreakChallengeTransaction
{
    private readonly AppDbContext _db;

    public StreakChallengeTransactionRepository(AppDbContext db) => _db = db;

    public async Task ActivateAsync(StreakChallengeActivateCommand cmd, CancellationToken ct)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var now = DateTimeOffset.UtcNow;
        var state = new StreakChallengeUserState
        {
            UserId             = cmd.UserId,
            EventId            = cmd.EventId,
            CycleId            = cmd.CycleId,
            EventVersion       = cmd.EventVersion,
            EventStatus        = "ACTIVE",
            ActivatedAt        = cmd.ActivatedAt,
            ExpiresAt          = cmd.ExpiresAt,
            CurrentLevel       = 0,
            LastCountedStageId = 0,
            CreatedAt          = now,
            UpdatedAt          = now,
        };
        _db.StreakChallengeUserStates.Add(state);

        for (int i = 0; i < cmd.Levels.Count; i++)
        {
            var levelDef = cmd.Levels[i];
            _db.StreakChallengeUserLevelStates.Add(new StreakChallengeUserLevelState
            {
                UserId        = cmd.UserId,
                EventId       = cmd.EventId,
                CycleId       = cmd.CycleId,
                LevelIndex    = levelDef.LevelIndex,
                LevelStatus   = i == 0 ? "READY" : "LOCKED",
                RequiredCount = levelDef.RequiredClearCount,
                CurrentCount  = 0,
                RewardState   = "NONE",
                UpdatedAt     = now,
            });
        }

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
    }

    public async Task StartLevelAsync(StreakChallengeStartLevelCommand cmd, CancellationToken ct)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var level = await _db.StreakChallengeUserLevelStates
            .FromSqlInterpolated($"SELECT * FROM streak_challenge_user_level_state WHERE user_id = {cmd.UserId} AND event_id = {cmd.EventId} AND cycle_id = {cmd.CycleId} AND level_index = {cmd.LevelIndex} FOR UPDATE")
            .FirstOrDefaultAsync(ct);

        if (level is null || level.LevelStatus != "READY")
        {
            await tx.RollbackAsync(ct);
            return;
        }

        level.LevelStatus = "STARTED";
        level.StartedAt   = DateTimeOffset.UtcNow;
        level.UpdatedAt   = DateTimeOffset.UtcNow;

        var state = await _db.StreakChallengeUserStates
            .FromSqlInterpolated($"SELECT * FROM streak_challenge_user_state WHERE user_id = {cmd.UserId} AND event_id = {cmd.EventId} AND cycle_id = {cmd.CycleId} FOR UPDATE")
            .FirstOrDefaultAsync(ct);
        if (state is not null)
        {
            state.UpdatedAt = DateTimeOffset.UtcNow;
        }

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
    }

    public async Task<StreakChallengeProgressResult> RecordProgressAsync(
        StreakChallengeProgressCommand cmd, CancellationToken ct)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var state = await _db.StreakChallengeUserStates
            .FromSqlInterpolated($"SELECT * FROM streak_challenge_user_state WHERE user_id = {cmd.UserId} AND event_id = {cmd.EventId} AND cycle_id = {cmd.CycleId} FOR UPDATE")
            .FirstAsync(ct);
        var level = await _db.StreakChallengeUserLevelStates
            .FromSqlInterpolated($"SELECT * FROM streak_challenge_user_level_state WHERE user_id = {cmd.UserId} AND event_id = {cmd.EventId} AND cycle_id = {cmd.CycleId} AND level_index = {cmd.LevelIndex} FOR UPDATE")
            .FirstAsync(ct);

        var now             = DateTimeOffset.UtcNow;
        var levelCompleted  = false;
        var eventCompleted  = false;

        if (cmd.IsSuccess)
        {
            level.CurrentCount++;
            state.LastCountedStageId = cmd.StageId;

            if (level.CurrentCount >= level.RequiredCount)
            {
                level.LevelStatus  = "COMPLETED";
                level.CompletedAt  = now;
                level.RewardState  = "PENDING";
                levelCompleted     = true;

                // Unlock next level
                var nextLevel = await _db.StreakChallengeUserLevelStates
                    .FirstOrDefaultAsync(l => l.UserId == cmd.UserId && l.EventId == cmd.EventId
                                           && l.CycleId == cmd.CycleId && l.LevelIndex == cmd.LevelIndex + 1, ct);
                if (nextLevel is not null && nextLevel.LevelStatus == "LOCKED")
                {
                    nextLevel.LevelStatus = "READY";
                    nextLevel.UpdatedAt   = now;
                    state.CurrentLevel    = cmd.LevelIndex + 1;
                }
                else if (nextLevel is null)
                {
                    // Final level — event completes after all rewards claimed (not here)
                    state.CurrentLevel = cmd.LevelIndex;
                }
            }
        }
        else
        {
            // fail: reset current level count only
            level.CurrentCount = 0;
        }

        level.UpdatedAt  = now;
        state.UpdatedAt  = now;

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return new StreakChallengeProgressResult
        {
            LevelCompleted = levelCompleted,
            EventCompleted = eventCompleted,
            NewCount       = level.CurrentCount,
        };
    }

    public async Task<StreakChallengeClaimResult> ClaimRewardAsync(
        StreakChallengeClaimCommand cmd, CancellationToken ct)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        // Idempotency: check if already claimed with this correlationId
        var existing = await _db.StreakChallengeRewardClaimHistories
            .FirstOrDefaultAsync(c => c.CorrelationId == cmd.CorrelationId, ct);

        if (existing is { ClaimStatus: "SUCCESS" })
        {
            // Return current balances without re-granting
            var bal = await _db.UserCurrencies
                .FirstOrDefaultAsync(c => c.UserId == cmd.UserId, ct);
            await tx.RollbackAsync(ct);
            return new StreakChallengeClaimResult
            {
                IsNewClaim       = false,
                SoftBalanceAfter = bal?.SoftAmount ?? 0,
            };
        }

        var level = await _db.StreakChallengeUserLevelStates
            .FromSqlInterpolated($"SELECT * FROM streak_challenge_user_level_state WHERE user_id = {cmd.UserId} AND event_id = {cmd.EventId} AND cycle_id = {cmd.CycleId} AND level_index = {cmd.LevelIndex} FOR UPDATE")
            .FirstAsync(ct);

        if (level.RewardState != "PENDING")
        {
            await tx.RollbackAsync(ct);
            throw new InvalidOperationException("STREAK_REWARD_NOT_PENDING");
        }

        // Insert claim history (pending)
        var claimHistory = new StreakChallengeRewardClaimHistory
        {
            UserId             = cmd.UserId,
            EventId            = cmd.EventId,
            CycleId            = cmd.CycleId,
            LevelIndex         = cmd.LevelIndex,
            RewardGroupId      = cmd.RewardGroupId,
            RewardGroupVersion = cmd.RewardGroupVersion,
            RewardMultiplier   = cmd.RewardMultiplier,
            ClaimStatus        = "PENDING",
            CorrelationId      = cmd.CorrelationId,
            CreatedAt          = DateTimeOffset.UtcNow,
            UpdatedAt          = DateTimeOffset.UtcNow,
        };
        _db.StreakChallengeRewardClaimHistories.Add(claimHistory);
        await _db.SaveChangesAsync(ct);

        // Grant rewards
        await _db.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT IGNORE INTO user_currency (user_id, soft_amount) VALUES ({cmd.UserId}, 0)", ct);
        await _db.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT IGNORE INTO inventory (user_id, item_id, quantity) VALUES ({cmd.UserId}, 0, 0)", ct);

        _db.ChangeTracker.Clear();

        var currency = await _db.UserCurrencies
            .FromSqlInterpolated($"SELECT * FROM user_currency WHERE user_id = {cmd.UserId} FOR UPDATE")
            .FirstAsync(ct);

        var inventoryUpdates = new Dictionary<int, int>();
        long softGranted     = 0;

        foreach (var item in cmd.RewardItems)
        {
            var amount = item.Amount * cmd.RewardMultiplier;
            if (item.ItemType == "SOFT_CURRENCY")
            {
                softGranted        += amount;
                currency.SoftAmount += amount;
            }
            else if (item.ItemType == "ITEM")
            {
                await _db.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO inventory (user_id, item_id, quantity) VALUES ({cmd.UserId}, {item.ItemId}, {amount}) ON DUPLICATE KEY UPDATE quantity = quantity + {amount}", ct);
                _db.ChangeTracker.Clear();

                var inv = await _db.Inventories
                    .FirstOrDefaultAsync(i => i.UserId == cmd.UserId && i.ItemId == item.ItemId, ct);
                inventoryUpdates[item.ItemId] = inv?.Quantity ?? 0;
            }
        }

        await _db.SaveChangesAsync(ct);

        // Mark reward claimed
        await _db.Database.ExecuteSqlInterpolatedAsync($"""
            UPDATE streak_challenge_user_level_state
            SET reward_state = 'CLAIMED', reward_claimed_at = NOW(), updated_at = NOW()
            WHERE user_id = {cmd.UserId} AND event_id = {cmd.EventId}
              AND cycle_id = {cmd.CycleId} AND level_index = {cmd.LevelIndex}
            """, ct);

        // Update claim history to SUCCESS
        await _db.Database.ExecuteSqlInterpolatedAsync($"""
            UPDATE streak_challenge_reward_claim_history
            SET claim_status = 'SUCCESS', updated_at = NOW()
            WHERE correlation_id = {cmd.CorrelationId}
            """, ct);

        // Check if all levels claimed → mark event completed
        var pendingLevels = await _db.StreakChallengeUserLevelStates
            .Where(l => l.UserId == cmd.UserId && l.EventId == cmd.EventId && l.CycleId == cmd.CycleId
                     && l.RewardState == "PENDING")
            .CountAsync(ct);

        var completedLevels = await _db.StreakChallengeUserLevelStates
            .Where(l => l.UserId == cmd.UserId && l.EventId == cmd.EventId && l.CycleId == cmd.CycleId
                     && l.LevelStatus == "COMPLETED")
            .CountAsync(ct);

        var totalLevels = await _db.StreakChallengeUserLevelStates
            .CountAsync(l => l.UserId == cmd.UserId && l.EventId == cmd.EventId && l.CycleId == cmd.CycleId, ct);

        var eventCompleted = pendingLevels == 0 && completedLevels == totalLevels && totalLevels > 0;
        if (eventCompleted)
        {
            await _db.Database.ExecuteSqlInterpolatedAsync($"""
                UPDATE streak_challenge_user_state
                SET event_status = 'COMPLETED', updated_at = NOW()
                WHERE user_id = {cmd.UserId} AND event_id = {cmd.EventId} AND cycle_id = {cmd.CycleId}
                """, ct);
        }

        await tx.CommitAsync(ct);

        return new StreakChallengeClaimResult
        {
            IsNewClaim       = true,
            SoftBalanceAfter = currency.SoftAmount,
            InventoryAfter   = inventoryUpdates,
            EventCompleted   = eventCompleted,
        };
    }

    public async Task LazyResetAsync(StreakChallengeLazyResetCommand cmd, CancellationToken ct)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        // Soft-delete: mark expired rewards
        await _db.Database.ExecuteSqlInterpolatedAsync($"""
            UPDATE streak_challenge_user_level_state
            SET reward_state = 'EXPIRED', updated_at = NOW()
            WHERE user_id = {cmd.UserId} AND event_id = {cmd.EventId}
              AND cycle_id = {cmd.CycleId} AND reward_state = 'PENDING'
            """, ct);

        // Mark event as expired
        await _db.Database.ExecuteSqlInterpolatedAsync($"""
            UPDATE streak_challenge_user_state
            SET event_status = 'EXPIRED', updated_at = NOW()
            WHERE user_id = {cmd.UserId} AND event_id = {cmd.EventId}
              AND cycle_id = {cmd.CycleId} AND event_status = 'ACTIVE'
            """, ct);

        await tx.CommitAsync(ct);
    }

    public async Task RecordAdAsync(StreakChallengeAdRewardHistory entry, CancellationToken ct)
    {
        _db.StreakChallengeAdRewardHistories.Add(entry);
        await _db.SaveChangesAsync(ct);
    }
}
