using Robust.Shared.GameStates;
using Robust.Shared.Network;

namespace Content.Shared._L5.OperateMob;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class OperatedMobComponent : Component
{
    [DataField, ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public NetUserId? Operator = default!;
}
