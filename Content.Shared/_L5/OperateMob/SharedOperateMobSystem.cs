using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._L5.OperateMob;

public abstract class SharedOperateMobSystem : EntitySystem
{
    [Dependency] private readonly IEntityManager _entity = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OperatedMobComponent, MindAddedMessage>(OnMindGotAdded);
    }

    /// <summary>
    /// Store the operator's user ID the first time the mob is controlled
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="comp"></param>
    /// <param name="args"></param>
    private void OnMindGotAdded(EntityUid uid, OperatedMobComponent comp, ref MindAddedMessage args)
    {
        if (comp.Operator != null)
            return;

        comp.Operator = args.Mind.Comp.OriginalOwnerUserId;
        Dirty(uid, comp);
    }

    /// <summary>
    /// Retrieve a list of all entities this user is the operator of.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public List<Entity<OperatedMobComponent>> GetAvailableMobs(NetUserId? userId)
    {
        List<Entity<OperatedMobComponent>> availableMobs = [];

        if (userId == null)
            return availableMobs;

        var query = EntityQueryEnumerator<OperatedMobComponent>();
        while (query.MoveNext(out var mob, out var comp))
        {
            if (comp.Operator == userId)
                availableMobs.Add((mob, comp));
        }

        return availableMobs;
    }

    public void OperateMob(NetUserId? player, Entity<OperatedMobComponent> mob, List<Entity<OperatedMobComponent>> availableMobs)
    {
        if (!_entity.TryGetNetEntity(mob, out var nMob))
            return;

        // Notify the server to swap the mind over.
        var ev = new OperateMobMessage(player!.Value, nMob.Value);
        RaiseNetworkEvent(ev);
    }
}

[Serializable, NetSerializable]
public sealed class OperateMobMessage(NetUserId user, NetEntity mob) : EntityEventArgs
{
    public NetUserId User = user;
    public NetEntity Mob = mob;
}
