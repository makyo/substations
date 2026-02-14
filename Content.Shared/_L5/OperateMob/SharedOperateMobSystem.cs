using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Robust.Shared.Network;

namespace Content.Shared._L5.OperateMob;

public class SharedOperateMobSystem : EntitySystem
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly SharedMindSystem _mind = default!;

    /// <summary>
    /// Retrieve a list of all entities this user is the operator of.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public List<Entity<MindContainerComponent>> GetOperatedEntities(NetUserId? userId)
    {
        List<Entity<MindContainerComponent>> availableMinds = new();

        if (userId == null)
            return availableMinds;

        var query = EntityQueryEnumerator<OperatedMobComponent>();
        while (query.MoveNext(out var mob, out var comp))
        {
            if (comp.Operator != userId)
                continue;

            if (!TryComp<MindContainerComponent>(mob, out var container))
                continue;

            availableMinds.Add((mob, container));
        }

        return availableMinds;
    }

    public void OperateMob(NetUserId? player, EntityUid mind)
    {
        _mind.ControlMob(player!.Value, mind);
    }
}
