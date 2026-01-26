using Content.Shared._L5.Moody;
using Content.Shared._L5.Moody.Components;
using Content.Shared.StatusEffect;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Prototypes;

namespace Content.Client._L5.Overlays;

public sealed class VariableSaturation : Overlay
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    private readonly ShaderInstance _shader;

    public VariableSaturation()
    {
        IoCManager.InjectDependencies(this);
        _shader = _prototypeManager.Index<ShaderPrototype>("GreyscaleFullscreen").InstanceUnique();
    }

    protected override bool BeforeDraw(in OverlayDrawArgs args)
    {
        if (_playerManager.LocalEntity is not { Valid: true })
            return false;

        return base.BeforeDraw(in args);
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        if (ScreenTexture == null)
            return;

        _shader?.SetParameter("SCREEN_TEXTURE", ScreenTexture);
        _shader?.SetParameter("strength", 0.5f);

        var handle = args.WorldHandle;
        var viewport = args.WorldBounds;

        handle.UseShader(_shader);
        handle.DrawRect(viewport, Color.DarkGray);
        handle.UseShader(null);
    }
}
