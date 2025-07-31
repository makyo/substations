using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Robust.Shared.Audio;
// ReSharper disable InconsistentNaming

namespace Content.Server._EE.Medical.CPR;

[RegisterComponent]
public sealed partial class CPRTrainingComponent : Component
{
    [DataField]
    public SoundSpecifier CPRSound = new SoundPathSpecifier("/Audio/Effects/CPR.ogg");

    [DataField]
    public TimeSpan DoAfterDuration = TimeSpan.FromSeconds(3);

    /// <summary>
    /// L5 - track doafter being active.
    /// </summary>
    [DataField]
    public DoAfterId? DoAfter;

    public EntityUid? CPRPlayingStream;
}
