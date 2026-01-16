namespace Content.Shared._L5.LongTermHealth;

[RegisterComponent]
[AutoGenerateComponentState,AutoGenerateComponentPause]
public sealed partial class LongTermHealthComponent : Component
{
    /// <summary>
    /// Effects to be applied once the threshold to apply them arrives.
    /// </summary>
    [DataField]
    public List<Effect> UpcomingEffects = new();

    /// <summary>
    /// Effects currently applied to the player.
    /// </summary>
    [DataField]
    public List<Effect> CurrentEffects = new();

    /// <summary>
    /// Any countdowns for temporary effects currently applied.
    /// </summary>
    [DataField]
    public Dictionary<Effect, TimeSpan> TemporaryEffectCountdowns = new();

    /// <summary>
    /// Effects that hve been applied in the past and how many times they've been applied.
    /// </summary>
    [DataField]
    public Dictionary<Effect, int> PreviousEffects = new();

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

public enum Effect
{
    TemporaryPain,
    PermanentPain,
    TemporaryImpairedMobility,
    PermanentImpairedMobility,
    Woozy,
    Moody,
    Paracusia,
    HearingLoss,
    VisionLoss,
    BurnReturn,
    PoisonReturn,
    TemporaryLungDamage,
    PermanentLungDamage,
}
