using Content.Shared.DeviceLinking;
using Content.Shared.Power;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._L5.StaticField;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class StaticFieldComponent : Component
{
    [DataField, AutoNetworkedField]
    public PowerDeviceVisuals Status = PowerDeviceVisuals.Powered;

    [DataField, AutoNetworkedField]
    public bool Powered = false;

    [DataField]
    public ProtoId<SinkPortPrototype> OnPort = "On";

    [DataField]
    public ProtoId<SinkPortPrototype> OffPort = "Off";

    [DataField]
    public ProtoId<SinkPortPrototype> TogglePort = "Toggle";

    public SoundSpecifier PowerUpSound = new SoundPathSpecifier("/Audio/_L5/Effects/StaticField/powerup.ogg");
    public SoundSpecifier PowerDownSound = new SoundPathSpecifier("/Audio/_L5/Effects/StaticField/powerdown.ogg");
}
