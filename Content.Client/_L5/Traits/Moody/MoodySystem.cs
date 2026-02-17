using Content.Client._L5.Overlays;
using Content.Shared._L5.Traits.Moody.Components;
using Content.Shared._L5.Traits.Moody;
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

    protected override void OnComponentInit(Entity<MoodyComponent> ent, ref ComponentInit evt)
    {
        if (_playerManager.LocalEntity == ent.Owner)
            _overlayManager.AddOverlay(_overlay);
    }

    protected override void OnComponentShutdown(Entity<MoodyComponent> ent, ref ComponentShutdown args)
    {
        if (_playerManager.LocalEntity == ent.Owner)
            _overlayManager.RemoveOverlay(_overlay);
    }

    protected override void OnLocalPlayerAttached(Entity<MoodyComponent> ent, ref LocalPlayerAttachedEvent args)
    {
        _overlayManager.AddOverlay(_overlay);
    }

    protected override void OnLocalPlayerDetached(Entity<MoodyComponent> ent, ref LocalPlayerDetachedEvent args)
    {
        _overlayManager.RemoveOverlay(_overlay);
    }


    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        // Handle showing/hiding overlay based on suppression status
        if (_playerManager.LocalEntity is not { } player)
            return;

        if (!TryComp<MoodyComponent>(player, out var comp))
            return;

        var isSuppressed = IsMoodySuppressed((player, comp));

        if (isSuppressed && _overlayManager.TryGetOverlay<VariableSaturation>(out var overlay))
            _overlayManager.RemoveOverlay(overlay);

        if (isSuppressed) // If it's suppressed and we don't have an overlay, just return
            return;

        if (!isSuppressed && !_overlayManager.HasOverlay<VariableSaturation>())
            _overlayManager.AddOverlay(_overlay);
    }
}
