using System.Linq;
using Content.Client._L5.OperateMob;
using Content.Client.Gameplay;
using Content.Client.Mind;
using Content.Client.UserInterface.Controls;
using Content.Shared._L5.OperateMob;
using Content.Shared.Input;
using Robust.Client.Player;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Input.Binding;

namespace Content.Client._L5.UserInterface.Systems;

public sealed class OperateUIController : UIController, IOnStateChanged<GameplayState>
{
    [Dependency] private readonly IEntityManager _entity = default!;
    [Dependency] private readonly IPlayerManager _player = default!;

    private SimpleRadialMenu? _menu;
    private SharedOperateMobSystem _operate = default!;

    public void OnStateEntered(GameplayState state)
    {
        CommandBinds.Builder
            .Bind(ContentKeyFunctions.OperateCharacter,
                InputCmdHandler.FromDelegate(_ => ToggleOperateMenu()))
            .Register<OperateUIController>();
        _operate = EntitySystemManager.GetEntitySystem<OperateMobSystem>();
    }

    public void OnStateExited(GameplayState state)
    {
        CommandBinds.Unregister<OperateUIController>();
    }

    private void ToggleOperateMenu()
    {
        if (_menu == null)
        {
            var userId = _player.LocalUser;
            var availableMobs = _operate.GetAvailableMobs(userId);

            var options = (
                from mob in availableMobs
                let character = _entity.GetComponent<MetaDataComponent>(mob.Owner)
                where mob.Owner != _player.LocalEntity
                select new RadialMenuActionOption<Entity<OperableComponent>>(m => _operate.OperateMob(userId!.Value, m, availableMobs), mob)
                {
                    IconSpecifier = new RadialMenuEntityIconSpecifier(mob.Owner),
                    ToolTip = character.EntityName,
                }).Cast<RadialMenuOptionBase>()
                .ToList();

            // No-op if they only have one mind.
            if (options.Count == 0)
                return;

            // Setup menu
            _menu = new SimpleRadialMenu();

            _menu.SetButtons(options);

            _menu.Open();

            _menu.OnClose += CloseMenu;

            _menu.OpenCentered();
        }
        else
        {
            CloseMenu();
        }
    }

    private void CloseMenu()
    {
        if (_menu == null)
            return;

        _menu.Dispose();
        _menu = null;
    }
}
