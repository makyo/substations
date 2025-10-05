using Content.Shared._L5.Traits.DietaryRestriction;
using Content.Shared.Medical;

namespace Content.Server._L5.Traits.DietaryRestriction;

public sealed class DietaryRestrictionSystem : SharedDietaryRestrictionSystem
{
    [Dependency] private readonly VomitSystem _vomit = default!;

    protected override void DoVomit(EntityUid user)
    {
        _vomit.Vomit(user);
    }
}
