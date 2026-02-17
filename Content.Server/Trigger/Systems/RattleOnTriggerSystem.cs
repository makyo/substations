using Content.Server.Radio.EntitySystems;
using Content.Server.Pinpointer;
using Content.Shared.Mobs.Components;
using Content.Shared.Trigger;
using Content.Shared.Trigger.Components.Effects;
using Robust.Server.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server.Trigger.Systems;

public sealed class RattleOnTriggerSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly RadioSystem _radio = default!;
    [Dependency] private readonly NavMapSystem _navMap = default!;
    [Dependency] private readonly TransformSystem _transform = default!; // L5 - trigger refactor for DeltaV

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RattleOnTriggerComponent, TriggerEvent>(OnTrigger);
    }

    private void OnTrigger(Entity<RattleOnTriggerComponent> ent, ref TriggerEvent args)
    {
        if (args.Key != null && !ent.Comp.KeysIn.Contains(args.Key))
            return;

        var target = ent.Comp.TargetUser ? args.User : ent.Owner;

        if (target == null)
            return;

        if (!TryComp<MobStateComponent>(target.Value, out var mobstate))
            return;

        args.Handled = true;

        if (!ent.Comp.Messages.TryGetValue(mobstate.CurrentState, out var messageId))
            return;

        // Gets the location of the user
        // L5 - migrated DeltaV lines for trigger refactor
        var position = _transform.GetMapCoordinates(Transform(ent)).Position; // DeltaV
        var posText = FormattedMessage.RemoveMarkupOrThrow(_navMap.GetNearestBeaconString(ent.Owner) + $" ({(int)position[0]}, {(int)position[1]})"); // DeltaV modified, adds the GPS coordinates on the message.

        var message = Loc.GetString(messageId, ("user", target.Value), ("position", posText));
        // Sends a message to the radio channel specified by the implant
        _radio.SendRadioMessage(ent.Owner, message, _prototypeManager.Index(ent.Comp.RadioChannel), ent.Owner);
    }
}
