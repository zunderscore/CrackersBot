using CrackersBot.Core.Actions.Discord;
using Discord;

namespace CrackersBot.Core.Commands;

public class CommandOutput(
    string? text = null,
    IEnumerable<EmbedInstance>? embeds = null,
    bool ephemeral = false
)
{
    public string? Text { get; } = text;
    public IEnumerable<EmbedInstance>? Embeds { get; } = embeds;
    public bool Ephemeral { get; } = ephemeral;

    public IEnumerable<Embed>? GetParsedEmbeds()
        => Embeds?.Select(e => e.BuildDiscordEmbed());
}