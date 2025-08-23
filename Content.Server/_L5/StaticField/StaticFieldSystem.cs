using Content.Server.Atmos.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Shared._L5.StaticField;
using Content.Shared.DeviceLinking.Events;
using Content.Shared.Power;
using Content.Shared.Power.EntitySystems;
using Robust.Server.Audio;

namespace Content.Server._L5.StaticField;

public sealed class StaticFieldSystem : EntitySystem
{
    [Dependency] private readonly AirtightSystem _airtightSystem = default!;
    [Dependency] private readonly AudioSystem _audioSystem = default!;
    [Dependency] private readonly SharedPowerReceiverSystem _receiverSystem = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<StaticFieldComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<StaticFieldComponent, PowerChangedEvent>(OnPowerChanged);
        SubscribeLocalEvent<StaticFieldComponent, SignalReceivedEvent>(OnSignalReceived);
    }

    private void OnMapInit(EntityUid ent, StaticFieldComponent comp, MapInitEvent args)
    {
        if (!TryComp<AirtightComponent>(ent, out var airtight))
            return;
        _airtightSystem.SetAirblocked((ent, airtight), false);

        _receiverSystem.SetPowerDisabled(ent, true);
    }

    private void OnPowerChanged(Entity<StaticFieldComponent> ent, ref PowerChangedEvent args)
    {
        SetState(ent, args.Powered);
    }

    private void OnSignalReceived(Entity<StaticFieldComponent> ent, ref SignalReceivedEvent args)
    {
        if (args.Port == ent.Comp.OffPort)
        {
            _receiverSystem.SetPowerDisabled(ent, true);
            SetState(ent, false);
        }
        else if (args.Port == ent.Comp.OnPort)
        {
            _receiverSystem.SetPowerDisabled(ent, false);
            SetState(ent, true);
        }
        else if (args.Port == ent.Comp.TogglePort)
        {
            _receiverSystem.TogglePower(ent, false);
            SetState(ent, !ent.Comp.Powered);
        }
    }

    private void SetState(Entity<StaticFieldComponent> ent, bool state)
    {
        ent.Comp.Powered = state;

        // Turn on/off the airtight status of the field.
        if (!TryComp<AirtightComponent>(ent, out var airtight))
            return;
        _airtightSystem.SetAirblocked((ent, airtight), state);

        // Play the power up/down sound
        _audioSystem.PlayPvs(
            state ? ent.Comp.PowerUpSound : ent.Comp.PowerDownSound,
            ent);

        Dirty(ent);
    }
}
