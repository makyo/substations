using Content.Shared._DV.SmartFridge;
using Robust.Shared.Analyzers;

namespace Content.Client._DV.SmartFridge;

// L5 - conflicts with upstream
public sealed class SmartFridgeUIDVSystem : EntitySystem
{
    [Dependency] private readonly SharedUserInterfaceSystem _uiSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SmartFridgeDVComponent, AfterAutoHandleStateEvent>(OnSmartFridgeAfterState);
    }

    private void OnSmartFridgeAfterState(Entity<SmartFridgeDVComponent> ent, ref AfterAutoHandleStateEvent args)
    {
        if (!_uiSystem.TryGetOpenUi<SmartFridgeBoundUserInterfaceDV>(ent.Owner, SmartFridgeUiKeyDV.Key, out var bui))
            return;

        bui.Refresh();
    }
}
