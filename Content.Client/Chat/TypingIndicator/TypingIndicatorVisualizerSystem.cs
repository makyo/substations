using Content.Shared.Chat.TypingIndicator;
using Robust.Client.GameObjects;
using Robust.Shared.Prototypes;
using Content.Shared.Inventory;

namespace Content.Client.Chat.TypingIndicator;

public sealed class TypingIndicatorVisualizerSystem : VisualizerSystem<TypingIndicatorComponent>
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;

    protected override void OnAppearanceChange(EntityUid uid, TypingIndicatorComponent component, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;

        var currentTypingIndicator = component.TypingIndicatorPrototype;

        var evt = new BeforeShowTypingIndicatorEvent();

        if (TryComp<InventoryComponent>(uid, out var inventoryComp))
            _inventory.RelayEvent((uid, inventoryComp), ref evt);

        var overrideIndicator = evt.GetMostRecentIndicator();

        if (overrideIndicator != null)
            currentTypingIndicator = overrideIndicator.Value;

        // Begin DeltaV Additions - AAC TypingIndicator Override
        if (component.TypingIndicatorOverridePrototype != null)
        {
            currentTypingIndicator = component.TypingIndicatorOverridePrototype.Value;
        }
        // End DeltaV Additions

        if (!_prototypeManager.TryIndex(currentTypingIndicator, out var proto))
        {
            Log.Error($"Unknown typing indicator id: {component.TypingIndicatorPrototype}");
            return;
        }

        var layerExists = SpriteSystem.LayerMapTryGet((uid, args.Sprite), TypingIndicatorLayers.Base, out var layer, false);
        if (!layerExists)
            layer = SpriteSystem.LayerMapReserve((uid, args.Sprite), TypingIndicatorLayers.Base);

        // L5 - synth typing indicators
        var useSynth = component.TypingIndicatorOverridePrototype == null && component.UseSyntheticVariant && !proto.NoSynthVariant;
        if (useSynth)
            SpriteSystem.LayerSetRsi((uid, args.Sprite), layer, proto.SynthSpritePath);
        else
            SpriteSystem.LayerSetRsi((uid, args.Sprite), layer, proto.SpritePath, proto.TypingState);

        args.Sprite.LayerSetShader(layer, proto.Shader);
        SpriteSystem.LayerSetOffset((uid, args.Sprite), layer, proto.Offset);

        AppearanceSystem.TryGetData<TypingIndicatorState>(uid, TypingIndicatorVisuals.State, out var state);
        SpriteSystem.LayerSetVisible((uid, args.Sprite), layer, state != TypingIndicatorState.None);
        switch (state)
        {
            // Begin L5 changes - synth typing indicators
            case TypingIndicatorState.Idle:
                if (useSynth)
                    SpriteSystem.LayerSetRsiState((uid, args.Sprite), layer, proto.SynthIdleState);
                else
                    SpriteSystem.LayerSetRsiState((uid, args.Sprite), layer, proto.IdleState);
                break;
            case TypingIndicatorState.Typing:
                if (useSynth && !proto.HasSynthVariant)
                    SpriteSystem.LayerSetRsiState((uid, args.Sprite), layer, proto.SynthFallbackState);
                else
                    SpriteSystem.LayerSetRsiState((uid, args.Sprite), layer, proto.TypingState);
                break;
            // End L5 changes
        }
    }
}
