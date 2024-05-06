using DiscordNet = Discord;
using CrackersBot.Core.Variables;
using System.Text.RegularExpressions;

namespace CrackersBot.Core.Actions.Discord
{
    public partial class Embed
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

        public DiscordNet.Embed BuildDiscordEmbed(IBotCore bot)
        {
            var builder = new DiscordNet.EmbedBuilder();

            if (!String.IsNullOrWhiteSpace(Title))
            {
                builder = builder.WithTitle(Title);
            }

            if (!String.IsNullOrWhiteSpace(Description))
            {
                builder = builder.WithDescription(Description);
            }

            if (!String.IsNullOrWhiteSpace(Color))
            {
                if (HexColorRegex().IsMatch(Color))
                {
                    var rawColor = $"0x{Color[1..]}";
                    builder = builder.WithColor(new DiscordNet.Color(Convert.ToUInt32(rawColor, 16)));
                }
                else
                {
                    Enum.TryParse<DiscordNet.Color>(Color, out var discordColor);
                    builder = builder.WithColor(discordColor);
                }
            }

            if (!String.IsNullOrWhiteSpace(Url))
            {
                builder = builder.WithUrl(Url);
            }

            if (!String.IsNullOrWhiteSpace(ImageUrl))
            {
                builder = builder.WithImageUrl(ImageUrl);
            }

            if (!String.IsNullOrWhiteSpace(ThumbnailUrl))
            {
                builder = builder.WithThumbnailUrl(ThumbnailUrl);
            }

            if (!String.IsNullOrWhiteSpace(Footer))
            {
                builder = builder.WithFooter(Footer);
            }

            if (Fields is not null)
            {
                foreach (var field in Fields)
                {
                    builder = builder.AddField(field.Name, field.Value, field.IsInline);
                }
            }

            if (AuthorName is not null)
            {
                builder.WithAuthor(AuthorName, AuthorIconUrl, AuthorUrl);
            }

            return builder.Build();
        }

        [GeneratedRegex("#[0-9A-F]{6}", RegexOptions.IgnoreCase, "en-US")]
        private static partial Regex HexColorRegex();
    }

    public record EmbedField(string Name, string Value, bool IsInline = false);
}