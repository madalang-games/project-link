using ProjectLink.Application.Stage;
using ProjectLink.Application.StreakChallenge;
using ProjectLink.Domain.Entities;
using ProjectLink.Domain.Exceptions;
using ProjectLink.Domain.Interfaces;
using ProjectLink.Domain.Stage;
using ProjectLink.Domain.StaticData;
using Xunit;

namespace ProjectLink.API.Tests;

public sealed class StageServiceTests
{
    [Fact]
    public async Task StartAsync_ReplacesActiveSessionWithNewPaidAttempt()
    {
        var cache = new TestStageSessionCache
        {
            Current = new StageSession
            {
                UserId = "user-1",
                StageId = 1,
                Token = "old-token",
                StartAtMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            },
        };
        var stamina = new TestStaminaRepository();
        var service = CreateService(cache, stamina);

        var response = await service.StartAsync("user-1", 1, CancellationToken.None);

        Assert.Equal(1, stamina.DeductCount);
        Assert.NotEqual("old-token", response.SessionToken);
        Assert.Equal(response.SessionToken, cache.Current!.Token);
        Assert.Equal(1, cache.Current.StageId);
    }

    [Fact]
    public async Task StartAsync_InvalidatesPreviousSessionToken()
    {
        var cache = new TestStageSessionCache
        {
            Current = new StageSession
            {
                UserId = "user-1",
                StageId = 1,
                Token = "old-token",
                StartAtMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            },
        };
        var service = CreateService(cache, new TestStaminaRepository());

        var response = await service.StartAsync("user-1", 1, CancellationToken.None);

        await Assert.ThrowsAsync<StageSessionNotFoundException>(() =>
            service.EndAsync("user-1", 1, "old-token", "fail", 0, 0, "test", CancellationToken.None));

        await service.EndAsync("user-1", 1, response.SessionToken, "fail", 0, 0, "test", CancellationToken.None);
        Assert.Null(cache.Current);
    }

    [Fact]
    public async Task EndAsync_SuccessRequestsStartStaminaRefund()
    {
        var cache = new TestStageSessionCache
        {
            Current = new StageSession
            {
                UserId = "user-1",
                StageId = 1,
                Token = "token",
                StartAtMs = DateTimeOffset.UtcNow.AddSeconds(-10).ToUnixTimeMilliseconds(),
                IsSetupPhase = false,
            },
        };
        var stageEndTx = new TestStageEndTransaction();
        var service = CreateService(cache, new TestStaminaRepository(), stageEndTx);

        await service.EndAsync("user-1", 1, "token", "success", 10_000, 10, "test", CancellationToken.None);

        Assert.NotNull(stageEndTx.LastCommand);
        Assert.Equal(1, stageEndTx.LastCommand.StaminaRefund);
        Assert.Equal(5, stageEndTx.LastCommand.MaxStamina);
        Assert.Equal(5, stageEndTx.LastCommand.RechargeIntervalMinutes);
    }

    [Fact]
    public async Task EndAsync_ReplayReturnsGrantedSoftRewardOnly()
    {
        var cache = new TestStageSessionCache
        {
            Current = new StageSession
            {
                UserId = "user-1",
                StageId = 1,
                Token = "token",
                StartAtMs = DateTimeOffset.UtcNow.AddSeconds(-10).ToUnixTimeMilliseconds(),
                IsSetupPhase = false,
            },
        };
        var stageEndTx = new TestStageEndTransaction
        {
            Result = new StageEndDbResult
            {
                SoftBalanceAfter = 100,
                SoftRewardGranted = 0,
            },
        };
        var service = CreateService(cache, new TestStaminaRepository(), stageEndTx);

        var response = await service.EndAsync("user-1", 1, "token", "success", 10_000, 10, "test", CancellationToken.None);

        Assert.Equal(100, response.SoftBalanceAfter);
        Assert.Equal(0, response.SoftReward);
    }

    static StageService CreateService(TestStageSessionCache cache, TestStaminaRepository stamina)
        => CreateService(cache, stamina, new TestStageEndTransaction());

    static StageService CreateService(TestStageSessionCache cache, TestStaminaRepository stamina, TestStageEndTransaction stageEndTx)
    {
        var staticData = new TestStaticDataService();
        var streakRepo = new NoopStreakChallengeRepository();
        var streakTx   = new NoopStreakChallengeTransaction();
        var streak     = new StreakChallengeService(streakRepo, streakTx, staticData);
        return new StageService(cache, stamina, new TestInventoryRepository(), staticData, null!, stageEndTx, streak);
    }

    sealed class NoopStreakChallengeRepository : IStreakChallengeRepository
    {
        public Task<StreakChallengeUserState?> GetActiveStateAsync(string userId, int eventId, CancellationToken ct) => Task.FromResult<StreakChallengeUserState?>(null);
        public Task<List<StreakChallengeUserLevelState>> GetLevelStatesAsync(string userId, int eventId, string cycleId, CancellationToken ct) => Task.FromResult(new List<StreakChallengeUserLevelState>());
        public Task<bool> HasAdUsedForLevelAsync(string userId, int eventId, string cycleId, int levelIndex, CancellationToken ct) => Task.FromResult(false);
        public Task<StreakChallengeRewardClaimHistory?> GetClaimHistoryByCorrelationAsync(string correlationId, CancellationToken ct) => Task.FromResult<StreakChallengeRewardClaimHistory?>(null);
    }

    sealed class NoopStreakChallengeTransaction : IStreakChallengeTransaction
    {
        public Task ActivateAsync(StreakChallengeActivateCommand cmd, CancellationToken ct) => Task.CompletedTask;
        public Task StartLevelAsync(StreakChallengeStartLevelCommand cmd, CancellationToken ct) => Task.CompletedTask;
        public Task<StreakChallengeProgressResult> RecordProgressAsync(StreakChallengeProgressCommand cmd, CancellationToken ct) => Task.FromResult(new StreakChallengeProgressResult());
        public Task<StreakChallengeClaimResult> ClaimRewardAsync(StreakChallengeClaimCommand cmd, CancellationToken ct) => Task.FromResult(new StreakChallengeClaimResult());
        public Task LazyResetAsync(StreakChallengeLazyResetCommand cmd, CancellationToken ct) => Task.CompletedTask;
        public Task RecordAdAsync(StreakChallengeAdRewardHistory entry, CancellationToken ct) => Task.CompletedTask;
    }

    sealed class TestStageSessionCache : IStageSessionCache
    {
        public StageSession? Current { get; set; }

        public Task<StageSession?> GetAsync(string userId, CancellationToken ct)
            => Task.FromResult(Current);

        public Task SetAsync(string userId, StageSession session, TimeSpan ttl, CancellationToken ct)
        {
            Current = session;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string userId, CancellationToken ct)
        {
            Current = null;
            return Task.CompletedTask;
        }
    }

    sealed class TestStaminaRepository : IStaminaRepository
    {
        public int DeductCount { get; private set; }

        public Task<StaminaState> GetComputedAsync(string userId, int maxStamina, int rechargeIntervalMinutes, CancellationToken ct)
            => Task.FromResult(new StaminaState { UserId = userId, Current = maxStamina, LastRechargedAt = DateTimeOffset.UtcNow });

        public Task<StaminaState> DeductAsync(string userId, int maxStamina, int rechargeIntervalMinutes, CancellationToken ct)
        {
            DeductCount++;
            return Task.FromResult(new StaminaState { UserId = userId, Current = maxStamina - DeductCount, LastRechargedAt = DateTimeOffset.UtcNow });
        }

        public Task<(StaminaState state, int added)> AddAsync(string userId, int maxStamina, int rechargeIntervalMinutes, int toAdd, CancellationToken ct)
            => Task.FromResult((new StaminaState { UserId = userId, Current = maxStamina, LastRechargedAt = DateTimeOffset.UtcNow }, toAdd));
    }

    sealed class TestInventoryRepository : IInventoryRepository
    {
        public Task<List<Inventory>> GetAllAsync(string userId, CancellationToken ct) => Task.FromResult(new List<Inventory>());
        public Task<int> GrantAsync(string userId, int itemId, int quantity, CancellationToken ct) => Task.FromResult(quantity);
        public Task<int> DeductAsync(string userId, int itemId, int quantity, CancellationToken ct) => Task.FromResult(0);
    }

    sealed class TestStaticDataService : IStaticDataService
    {
        public IngameStageData? GetStage(int stageId) => GetAllStages().FirstOrDefault(x => x.StageId == stageId);
        public IngameItemData? GetItem(int itemId) => null;
        public IReadOnlyList<IngameStageData> GetAllStages() => [new IngameStageData { StageId = 1, Width = 5, Height = 5, TimeLimit = 60 }];
        public IReadOnlyList<IngameItemData> GetAllItems() => [];
        public OutgameStaminaConfigData GetStaminaConfig() => new() { ConfigId = 1, MaxStamina = 5, RechargeSeconds = 300 };
        public IReadOnlyList<OutgameAvatarData> GetAllAvatars() => [];
        public IReadOnlyList<OutgameShopCatalogData> GetShopCatalog() => [];
        public OutgameShopCatalogData? GetShopProduct(int productId) => null;
        public IReadOnlyList<OutgameSeasonEventData> GetAllSeasonEvents() => [];
        public StreakChallengeEventData? GetStreakChallengeEvent(int eventId, int version) => null;
        public StreakChallengeEventData? GetLatestEnabledStreakChallengeEvent() => null;
        public IReadOnlyList<StreakChallengeLevelData> GetStreakChallengeLevels(int eventId, int version) => [];
        public IReadOnlyList<StreakChallengeRewardItemData> GetStreakChallengeRewardItems(int rewardGroupId, int rewardGroupVersion) => [];
    }

    sealed class TestStageEndTransaction : IStageEndTransaction
    {
        public StageEndDbCommand? LastCommand { get; private set; }
        public StageEndDbResult Result { get; set; } = new();

        public Task<StageEndDbResult> ExecuteAsync(StageEndDbCommand command, CancellationToken ct)
        {
            LastCommand = command;
            return Task.FromResult(Result);
        }
    }
}
