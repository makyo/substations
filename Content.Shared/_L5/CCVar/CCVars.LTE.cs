using Robust.Shared.Configuration;

namespace Content.Shared._L5.CCVar;

[CVarDefs]
public sealed partial class L5CCVars
{
    /// <summary>
    /// Whether long term effects from injuries are enabled.
    /// </summary>
    public static readonly CVarDef<bool> LongTermEffectsEnabled =
        CVarDef.Create("l5.long_term_effects.enabled", true, CVar.REPLICATED);

    /// <summary>
    /// Whether long term effects can be permanent (through the end of the round).
    /// </summary>
    public static readonly CVarDef<bool> PermanentLongTermEffectsEnabled =
        CVarDef.Create("l5.long_term_effects.permanent_enabled", false, CVar.REPLICATED);

    /// <summary>
    /// The duration for temporary long term effects and damage return periods.
    /// </summary>
    public static readonly CVarDef<int> LongTermEffectsDuration =
        CVarDef.Create("l5.long_term_effects.duration", 60 * 20, CVar.REPLICATED);

    /// <summary>
    /// The factor by which laying down reduces effects or speeds recovery.
    ///
    /// E.g: at 0.5, recovery times are twice as fast and return damages are halved.
    /// </summary>
    public static readonly CVarDef<float> LongTermEffectsRestFactor =
        CVarDef.Create("l5.long_term_effects.rest_factor", 0.5f, CVar.REPLICATED);

    // BRUTE Injuries

    // -- Pain

    /// <summary>
    /// The threshold at which the pain trait is added temporarily.
    /// </summary>
    public static readonly CVarDef<float> BrutePainTemporaryThreshold =
        CVarDef.Create("l5.long_term_effects.brute.pain.temporary_threshold", 100f, CVar.REPLICATED);

    /// <summary>
    /// The threshold at which the pain trait is added permanently.
    ///
    /// Only applies if permanent LTEs are enabled.
    /// </summary>
    public static readonly CVarDef<float> BrutePainPermanentThreshold =
        CVarDef.Create("l5.long_term_effects.brute.pain.permanent_threshold", 200f, CVar.REPLICATED);

    /// <summary>
    /// The chance that temporary pain becomes permanent even if the permanent threshold isn't reached.
    ///
    /// Only applies if permanent LTEs are enabled.
    /// </summary>
    public static readonly CVarDef<float> BrutePermanentPainChance =
        CVarDef.Create("l5.long_term_effects.brute.pain.permanent_chance", 0.05f, CVar.REPLICATED);

    // -- Impaired Mobility

    /// <summary>
    /// The threshold at which BRUTE damage to the body leads to temporary impaired mobility.
    /// </summary>
    public static readonly CVarDef<float> BruteImpairedMobilityBodyTemporaryThreshold =
        CVarDef.Create("l5.long_term_effects.brute.impaired_mobility.body.temporary_threshold", 100f, CVar.REPLICATED);

    /// <summary>
    /// The threshold at which BRUTE damage to the body leads to permanent impaired mobility.
    ///
    /// Only applies if permanent LTEs are enabled.
    /// </summary>
    public static readonly CVarDef<float> BruteImpairedMobilityBodyPermanentThreshold =
        CVarDef.Create("l5.long_term_effects.brute.impaired_mobility.body.permanent_threshold", 200f, CVar.REPLICATED);

    /// <summary>
    /// The threshold at which injuries to the legs temporarily adds the impaired mobility trait.
    /// </summary>
    public static readonly CVarDef<float> BruteImpairedMobilityLegsTemporaryThreshold =
        CVarDef.Create("l5.long_term_effects.brute.impaired_mobility.legs.temporary_threshold", 50f, CVar.REPLICATED);

    /// <summary>
    /// The threshold at which injuries to the legs permanently adds the impaired mobility trait.
    ///
    /// Only applies if permanent LTEs are enabled.
    /// </summary>
    public static readonly CVarDef<float> BruteImpairedMobilityLegsPermanentThreshold =
        CVarDef.Create("l5.long_term_effects.brute.impaired_mobility.legs.permanent_threshold", 100f, CVar.REPLICATED);


    /// <summary>
    /// The chance that temporary impaired mobility becomes permanent even if the permanent threshold isn't reached.
    ///
    /// Only applies if permanent LTEs are enabled.
    /// </summary>
    public static readonly CVarDef<float> BrutePermanentImpairedMobilityChance =
        CVarDef.Create("l5.long_term_effects.brute.impaired_mobility.permanent_chance", 0.05f, CVar.REPLICATED);

    // -- Traumatic Brain Injuries

    /// <summary>
    /// The chance that receiving double the threshold of any of these makes them a permanent effect requiring ongoing
    /// treatment.
    ///
    /// Only applies if permanent LTEs are enabled.
    /// </summary>
    public static readonly CVarDef<float> TBIPermanentChance =
        CVarDef.Create("l5.long_term_effects.brute.tbi.permanent_chance", 0.25f, CVar.REPLICATED);

    /// <summary>
    /// The amount of blunt damage received to the head to trigger a woozy status.
    /// </summary>
    public static readonly CVarDef<float> TBIWoozyThreshold =
        CVarDef.Create("l5.long_term_effects.brute.tbi.woozy_threshold", 20f, CVar.REPLICATED);

    /// <summary>
    /// The amount of blunt damage received to the head to add a mood disorder.
    /// </summary>
    public static readonly CVarDef<float> TBIMoodThreshold =
        CVarDef.Create("l5.long_term_effects.brute.tbi.mood_threshold", 30f, CVar.REPLICATED);

    /// <summary>
    /// The amount of blunt damage received to the head to add paracusia.
    /// </summary>
    public static readonly CVarDef<float> TBIParacusiaThreshold =
        CVarDef.Create("l5.long_term_effects.brute.tbi.paracusia_threshold", 40f, CVar.REPLICATED);

    /// <summary>
    /// The amount of blunt damage received to the head to add hearing loss.
    /// </summary>
    public static readonly CVarDef<float> TBIHearingLossThreshold =
        CVarDef.Create("l5.long_term_effects.brute.tbi.hearing_loss_threshold", 50f, CVar.REPLICATED);

    // BURN Injuries

    /// <summary>
    /// The factor by which burn damage reappears over time, representing extended treatment.
    /// </summary>
    public static readonly CVarDef<float> BurnReturnFacotr =
        CVarDef.Create("l5.long_term_effects.burn.return_factor", 0.1f, CVar.REPLICATED);

    /// <summary>
    /// Whether or not to treate "tend burns" surgery as receiving a skin graft that requires ongoing therapy.
    /// </summary>
    public static readonly CVarDef<bool> BurnGraftEnabled =
        CVarDef.Create("l5.long_term_effects.burn.graft_enabled", true, CVar.REPLICATED);

    /// <summary>
    /// The threshold at which burns on the head lead to vision loss.
    /// </summary>
    public static readonly CVarDef<float> BurnHeadVisionLossThreshold =
        CVarDef.Create("l5.long_term_effects.burn.vision_loss_threshold", 30f, CVar.REPLICATED);

    // TOXIN Injuries

    /// <summary>
    /// The factor by which poison damage reappears over time, representing the toxins working their way out of the system.
    /// </summary>
    public static readonly CVarDef<float> PoisonReturnFactor =
        CVarDef.Create("l5.long_term_effects.poison.return_factor", 0.1f, CVar.REPLICATED);

    // AIRLOSS injuries

    /// <summary>
    /// The threshold at which asphyxiation leads to temporary lung damage, represented by return airloss.
    /// </summary>
    public static readonly CVarDef<float> AsphyxLungDamageTemporaryThreshold =
        CVarDef.Create("l5.long_term_effects.airloss.lung_damage.temporary_threshold", 75f, CVar.REPLICATED);

    /// <summary>
    /// The threshold at which asphyxiation leads to permanent lung damage, represented by return airloss.
    ///
    /// Only applies if permanent LTEs are enabled.
    /// </summary>
    public static readonly CVarDef<float> AsphyxLungDamagePermanentThreshold =
        CVarDef.Create("l5.long_term_effects.airloss.lung_damage.permanent_threshold", 150f, CVar.REPLICATED);

    /// <summary>
    /// The chance that temporary lung damage becomes permanent.
    ///
    /// Only applies if permanent LTEs are enabled.
    /// </summary>
    public static readonly CVarDef<float> AsphyxLungDamagePermanentChance =
        CVarDef.Create("l5.long_term_effects.airloss.lung_damage.permanent_chance", 0.05f, CVar.REPLICATED);

    /// <summary>
    /// The threshold at which AIRLOSS leads to temporary brain damage by way of oxygen deprivation.
    /// </summary>
    public static readonly CVarDef<float> AirlossBrainDamageTemporaryThreshold =
        CVarDef.Create("l5.long_term_effects.airloss.brain_damage.temporary_threshold", 150f, CVar.REPLICATED);

    /// <summary>
    /// The threshold at which AIRLOSS leads to temporary brain damage by way of oxygen deprivation.
    /// </summary>
    public static readonly CVarDef<float> AirlossBrainDamagePermanentThreshold =
        CVarDef.Create("l5.long_term_effects.airloss.brain_damage.permanent_threshold", 200f, CVar.REPLICATED);

    /// <summary>
    /// The chance that temporary brain damage becomes permanent.
    ///
    /// Only applies if permanent LTEs are enabled.
    /// </summary>
    public static readonly CVarDef<float> AsphyxLungDamagePermanentChance =
        CVarDef.Create("l5.long_term_effects.airloss.brain_damage.permanent_chance", 0.05f, CVar.REPLICATED);

    // GENE Injuries

    /// <summary>
    /// The amount of genetic damage at which point a new random LTE is rolled.
    ///
    /// E.g: if set to 25, roll a new one at 25, 50, 75, etc.
    public static readonly CVarDef<float> GeneticNewEffectRollAmount =
        CVarDef.Create("l5.long_term_effects.genetic.new_effect_roll_amount", 25f, CVar.REPLICATED);
}
