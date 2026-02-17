using Content.Server.VoiceMask;
using Content.Shared.Implants;

namespace Content.Server.DV.Implants;

public sealed class SubdermalBionicSyrinxImplantSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        // L5 — This is overidden by the upstream voice mask implant
        // SubscribeLocalEvent<VoiceMaskComponent, ImplantImplantedEvent>(OnInsert);
    }

    // private void OnInsert(Entity<VoiceMaskComponent> ent, ref ImplantImplantedEvent args)
    // {
    //     // Update the name so it's the entities default name. You can't take it off like a voice mask so it's important!
    //     ent.Comp.VoiceMaskName = Name(args.Implanted);
    // }
}
