using CrackersBot.Core.Variables;
using Discord;
using System.Text.RegularExpressions;

namespace CrackersBot.Core.Actions.Discord
{
    public partial class EmbedInstance
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? AuthorName { get; set; }
        public string? AuthorIconUrl { get; set; }
        public string? AuthorUrl { get; set; }
        public string? Color { get; set; }
        public string? Url { get; set; }
        public string? ImageUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? Footer { get; set; }
        public List<EmbedField>? Fields { get; set; }

        public Embed BuildDiscordEmbed(IBotCore bot, RunContext context)
        {
            var builder = new EmbedBuilder();

            var title = DefaultVariableProcessor.ProcessVariables(bot, Title, context);
            if (!String.IsNullOrWhiteSpace(title))
            {
                builder = builder.WithTitle(title);
            }

            var description = DefaultVariableProcessor.ProcessVariables(bot, Description, context);
            if (!String.IsNullOrWhiteSpace(description))
            {
                builder = builder.WithDescription(description);
            }

            var color = DefaultVariableProcessor.ProcessVariables(bot, Color, context);
            if (!String.IsNullOrWhiteSpace(color))
            {
                if (HexColorRegex().IsMatch(color))
                {
                    var rawColor = $"0x{color[1..]}";
                    builder = builder.WithColor(new Color(Convert.ToUInt32(rawColor, 16)));
                }
                else
                {
                    if (Enum.TryParse<Color>(color, out var discordColor))
                    {
                        builder = builder.WithColor(discordColor);
                    }
                }
            }

            var url = DefaultVariableProcessor.ProcessVariables(bot, Url, context);
            if (!String.IsNullOrWhiteSpace(url))
            {
                builder = builder.WithUrl(url);
            }

            var imageUrl = DefaultVariableProcessor.ProcessVariables(bot, ImageUrl, context);
            if (!String.IsNullOrWhiteSpace(imageUrl))
            {
                builder = builder.WithImageUrl(imageUrl);
            }

            var thumbnailUrl = DefaultVariableProcessor.ProcessVariables(bot, ThumbnailUrl, context);
            if (!String.IsNullOrWhiteSpace(thumbnailUrl))
            {
                builder = builder.WithThumbnailUrl(thumbnailUrl);
            }

            var footer = DefaultVariableProcessor.ProcessVariables(bot, Footer, context);
            if (!String.IsNullOrWhiteSpace(footer))
            {
                builder = builder.WithFooter(footer);
            }

            if (Fields is not null)
            {
                foreach (var field in Fields)
                {
                    builder = builder.AddField(
                        DefaultVariableProcessor.ProcessVariables(bot, field.Name, context),
                        DefaultVariableProcessor.ProcessVariables(bot, field.Value, context),
                        field.IsInline
                    );
                }
            }

            var authorName = DefaultVariableProcessor.ProcessVariables(bot, AuthorName, context);
            if (!String.IsNullOrWhiteSpace(authorName))
            {
                var authorIconUrl = DefaultVariableProcessor.ProcessVariables(bot, AuthorIconUrl, context);
                var authorUrl = DefaultVariableProcessor.ProcessVariables(bot, AuthorUrl, context);

                builder.WithAuthor(
                    authorName,
                    String.IsNullOrEmpty(authorIconUrl) ? null : authorIconUrl,
                    String.IsNullOrEmpty(authorUrl) ? null : authorUrl
                );
            }

            return builder.Build();
        }

        [GeneratedRegex("#[0-9A-F]{6}", RegexOptions.IgnoreCase, "en-US")]
        private static partial Regex HexColorRegex();
    }

    public record EmbedField(string Name, string Value, bool IsInline = false);
}