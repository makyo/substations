using Robust.Shared.Serialization;

namespace Content.Shared.Eye
{
    [Flags]
    [FlagsFor(typeof(VisibilityMaskLayer))]
    public enum VisibilityFlags : int
    {
        None = 0,
        Normal = 1 << 0,
        Ghost  = 1 << 1, // Observers and revenants.
        // DeltaV - 4 is occupied by PsionicInvisibility and changing that massively fucks up stuff:
        Subfloor = 1 << 3, // Pipes, disposal chutes, cables etc. while hidden under tiles. Can be revealed with a t-ray.
        PsionicInvisibility = 1 << 2, // DeltaV - Psionic Invisibility
        TelegnosticProjection = PsionicInvisibility | Normal, // DeltaV - Telegnostic Projection
        CosmicCultMonument = 1 << 4, // DeltaV - Cosmic Cult
        // L5 - was 3
        Admin = 1 << 5, // Reserved for admins in stealth mode and admin tools.
    }
}
