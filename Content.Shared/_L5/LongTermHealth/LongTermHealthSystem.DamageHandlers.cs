using Content.Shared._DV.Pain;
using Content.Shared._L5.CCVar;
using Content.Shared._L5.Traits.HardOfHearing;
using Content.Shared.Damage.Systems;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Traits.Assorted;
using Robust.Shared.Random;

namespace Content.Shared._L5.LongTermHealth;

public sealed partial class LongTermHealthSystem
{
    private void HandleAirloss(EntityUid owner, ref LongTermHealthComponent component, DamageChangedEvent args)
    {
        var dict = args.DamageDelta!.DamageDict;
        var airlossDeltaTotal = dict["Asphyxiation"] + dict["Bloodloss"];
        var airlossTotal = args.Damageable.Damage["Asphyxiation"] + args.Damageable.Damage["Bloodloss"];

        // Lung damage
        if (dict["Asphyxiation"] > FixedPoint2.Zero)
        {
            PrepareEffect(
                ref component,
                args.Damageable.Damage["Asphyxiation"],
                EffectType.MildLungDamage,
                EffectType.SevereLungDamage,
                L5CCVars.AsphyxLungDamageMildThreshold,
                L5CCVars.AsphyxLungDamageSevereThreshold);
        }
        else if (dict["Asphyxiation"] < FixedPoint2.Zero)
        {
            ApplyEffect(
                ref component,
                args.Damageable.Damage["Asphyxiation"],
                EffectType.MildLungDamage,
                EffectType.SevereLungDamage,
                L5CCVars.AsphyxLungDamageMildThreshold,
                L5CCVars.AsphyxLungDamageSevereThreshold);
        }

        // Brain damage
        if (airlossDeltaTotal > FixedPoint2.Zero)
        {
            // We can't use PrepareEffect here because we also need to clear the upcoming TBIs, but the logic is otherwise the same.
            if (!component.CurrentEffects.ContainsKey(EffectType.SevereBrainDamage) &&
                airlossTotal >
                _configurationManager.GetCVar(L5CCVars.AirlossBrainDamageSevereThreshold))
            {
                // We only want to add one TBI at a time
                ClearUpcomingTBIs(ref component);

                component.UpcomingEffects.Remove(EffectType.MildBrainDamage);
                component.UpcomingEffects[EffectType.SevereBrainDamage] = true;
            }
            else if (!component.CurrentEffects.ContainsKey(EffectType.MildBrainDamage) &&
                     airlossTotal >
                     _configurationManager.GetCVar(L5CCVars.AirlossBrainDamageMildThreshold))
            {
                ClearUpcomingTBIs(ref component);

                component.UpcomingEffects[EffectType.MildBrainDamage] = true;
                component.UpcomingEffects.Remove(EffectType.SevereBrainDamage);
            }
        }
        else if (airlossDeltaTotal < FixedPoint2.Zero)
        {
            // We can't use ApplyEffect here because we're technically rolling from a class of effects.
            if (component.UpcomingEffects.ContainsKey(EffectType.SevereBrainDamage) &&
                airlossTotal < _configurationManager.GetCVar(L5CCVars.AirlossBrainDamageSevereThreshold))
            {
                component.UpcomingEffects.Remove(EffectType.SevereBrainDamage);

                var tbi = _random.Pick(EffectTypeExtensions.SevereTBIs);

                // Bail if they already have that TBI
                if (component.CurrentEffects.ContainsKey(tbi))
                    return;

                // Bail if they already have one of the matching components
                if (tbi == EffectType.SevereParacusia && HasComp<ParacusiaComponent>(owner))
                    return;
                if (tbi == EffectType.SevereHearingLoss && HasComp<HardOfHearingComponent>(owner))
                    return;
                if (tbi == EffectType.SevereVisionLoss && HasComp<BlurryVisionComponent>(owner))
                    return;

                var duration = severeEffectSeconds;
                if (healDecayEnabled && component.PreviousEffects.TryGetValue(EffectType.SevereBrainDamage, out var count))
                    duration *= healDecayFactor * count;

                component.CurrentEffects[EffectType.SevereBrainDamage] = TimeSpan.FromSeconds(duration);
                component.CurrentEffects[tbi] = TimeSpan.FromSeconds(duration);
            }
            else if (component.UpcomingEffects.ContainsKey(EffectType.MildBrainDamage) &&
                     airlossTotal < _configurationManager.GetCVar(L5CCVars.AirlossBrainDamageMildThreshold))
            {
                component.UpcomingEffects.Remove(EffectType.MildBrainDamage);

                var tbi = _random.Pick(EffectTypeExtensions.MildTBIs);

                // Bail if they already have that TBI
                if (component.CurrentEffects.ContainsKey(tbi))
                    return;

                // Bail if they already have one of the matching components
                if (tbi == EffectType.MildParacusia && HasComp<ParacusiaComponent>(owner) ||
                    tbi == EffectType.MildHearingLoss && HasComp<HardOfHearingComponent>(owner) ||
                    tbi == EffectType.MildVisionLoss && HasComp<BlurryVisionComponent>(owner))
                    return;

                var duration = severeEffectSeconds;
                if (healDecayEnabled && component.PreviousEffects.TryGetValue(EffectType.MildBrainDamage, out var count))
                    duration *= healDecayFactor * count;

                component.CurrentEffects[EffectType.MildBrainDamage] = TimeSpan.FromSeconds(duration);
                component.CurrentEffects[tbi] = TimeSpan.FromSeconds(duration);
            }
        }
    }

    private void HandleBrute(EntityUid owner, ref LongTermHealthComponent component, DamageChangedEvent args)
    {
        var dict = args.DamageDelta!.DamageDict;
        var bruteDeltaTotal = dict["Blunt"] + dict["Slash"] + dict["Piercing"];
        var bruteTotal = args.Damageable.Damage["Blunt"] + args.Damageable.Damage["Slash"] + args.Damageable.Damage["Piercing"];

        if (bruteDeltaTotal > FixedPoint2.Zero)
        {
            if (!HasComp<PainComponent>(owner))
                PrepareEffect(
                    ref component,
                    bruteTotal,
                    EffectType.MildPain,
                    EffectType.SeverePain,
                    L5CCVars.BrutePainMildThreshold,
                    L5CCVars.BrutePainSevereThreshold);

            if (!HasComp<ImpairedMobilityComponent>(owner))
                PrepareEffect(
                    ref component,
                    bruteTotal,
                    EffectType.MildImpairedMobility,
                    EffectType.SevereImpairedMobility,
                    L5CCVars.BruteImpairedMobilityBodyMildThreshold,
                    L5CCVars.BruteImpairedMobilityBodySevereThreshold);
        }
        else if (bruteDeltaTotal < FixedPoint2.Zero)
        {
            ApplyEffect(
                ref component,
                bruteTotal,
                EffectType.MildPain,
                EffectType.SeverePain,
                L5CCVars.BrutePainMildThreshold,
                L5CCVars.BrutePainSevereThreshold);

            ApplyEffect(
                ref component,
                bruteTotal,
                EffectType.MildImpairedMobility,
                EffectType.SevereImpairedMobility,
                L5CCVars.BruteImpairedMobilityBodyMildThreshold,
                L5CCVars.BruteImpairedMobilityBodySevereThreshold);
        }

        // TODO: TBI for damage to the head.
        // TODO: impaired mobility for damage to the legs.
    }

    private void HandleBurn(EntityUid owner, ref LongTermHealthComponent component, DamageChangedEvent args)
    {
        var delta = args.DamageDelta!.DamageDict;
        var burnDeltaTotal = delta["Heat"] + delta["Cold"] + delta["Shock"] + delta["Caustic"];
        var damage = args.Damageable.Damage;
        var burnTotal = damage["Heat"] + damage["Cold"] + damage["Shock"] + damage["Caustic"];

        if (burnDeltaTotal > FixedPoint2.Zero &&
            !component.CurrentEffects.ContainsKey(EffectType.BurnReturn) &&
            burnTotal > _configurationManager.GetCVar(L5CCVars.BurnReturnThreshold))
            component.UpcomingEffects[EffectType.BurnReturn] = true;
        else if (burnDeltaTotal < FixedPoint2.Zero && component.UpcomingEffects.ContainsKey(EffectType.BurnReturn))
        {
            var duration = mildEffectSeconds;
            component.UpcomingEffects.Remove(EffectType.BurnReturn);

            if (healDecayEnabled && component.PreviousEffects.TryGetValue(EffectType.BurnReturn, out var count))
                duration *= healDecayFactor * count;

            component.CurrentEffects[EffectType.BurnReturn] = TimeSpan.FromSeconds(duration);
        }

        // TODO: If graft care is enabled, add this if they receive tend burns surgery no matter how mild.
    }

    private void HandleToxin(EntityUid owner, ref LongTermHealthComponent component, DamageChangedEvent args)
    {
        var dict = args.DamageDelta!.DamageDict;
        var toxinDeltaTotal = dict["Poison"] + dict["Radiation"];
        var damage = args.Damageable.Damage;
        var toxinTotal = damage["Poison"] + damage["Radiation"];

        if (toxinDeltaTotal > FixedPoint2.Zero &&
            !component.CurrentEffects.ContainsKey(EffectType.PoisonReturn) &&
            toxinTotal > _configurationManager.GetCVar(L5CCVars.PoisonReturnThreshold))
            component.UpcomingEffects[EffectType.BurnReturn] = true;
        else if (toxinDeltaTotal < FixedPoint2.Zero && component.UpcomingEffects.ContainsKey(EffectType.PoisonReturn))
        {
            var duration = mildEffectSeconds;
            component.UpcomingEffects.Remove(EffectType.PoisonReturn);

            if (healDecayEnabled && component.PreviousEffects.TryGetValue(EffectType.PoisonReturn, out var count))
                duration *= healDecayFactor * count;

            component.CurrentEffects[EffectType.PoisonReturn] = TimeSpan.FromSeconds(duration);
        }
    }

    private void HandleGenetic(EntityUid owner, ref LongTermHealthComponent component, DamageChangedEvent args)
    {
        var geneticDeltaTotal = args.DamageDelta!.DamageDict["Cellular"];
        var geneticTotal = args.Damageable.Damage["Cellular"];
        var effectCount = geneticTotal.Int() / _configurationManager.GetCVar(L5CCVars.GeneticNewEffectRollAmount);

        if (geneticDeltaTotal > FixedPoint2.Zero &&
            effectCount > component.UpcomingGeneticEffects)
        {
            component.UpcomingGeneticEffects++;
        }
        else if (geneticDeltaTotal < FixedPoint2.Zero &&
                 effectCount < component.UpcomingGeneticEffects)
        {
            component.UpcomingGeneticEffects--;

            // Get a random effect
            var effect = _random.Pick(EffectTypeExtensions.GeneticEffects);

            // Bail if they already have that effect
            if (component.CurrentEffects.ContainsKey(effect))
                return;

            // Bail if they already have one of the matching components
            if (effect == EffectType.SevereParacusia && HasComp<ParacusiaComponent>(owner) ||
                effect == EffectType.SevereHearingLoss && HasComp<HardOfHearingComponent>(owner) ||
                effect == EffectType.SevereVisionLoss && HasComp<BlurryVisionComponent>(owner) ||
                effect == EffectType.SeverePain && HasComp<PainComponent>(owner) ||
                effect == EffectType.SevereImpairedMobility && HasComp<ImpairedMobilityComponent>(owner))
                return;

            var duration = severeEffectSeconds;
            if (healDecayEnabled && component.PreviousEffects.TryGetValue(effect, out var count))
                duration *= healDecayFactor * count;

            component.CurrentEffects[effect] = TimeSpan.FromSeconds(duration);
        }
    }
}
