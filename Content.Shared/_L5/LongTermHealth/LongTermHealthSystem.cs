using Content.Shared._DV.ChronicPain.Components;
using Content.Shared._L5.CCVar;
using Content.Shared._L5.Moody.Components;
using Content.Shared._L5.Traits.HardOfHearing;
using Content.Shared.Buckle;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Damage.Systems;
using Content.Shared.Drunk;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.FixedPoint;
using Content.Shared.StatusEffectNew;
using Content.Shared.Toilet.Components;
using Content.Shared.Traits.Assorted;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Timing;

namespace Content.Shared._L5.LongTermHealth;

public sealed partial class LongTermHealthSystem : EntitySystem
{
    [Dependency] private readonly SharedBuckleSystem _buckle = default!;
    [Dependency] private readonly IConfigurationManager _config = default!;
    [Dependency] private readonly DamageableSystem _damage = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly StatusEffectsSystem _statusEffects = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    private float _mildEffectSeconds, _severeEffectSeconds, _healDecayFactor;
    private bool _healDecayEnabled;

    public override void Initialize()
    {
        base.Initialize();

        _mildEffectSeconds = _config.GetCVar(L5CCVars.LongTermEffectsDuration);
        _severeEffectSeconds = _mildEffectSeconds * _config.GetCVar(L5CCVars.LongTermEffectSevereMultiplier);
        _healDecayEnabled = _config.GetCVar(L5CCVars.LongTermEffectsHealDecayEnabled);
        _healDecayFactor = 1f;
        if (_healDecayEnabled)
            _healDecayFactor = _config.GetCVar(L5CCVars.LongTermEffectsHealDecayFactor);

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

        OnAirloss(uid, ref component, args);
        OnBrute(uid, ref component, args);
        OnBurn(uid, ref component, args);
        OnToxin(uid, ref component, args);
        OnGenetic(uid, ref component, args);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var curTime = _timing.CurTime;

        var query = EntityQueryEnumerator<LongTermHealthComponent>();
        while (query.MoveNext(out var ent, out var comp))
        {
            if (comp.NextUpdate > curTime)
                continue;

            // Tick up return damages
            foreach (var key in comp.CurrentEffects.Keys)
            {
                // They heal faster if they're resting.
                var intervalFactor = 1f;
                if (_buckle.IsBuckled(ent))
                    intervalFactor = _config.GetCVar(L5CCVars.LongTermEffectsRestFactor);

                comp.CurrentEffects[key] -= comp.UpdateInterval * intervalFactor;
                if (comp.CurrentEffects[key] < TimeSpan.Zero)
                {
                    // remove components/etc
                    comp.CurrentEffects.Remove(key);
                    comp.PreviousEffects[key]++;
                    Remove(key, ref ent);
                }
                else
                {
                    ApplyEffect(key, comp, ref ent);
                }
            }

            comp.NextUpdate += comp.UpdateInterval;
        }
    }
}
