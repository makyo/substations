using Content.Server.Body.Components;
using Content.Server.Medical;
using Content.Server.Popups;
using Content.Shared._L5.Traits.DietaryRestriction;
using Content.Shared.Body.Organ;
using Content.Shared.Chemistry.Components;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Popups;
using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._L5.Traits.DietaryRestriction;

public sealed class DietaryRestrictionSystem : EntitySystem
{
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly VomitSystem _vomit = default!;
    [Dependency] private readonly EntityWhitelistSystem _whitelist = default!;

    public bool TryFood(EntityUid user, EntityUid food, List<Entity<StomachComponent,OrganComponent>> stomachs)
    {
        if (!TryComp<DietaryRestrictionComponent>(user, out var restrictions))
        {
            return true;
        }

        foreach (var restriction in restrictions.Restrictions)
        {
            // If they CAN'T eat it, bail early.
            if (_whitelist.IsWhitelistPass(restriction.MustNotHave, food)
                || _whitelist.IsWhitelistFail(restriction.MustHave, food))
            {
                return false;
            }

            // If they're allergic, they can eat it, but it makes them sick.
            if (_whitelist.IsWhitelistPass(restriction.AllergicTo, food))
            {
                // Add histamine to each stomach and notify user.
                _popup.PopupEntity(Loc.GetString($"trait-dietary-restriction-{restriction.ID}-fail"), food, user, PopupType.SmallCaution);
                foreach (var ent in stomachs)
                {
                    if (TryComp<SolutionComponent>(ent.Comp1.Solution, out var solution))
                    {
                        solution.Solution.AddReagent("Histamine", FixedPoint2.New(5));
                    }
                }
            }

            // If they don't want it, let them know.
            if (_whitelist.IsWhitelistFail(restriction.Exceptions, food)
                || _whitelist.IsWhitelistPass(restriction.DoesNotWant, food)
                || _whitelist.IsWhitelistFail(restriction.Wants, food))
            {
                // notify the user, give a chance of them vomiting.
                _popup.PopupEntity(Loc.GetString($"trait-dietary-restriction-{restriction.ID}-fail"), food, user, PopupType.SmallCaution);
                if (_random.Prob(restrictions.ChanceOfVomiting))
                {
                    // TODO delay this
                    _vomit.Vomit(user);
                }
            }
        }
        return true;
    }
}
