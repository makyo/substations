using Content.Shared._DV.ChronicPain.Components;
using Content.Shared._L5.CCVar;
using Content.Shared._L5.Traits.Moody.Components;
using Content.Shared._L5.Traits.HardOfHearing;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Damage.Systems;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Traits.Assorted;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared._L5.LongTermHealth;

public abstract partial class SharedLongTermHealthSystem
{
    #region Adding effects

    private void OnAirloss(EntityUid owner, ref LongTermHealthComponent component, DamageChangedEvent args)
    {
        args.DamageDelta!.DamageDict.TryGetValue("Asphyxiation", out var asphyxDelta);
        args.DamageDelta!.DamageDict.TryGetValue("Bloodloss", out var bloodlossDelta);
        var airlossDelta = asphyxDelta +  bloodlossDelta;

        var airlossTotal = args.Damageable.Damage["Asphyxiation"] + args.Damageable.Damage["Bloodloss"];

        // Lung damage
        if (asphyxDelta > FixedPoint2.Zero)
        {
            PrepareEffect(
                ref component,
                args.Damageable.Damage["Asphyxiation"],
                EffectType.MildLungDamage,
                EffectType.SevereLungDamage,
                L5CCVars.AsphyxLungDamageMildThreshold,
                L5CCVars.AsphyxLungDamageSevereThreshold);
        }
        else if (asphyxDelta < FixedPoint2.Zero)
        {
            InitializeEffect(
                ref component,
                owner,
                args.Damageable.Damage["Asphyxiation"],
                EffectType.MildLungDamage,
                EffectType.SevereLungDamage,
                L5CCVars.AsphyxLungDamageMildThreshold,
                L5CCVars.AsphyxLungDamageSevereThreshold);
        }

        // Brain damage
        if (airlossDelta > FixedPoint2.Zero)
        {
            // We can't use PrepareEffect here because we also need to clear the upcoming TBIs, but the logic is otherwise the same.
            if (!component.CurrentEffects.ContainsKey(EffectType.SevereBrainDamage) &&
                airlossTotal >
                _config.GetCVar(L5CCVars.AirlossBrainDamageSevereThreshold))
            {
                // We only want to add one TBI at a time
                ClearUpcomingTBIs(ref component);

                component.UpcomingEffects.Remove(EffectType.MildBrainDamage);
                component.UpcomingEffects[EffectType.SevereBrainDamage] = true;
            }
            else if (!component.CurrentEffects.ContainsKey(EffectType.MildBrainDamage) &&
                     airlossTotal >
                     _config.GetCVar(L5CCVars.AirlossBrainDamageMildThreshold))
            {
                ClearUpcomingTBIs(ref component);

                component.UpcomingEffects[EffectType.MildBrainDamage] = true;
                component.UpcomingEffects.Remove(EffectType.SevereBrainDamage);
            }
        }
        else if (airlossDelta < FixedPoint2.Zero)
        {
            // We can't use ApplyEffect here because we're technically rolling from a class of effects.
            if (component.UpcomingEffects.ContainsKey(EffectType.SevereBrainDamage) &&
                airlossTotal < _config.GetCVar(L5CCVars.AirlossBrainDamageSevereThreshold))
            {
                component.UpcomingEffects.Remove(EffectType.SevereBrainDamage);

                var tbi = _random.Pick(EffectTypeExtensions.SevereTBIs);

                // Bail if they already have that TBI
                if (component.CurrentEffects.ContainsKey(tbi))
                    return;

                // Bail if they already have one of the matching components
                if (BailEarly(tbi, owner))
                    return;

                var duration = _severeEffectSeconds;
                if (_healDecayEnabled && component.PreviousEffects.TryGetValue(EffectType.SevereBrainDamage, out var count))
                    duration *= _healDecayFactor * count;

                component.CurrentEffects[EffectType.SevereBrainDamage] = TimeSpan.FromSeconds(duration);
                component.CurrentEffects[tbi] = TimeSpan.FromSeconds(duration);
            }
            else if (component.UpcomingEffects.ContainsKey(EffectType.MildBrainDamage) &&
                     airlossTotal < _config.GetCVar(L5CCVars.AirlossBrainDamageMildThreshold))
            {
                component.UpcomingEffects.Remove(EffectType.MildBrainDamage);

                var tbi = _random.Pick(EffectTypeExtensions.MildTBIs);

                // Bail if they already have that TBI
                if (component.CurrentEffects.ContainsKey(tbi))
                    return;

                // Bail if they already have one of the matching components
                if (BailEarly(tbi, owner))
                    return;

                var duration = _mildEffectSeconds;
                if (_random.Prob(_config.GetCVar(L5CCVars.ChanceToBecomeSevere)))
                    duration = _severeEffectSeconds;

                if (_healDecayEnabled && component.PreviousEffects.TryGetValue(EffectType.MildBrainDamage, out var count))
                    duration *= _healDecayFactor * count;

                component.CurrentEffects[EffectType.MildBrainDamage] = TimeSpan.FromSeconds(duration);
                component.CurrentEffects[tbi] = TimeSpan.FromSeconds(duration);
            }
        }
    }

    private void OnBrute(EntityUid owner, ref LongTermHealthComponent component, DamageChangedEvent args)
    {
        args.DamageDelta!.DamageDict.TryGetValue("Blunt", out var bluntDelta);
        args.DamageDelta!.DamageDict.TryGetValue("Slash", out var slashDelta);
        args.DamageDelta!.DamageDict.TryGetValue("Piercing", out var pierceDelta);
        var bruteDelta = bluntDelta + slashDelta + pierceDelta;

        var bruteTotal = args.Damageable.Damage["Blunt"] + args.Damageable.Damage["Slash"] + args.Damageable.Damage["Piercing"];

        if (bruteDelta > FixedPoint2.Zero)
        {
            PrepareEffect(
                ref component,
                bruteTotal,
                EffectType.MildPain,
                EffectType.SeverePain,
                L5CCVars.BrutePainMildThreshold,
                L5CCVars.BrutePainSevereThreshold);
            PrepareEffect(
                ref component,
                bruteTotal,
                EffectType.MildImpairedMobility,
                EffectType.SevereImpairedMobility,
                L5CCVars.BruteImpairedMobilityBodyMildThreshold,
                L5CCVars.BruteImpairedMobilityBodySevereThreshold);
        }
        else if (bruteDelta < FixedPoint2.Zero)
        {
            InitializeEffect(
                ref component,
                owner,
                bruteTotal,
                EffectType.MildPain,
                EffectType.SeverePain,
                L5CCVars.BrutePainMildThreshold,
                L5CCVars.BrutePainSevereThreshold);

            InitializeEffect(
                ref component,
                owner,
                bruteTotal,
                EffectType.MildImpairedMobility,
                EffectType.SevereImpairedMobility,
                L5CCVars.BruteImpairedMobilityBodyMildThreshold,
                L5CCVars.BruteImpairedMobilityBodySevereThreshold);
        }

        // TODO: TBI for damage to the head and impaired mobility for damage to the legs, pending either shitmed or offmed.
    }

    private void OnBurn(EntityUid owner, ref LongTermHealthComponent component, DamageChangedEvent args)
    {
        args.DamageDelta!.DamageDict.TryGetValue("Heat", out var heatDelta);
        args.DamageDelta!.DamageDict.TryGetValue("Cold", out var coldDelta);
        args.DamageDelta!.DamageDict.TryGetValue("Shock", out var shockDelta);
        args.DamageDelta!.DamageDict.TryGetValue("Caustic", out var causticDelta);
        var burnDelta = heatDelta + coldDelta + shockDelta + causticDelta;

        var burnTotal = args.Damageable.Damage["Heat"] + args.Damageable.Damage["Cold"] +
                        args.Damageable.Damage["Shock"] + args.Damageable.Damage["Caustic"];

        if (burnDelta > FixedPoint2.Zero &&
            !component.CurrentEffects.ContainsKey(EffectType.BurnReturn) &&
            burnTotal > _config.GetCVar(L5CCVars.BurnReturnThreshold))
            component.UpcomingEffects[EffectType.BurnReturn] = true;
        else if (burnDelta < FixedPoint2.Zero && component.UpcomingEffects.ContainsKey(EffectType.BurnReturn))
        {
            var duration = _mildEffectSeconds;
            if (_random.Prob(_config.GetCVar(L5CCVars.ChanceToBecomeSevere)))
                duration = _severeEffectSeconds;

            component.UpcomingEffects.Remove(EffectType.BurnReturn);

            if (_healDecayEnabled && component.PreviousEffects.TryGetValue(EffectType.BurnReturn, out var count))
                duration *= _healDecayFactor * count;

            component.CurrentEffects[EffectType.BurnReturn] = TimeSpan.FromSeconds(duration);
        }

        // TODO: If graft care is enabled, add this if they receive tend burns surgery no matter how mild.
    }

    private void OnToxin(EntityUid owner, ref LongTermHealthComponent component, DamageChangedEvent args)
    {
        args.DamageDelta!.DamageDict.TryGetValue("Radiation", out var radiationDelta);
        args.DamageDelta!.DamageDict.TryGetValue("Poison", out var poisonDelta);
        var toxinDelta = radiationDelta + poisonDelta;
        var damage = args.Damageable.Damage;
        var toxinTotal = damage["Poison"] + damage["Radiation"];

        if (toxinDelta > FixedPoint2.Zero &&
            !component.CurrentEffects.ContainsKey(EffectType.PoisonReturn) &&
            toxinTotal > _config.GetCVar(L5CCVars.PoisonReturnThreshold))
            component.UpcomingEffects[EffectType.PoisonReturn] = true;
        else if (toxinDelta < FixedPoint2.Zero && component.UpcomingEffects.ContainsKey(EffectType.PoisonReturn))
        {
            var duration = _mildEffectSeconds;
            if (_random.Prob(_config.GetCVar(L5CCVars.ChanceToBecomeSevere)))
                duration = _severeEffectSeconds;

            component.UpcomingEffects.Remove(EffectType.PoisonReturn);

            if (_healDecayEnabled && component.PreviousEffects.TryGetValue(EffectType.PoisonReturn, out var count))
                duration *= _healDecayFactor * count;

            component.CurrentEffects[EffectType.PoisonReturn] = TimeSpan.FromSeconds(duration);
        }
    }

    private void OnGenetic(EntityUid owner, ref LongTermHealthComponent component, DamageChangedEvent args)
    {
        args.DamageDelta!.DamageDict.TryGetValue("Cellular",  out var cellularDelta);
        var geneticTotal = args.Damageable.Damage["Cellular"];
        var effectCount = geneticTotal.Int() / _config.GetCVar(L5CCVars.GeneticNewEffectRollAmount);

        if (cellularDelta > FixedPoint2.Zero &&
            effectCount > component.UpcomingGeneticEffects)
            component.UpcomingGeneticEffects++;
        else if (cellularDelta < FixedPoint2.Zero &&
                 effectCount < component.UpcomingGeneticEffects)
        {
            component.UpcomingGeneticEffects--;

            // Ensure we always get a random effect
            EffectType effect = default;
            foreach (var _ in EffectTypeExtensions.GeneticEffects)
            {
                effect = _random.Pick(EffectTypeExtensions.GeneticEffects);
                if (!component.CurrentEffects.ContainsKey(effect))
                    break;
            }

            // Boy, you are *fucked up* right now...
            if (effect == default)
                return;

            // Bail if they already have one of the matching components
            if (BailEarly(effect, owner))
                return;

            var duration = _severeEffectSeconds;
            if (_healDecayEnabled && component.PreviousEffects.TryGetValue(effect, out var count))
                duration *= _healDecayFactor * count;

            component.CurrentEffects[effect] = TimeSpan.FromSeconds(duration);
        }
    }

    #endregion

    #region Utilities

    private void ClearUpcomingTBIs(ref LongTermHealthComponent component)
    {
        foreach (var tbi in EffectTypeExtensions.AllTBIs)
        {
            component.UpcomingEffects.Remove(tbi);
        }
    }

    private bool BailEarly(EffectType effect, EntityUid owner)
    {
        return (
            effect is EffectType.SevereParacusia or EffectType.MildParacusia && HasComp<ParacusiaComponent>(owner) ||
            effect is EffectType.SevereHearingLoss or EffectType.MildHearingLoss && HasComp<HardOfHearingComponent>(owner) ||
            effect is EffectType.SevereVisionLoss or EffectType.MildVisionLoss && HasComp<BlurryVisionComponent>(owner) ||
            effect is EffectType.SeverePain or EffectType.MildPain && HasComp<ChronicPainComponent>(owner) ||
            effect is EffectType.SevereImpairedMobility or EffectType.MildImpairedMobility && HasComp<ImpairedMobilityComponent>(owner));
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
            damage > _config.GetCVar(severeCVar))
        {
            component.UpcomingEffects.Remove(mildType);
            component.UpcomingEffects[severeType] = true;
        }
        else if (!component.CurrentEffects.ContainsKey(mildType) &&
                 damage > _config.GetCVar(mildCVar))
        {
            component.UpcomingEffects.Remove(severeType);
            component.UpcomingEffects[mildType] = true;
        }
    }

    private void InitializeEffect(
        ref LongTermHealthComponent component,
        EntityUid owner,
        FixedPoint2 damage,
        EffectType mildType,
        EffectType severeType,
        CVarDef<float> mildCVar,
        CVarDef<float> severeCVar)
    {
        if (component.UpcomingEffects.ContainsKey(severeType) &&
            damage < _config.GetCVar(severeCVar))
        {
            var duration = _severeEffectSeconds;
            component.UpcomingEffects.Remove(severeType);

            if (BailEarly(severeType, owner))
                return;

            if (_healDecayEnabled && component.PreviousEffects.TryGetValue(severeType, out var count))
                duration *= _healDecayFactor * count;

            component.CurrentEffects[severeType] = TimeSpan.FromSeconds(duration);
        }
        else if (component.UpcomingEffects.ContainsKey(mildType) &&
                 damage < _config.GetCVar(mildCVar))
        {
            var duration = _mildEffectSeconds;

            var type = mildType;
            if (_random.Prob(_config.GetCVar(L5CCVars.ChanceToBecomeSevere)))
            {
                type = severeType;
                duration = _severeEffectSeconds;
            }

            component.UpcomingEffects.Remove(type);

            if (BailEarly(type, owner))
                return;

            if (_healDecayEnabled && component.PreviousEffects.TryGetValue(type, out var count))
                duration *= _healDecayFactor * count;

            component.CurrentEffects[type] = TimeSpan.FromSeconds(duration);
        }
    }

    #endregion

    #region Applying effects

    private void ApplyEffect(EffectType key, LongTermHealthComponent comp, ref EntityUid ent)
    {
        switch (key)
        {
            case EffectType.BurnReturn:
                _damage.ChangeDamage(ent,
                    new DamageSpecifier(
                        _proto.Index(new ProtoId<DamageTypePrototype>("Caustic")), // Requires ointment to heal oneself.
                        comp.CurrentEffects[key].Seconds * _config.GetCVar(L5CCVars.BurnReturnFactor)),
                    ignoreResistances: true);
                break;

            case EffectType.MildHearingLoss:
            case EffectType.SevereHearingLoss:
                EnsureComp<HardOfHearingComponent>(ent);
                break;

            case EffectType.MildImpairedMobility:
            case EffectType.SevereImpairedMobility:
                EnsureComp<ImpairedMobilityComponent>(ent);
                break;

            case EffectType.MildLungDamage:
            case EffectType.SevereLungDamage:
                _damage.ChangeDamage(ent,
                    new DamageSpecifier(
                        _proto.Index(new ProtoId<DamageTypePrototype>("Asphyxiation")),
                        comp.CurrentEffects[key].Seconds * _config.GetCVar(L5CCVars.AsphyxReturnFactor)),
                    ignoreResistances: true);
                break;

            case EffectType.MildMoody:
            case EffectType.SevereMoody:
                EnsureComp<MoodyComponent>(ent);
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
                        _proto.Index(new ProtoId<DamageTypePrototype>("Poison")),
                        comp.CurrentEffects[key].Seconds * _config.GetCVar(L5CCVars.PoisonReturnFactor)),
                    ignoreResistances: true);
                break;

            case EffectType.MildVisionLoss:
            case EffectType.SevereVisionLoss:
                EnsureComp<BlurryVisionComponent>(ent);
                break;

            case EffectType.MildWoozy:
            case EffectType.SevereWoozy:
                // Update this every time because they may be resting, which modifies how fast they recover.
                _statusEffects.TrySetStatusEffectDuration(ent, "StatusEffectWoozy", comp.CurrentEffects[key]);
                break;
        }
    }

    #endregion

    #region Removing effects

    private void RemoveEffect(EffectType key, ref EntityUid ent)
    {
        switch (key)
        {
            case EffectType.MildHearingLoss:
            case EffectType.SevereHearingLoss:
                RemComp<HardOfHearingComponent>(ent);
                break;
            case EffectType.MildImpairedMobility:
            case EffectType.SevereImpairedMobility:
                RemComp<ImpairedMobilityComponent>(ent);
                break;
            case EffectType.MildMoody:
            case EffectType.SevereMoody:
                RemComp<MoodyComponent>(ent);
                break;
            case EffectType.MildPain:
            case EffectType.SeverePain:

                RemComp<ChronicPainComponent>(ent);
                break;
            case EffectType.MildParacusia:
            case EffectType.SevereParacusia:
                RemComp<ParacusiaComponent>(ent);
                break;
            case EffectType.MildVisionLoss:
            case EffectType.SevereVisionLoss:
                RemComp<BlurryVisionComponent>(ent);
                break;
            case EffectType.MildWoozy:
            case EffectType.SevereWoozy:
                _statusEffects.TryRemoveStatusEffect(ent, "StatusEffectWoozy");
                break;
        }
    }

    #endregion
}
