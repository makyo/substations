using Content.Shared.Dataset;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._L5.Traits.Moody.Components;

[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState, AutoGenerateComponentPause]
public sealed partial class MoodyComponent : Component
{
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    [AutoPausedField]
    [AutoNetworkedField]
    public TimeSpan NextUpdateTime;

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    [AutoPausedField]
    [AutoNetworkedField]
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
    public TimeSpan MinimumPopupDelay = TimeSpan.FromSeconds(40);

    /// <summary>
    /// Maximum time between pain popups.
    /// </summary>
    [DataField]
    public TimeSpan MaximumPopupDelay = TimeSpan.FromSeconds(60);
};
