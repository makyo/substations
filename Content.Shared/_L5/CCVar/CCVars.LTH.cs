using Robust.Shared.Configuration;

namespace Content.Shared._L5.CCVar;

public sealed partial class L5CCVars
{
    /// <summary>
    /// Whether long term effects from injuries are enabled.
    /// </summary>
    public static readonly CVarDef<bool> LongTermHealthEnabled =
        CVarDef.Create("l5.long_term_health.enabled", true, CVar.REPLICATED);

    /// <summary>
    /// The duration for long term effects and damage return periods.
    /// </summary>
    public static readonly CVarDef<float> LongTermEffectsDuration =
        CVarDef.Create("l5.long_term_health.duration", 60f * 15f, CVar.REPLICATED);

    /// <summary>
    /// The multiplier for severe versus mild long term effects
    /// </summary>
    public static readonly CVarDef<float> LongTermEffectSevereMultiplier =
        CVarDef.Create("l5.long_term_health.severe_factor", 1.5f, CVar.REPLICATED);

    /// <summary>
    /// Whether or not healing should get slower the more times an effect is applied
    /// </summary>
    public static readonly CVarDef<bool> LongTermEffectsHealDecayEnabled =
        CVarDef.Create("l5.long_term_health.heal_decay_enabled", true, CVar.REPLICATED);

    /// <summary>
    /// The factor by which healing gets slower the more times an effect is applied
    /// </summary>
    public static readonly CVarDef<float> LongTermEffectsHealDecayFactor =
        CVarDef.Create("l5.long_term_health.heal_decay_factor", 1.5f, CVar.REPLICATED);

    /// <summary>
    /// The factor by which laying down reduces effects or speeds recovery.
    ///
    /// E.g: at 2, recovery times are twice as fast and return damages are halved.
    /// </summary>
    public static readonly CVarDef<float> LongTermEffectsRestFactor =
        CVarDef.Create("l5.long_term_health.rest_factor", 2f, CVar.REPLICATED);

    /// <summary>
    /// How many times a mild mild effect can be applied before it becomes severe
    /// </summary>
    public static readonly CVarDef<int> RepeatsToBecomeSevere =
        CVarDef.Create("l5.long_term_health.repeats_to_become_severe", 2, CVar.REPLICATED);

    /// <summary>
    /// The chance that mild effects become severe even if the severe threshold isn't reached.
    /// </summary>
    public static readonly CVarDef<float> ChanceToBecomeSevere =
        CVarDef.Create("l5.long_term_health.severe_chance", 0.05f, CVar.REPLICATED);

    // BRUTE Injuries

    // -- Pain

    /// <summary>
    /// The threshold at which the pain trait is added temporarily.
    /// </summary>
    public static readonly CVarDef<float> BrutePainMildThreshold =
        CVarDef.Create("l5.long_term_health.brute.pain.mild_threshold", 100f, CVar.REPLICATED);

    /// <summary>
    /// The threshold at which the pain trait is added severely.
    ///
    /// Only applies if severe LTEs are enabled.
    /// </summary>
    public static readonly CVarDef<float> BrutePainSevereThreshold =
        CVarDef.Create("l5.long_term_health.brute.pain.severe_threshold", 200f, CVar.REPLICATED);

    // -- Impaired Mobility

    /// <summary>
    /// The threshold at which BRUTE damage to the body leads to mild impaired mobility.
    /// </summary>
    public static readonly CVarDef<float> BruteImpairedMobilityBodyMildThreshold =
        CVarDef.Create("l5.long_term_health.brute.impaired_mobility.body.mild_threshold", 100f, CVar.REPLICATED);

    /// <summary>
    /// The threshold at which BRUTE damage to the body leads to severe impaired mobility.
    ///
    /// Only applies if severe LTEs are enabled.
    /// </summary>
    public static readonly CVarDef<float> BruteImpairedMobilityBodySevereThreshold =
        CVarDef.Create("l5.long_term_health.brute.impaired_mobility.body.severe_threshold", 200f, CVar.REPLICATED);

    /// <summary>
    /// The threshold at which injuries to the legs leads to mild impaired mobility.
    /// </summary>
    public static readonly CVarDef<float> BruteImpairedMobilityLegsMildThreshold =
        CVarDef.Create("l5.long_term_health.brute.impaired_mobility.legs.mild_threshold", 50f, CVar.REPLICATED);

    /// <summary>
    /// The threshold at which injuries to the legs leads to severe impaired mobility.
    ///
    /// Only applies if severe LTEs are enabled.
    /// </summary>
    public static readonly CVarDef<float> BruteImpairedMobilityLegsSevereThreshold =
        CVarDef.Create("l5.long_term_health.brute.impaired_mobility.legs.severe_threshold", 100f, CVar.REPLICATED);

    // -- Traumatic Brain Injuries

    /// <summary>
    /// The chance that receiving double the threshold of any of the traumatic brain injuries makes them a severe effect requiring ongoing
    /// treatment.
    ///
    /// Only applies if severe LTEs are enabled.
    /// </summary>
    public static readonly CVarDef<float> TBISevereChance =
        CVarDef.Create("l5.long_term_health.brute.tbi.severe_chance", 0.25f, CVar.REPLICATED);

    // BURN Injuries

    /// <summary>
    /// The threshold at which point burn return damage starts to occur.
    /// </summary>
    public static readonly CVarDef<float> BurnReturnThreshold =
        CVarDef.Create("l5.long_term_health.burn.return_threshold", 50f, CVar.REPLICATED);

    /// <summary>
    /// The factor by which burn damage reappears over time, representing extended treatment.
    /// </summary>
    public static readonly CVarDef<float> BurnReturnFactor =
        CVarDef.Create("l5.long_term_health.burn.return_factor", 0.1f, CVar.REPLICATED);

    /// <summary>
    /// Whether or not to treat "tend burns" surgery as receiving a skin graft that requires ongoing therapy.
    /// </summary>
    public static readonly CVarDef<bool> BurnGraftEnabled =
        CVarDef.Create("l5.long_term_health.burn.graft_enabled", true, CVar.REPLICATED);

    // TOXIN Injuries

    /// <summary>
    /// The threshold at which point poison return damage starts to occur.
    /// </summary>
    public static readonly CVarDef<float> PoisonReturnThreshold =
        CVarDef.Create("l5.long_term_health.poison.return_threshold", 50f, CVar.REPLICATED);

    /// <summary>
    /// The factor by which poison damage reappears over time, representing the toxins working their way out of the system.
    /// </summary>
    public static readonly CVarDef<float> PoisonReturnFactor =
        CVarDef.Create("l5.long_term_health.poison.return_factor", 0.1f, CVar.REPLICATED);

    // AIRLOSS injuries

    /// <summary>
    /// The threshold at which asphyxiation leads to mild lung damage, represented by return asphyxiation.
    /// </summary>
    public static readonly CVarDef<float> AsphyxLungDamageMildThreshold =
        CVarDef.Create("l5.long_term_health.airloss.lung_damage.mild_threshold", 50f, CVar.REPLICATED);

    /// <summary>
    /// The threshold at which asphyxiation leads to severe lung damage, represented by return asphyxiation.
    /// </summary>
    public static readonly CVarDef<float> AsphyxLungDamageSevereThreshold =
        CVarDef.Create("l5.long_term_health.airloss.lung_damage.severe_threshold", 100f, CVar.REPLICATED);

    /// <summary>
    /// The factor by which asphyxiation damage reappears over time, representing lung damage.
    /// </summary>
    public static readonly CVarDef<float> AsphyxReturnFactor =
        CVarDef.Create("l5.long_term_health.airloss.return_factor", 0.1f, CVar.REPLICATED);

    /// <summary>
    /// The threshold at which AIRLOSS leads to mild brain damage by way of oxygen deprivation.
    /// </summary>
    public static readonly CVarDef<float> AirlossBrainDamageMildThreshold =
        CVarDef.Create("l5.long_term_health.airloss.brain_damage.mild_threshold", 100f, CVar.REPLICATED);

    /// <summary>
    /// The threshold at which AIRLOSS leads to severe brain damage by way of oxygen deprivation.
    /// </summary>
    public static readonly CVarDef<float> AirlossBrainDamageSevereThreshold =
        CVarDef.Create("l5.long_term_health.airloss.brain_damage.severe_threshold", 175f, CVar.REPLICATED);

    // GENE Injuries

    /// <summary>
    /// The amount of genetic damage at which point a new random LTE is rolled.
    ///
    /// E.g: if set to 25, roll a new one at 25, 50, 75, etc.
    /// </summary>
    public static readonly CVarDef<int> GeneticNewEffectRollAmount =
        CVarDef.Create("l5.long_term_health.genetic.new_effect_roll_amount", 25, CVar.REPLICATED);
}
