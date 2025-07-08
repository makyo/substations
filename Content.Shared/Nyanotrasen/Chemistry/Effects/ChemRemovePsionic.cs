using Content.Shared.EntityEffects;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Shared.Nyanotrasen.Chemistry.Effects
{
    // L5 - modified to work with wizden #35614

    /// <summary>
    /// Rerolls psionics once.
    /// </summary>
    [UsedImplicitly]
    public sealed partial class ChemRemovePsionic : EventEntityEffect<ChemRemovePsionic>
    {
        protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
            => Loc.GetString("reagent-effect-guidebook-chem-remove-psionic", ("chance", Probability));
    }
}
