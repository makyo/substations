using Robust.Client.UserInterface.RichText;
using Robust.Shared.Utility;

namespace Content.Client._L5.UserInterface.RichText;

public sealed class L5BulletTag : IMarkupTagHandler
{
    public string Name => "bullet";

    /// <inheritdoc/>
    public string TextBefore(MarkupNode _) => " ● "; // L5 — The font they use is terrible and normal bullets look dinky. U+25CF BLACK CIRCLE looks like an actual bullet
}
