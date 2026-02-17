using System.Linq;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Interaction;
using Content.Shared.Popups;

namespace Content.Shared._DV.Access;

public sealed class ReverseAgentIDCardSystem : EntitySystem
{
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly AccessReaderSystem _accessReader = default!; // L5 - wizden access reader refactor

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ReverseAgentIDCardComponent, AfterInteractEvent>(OnAfterInteract);
        SubscribeLocalEvent<ReverseAgentAccessConfiguratorComponent, AfterInteractEvent>(OnAccessReader);
    }

    private void OnAfterInteract(Entity<ReverseAgentIDCardComponent> ent, ref AfterInteractEvent args)
    {
        if (args.Target == null || !args.CanReach || !TryComp<AccessComponent>(args.Target, out var targetAccess))
            return;

        if (!TryComp<AccessComponent>(ent, out var access) || !HasComp<IdCardComponent>(ent))
            return;

        if (ent.Comp.Overwrite)
        {
            targetAccess.Tags.Clear();
            targetAccess.Tags.UnionWith(access.Tags);
            _popup.PopupClient(Loc.GetString("reverse-agent-access-overwrote"), args.User, args.User);
        }
        else
        {
            targetAccess.Tags.UnionWith(access.Tags);
            _popup.PopupClient(Loc.GetString("reverse-agent-access-added"), args.User, args.User);
        }

        Dirty(ent, access);
    }

    private void OnAccessReader(Entity<ReverseAgentAccessConfiguratorComponent> ent, ref AfterInteractEvent args)
    {
        if (args.Target == null || !args.CanReach || !TryComp<AccessReaderComponent>(args.Target, out var targetAccess))
            return;

        if (!TryComp<AccessReaderComponent>(ent, out var access))
            return;

        // Begin L5 changes - wizden access reader refactor
        var readerEnt = new Entity<AccessReaderComponent>(args.Target.Value, targetAccess);
        _accessReader.ClearDenyTags(readerEnt);
        _accessReader.TryClearAccesses(readerEnt);
        _accessReader.ClearAccessKeys(readerEnt);

        _accessReader.SetDenyTags(readerEnt, access.DenyTags);
        _accessReader.TryAddAccesses(readerEnt, access.AccessLists);
        _accessReader.SetAccessKeys(readerEnt, access.AccessKeys);
        // End L5 changes
        _popup.PopupClient(Loc.GetString("reverse-agent-access-overwrote"), args.User, args.User);
        Dirty(ent, access);
    }
}
