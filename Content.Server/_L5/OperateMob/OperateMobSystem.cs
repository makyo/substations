using Content.Server.Mind;
using Content.Shared._L5.OperateMob;

namespace Content.Server._L5.OperateMob;

public sealed class OperateMobSystem : SharedOperateMobSystem
{
    [Dependency] private readonly IEntityManager _entity = default!;
    [Dependency] private readonly MindSystem _mind = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeNetworkEvent<OperateMobMessage>(OperateMob);
    }

    private void OperateMob(OperateMobMessage ev)
    {
        if (_entity.TryGetEntity(ev.Mob, out var mob))
            _mind.ControlMob(ev.User, mob.Value);
    }
}
