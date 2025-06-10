using Content.Server.Body.Systems;
using Content.Server.Medical;
using Content.Shared._L5.Traits.DietaryRestriction;
using Content.Shared.Chemistry.Components;
using Robust.Shared.Random;

namespace Content.Server._L5.Traits.DietaryRestriction;

public sealed class DietaryRestrictionSystem : EntitySystem
{
    [Dependency] private readonly BloodstreamSystem _bloodstreamSystem = default!;
    [Dependency] private readonly VomitSystem _vomit  = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeNetworkEvent<AllergenIngestedEvent>(OnAllergenIngested);
    }

    /// <summary>
    /// Adds histamine to the chemstream and potentially vomits when an allergen is ingested
    /// </summary>
    /// <param name="ev"></param>
    private void OnAllergenIngested(AllergenIngestedEvent ev)
    {
        _bloodstreamSystem.TryAddToChemicals(ev.User, new Solution("Histamine", ev.HistamineAmount));
        if (ev.Vomit)
            _vomit.Vomit(ev.User);
    }
}
