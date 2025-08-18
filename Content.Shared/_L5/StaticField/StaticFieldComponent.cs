namespace Content.Shared._L5.StaticField;

[RegisterComponent]
public sealed partial class StaticFieldComponent : Component
{
    [DataField]
    public bool Powered = false;

    public bool AlwaysPowered = false;
}
