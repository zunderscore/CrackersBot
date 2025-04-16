using CrackersBot.Core.Actions.Discord;
using Discord;

namespace CrackersBot.Web.Services
{
    public partial class BotCore
    {
        private const string ACCENT_COLOR = "#0066FF";
        private const string ACCENT_COLOR_ERROR = "#FF0000";

        public Embed GetStartupMessageEmbed()
        {
            return new EmbedInstance()
            {
                Title = "CrackersBot is online!",
                Color = ACCENT_COLOR,
                Fields = [
                    new("Actions", $"Registered actions: {RegisteredActions.Count}"),
                    new("Variables", $"Registered variables: {RegisteredVariables.Count}"),
                    new("Filters", $"Registered filters: {RegisteredFilters.Count}"),
                    new("Event Handlers", $"Registered event handlers: {RegisteredEventHandlers.Count}"),
                    new("Guilds", $"Guild configurations loaded: {Guilds.Count}"),
                ]
            }.BuildDiscordEmbed();
        }

        public Embed GetReconnectEmbed()
        {
            return new EmbedInstance()
            {
                Title = "CrackersBot has reconnected",
                Color = ACCENT_COLOR
            }.BuildDiscordEmbed();
        }

        public async Task HandleDMCommandAsync(string message)
        {
            var messageParts = message.Split(' ');

            if (messageParts.Length > 0)
            {
                var keyword = messageParts[0].ToLower();

                switch (keyword)
                {
                    case "ping":
                        await SendMessageToTheCaptainAsync("PONG!");
                        break;

                    case "status":
                        await SendStatusMessage(messageParts[1..]);
                        break;

                    case "reload":
                        if (messageParts.Length > 1)
                        {
                            var reloadKeyword = messageParts[1].ToLower();
                            switch (reloadKeyword)
                            {
                                case "configs":
                                    await ReloadGuildsAsync();
                                    break;

                                case "guild":
                                    await ReloadGuildConfigAsync(messageParts[2..]);
                                    break;

                                default:
                                    await SendMessageToTheCaptainAsync(
                                        BuildErrorMessage("Reload", $"Unknown reload type **{reloadKeyword}**")
                                    );
                                    break;
                            }
                        }
                        else
                        {
                            await SendMessageToTheCaptainAsync(
                                BuildErrorMessage("Reload", "Please specify an item to reload")
                            );
                        }
                        break;

                    default:
                        await SendMessageToTheCaptainAsync(
                            BuildErrorMessage("Unknown Command", "I don't know what that means")
                        );
                        break;
                }
            }
        }

        private Embed BuildErrorMessage(string title, string message)
        {
            return new EmbedInstance()
            {
                Title = title,
                Description = message,
                Color = ACCENT_COLOR_ERROR
            }.BuildDiscordEmbed();
        }

        private async Task SendStatusMessage(string[] parameters)
        {
            if (parameters.Length == 0)
            {
                var embed = new EmbedInstance()
                {
                    Title = "CrackersBot Status",
                    Color = ACCENT_COLOR,
                    Fields = [
                        new("Started", $"{StartupTime:F} UTC"),
                        new("Uptime", DateTime.UtcNow.Subtract(StartupTime).ToHumanReadableString()),
                        new("Guilds Connected", DiscordClient.Guilds.Count.ToString())
                    ]
                }.BuildDiscordEmbed();

                await SendMessageToTheCaptainAsync(embed);
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
                            Fields = DiscordClient.Guilds
                                .Select(g => new Core.Actions.Discord.EmbedField(g.Name, g.Id.ToString(), true))
                                .ToList()
                        }.BuildDiscordEmbed();
                        await SendMessageToTheCaptainAsync(guildsEmbed);
                        break;

                    case "guild":
                        if (parameters.Length > 1)
                        {
                            if (UInt64.TryParse(parameters[1], out var guildId))
                            {
                                await SendMessageToTheCaptainAsync(GetGuildStatus(guildId));
                            }
                            else
                            {
                                await SendMessageToTheCaptainAsync(
                                    BuildErrorMessage("Guild Status", "Invalid guild ID specified")
                                );
                            }
                        }
                        else
                        {
                            await SendMessageToTheCaptainAsync(
                                BuildErrorMessage("Guild Status", "You must specify a guild ID")
                            );
                        }

                        break;

                    default:
                        await SendMessageToTheCaptainAsync($"Unknown status type {statusType}");
                        break;
                }
            }
        }

        private Embed GetGuildStatus(ulong guildId)
        {
            if (DiscordClient.Guilds.Any(g => g.Id == guildId))
            {
                var guild = DiscordClient.Guilds.First(g => g.Id == guildId);
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
                }.BuildDiscordEmbed();
            }
            else
            {
                return new EmbedInstance()
                {
                    Title = "Guild Status",
                    Color = ACCENT_COLOR_ERROR,
                    Description = $"No guild foung with ID {guildId}"
                }.BuildDiscordEmbed();
            }
        }

        private async Task ReloadGuildsAsync()
        {
            await SendMessageToTheCaptainAsync("SQUAK! Roger roger. Reloading configs...");
            await LoadGuildConfigsAsync();
            await SendMessageToTheCaptainAsync($"Guild configs have been reloaded. Listening for events/commands for {Guilds.Count} guild{(Guilds.Count == 1 ? String.Empty : "s")}.");
        }

        private async Task ReloadGuildConfigAsync(string[] parameters)
        {
            if (parameters.Length > 0)
            {
                if (UInt64.TryParse(parameters[0], out var guildId))
                {
                    await SendMessageToTheCaptainAsync("Roger roger, attempting to reload guild...");
                    await LoadGuildConfigAsync(guildId);

                    if (Guilds.TryGetValue(guildId, out var guild))
                    {
                        await RegisterGuildCommandsAsync(guild);
                        await SendMessageToTheCaptainAsync("Guild reloaded");
                    }
                    else
                    {
                        await SendMessageToTheCaptainAsync("Doesn't look like there's a guild config with that guild ID");
                    }
                }
                else
                {
                    await SendMessageToTheCaptainAsync("That's not a valid guild ID");
                }
            }
            else
            {
                await SendMessageToTheCaptainAsync("You have to specify a guild ID to reload, cap'n");
            }
        }
    }
}