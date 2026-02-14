using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Robust.Shared.Network;

namespace Content.Shared._L5.OperateMob;

public class SharedOperateMobSystem : EntitySystem
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly SharedMindSystem _mind = default!;

    public List<MindContainerComponent> GetAvailableMinds(NetUserId? userId)
    {
        List<MindContainerComponent> availableMinds = new();

        if (userId == null)
            return availableMinds;

        var query = EntityQueryEnumerator<OperatedMobComponent>();
        while (query.MoveNext(out var mob, out var comp))
        {
            if (comp.Operator != userId)
                continue;

            if (!TryComp<MindContainerComponent>(mob, out var container))
                continue;

            availableMinds.Add(container);
        }

        return availableMinds;
    }

    public void OperateMob(NetUserId? player, EntityUid mind)
    {
        _mind.ControlMob(player!.Value, mind);
    }
}
