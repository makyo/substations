using Robust.Client.UserInterface;
using Robust.Shared.Input;
using Content.Client.UserInterface.Controls;
using Content.Shared._DV.SmartFridge;

namespace Content.Client._DV.SmartFridge;

// L5 - various renames for conflicts with upstream

public sealed class SmartFridgeBoundUserInterfaceDV : BoundUserInterface
{
    private SmartFridgeMenuDV? _menu;

    public SmartFridgeBoundUserInterfaceDV(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _menu = this.CreateWindow<SmartFridgeMenuDV>();
        _menu.OnItemSelected += OnItemSelected;
        _menu.OnRemoveButtonPressed += OnRemoveButtonPressed;
        Refresh();
    }

    public void Refresh()
    {
        if (_menu is not {} menu || !EntMan.TryGetComponent(Owner, out SmartFridgeDVComponent? fridge))
            return;

        menu.Populate((Owner, fridge));
    }

    private void OnItemSelected(GUIBoundKeyEventArgs args, ListData data)
    {
        if (args.Function != EngineKeyFunctions.UIClick)
            return;

        if (data is not SmartFridgeListDataDV entry)
            return;
        SendPredictedMessage(new SmartFridgeDispenseItemMessage(entry.Entry));
    }

    private void OnRemoveButtonPressed(SmartFridgeListDataDV data)
    {
        SendPredictedMessage(new SmartFridgeRemoveEntryMessage(data.Entry));
    }
}
