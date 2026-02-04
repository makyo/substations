using Content.Server.Maps.NameGenerators;
using JetBrains.Annotations;
using Robust.Shared.Random;

namespace Content.Server._L5.Maps.NameGenerators;

[UsedImplicitly]
public sealed partial class SysConNameGenerator : StationNameGenerator
{
    [DataField("stationType")] public string StationType = default!;

    public override string FormatName(string input)
    {
        var random = IoCManager.Resolve<IRobustRandom>();
        char[] midChar = ['A', 'C', 'N', 'P', 'S'];

        // Make something like "SCCS Nucleus (UN-442-C-53)"
        return string.Format(input, $"SC{StationType}", $"(UN-{random.Next(100, 999)}-{random.Pick(midChar)}-{random.Next(1, 99):D2})");
    }
}
