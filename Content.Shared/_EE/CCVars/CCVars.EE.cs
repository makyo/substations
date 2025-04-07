using Robust.Shared.Configuration;

namespace Content.Shared._EE.CCVars;

[CVarDefs]
public sealed partial class EECVars
{

    /// <summary>
    ///     EE: Is ore material enabled.
    /// </summary>
    public static readonly CVarDef<bool> SiloEnabled =
        CVarDef.Create("silo.silo_enabled", true, CVar.SERVER | CVar.REPLICATED);
}
