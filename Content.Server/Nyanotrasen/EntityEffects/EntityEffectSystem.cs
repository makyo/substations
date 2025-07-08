using Content.Server.Abilities.Psionics;
using Content.Server.Psionics;
using Content.Shared.EntityEffects;
using Content.Shared.Nyanotrasen.Chemistry.Effects;

namespace Content.Server.EntityEffects;
// L5 - use new entity effect system introduced in wizden #35614

public sealed partial class EntityEffectSystem
{
    private void NyanotrasenInitialize()
    {
        SubscribeLocalEvent<ExecuteEntityEffectEvent<ChemRemovePsionic>>(OnExecuteChemRemovePsionic);
        SubscribeLocalEvent<ExecuteEntityEffectEvent<ChemRerollPsionic>>(OnExecuteChemRerollPsionic);
    }

    private void OnExecuteChemRemovePsionic(ref ExecuteEntityEffectEvent<ChemRemovePsionic> args)
    {
        if (args.Args is EntityEffectReagentArgs reagentArgs)
        {
            if (reagentArgs.Scale != 1f)
                return;
        }

        var psySys = args.Args.EntityManager.EntitySysManager.GetEntitySystem<PsionicAbilitiesSystem>();

        psySys.RemovePsionics(args.Args.TargetEntity);
    }

    private void OnExecuteChemRerollPsionic(ref ExecuteEntityEffectEvent<ChemRerollPsionic> args)
    {
        var psySys = args.Args.EntityManager.EntitySysManager.GetEntitySystem<PsionicsSystem>();

        psySys.RerollPsionics(args.Args.TargetEntity, bonusMuliplier: args.Effect.BonusMuliplier);
    }
}
