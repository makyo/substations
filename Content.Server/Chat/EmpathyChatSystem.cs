using System.Linq;
using Robust.Shared.Utility;
using Content.Server.Chat.Managers;
using Content.Server.Chat.Systems;
using Content.Server.Administration.Managers;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Content.Shared.Chat;
using Content.Shared.Shadowkin;
using Content.Shared.Speech;
using Robust.Shared.Prototypes;

namespace Content.Server.Chat;

public sealed partial class EmpathyChatSystem : EntitySystem
{
    [Dependency] private readonly IChatManager _chatManager = default!;
    [Dependency] private readonly IAdminManager _adminManager = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SpeechComponent, EntitySpokeEvent>(OnSpeak);
    }

    private void OnSpeak(EntityUid uid, SpeechComponent component, EntitySpokeEvent args)
    {
        if (args.Source != uid)
            return;

        SendEmpathyChat(args.Source, args.Message, false);
    }

    /// <summary>
    /// Send a Message in the Shadowkin Empathy Chat.
    /// </summary>
    /// <param name="source">The entity making the message</param>
    /// <param name="message">The contents of the message</param>
    /// <param name="hideChat">Set the ChatTransmitRange</param>
    public void SendEmpathyChat(EntityUid source, string message, bool hideChat)
    {
        var clients = GetEmpathChatClients();
        string wrappedMessage;

        wrappedMessage = Loc.GetString("chat-manager-send-empathy-chat-wrap-message",
                ("source", source),
                ("message", FormattedMessage.EscapeText(message)));

        _chatManager.ChatMessageToMany(ChatChannel.Telepathic, message, wrappedMessage, source, hideChat, true, clients.ToList(), Color.FromHex("#be3cc5"));
    }

    private IEnumerable<INetChannel> GetEmpathChatClients()
    {
        return Filter.Empty()
            .AddWhereAttachedEntity(entity =>
            CanHearEmpathy(entity))
            .Recipients
            .Union(_adminManager.ActiveAdmins)
            .Select(p => p.Channel);
    }

    /// <summary>
    /// Check if an entity can hear Empathy.
    /// (Admins will always be able to hear Empathy)
    /// L5 - simplified for our lack of the language system
    /// </summary>
    /// <param name="entity">The entity to check</param>
    public bool CanHearEmpathy(EntityUid entity)
    {
        if (HasComp<ShadowkinComponent>(entity))
            return true;

        if (_adminManager.IsAdmin(entity))
            return true;

        return false;
    }
}
