using Content.Shared.Actions;
using Content.Shared.Interaction;

namespace Content.Shared._L5.Traits.DietaryRestriction;

public sealed class AllergenIngestedEvent(EntityUid user, float histamineAmount, bool vomit) : EntityEventArgs
{
    public readonly EntityUid User = user;
    public readonly float HistamineAmount = histamineAmount;
    public readonly bool Vomit = vomit;
};
