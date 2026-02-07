using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Players;
using Robust.Shared.Network;

namespace Content.Shared._L5.OperateMob;

public sealed class SharedOperateMobSystem : EntitySystem
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly SharedMindSystem _mind = default!;

    public List<MindContainerComponent> GetAvailableMinds(NetUserId? userId)
    {
        List<MindContainerComponent> availableMinds = new();

        if (userId == null)
            return availableMinds;

        EntityUid userMindId = default!;

        foreach (var mind in _entityManager.EntityQuery<MindComponent>())
        {
            if (mind.OwnedEntity == null || mind.OriginalOwnerUserId != userId)
                continue;

            userMindId = mind.Owner;
            break;
        }

        foreach (var container in _entityManager.EntityQuery<MindContainerComponent>())
        {
            if (container.OriginalMind == userMindId)
                availableMinds.Add(container);
        }
        return availableMinds;
    }

    public void OperateMob(NetUserId? player, EntityUid mind)
    {
        _mind.ControlMob(player!.Value, mind);
    }
}
