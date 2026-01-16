using Content.Shared._L5.CCVar;
using Content.Shared.Damage.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Timing;

namespace Content.Shared._L5.LongTermHealth;

public sealed class LongTermHealthSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _configurationManager = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<LongTermHealthComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<LongTermHealthComponent, DamageChangedEvent>(OnDamageChanged);
    }

    private void OnMapInit(Entity<LongTermHealthComponent> ent, ref MapInitEvent evt)
    {
        ent.Comp.NextUpdate = _timing.CurTime + ent.Comp.UpdateInterval;
    }

    private void OnDamageChanged(EntityUid uid, LongTermHealthComponent component, DamageChangedEvent args)
    {
        var temporaryEffectSeconds = _configurationManager.GetCVar(L5CCVars.LongTermEffectsDuration);
        // Decide, based on the damage specifier, what events need to be added to upcoming, moved to current, or moved to past.
        // Apply any events as needed
        // Note to self: don't add to datafields if the player already has traits with those effects (don't add pain if they have the chronic pain trait)
        // component.TemporaryEffectCountdowns[key] = TimeSpan.FromSeconds(temporaryEffectSeconds)
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var curTime = _timing.CurTime;


        var query = EntityQueryEnumerator<LongTermHealthComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (comp.NextUpdate > curTime)
                continue;

            // Tick up return damages
            foreach (var key in comp.TemporaryEffectCountdowns.Keys)
            {
                comp.TemporaryEffectCountdowns[key] -= comp.UpdateInterval;
                if (comp.TemporaryEffectCountdowns[key] < TimeSpan.Zero)
                {
                    // Tick down timer on temporary events (or maybe there's a timer remove thing)
                    // remove components/etc
                    comp.TemporaryEffectCountdowns.Remove(key);
                }
            }

            comp.NextUpdate += comp.UpdateInterval;
        }
    }
}
