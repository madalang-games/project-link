using System.Collections.Generic;

namespace ProjectLink.Services
{
    public sealed class LobbyScreenModel
    {
        public string DisplayName { get; set; }
        public int AvatarId { get; set; }
        public string AvatarIconPath { get; set; }
        public int StaminaCurrent { get; set; }
        public int StaminaMax { get; set; }
        public string NextRechargeAt { get; set; }
        public long SoftCurrency { get; set; }
        public int HighestStageId { get; set; }
        public int NextUnlockedStageId { get; set; }
        public int TotalStarsEarned { get; set; }
        public bool CanPlay { get; set; }
        public StreakChallengeModel StreakChallenge { get; set; }
        public SeasonEventModel SeasonEvent { get; set; }
    }

    public sealed class StreakChallengeModel
    {
        public string EventStatus          { get; set; }
        public string RemainingTimeIso     { get; set; }
        public int    CurrentLevel         { get; set; }
        public int    CurrentLevelCount    { get; set; }
        public int    CurrentLevelRequired { get; set; }
        public bool   HasPendingReward     { get; set; }
    }

    public sealed class SeasonEventModel
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public string EndAt { get; set; }
        public bool IsActive { get; set; }
        public string MetricLabel { get; set; }
    }

    public sealed class ShopScreenModel
    {
        public long SoftBalance { get; set; }
        public List<ShopProductModel> Products { get; } = new();
    }

    public sealed class ShopProductModel
    {
        public int ProductId { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public int GrantItemId { get; set; }
        public int GrantQuantity { get; set; }
        public int PriceSoft { get; set; }
        public string PriceIapSku { get; set; }
        public int SortOrder { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public bool IsIapProduct => !string.IsNullOrEmpty(PriceIapSku);
    }

    public sealed class EnergyPopupModel
    {
        public int Current { get; set; }
        public int Max { get; set; }
        public string NextRechargeAt { get; set; }
        public int AdRewardAmount { get; set; }
    }
}
