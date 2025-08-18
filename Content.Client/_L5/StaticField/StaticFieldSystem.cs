using Content.Client.Atmos.Visualizers;
using Content.Shared._L5.StaticField;
using Content.Shared.Light.Components;
using Content.Shared.Power;
using Content.Shared.Power.EntitySystems;
using Robust.Client.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Client._L5.StaticField;

public sealed class StaticFieldSystem : EntitySystem
{
    [Dependency] private readonly SharedPowerReceiverSystem _receiver = default!;
    [Dependency] private readonly SpriteSystem _spriteSystem = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StaticFieldComponent,PowerChangedEvent>(OnPowerChanged);
    }

    private void OnPowerChanged(Entity<StaticFieldComponent> ent, ref PowerChangedEvent evt)
    {
        if (!TryComp<SpriteComponent>(ent, out var sprite))
            return;

        if (ent.Comp.Powered)
        {
            _spriteSystem.LayerSetRsi((ent, sprite), StaticFieldVisualLayers.Powered, sprite.BaseRSI);
        }
        else
        {
            _spriteSystem.LayerSetRsi((ent, sprite), StaticFieldVisualLayers.Unpowered, sprite.BaseRSI);
        }
        Dirty(ent);
    }
}

public enum StaticFieldVisualLayers : byte
{
    Powered,

    Unpowered,
}
