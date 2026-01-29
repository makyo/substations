using System.Numerics;
using Content.Server.Administration;
using Content.Server.NPC.HTN;
using Content.Server.NPC.Systems;
using Content.Shared.Administration;
using Content.Shared.Climbing.Components;
using Content.Shared.Interaction.Components;
using Content.Shared.Movement.Components;
using Content.Shared.Tag;
using Robust.Server.GameObjects;
using Robust.Shared.Console;
using Robust.Shared.Map;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;

namespace Content.Server._L5.Commands;

[AdminCommand((AdminFlags.Fun))]
internal sealed class MakeFollowerCommand : LocalizedEntityCommands
{
    [Dependency] private readonly NPCSystem _npc = default!;
    [Dependency] private readonly PhysicsSystem _physics = default!;
    [Dependency] private readonly TagSystem _tag = default!;

    public override string Command => "makefollower";
    public override string Description => Loc.GetString("cmd-makefollower-desc");
    public override string Help => Loc.GetString("cmd-makefollower-help");

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length < 2)
        {
            shell.WriteError(Loc.GetString("cmd-makefollower-error-uids-required"));
            return;
        }

        if (!NetEntity.TryParse(args[0], out var followerNetUid))
        {
            shell.WriteError(Loc.GetString("cmd-parse-failure-uid", ("arg", args[0])));
            return;
        }
        var follower = EntityManager.GetEntity(followerNetUid);

        if (!NetEntity.TryParse(args[1], out var targetNetUid))
        {
            shell.WriteError(Loc.GetString("cmd-parse-failure-uid", ("arg", args[1])));
            return;
        }
        var target = EntityManager.GetEntity(targetNetUid);

        // Ensure the follower can move and interact as it needs.
        EntityManager.EnsureComponent<PhysicsComponent>(follower);
        _physics.SetBodyType(follower, BodyType.KinematicController);
        EntityManager.EnsureComponent<InputMoverComponent>(follower);
        EntityManager.EnsureComponent<MobMoverComponent>(follower);
        EntityManager.EnsureComponent<MovementSpeedModifierComponent>(follower);
        EntityManager.EnsureComponent<ClimbingComponent>(follower);
        EntityManager.EnsureComponent<ComplexInteractionComponent>(follower);
        _tag.TryAddTag(follower, "DoorBumpOpener");

        // Add or replace the HTN component with the requested task.
        EntityManager.EnsureComponent<HTNComponent>(follower, out var htn);
        htn.RootTask = new HTNCompoundTask()
        {
            Task = "FollowCompound",
        };

        _npc.SetBlackboard(follower, "TargetCoordinates", new EntityCoordinates(target, Vector2.Zero));
        _npc.SetBlackboard(follower, "Target", target);
        _npc.SetBlackboard(follower, "IdleTime", 5f);
        _npc.SetBlackboard(follower, "NavClimb", true);
        _npc.SetBlackboard(follower, "NavInteract", true);

        shell.WriteLine($"entity {follower} set to follow {target}");
    }

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        return args.Length switch
        {
            1 => CompletionResult.FromOptions(CompletionHelper.NetEntities(args[0], EntityManager)),
            2 => CompletionResult.FromOptions(CompletionHelper.NetEntities(args[1], EntityManager)),
            _ => CompletionResult.Empty,
        };
    }
}
