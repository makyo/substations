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

    private void OnMindGotAdded(EntityUid uid, MindComponent comp, ref MindGotAddedEvent args)
    {
        if (!TryComp<OperatedMobComponent>(args.Container, out var operatedMob))
            return;

        operatedMob.Operator = comp.OriginalOwnerUserId;

        Dirty(uid, comp);
    }
}
