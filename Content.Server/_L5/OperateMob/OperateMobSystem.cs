using Content.Shared._L5.OperateMob;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;

namespace Content.Server._L5.OperateMob;

public sealed class OperateMobSystem : SharedOperateMobSystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MindComponent, MindGotAddedEvent>(OnMindGotAdded);
    }

    /// <summary>
    /// Store the operator's user ID the first time the mob is controlled
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="comp"></param>
    /// <param name="args"></param>
    private void OnMindGotAdded(EntityUid uid, MindComponent comp, ref MindGotAddedEvent args)
    {
        if (!TryComp<OperatedMobComponent>(args.Container, out var operatedMob) || operatedMob.Operator != null)
            return;

        operatedMob.Operator = comp.OriginalOwnerUserId;

        Dirty(uid, comp);
    }
}
