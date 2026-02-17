using Content.Shared.Humanoid.Markings;
using Robust.Shared.Serialization;

namespace Content.Shared.Humanoid
{
    /// <summary>
    ///  These are the layer defines for the humanoid sprite system.
    /// </summary>
    [Serializable, NetSerializable]
    public enum HumanoidVisualLayers : byte
    {
        Special, // for the cat ears
        Tail,
        Hair,
        FacialHair,
        UndergarmentTop,
        UndergarmentBottom,
        Chest,
        Head,
        Snout,
        SnoutCover, // things layered over snouts (i.e. noses)
        HeadSide, // side parts (i.e., frills)
        HeadTop,  // top parts (i.e., ears)
        NeckFluff, // TheDen - Ovinia, for fluff on necks
        TailBehind, // FLOOF - add tails that dont have to go through a brutal cookiecutter to work
        TailBehindBackpack, // imp - to layer behind backpacks, treat this like an oversuit
        TailOversuit, // FLOOF - add tails that dont have to go through a brutal cookiecutter to work
        TailUnderlay, // imp - temporary until i have a better way to do two part tails
        Eyes,
        RArm,
        LArm,
        RHand,
        LHand,
        RLeg,
        LLeg,
        RFoot,
        LFoot,
        Handcuffs,
        StencilMask,
        Ensnare,
        Fire,
        LArmExtension, // Frontier: a species-specific extension layer, e.g. for harpy wings
        RArmExtension, // Frontier: a species-specific extension layer, e.g. for harpy wings
        Face, // Floof
    }
}
