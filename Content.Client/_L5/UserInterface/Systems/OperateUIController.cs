using System.Linq;
using Content.Client.Gameplay;
using Content.Client.Mind;
using Content.Client.UserInterface.Controls;
using Content.Shared._L5.OperateMob;
using Content.Shared.Input;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Input.Binding;
using Robust.Shared.Network;

namespace Content.Client._L5.UserInterface.Systems;

public sealed class OperateUIController : UIController, IOnStateChanged<GameplayState>
{
    [Dependency] private readonly IEntityManager _entity = default!;
    [Dependency] private readonly IPlayerManager _player = default!;

    private SimpleRadialMenu? _menu;

    public void OnStateEntered(GameplayState state)
    {
        CommandBinds.Builder
            .Bind(ContentKeyFunctions.OperateCharacter,
                InputCmdHandler.FromDelegate(_ => ToggleOperateMenu()))
            .Register<OperateUIController>();
    }

    public void OnStateExited(GameplayState state)
    {
        CommandBinds.Unregister<OperateUIController>();
    }

    private void ToggleOperateMenu()
    {
        var _operate = EntitySystemManager.GetEntitySystem<SharedOperateMobSystem>();
        if (_menu == null)
        {
            // Setup menu
            _menu = new SimpleRadialMenu();

            var userId = _player.LocalUser;
            var availableMinds = _operate.GetOperatedEntities(userId);

            var options = (
                from mind in availableMinds
                let character = _entity.GetComponent<MetaDataComponent>(mind.Owner)
                select new RadialMenuActionOption<Entity<MindContainerComponent>>(m => _operate.OperateMob(userId!.Value, m.Owner), mind)
                {
                    IconSpecifier = new RadialMenuEntityIconSpecifier(mind.Owner),
                    ToolTip = character.EntityName,
                }).Cast<RadialMenuOptionBase>()
                .ToList();

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
