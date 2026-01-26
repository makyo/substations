using Content.Client._L5.Overlays;
using Content.Shared._L5.Moody;
using Content.Shared._L5.Moody.Components;
using Robust.Client.Graphics;
using Robust.Shared.Player;

namespace Content.Client._L5.Traits.Moody;

public sealed class MoodySystem : SharedMoodySystem
{
    [Dependency] private readonly ISharedPlayerManager _playerManager = default!;
    [Dependency] private readonly IOverlayManager _overlayManager = default!;

    private VariableSaturation _overlay = default!;

    public override void Initialize()
    {
        base.Initialize();
        _overlay = new();
    }

    protected override void OnComponentInit(EntityUid source, MoodyComponent component, ref ComponentInit evt)
    {
        if (_playerManager.LocalEntity == source)
            _overlayManager.AddOverlay(_overlay);
    }

    protected override void OnComponentShutdown(EntityUid uid, MoodyComponent component, ComponentShutdown args)
    {
        if (_playerManager.LocalEntity == uid)
            _overlayManager.RemoveOverlay(_overlay);
    }

    protected override void OnLocalPlayerAttached(EntityUid uid, MoodyComponent component, LocalPlayerAttachedEvent args)
    {
        _overlayManager.AddOverlay(_overlay);
    }

    protected override void OnLocalPlayerDetached(EntityUid uid, MoodyComponent component, LocalPlayerDetachedEvent args)
    {
        _overlayManager.RemoveOverlay(_overlay);
    }
}
