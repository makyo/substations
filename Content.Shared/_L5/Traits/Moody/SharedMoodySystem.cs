using Content.Shared._L5.Traits.Moody.Components;
using Content.Shared.Popups;
using Content.Shared.StatusEffectNew;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._L5.Traits.Moody;

public abstract class SharedMoodySystem : EntitySystem
{
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly StatusEffectsSystem _status = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MoodyComponent, MapInitEvent>(OnMapInit);

        SubscribeLocalEvent<MoodyComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<MoodyComponent, ComponentShutdown>(OnComponentShutdown);
        SubscribeLocalEvent<MoodyComponent, LocalPlayerAttachedEvent>(OnLocalPlayerAttached);
        SubscribeLocalEvent<MoodyComponent, LocalPlayerDetachedEvent>(OnLocalPlayerDetached);
    }

    protected virtual void OnComponentShutdown(Entity<MoodyComponent> entity, ref ComponentShutdown args) { }
    protected virtual void OnComponentInit(Entity<MoodyComponent> entity, ref ComponentInit args) { }
    protected virtual void OnLocalPlayerAttached(Entity<MoodyComponent> entity, ref LocalPlayerAttachedEvent args) { }
    protected virtual void OnLocalPlayerDetached(Entity<MoodyComponent> entity, ref LocalPlayerDetachedEvent args) { }

    protected bool IsMoodySuppressed(Entity<MoodyComponent?> ent)
    {
        if (!Resolve(ent.Owner, ref ent.Comp, false))
            return true;

        return _status.HasEffectComp<MoodySuppressedStatusEffectComponent>(ent.Owner);
    }

    private void OnMapInit(Entity<MoodyComponent> ent, ref MapInitEvent args)
    {
        ent.Comp.NextUpdateTime = _timing.CurTime;
        ent.Comp.NextPopupTime = _timing.CurTime;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var curTime = _timing.CurTime;
        var query = EntityQueryEnumerator<MoodyComponent>();

        while (query.MoveNext(out var uid, out var component))
        {
            if (curTime < component.NextUpdateTime)
                continue;

            if (curTime >= component.NextPopupTime)
                ShowMoodPopup((uid, component));

            component.NextUpdateTime = curTime + TimeSpan.FromSeconds(5);
        }
    }

    private void ShowMoodPopup(Entity<MoodyComponent> entity)
    {
        // Don't notify
        if (IsMoodySuppressed((entity.Owner, entity.Comp)))
            return;

        if (!_proto.TryIndex(entity.Comp.DatasetPrototype, out var dataset))
            return;

        var effects = dataset.Values;
        if (effects.Count == 0)
            return;

        var effect = _random.Pick(effects);
        _popup.PopupEntity(Loc.GetString(effect), entity, entity);

        // Set next popup time
        var delay = _random.Next(entity.Comp.MinimumPopupDelay, entity.Comp.MaximumPopupDelay);
        entity.Comp.NextPopupTime = _timing.CurTime + delay;
    }
}
