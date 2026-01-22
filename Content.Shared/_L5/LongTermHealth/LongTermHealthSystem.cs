using Content.Shared._L5.CCVar;
using Content.Shared.Damage;
using Content.Shared.Damage.Systems;
using Content.Shared.FixedPoint;
using Content.Shared.Toilet.Components;
using Robust.Shared.Configuration;
using Robust.Shared.Random;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Timing;

namespace Content.Shared._L5.LongTermHealth;

public sealed partial class LongTermHealthSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _configurationManager = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    private float mildEffectSeconds, severeEffectSeconds, healDecayFactor;
    private bool healDecayEnabled;

    public override void Initialize()
    {
        base.Initialize();

        mildEffectSeconds = _configurationManager.GetCVar(L5CCVars.LongTermEffectsDuration);
        severeEffectSeconds = mildEffectSeconds * _configurationManager.GetCVar(L5CCVars.LongTermEffectSevereMultiplier);
        healDecayEnabled = _configurationManager.GetCVar(L5CCVars.LongTermEffectsHealDecayEnabled);
        healDecayFactor = 1f;
        if (healDecayEnabled)
            healDecayFactor = _configurationManager.GetCVar(L5CCVars.LongTermEffectsHealDecayFactor);

        SubscribeLocalEvent<LongTermHealthComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<LongTermHealthComponent, DamageChangedEvent>(OnDamageChanged);
    }

    private void OnMapInit(Entity<LongTermHealthComponent> ent, ref MapInitEvent evt)
    {
        ent.Comp.NextUpdate = _timing.CurTime + ent.Comp.UpdateInterval;
    }

    private void OnDamageChanged(EntityUid uid, LongTermHealthComponent component, DamageChangedEvent args)
    {
        if (args.DamageDelta == null)
            return;

        HandleAirloss(uid, ref component, args);
        HandleBrute(uid, ref component, args);
        HandleBurn(uid, ref component, args);
        HandleToxin(uid, ref component, args);
        HandleGenetic(uid, ref component, args);
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
            foreach (var key in comp.CurrentEffects.Keys)
            {
                comp.CurrentEffects[key] -= comp.UpdateInterval;
                if (comp.CurrentEffects[key] < TimeSpan.Zero)
                {
                    // remove components/etc
                    comp.CurrentEffects.Remove(key);
                }
            }

            comp.NextUpdate += comp.UpdateInterval;
        }
    }

    private void ClearUpcomingTBIs(ref LongTermHealthComponent component)
    {
        foreach (var tbi in EffectTypeExtensions.AllTBIs)
        {
            if (component.UpcomingEffects.ContainsKey(tbi))
            {
                component.UpcomingEffects.Remove(tbi);
            }
        }
    }

    private void PrepareEffect(
        ref LongTermHealthComponent component,
        FixedPoint2 damage,
        EffectType mildType,
        EffectType severeType,
        CVarDef<float> mildCVar,
        CVarDef<float> severeCVar)
    {
        if (!component.CurrentEffects.ContainsKey(severeType) &&
            damage > _configurationManager.GetCVar(severeCVar))
        {
            component.UpcomingEffects.Remove(mildType);
            component.UpcomingEffects[severeType] = true;
        }
        else if (!component.CurrentEffects.ContainsKey(mildType) &&
                 damage > _configurationManager.GetCVar(mildCVar))
        {
            component.UpcomingEffects.Remove(severeType);
            component.UpcomingEffects[mildType] = true;
        }
    }

    private void ApplyEffect(
        ref LongTermHealthComponent component,
        FixedPoint2 damage,
        EffectType mildType,
        EffectType severeType,
        CVarDef<float> mildCVar,
        CVarDef<float> severeCVar)
    {
        if (component.UpcomingEffects.ContainsKey(severeType) &&
            damage < _configurationManager.GetCVar(severeCVar))
        {
            var duration = severeEffectSeconds;
            component.UpcomingEffects.Remove(severeType);

            if (healDecayEnabled && component.PreviousEffects.TryGetValue(severeType, out var count))
                duration *= healDecayFactor * count;

            component.CurrentEffects[severeType] = TimeSpan.FromSeconds(duration);
        }
        else if (component.UpcomingEffects.ContainsKey(mildType) &&
                 damage < _configurationManager.GetCVar(mildCVar))
        {
            var duration = mildEffectSeconds;
            component.UpcomingEffects.Remove(mildType);

            if (healDecayEnabled && component.PreviousEffects.TryGetValue(mildType, out var count))
                duration *= healDecayFactor * count;

            component.CurrentEffects[mildType] = TimeSpan.FromSeconds(duration);
        }
    }
}
