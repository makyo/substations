using System.Linq;

namespace Content.Shared._L5.LongTermHealth;

[RegisterComponent]
[AutoGenerateComponentState,AutoGenerateComponentPause]
public sealed partial class LongTermHealthComponent : Component
{
    /// <summary>
    /// Effects to be applied once the threshold to apply them arrives.
    /// </summary>
    [DataField]
    public Dictionary<EffectType, bool> UpcomingEffects = new();

    /// <summary>
    /// Effects currently applied to the player and their countdowns
    /// </summary>
    [DataField]
    public Dictionary<EffectType, TimeSpan> CurrentEffects = new();

    /// <summary>
    /// Effects that hve been applied in the past and how many times they've been applied.
    /// </summary>
    [DataField]
    public Dictionary<EffectType, int> PreviousEffects = new();

    /// <summary>
    /// How many effects from genetic damage will be applied next update.
    /// </summary>
    [DataField]
    public int UpcomingGeneticEffects;

    /// <summary>
    /// How often the effects should update. Doesn't need to be too frequently.
    /// </summary>
    [DataField]
    [AutoNetworkedField]
    public TimeSpan UpdateInterval = TimeSpan.FromSeconds(2);

    [DataField]
    [AutoPausedField]
    public TimeSpan NextUpdate = TimeSpan.Zero;
}

public enum EffectType
{
    MildPain,
    SeverePain,

    MildImpairedMobility,
    SevereImpairedMobility,

    MildMoody,
    SevereMoody,
    MildHearingLoss,
    SevereHearingLoss,
    MildParacusia,
    SevereParacusia,
    MildVisionLoss,
    SevereVisionLoss,
    MildWoozy,
    SevereWoozy,

    BurnReturn,
    PoisonReturn,

    MildBrainDamage,
    SevereBrainDamage,
    MildLungDamage,
    SevereLungDamage,
}

public static class EffectTypeExtensions
{
    public static EffectType[] MildEffects =
    [
        EffectType.MildPain,
        EffectType.MildImpairedMobility,
        EffectType.MildLungDamage,
        EffectType.MildMoody,
        EffectType.MildHearingLoss,
        EffectType.MildParacusia,
        EffectType.MildVisionLoss,
        EffectType.MildWoozy,
    ];

    public static EffectType[] SevereEffects =
    [
        EffectType.SeverePain,
        EffectType.SevereImpairedMobility,
        EffectType.SevereLungDamage,
        EffectType.SevereMoody,
        EffectType.SevereHearingLoss,
        EffectType.SevereParacusia,
        EffectType.SevereVisionLoss,
        EffectType.SevereWoozy,
    ];

    public static readonly EffectType[] MildTBIs =
    [
        EffectType.MildHearingLoss,
        EffectType.MildMoody,
        EffectType.MildParacusia,
        EffectType.MildVisionLoss,
        EffectType.MildWoozy,
    ];

    public static readonly EffectType[] SevereTBIs =
    [
        EffectType.SevereHearingLoss,
        EffectType.SevereMoody,
        EffectType.SevereParacusia,
        EffectType.SevereVisionLoss,
        EffectType.SevereWoozy,
    ];

    public static readonly EffectType[] AllTBIs = MildTBIs.Concat(SevereTBIs).ToArray();

    // Given that you were rotting, these are all pretty severe.
    public static readonly EffectType[] GeneticEffects =
    [
        EffectType.SeverePain,
        EffectType.SevereImpairedMobility,
        EffectType.SevereLungDamage,
        EffectType.SevereMoody,
        EffectType.SevereHearingLoss,
        EffectType.SevereParacusia,
        EffectType.SevereVisionLoss,
        EffectType.SevereWoozy,
        EffectType.BurnReturn,
        EffectType.PoisonReturn,
    ];
}
