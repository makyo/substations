using Content.Server.Atmos.Components;
using Content.Shared._L5.StaticField;
using Content.Shared.Power;
using Content.Shared.Power.EntitySystems;

namespace Content.Server._L5.StaticField;

public sealed class StaticFieldSystem : EntitySystem
{

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<StaticFieldComponent, PowerChangedEvent>(OnPowerChanged);
    }

    private void OnPowerChanged(Entity<StaticFieldComponent> ent, ref PowerChangedEvent evt)
    {
        ent.Comp.Powered = evt.Powered;
        if (evt.Powered)
        {
            EnsureComp<AirtightComponent>(ent);
        }
        else
        {
            RemCompDeferred<AirtightComponent>(ent);
        }
        Dirty(ent);
    }
}
