using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Client.Resources;
using Content.Shared.StatusIcon;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.RichText;
using Robust.Client.ResourceManagement;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client._L5.UserInterface.RichText;

public sealed class SCLogoTagHandler : IMarkupTag
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IEntitySystemManager _entitySystem = default!;
    private SpriteSystem? _spriteSystem;
    private IResourceCache? _resourceCache;

    public string Name => "SClogo";

    public bool TryGetControl(MarkupNode node, [NotNullWhen(true)] out Control? control)
    {
        _spriteSystem ??= _entitySystem.GetEntitySystem<SpriteSystem>();
        _resourceCache ??= IoCManager.Resolve<IResourceCache>();

        var icon = new TextureRect
        {
            Texture = _resourceCache.GetTexture("/Textures/_L5/Interface/Paper/paper_logo_syscon.png"),
            TextureScale = new Vector2(0.5f, 0.5f),
        };

        control = icon;
        return true;
    }
}
