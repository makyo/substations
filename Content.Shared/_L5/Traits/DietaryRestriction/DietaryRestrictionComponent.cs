using Robust.Shared.Prototypes;

namespace Content.Shared._L5.Traits.DietaryRestriction;

public sealed partial class DietaryRestrictionComponent : Component
{
    /// <summary>
    /// A list of restrictions that the character has on their diet.
    /// </summary>
    [DataField]
    public List<DietaryRestrictionPrototype> Restrictions = new();
}
