using Content.Shared._DV.ChronicPain.Components;
using Content.Shared._L5.CCVar;
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

    private float mildEffectSeconds, severeEffectSeconds, healDecayFactor;
    private bool healDecayEnabled;

    public override void Initialize()
    {
        base.Initialize();

        mildEffectSeconds = _config.GetCVar(L5CCVars.LongTermEffectsDuration);
        severeEffectSeconds = mildEffectSeconds * _config.GetCVar(L5CCVars.LongTermEffectSevereMultiplier);
        healDecayEnabled = _config.GetCVar(L5CCVars.LongTermEffectsHealDecayEnabled);
        healDecayFactor = 1f;
        if (healDecayEnabled)
            healDecayFactor = _config.GetCVar(L5CCVars.LongTermEffectsHealDecayFactor);

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
                var intervalFactor = 1f;
                if (_buckle.IsBuckled(ent))
                {
                    intervalFactor = _config.GetCVar(L5CCVars.LongTermEffectsRestFactor);
                }
                comp.CurrentEffects[key] -= comp.UpdateInterval * intervalFactor;
                if (comp.CurrentEffects[key] < TimeSpan.Zero)
                {
                    // remove components/etc
                    comp.CurrentEffects.Remove(key);
                    comp.PreviousEffects[key]++;
                    Remove(key, ref comp);
                }
                else
                {
                    switch (key)
                    {
                        case EffectType.BurnReturn:
                            _damage.ChangeDamage(ent,
                                new DamageSpecifier(
                                    _proto.Index<DamageTypePrototype>("Caustic"), // Requires ointment to heal oneself.
                                    comp.CurrentEffects[key].Seconds * _config.GetCVar(L5CCVars.BurnReturnFactor)),
                                ignoreResistances: true);
                            break;

                        case EffectType.MildHearingLoss:
                        case EffectType.SevereHearingLoss:
                            EnsureComp<HardOfHearingComponent>(ent);
                            break;

                        case EffectType.MildLungDamage:
                        case EffectType.SevereLungDamage:
                            _damage.ChangeDamage(ent,
                                new DamageSpecifier(
                                    _proto.Index<DamageTypePrototype>("Asphyxiation"),
                                    comp.CurrentEffects[key].Seconds * _config.GetCVar(L5CCVars.AsphyxReturnFactor)),
                                ignoreResistances: true);
                            break;

                        case EffectType.MildPain:
                        case EffectType.SeverePain:
                            EnsureComp<ChronicPainComponent>(ent);
                            break;

                        case EffectType.MildParacusia:
                        case EffectType.SevereParacusia:
                            EnsureComp<ParacusiaComponent>(ent);
                            break;

                        case EffectType.PoisonReturn:
                            _damage.ChangeDamage(ent,
                                new DamageSpecifier(
                                    _proto.Index<DamageTypePrototype>("Poison"),
                                    comp.CurrentEffects[key].Seconds * _config.GetCVar(L5CCVars.PoisonReturnFactor)),
                                ignoreResistances: true);
                            break;

                        case EffectType.MildVisionLoss:
                        case EffectType.SevereVisionLoss:
                            EnsureComp<BlurryVisionComponent>(ent);
                            break;

                        case EffectType.MildWoozy:
                        case EffectType.SevereWoozy:
                            if (!_statusEffects.HasStatusEffect(ent, "StatusEffectWoozy"))
                                _statusEffects.TrySetStatusEffectDuration(ent, "StatusEffectWoozy", comp.CurrentEffects[key]);
                            break;
                    }
                }
            }

            comp.NextUpdate += comp.UpdateInterval;
        }
    }
}
