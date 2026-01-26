using Content.Shared.Dataset;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._L5.Moody.Components;

[RegisterComponent, AutoGenerateComponentPause]
public sealed partial class MoodyComponent : Component
{
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    [AutoPausedField]
    public TimeSpan NextUpdateTime;

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    [AutoPausedField]
    public TimeSpan NextPopupTime;

    /// <summary>
    /// The dataset of pain effect messages to display.
    /// </summary>
    [DataField]
    public ProtoId<LocalizedDatasetPrototype> DatasetPrototype = "MoodyEffects";

    /// <summary>
    /// Minimum time between pain popups.
    /// </summary>
    [DataField]
    public TimeSpan MinimumPopupDelay = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Maximum time between pain popups.
    /// </summary>
    [DataField]
    public TimeSpan MaximumPopupDelay = TimeSpan.FromSeconds(10);
};
