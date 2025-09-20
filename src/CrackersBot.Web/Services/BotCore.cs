using System.Collections.Concurrent;
using CrackersBot.Core;
using CrackersBot.Core.Actions;
using CrackersBot.Core.Commands;
using CrackersBot.Core.Events;
using CrackersBot.Core.Filters;
using CrackersBot.Core.Variables;
using Discord;
using Discord.WebSocket;
using Microsoft.Azure.Cosmos;

namespace CrackersBot.Web.Services;

public partial class BotCore(
    IConfiguration config,
    BotServiceProvider botServices
) : IBotCore
{
    private const ulong ZUNDERSCORE_USER_ID = 209052262671187968;

    private readonly IConfiguration _config = config;
    private readonly BotServiceProvider _botServices = botServices;
    private readonly DiscordSocketClient _discordSocketClient = botServices.GetBotService<DiscordSocketClient>();
    private readonly ILogger _logger = botServices.GetLogger<BotCore>();
    private readonly IActionManager _actionManager = botServices.GetBotService<IActionManager>();
    private readonly IEventManager _eventManager = botServices.GetBotService<IEventManager>();
    private readonly IFilterManager _filterManager = botServices.GetBotService<IFilterManager>();
    private readonly IVariableManager _variableManager = botServices.GetBotService<IVariableManager>();
    private readonly ICommandManager _commandManager = botServices.GetBotService<ICommandManager>();

    public ILogger Logger => _logger;

    public DiscordSocketClient DiscordClient => _discordSocketClient;

    internal DateTime StartupTime { get; } = DateTime.UtcNow;

    public ConcurrentDictionary<ulong, GuildConfig> Guilds { get; } = new();

    #region Core Functions

    internal async Task StartBotCoreAsync()
    {
        CoreHelpers.RegisterCoreActions(_botServices);
        CoreHelpers.RegisterCoreVariables(_botServices);
        CoreHelpers.RegisterCoreEvents(_botServices);
        CoreHelpers.RegisterCoreFilters(_botServices);

        SetupEventListeners();

        await _discordSocketClient.LoginAsync(TokenType.Bot, _config["Discord:BotToken"]);
        await _discordSocketClient.StartAsync();

        await _discordSocketClient.SetActivityAsync(new Game("with Loaf", ActivityType.Playing));
    }

    internal async Task StopBotCoreAsync()
    {
        await _discordSocketClient.StopAsync();
        await _discordSocketClient.DisposeAsync();
    }

    public async Task LoadGuildConfigsAsync()
    {
        try
        {
            using var cosmosClient = new CosmosClient(_config["CosmosEndpoint"], _config["CosmosKey"]);
            var container = cosmosClient.GetContainer("CrackersBot", "CrackersBot");
            using var iterator = container.GetItemQueryIterator<GuildConfig>();

            Guilds.Clear();

            while (iterator.HasMoreResults)
            {
                foreach (var guild in await iterator.ReadNextAsync())
                {
                    await AddGuildConfigAsync(guild);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading guild configs");
        }
    }

    public async Task LoadGuildConfigAsync(ulong guildId)
    {
        using var cosmosClient = new CosmosClient(_config["CosmosEndpoint"], _config["CosmosKey"]);
        var container = cosmosClient.GetContainer("CrackersBot", "CrackersBot");
        using var iterator = container.GetItemQueryIterator<GuildConfig>($"SELECT * FROM GuildConfigs c WHERE c.GuildId = \"{guildId}\"");

        while (iterator.HasMoreResults)
        {
            foreach (var guild in await iterator.ReadNextAsync())
            {
                if (guild is not null)
                {
                    await AddGuildConfigAsync(guild);
                }
                else
                {
                    Logger.LogDebug("No guild config found for guild ID {guildId}", guildId);
                }
            }
        }
    }

    public async Task AddGuildConfigAsync(GuildConfig guild)
    {
        if (Guilds.ContainsKey(guild.GuildId))
        {
            Guilds.TryRemove(guild.GuildId, out var _);
        }

        guild.EventHandlers.ForEach(h => h.SetGuild(guild));

        Guilds.TryAdd(guild.GuildId, guild);
        await RegisterGuildCommandsAsync(guild);
    }

    internal async Task RegisterGuildCommandsAsync(GuildConfig guild)
    {
        var socketGuild = _discordSocketClient.GetGuild(guild.GuildId);

        if (socketGuild is not null)
        {
            try
            {
                var commandsToAdd = guild.Commands
                    .Where(c => c.Enabled)
                    .Select(c => c.BuildCommand())
                    .ToArray();

                await socketGuild.BulkOverwriteApplicationCommandAsync(commandsToAdd);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unable to setup guild commands for guild {guildId}", guild.GuildId);
            }
        }
    }

    internal async Task SendMessageToTheCaptainAsync(string message)
    {
        ArgumentNullException.ThrowIfNull(message);

        await (await _discordSocketClient.GetUserAsync(ZUNDERSCORE_USER_ID))
            .SendMessageAsync(message);
    }

    internal async Task SendMessageToTheCaptainAsync(Embed? embed)
    {
        ArgumentNullException.ThrowIfNull(embed);

        await (await _discordSocketClient.GetUserAsync(ZUNDERSCORE_USER_ID))
            .SendMessageAsync(embed: embed);
    }

    #endregion

    public async Task TriggerEvent(
        string eventId,
        Func<RunContext, Core.Events.EventHandler, Task<RunContext>>? contextBuilder = null
    )
    {
        LogEventTriggered(eventId);

        foreach (var (guildId, guild) in Guilds)
        {
            await _eventManager.HandleGuildEvents(eventId, guild, contextBuilder);
        }
    }
}