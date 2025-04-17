using Robust.Shared.Prototypes;

namespace Content.Shared._L5.Traits.DietaryRestriction;

public sealed partial class DietaryRestrictionComponent : Component
{
    /// <summary>
    /// A list of restrictions that the character has on their diet.
    /// </summary>
    [DataField]
    public List<DietaryRestrictionPrototype> Restrictions = new();

    /// <summary>
    /// The chance of vomiting if a user eats something they don't want
    /// It's an oversimplification, but the more "important" a restriction is to a person,
    /// the greater the chance they'll be made ill by eating it. This will probably be
    /// replaced by the mood system when added.
    /// </summary>
    [DataField]
    public float ChanceOfVomiting = 0.3f;
}
