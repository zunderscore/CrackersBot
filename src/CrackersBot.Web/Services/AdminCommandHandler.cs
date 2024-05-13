using CrackersBot.Core.Actions.Discord;
using Discord;

namespace CrackersBot.Web.Services
{
    internal static class AdminCommandHandler
    {
        private const string ACCENT_COLOR = "#0066FF";

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

                    case "reload":
                        if (messageParts.Length > 1)
                        {
                            var reloadKeyword = messageParts[1].ToLower();
                            switch (reloadKeyword)
                            {
                                case "configs":
                                    await bot.SendMessageToTheCaptainAsync("SQUAK! Roger roger. Reloading configs...");
                                    await bot.LoadGuildConfigsAsync();
                                    await bot.SendMessageToTheCaptainAsync($"Guild configs have been reloaded. Listening for events/commands for {bot.Guilds.Count} guild{(bot.Guilds.Count == 1 ? String.Empty : "s")}.");
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
    }
}