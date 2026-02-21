using Robust.Client.GameObjects;

namespace Content.Client._RMC14;

public sealed class RotationDrawDepthSystem : EntitySystem
{
    [Dependency] private readonly SpriteSystem _sprite = default!; // L5

    public override void FrameUpdate(float frameTime)
    {
        var query = EntityQueryEnumerator<RotationDrawDepthComponent, SpriteComponent, TransformComponent>();
        while (query.MoveNext(out var ent, out var rotation, out var sprite, out var xform))
        {
            // TODO RMC14 this needs to support rotated viewports eventually
            var dir = xform.LocalRotation.GetCardinalDir();
            switch (dir)
            {
                case Direction.South:
                    _sprite.SetDrawDepth((ent, sprite), rotation.SouthDrawDepth); // L5 — using sprite system
                    break;
                default:
                    _sprite.SetDrawDepth((ent, sprite), rotation.DefaultDrawDepth); // L5 — using sprite system
                    break;
            }
        }
    }
}
