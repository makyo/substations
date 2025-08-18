using Robust.Shared.GameStates;

namespace Content.Shared._L5.StaticField;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class StaticFieldComponent : Component
{
    [DataField, AutoNetworkedField]
    public bool Powered = false;

    public bool AlwaysPowered = false;
}
