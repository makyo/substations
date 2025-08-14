using Content.Server.Atmos.Rotting;
using Content.Server.DoAfter;
using Content.Server.Popups;
using Content.Shared._EE.CCVars;
using Content.Shared.Atmos.Rotting;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Inventory;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Verbs;
using Robust.Server.Audio;
using Robust.Shared.Audio;
using Robust.Shared.Configuration;
using Robust.Shared.Utility;
// ReSharper disable InconsistentNaming

namespace Content.Server._EE.Medical.CPR;

public sealed class CPRSystem : EntitySystem
{
    [Dependency] private readonly PopupSystem _popupSystem = default!;
    [Dependency] private readonly DoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly MobStateSystem _mobStateSystem = default!;
    [Dependency] private readonly IngestionSystem _ingestion = default!; // L5 - Food to IngestionSystem
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly RottingSystem _rottingSystem = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly IConfigurationManager _configuration = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<CPRTrainingComponent, GetVerbsEvent<InnateVerb>>(AddCPRVerb);
        SubscribeLocalEvent<CPRTrainingComponent, Shared._EE.Medical.CPRDoAfterEvent>(OnCPRDoAfter);
    }

    private void AddCPRVerb(Entity<CPRTrainingComponent> performer, ref GetVerbsEvent<InnateVerb> args)
    {
        // Begin L5 additions
        // Respect CVar
        if (!_configuration.GetCVar(CPRCCVars.EnableCPR))
            return;

        // Track doafter being active
        if (performer.Comp.DoAfter is not null)
            return;
        // End L5 additions


        if (!args.CanInteract || !args.CanAccess || !TryComp<MobStateComponent>(args.Target, out var targetState)
            || targetState.CurrentState == MobState.Alive)
            return;

        var target = args.Target;
        InnateVerb verb = new()
        {
            Act = () => { StartCPR(performer, target); },
            Text = Loc.GetString("cpr-verb"),
            Icon = new SpriteSpecifier.Rsi(new("Interface/Alerts/human_alive.rsi"), "health4"),
            Priority = 2
        };

        args.Verbs.Add(verb);
    }

    private void StartCPR(Entity<CPRTrainingComponent> performer, EntityUid target)
    {
        // Begin L5 additions - track doafter being active (should be unreachable)
        if (performer.Comp.DoAfter is not null)
            return;
        // End L5 additions

        if (HasComp<RottingComponent>(target))
        {
            _popupSystem.PopupEntity(Loc.GetString("cpr-target-rotting", ("entity", target)), performer, performer);
            return;
        }

        if (_inventory.TryGetSlotEntity(target, "outerClothing", out var outer))
        {
            _popupSystem.PopupEntity(Loc.GetString("cpr-must-remove", ("clothing", outer)), performer, performer);
            return;
        }

        // L5 - adjust for food refactor:
        if (!_ingestion.HasMouthAvailable(performer, performer) || _ingestion.HasMouthAvailable(target, performer))
            return;

        _popupSystem.PopupEntity(Loc.GetString("cpr-start-second-person", ("target", target)), target, performer);
        _popupSystem.PopupEntity(Loc.GetString("cpr-start-second-person-patient", ("user", performer)), target, target);

        var doAfterArgs = new DoAfterArgs(
            EntityManager, performer, performer.Comp.DoAfterDuration, new Shared._EE.Medical.CPRDoAfterEvent(), performer, target,
            performer)
        {
            BreakOnMove = true,
            NeedHand = true,
            BlockDuplicate = true
        };

        // Begin L5 modifications - track doafter
        _doAfterSystem.TryStartDoAfter(doAfterArgs, out var id);
        performer.Comp.DoAfter = id;
        // End L5 modifications

        var playingStream = _audio.PlayPvs(performer.Comp.CPRSound, performer, AudioParams.Default.WithLoop(true));
        if (!playingStream.HasValue)
            return;

        performer.Comp.CPRPlayingStream = playingStream.Value.Entity;
    }

    private void OnCPRDoAfter(Entity<CPRTrainingComponent> performer, ref Shared._EE.Medical.CPRDoAfterEvent args)
    {
        if (args.Cancelled || args.Handled || !args.Target.HasValue)
        {
            performer.Comp.CPRPlayingStream = _audio.Stop(performer.Comp.CPRPlayingStream);
            performer.Comp.DoAfter = null; // L5 - track doafter
            return;
        }

        // L5 changes:
        // - Respect CVars
        // - No resuscitation chance
        _damageable.TryChangeDamage(args.Target,
            new DamageSpecifier
            {
                DamageDict =
                {
                    ["Asphyxiation"] = performer.Comp.DoAfterDuration.Seconds * _configuration.GetCVar(CPRCCVars.CPRAirlossReductionMultiplier),
                },
            },
            true,
            origin: performer);


        _rottingSystem.ReduceAccumulator(
            (EntityUid)args.Target,
            performer.Comp.DoAfterDuration * _configuration.GetCVar(CPRCCVars.CPRRotReductionMultiplier));


        var isAlive = _mobStateSystem.IsAlive(args.Target.Value);
        args.Repeat = !isAlive;

        // Begin L5 modifications - track doafter
        if (isAlive)
        {
            performer.Comp.CPRPlayingStream = _audio.Stop(performer.Comp.CPRPlayingStream);
            performer.Comp.DoAfter = null;
        }
        // End L5 modifications
    }
}
