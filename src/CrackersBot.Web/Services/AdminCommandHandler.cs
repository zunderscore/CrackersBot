using CrackersBot.Core.Actions.Discord;
using Discord;

namespace CrackersBot.Web.Services
{
    internal static class AdminCommandHandler
    {
        private const string ACCENT_COLOR = "#0066FF";
        private const string ACCENT_COLOR_ERROR = "#FF0000";

        public static Embed GetStartupMessageEmbed(BotCore bot)
        {
            return new EmbedInstance()
            {
                Title = "CrackersBot is online!",
                Color = ACCENT_COLOR,
                Fields = [
                    new("Actions", $"Registered actions: {bot.RegisteredActions.Count}"),
                    new("Variables", $"Registered variables: {bot.RegisteredVariables.Count}"),
                    new("Filters", $"Registered filters: {bot.RegisteredFilters.Count}"),
                    new("Event Handlers", $"Registered event handlers: {bot.RegisteredEventHandlers.Count}"),
                    new("Guilds", $"Guild configurations loaded: {bot.Guilds.Count}"),
                ]
            }.BuildDiscordEmbed(bot, new());
        }

        public static async Task HandleDMCommandAsync(BotCore bot, string message)
        {
            var messageParts = message.Split(' ');

            if (messageParts.Length > 0)
            {
                var keyword = messageParts[0].ToLower();

                switch (keyword)
                {
                    case "ping":
                        await bot.SendMessageToTheCaptainAsync("PONG!");
                        break;

                    case "status":
                        await SendStatusMessage(bot, messageParts[1..]);
                        break;

                    case "reload":
                        if (messageParts.Length > 1)
                        {
                            var reloadKeyword = messageParts[1].ToLower();
                            switch (reloadKeyword)
                            {
                                case "configs":
                                    await ReloadGuildsAsync(bot);
                                    break;

                                case "guild":
                                    await ReloadGuildConfigAsync(bot, messageParts[2..]);
                                    break;

                                default:
                                    await bot.SendMessageToTheCaptainAsync($"I would, but I don't know what {reloadKeyword} is");
                                    break;
                            }
                        }
                        else
                        {
                            await bot.SendMessageToTheCaptainAsync("I need to know what to reload. Try again.");
                        }
                        break;

                    default:
                        await bot.SendMessageToTheCaptainAsync("That's cool I guess");
                        break;
                }
            }
        }

        private static Embed BuildErrorMessage(BotCore bot, string title, string message)
        {
            return new EmbedInstance()
            {
                Title = title,
                Description = message,
                Color = ACCENT_COLOR_ERROR
            }.BuildDiscordEmbed(bot);
        }

        private static async Task SendStatusMessage(BotCore bot, string[] parameters)
        {
            if (parameters.Length == 0)
            {
                var embed = new EmbedInstance()
                {
                    Title = "CrackersBot Status",
                    Color = ACCENT_COLOR,
                    Fields = [
                        new("Started", $"{bot.StartupTime:F} UTC"),
                        new("Uptime", DateTime.UtcNow.Subtract(bot.StartupTime).ToHumanReadableString()),
                        new("Guilds Connected", bot.DiscordClient.Guilds.Count.ToString())
                    ]
                }.BuildDiscordEmbed(bot);

                await bot.SendMessageToTheCaptainAsync(embed);
            }
            else
            {
                var statusType = parameters[0].ToLower();

                switch (statusType)
                {
                    case "guilds":
                        var guildsEmbed = new EmbedInstance()
                        {
                            Title = "Connected Guilds",
                            Color = ACCENT_COLOR,
                            Fields = bot.DiscordClient.Guilds
                                .Select(g => new Core.Actions.Discord.EmbedField(g.Name, g.Id.ToString(), true))
                                .ToList()
                        }.BuildDiscordEmbed(bot);
                        await bot.SendMessageToTheCaptainAsync(guildsEmbed);
                        break;

                    case "guild":
                        if (parameters.Length > 1)
                        {
                            if (UInt64.TryParse(parameters[1], out var guildId))
                            {
                                await bot.SendMessageToTheCaptainAsync(GetGuildStatus(bot, guildId));
                            }
                            else
                            {
                                await bot.SendMessageToTheCaptainAsync(
                                    BuildErrorMessage(bot, "Guild Status", "Invalid guild ID specified")
                                );
                            }
                        }
                        else
                        {
                            await bot.SendMessageToTheCaptainAsync(
                                BuildErrorMessage(bot, "Guild Status", "You must specify a guild ID")
                            );
                        }

                        break;

                    default:
                        await bot.SendMessageToTheCaptainAsync($"Unknown status type {statusType}");
                        break;
                }
            }
        }

        private static Embed GetGuildStatus(BotCore bot, ulong guildId)
        {
            if (bot.DiscordClient.Guilds.Any(g => g.Id == guildId))
            {
                var guild = bot.DiscordClient.Guilds.First(g => g.Id == guildId);
                return new EmbedInstance()
                {
                    Title = $"{guild.Name} Status",
                    Color = ACCENT_COLOR,
                    Fields = [
                        new("Created", $"{guild.CreatedAt.UtcDateTime:F} UTC"),
                        new("Age", DateTime.UtcNow.Subtract(guild.CreatedAt.UtcDateTime).ToHumanReadableString()),
                        new("Owner", $"<@{guild.OwnerId}> ({guild.Owner.Username})"),
                        new("Users", guild.Users.Count.ToString()),
                        new("Total Channels", guild.Channels.Count.ToString()),
                        new("Text Channels", guild.TextChannels.Count.ToString()),
                        new("Forum Channels", guild.ForumChannels.Count.ToString()),
                        new("Voice Channels", guild.VoiceChannels.Count.ToString())
                    ]
                }.BuildDiscordEmbed(bot);
            }
            else
            {
                return new EmbedInstance()
                {
                    Title = "Guild Status",
                    Color = ACCENT_COLOR_ERROR,
                    Description = $"No guild foung with ID {guildId}"
                }.BuildDiscordEmbed(bot);
            }
        }

        private static async Task ReloadGuildsAsync(BotCore bot)
        {
            await bot.SendMessageToTheCaptainAsync("SQUAK! Roger roger. Reloading configs...");
            await bot.LoadGuildConfigsAsync();
            await bot.SendMessageToTheCaptainAsync($"Guild configs have been reloaded. Listening for events/commands for {bot.Guilds.Count} guild{(bot.Guilds.Count == 1 ? String.Empty : "s")}.");
        }

        private static async Task ReloadGuildConfigAsync(BotCore bot, string[] parameters)
        {
            if (parameters.Length > 0)
            {
                if (UInt64.TryParse(parameters[0], out var guildId))
                {
                    await bot.SendMessageToTheCaptainAsync("Roger roger, attempting to reload guild...");
                    await bot.LoadGuildConfigAsync(guildId);

                    if (bot.Guilds.TryGetValue(guildId, out var guild))
                    {
                        await bot.RegisterGuildCommandsAsync(guild);
                        await bot.SendMessageToTheCaptainAsync("Guild reloaded");
                    }
                    else
                    {
                        await bot.SendMessageToTheCaptainAsync("Doesn't look like there's a guild config with that guild ID");
                    }
                }
                else
                {
                    await bot.SendMessageToTheCaptainAsync("That's not a valid guild ID");
                }
            }
            else
            {
                await bot.SendMessageToTheCaptainAsync("You have to specify a guild ID to reload, cap'n");
            }
        }
    }
}