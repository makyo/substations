using System.Linq;
using Content.Client._L5.OperateMob;
using Content.Client.Gameplay;
using Content.Client.UserInterface.Controls;
using Content.Shared.Input;
using Content.Shared.Mind.Components;
using Robust.Client.Player;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Input.Binding;

namespace Content.Client._L5.UserInterface.Systems;

public sealed class OperateUIController : UIController, IOnStateChanged<GameplayState>
{
    [Dependency] private readonly IEntityManager _entity = default!;
    [Dependency] private readonly IPlayerManager _player = default!;

    private SimpleRadialMenu? _menu;
    private OperateMobSystem _operate = default!;

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
