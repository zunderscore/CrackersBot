using System.Text.RegularExpressions;
using CrackersBot.Core;

namespace CrackersBot.Web.Services.Automation
{
    public class Embed
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public ulong? AuthorId { get; set; }
        public string? Color { get; set; }
        public string? Url { get; set; }
        public string? ImageUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? Footer { get; set; }
        public List<EmbedField> Fields { get; set; } = new List<EmbedField>();

        public Discord.Embed BuildDiscordEmbed(IBotCore bot, Dictionary<string, object> context)
        {
            var builder = new Discord.EmbedBuilder();

            if (!String.IsNullOrWhiteSpace(Title))
            {
                builder = builder.WithTitle(bot.ProcessVariables(Title, context));
            }

            if (!String.IsNullOrWhiteSpace(Description))
            {
                builder = builder.WithDescription(bot.ProcessVariables(Description, context));
            }

            if (!String.IsNullOrWhiteSpace(Color))
            {
                var color = bot.ProcessVariables(Color, context);

                if (new Regex("#[0-9A-F]{6}", RegexOptions.IgnoreCase).IsMatch(Color))
                {
                    var rawColor = $"0x{Color.Substring(1)}";
                    builder = builder.WithColor(new Discord.Color(Convert.ToUInt32(rawColor, 16)));
                }
                else 
                {
                    Enum.TryParse<Discord.Color>(bot.ProcessVariables(Color, context), out var discordColor);
                    builder = builder.WithColor(discordColor);
                }
            }

            if (!String.IsNullOrWhiteSpace(Url))
            {
                builder = builder.WithUrl(bot.ProcessVariables(Url, context));
            }

            if (!String.IsNullOrWhiteSpace(ImageUrl))
            {
                builder = builder.WithImageUrl(bot.ProcessVariables(ImageUrl, context));
            }

            if (!String.IsNullOrWhiteSpace(ThumbnailUrl))
            {
                builder = builder.WithThumbnailUrl(bot.ProcessVariables(ThumbnailUrl, context));
            }

            if (!String.IsNullOrWhiteSpace(Footer))
            {
                builder = builder.WithFooter(bot.ProcessVariables(Footer, context));
            }

            foreach (var field in Fields)
            {
                var name = bot.ProcessVariables(field.Name, context);
                var value = bot.ProcessVariables(field.Value, context);

                builder = builder.AddField(name, value, field.IsInline);
            }

            return builder.Build();
        }
    }

    public class EmbedField
    {
        public EmbedField(string name, string value, bool isInline = false)
        {
            Name = name;
            Value = value;
            IsInline = isInline;
        }

        public string Name { get; set; } = String.Empty;
        public string Value { get; set; } = String.Empty;
        public bool IsInline { get; set; } = false;
    }
}