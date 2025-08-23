using Content.Shared.DeviceLinking;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._L5.StaticField;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class StaticFieldComponent : Component
{
    [DataField, AutoNetworkedField]
    public bool Powered = false;

    [DataField]
    public ProtoId<SinkPortPrototype> OnPort = "On";

    [DataField]
    public ProtoId<SinkPortPrototype> OffPort = "Off";

    [DataField]
    public ProtoId<SinkPortPrototype> TogglePort = "Toggle";

    [DataField]
    public SoundSpecifier PowerUpSound = new SoundPathSpecifier("/Audio/_L5/Effects/StaticField/powerup.ogg");

    [DataField]
    public SoundSpecifier PowerDownSound = new SoundPathSpecifier("/Audio/_L5/Effects/StaticField/powerdown.ogg");
}
