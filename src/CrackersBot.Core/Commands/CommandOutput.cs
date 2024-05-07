using CrackersBot.Core.Actions.Discord;
using Discord;

namespace CrackersBot.Core.Commands
{
    public class CommandOutput(
        string? text = null,
        IEnumerable<EmbedDefinition>? embeds = null,
        bool ephemeral = false
    )
    {
        public string? Text { get; } = text;
        public IEnumerable<EmbedDefinition>? Embeds { get; } = embeds;
        public bool Ephemeral { get; } = ephemeral;

        public IEnumerable<Embed>? GetParsedEmbeds(IBotCore bot, RunContext context)
            => Embeds?.Select(e => e.BuildDiscordEmbed(bot, context));
    }
}