using Content.Shared._L5.CCVar;
using Content.Shared._L5.Traits.HardOfHearing;
using Content.Shared.Body.Components;
using Content.Shared.Damage;
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
        var airlossTotal = dict["Asphyxiation"] + dict["Bloodloss"];

        // Lung damage
        if (dict["Asphyxiation"] > FixedPoint2.Zero)
        {
            if (!component.CurrentEffects.ContainsKey(EffectType.SevereLungDamage) &&
                args.Damageable.Damage["Asphyxiation"] >
                _configurationManager.GetCVar(L5CCVars.AsphyxLungDamageSevereThreshold))
            {
                component.UpcomingEffects.Remove(EffectType.MildLungDamage);
                component.UpcomingEffects[EffectType.SevereLungDamage] = true;
            }
            else if (!component.CurrentEffects.ContainsKey(EffectType.MildLungDamage) &&
                     args.Damageable.Damage["Asphyxiation"] >
                     _configurationManager.GetCVar(L5CCVars.AsphyxLungDamageMildThreshold))
            {
                component.UpcomingEffects[EffectType.MildLungDamage] = true;
                component.UpcomingEffects.Remove(EffectType.SevereLungDamage);

            }
        }
        else if (dict["Asphyxiation"] < FixedPoint2.Zero)
        {
            if (component.UpcomingEffects.ContainsKey(EffectType.SevereLungDamage) &&
                args.Damageable.Damage["Asphyxiation"] <
                _configurationManager.GetCVar(L5CCVars.AsphyxLungDamageSevereThreshold))
            {
                component.UpcomingEffects.Remove(EffectType.SevereLungDamage);

                var duration = severeEffectSeconds;
                if (healDecayEnabled && component.PreviousEffects.TryGetValue(EffectType.SevereLungDamage, out var count))
                    duration *= healDecayFactor * count;

                component.CurrentEffects[EffectType.SevereLungDamage] = TimeSpan.FromSeconds(duration);
            }
            else if (component.UpcomingEffects.ContainsKey(EffectType.MildLungDamage) &&
                     args.Damageable.Damage["Asphyxiation"] <
                     _configurationManager.GetCVar(L5CCVars.AsphyxLungDamageMildThreshold))
            {
                component.UpcomingEffects.Remove(EffectType.MildLungDamage);

                var duration = mildEffectSeconds;
                if (healDecayEnabled && component.PreviousEffects.TryGetValue(EffectType.MildLungDamage, out var count))
                    duration *= healDecayFactor * count;

                component.CurrentEffects[EffectType.SevereLungDamage] = TimeSpan.FromSeconds(duration);
            }
        }

        // Brain damage
        if (airlossTotal > FixedPoint2.Zero)
        {
            if (!component.CurrentEffects.ContainsKey(EffectType.SevereBrainDamage) &&
                args.Damageable.Damage["Asphyxiation"] + args.Damageable.Damage["Bloodloss"] >
                _configurationManager.GetCVar(L5CCVars.AirlossBrainDamageSevereThreshold))
            {
                // We only want to add one TBI at a time
                ClearUpcomingTBIs(ref component);

                component.UpcomingEffects.Remove(EffectType.MildBrainDamage);
                component.UpcomingEffects[EffectType.SevereBrainDamage] = true;
            }
            else if (!component.CurrentEffects.ContainsKey(EffectType.MildBrainDamage) &&
                     args.Damageable.Damage["Asphyxiation"] + args.Damageable.Damage["Bloodloss"] >
                     _configurationManager.GetCVar(L5CCVars.AirlossBrainDamageMildThreshold))
            {
                ClearUpcomingTBIs(ref component);

                component.UpcomingEffects[EffectType.MildBrainDamage] = true;
                component.UpcomingEffects.Remove(EffectType.SevereBrainDamage);
            }
        }
        else if (airlossTotal < FixedPoint2.Zero)
        {
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
                if (tbi == EffectType.MildParacusia && HasComp<ParacusiaComponent>(owner))
                    return;
                if (tbi == EffectType.MildHearingLoss && HasComp<HardOfHearingComponent>(owner))
                    return;
                if (tbi == EffectType.MildVisionLoss && HasComp<BlurryVisionComponent>(owner))
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

        // Chronic pain
        if (bruteDeltaTotal > FixedPoint2.Zero)
        {
            if (!component.CurrentEffects.ContainsKey(EffectType.SeverePain) &&
                bruteTotal > _configurationManager.GetCVar(L5CCVars.BrutePainSevereThreshold))
            {
                component.UpcomingEffects.Remove(EffectType.MildPain);
                component.UpcomingEffects[EffectType.SeverePain] = true;
            }
            else if (!component.CurrentEffects.ContainsKey(EffectType.MildPain) &&
                     bruteTotal > _configurationManager.GetCVar(L5CCVars.BrutePainMildThreshold))
            {
                component.UpcomingEffects.Remove(EffectType.SeverePain);
                component.UpcomingEffects[EffectType.MildPain] = true;
            }
        }
        else if (bruteDeltaTotal < FixedPoint2.Zero)
        {
        }
    }
}
