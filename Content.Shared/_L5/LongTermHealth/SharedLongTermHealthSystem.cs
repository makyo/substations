using Content.Shared._L5.CCVar;
using Content.Shared.Buckle;
using Content.Shared.Damage.Systems;
using Content.Shared.Popups;
using Content.Shared.StatusEffectNew;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._L5.LongTermHealth;

public abstract partial class SharedLongTermHealthSystem : EntitySystem
{
    [Dependency] private readonly SharedBuckleSystem _buckle = default!;
    [Dependency] private readonly IConfigurationManager _config = default!;
    [Dependency] private readonly DamageableSystem _damage = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly StatusEffectsSystem _statusEffects = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    // Constants used for popup messages
    // see Resource/Locale/en-US/_L5/long-term-health/effect-messages.ftl
    private const int MessagesPerEffect = 4;
    private const float EffectMessageProbability = 0.05f;
    private const float EndingMessageProbability = 0.5f;

    private float _mildEffectSeconds, _severeEffectSeconds, _healDecayFactor, _maxBurnReturn, _maxPoisonReturn, _maxAsphyxReturn;
    private bool _healDecayEnabled;

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
        if (!_config.GetCVar(L5CCVars.LongTermHealthEnabled))
            return;

        if (args.DamageDelta == null)
            return;

        _mildEffectSeconds = _config.GetCVar(L5CCVars.LongTermEffectsDuration);
        _severeEffectSeconds = _mildEffectSeconds * _config.GetCVar(L5CCVars.LongTermEffectSevereMultiplier);
        _healDecayEnabled = _config.GetCVar(L5CCVars.LongTermEffectsHealDecayEnabled);
        _healDecayFactor = 1f;
        if (_healDecayEnabled)
            _healDecayFactor = _config.GetCVar(L5CCVars.LongTermEffectsHealDecayFactor);
        _maxBurnReturn = _config.GetCVar(L5CCVars.MaxBurnReturn);
        _maxPoisonReturn = _config.GetCVar(L5CCVars.MaxPoisonReturn);
        _maxAsphyxReturn = _config.GetCVar(L5CCVars.MaxAsphyxReturn);

        OnAirloss(uid, ref component, args);
        OnBrute(uid, ref component, args);
        OnBurn(uid, ref component, args);
        OnToxin(uid, ref component, args);
        OnGenetic(uid, ref component, args);
    }

    public override void Update(float frameTime)
    {
        if (!_config.GetCVar(L5CCVars.LongTermHealthEnabled))
            return;

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

                // If the key has already been deleted, move on.
                if (!comp.CurrentEffects.ContainsKey(key))
                    continue;

                comp.CurrentEffects[key] -= comp.UpdateInterval * intervalFactor;
                if (comp.CurrentEffects[key] < TimeSpan.Zero)
                {
                    // Remove from the current effects.
                    comp.CurrentEffects.Remove(key);

                    // Add to the previous effects.
                    comp.PreviousEffects.TryAdd(key, 0);
                    comp.PreviousEffects[key]++;

                    // Remove the effect components/etc.
                    RemoveEffect(key, ent);
                }
                else
                {
                    // Ensure that effect components or return damage are applied.
                    ApplyEffect(key, comp, ent);
                }
            }

            comp.NextUpdate += comp.UpdateInterval;
        }
    }
}
