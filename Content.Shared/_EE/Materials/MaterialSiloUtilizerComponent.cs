using Robust.Shared.GameStates;

namespace Content.Shared._EE.Materials;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MaterialSiloUtilizerComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntityUid? Silo;
}
