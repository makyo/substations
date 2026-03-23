using Content.Server.Maps.NameGenerators;
using Robust.Shared.Random;

namespace Content.Server._L5.Maps.NameGenerators;

public sealed partial class GEXNameGenerator : StationNameGenerator
{
    public override string FormatName(string input)
    {
        var random = IoCManager.Resolve<IRobustRandom>();
        char[] midChar = ['A', 'C', 'N', 'P', 'S'];

        // Make something like "GEX-LEPUS-153 (UN-442-C-53)"
        return string.Format(input, "GEX", $"{random.Next(1, 999):D2} (UN-{random.Next(100, 999)}-{random.Pick(midChar)}-{random.Next(1, 99):D2})");
    }
}
