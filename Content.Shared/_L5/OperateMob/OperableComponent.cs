using Robust.Shared.GameStates;
using Robust.Shared.Network;

namespace Content.Shared._L5.OperateMob;

[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class OperableComponent : Component
{
    [DataField]
    [AutoNetworkedField]
    public NetUserId? Operator;
}
