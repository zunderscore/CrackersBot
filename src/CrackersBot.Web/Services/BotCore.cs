using CrackersBot.Core;
using CrackersBot.Core.Actions;
using CrackersBot.Core.Events;
using CrackersBot.Core.Filters;
using CrackersBot.Core.Variables;
using Discord;
using Discord.WebSocket;
using Microsoft.Azure.Cosmos;
using System.Collections.Concurrent;

namespace CrackersBot.Web.Services
{
    public partial class BotCore : IBotCore
    {
        private const ulong ZUNDERSCORE_USER_ID = 209052262671187968;

        private readonly IConfiguration _config;
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly ILogger _logger;

        public BotCore(IConfiguration config, ILogger<BotCore> logger)
        {
            _config = config;
            _logger = logger;
            _discordSocketClient = new DiscordSocketClient(new DiscordSocketConfig()
            {
                MessageCacheSize = 50,
                GatewayIntents = GatewayIntents.All,
                AlwaysDownloadUsers = true
            });
        }

        public ILogger Logger => _logger;

        public DiscordSocketClient DiscordClient => _discordSocketClient;

        public ConcurrentDictionary<ulong, GuildConfig> Guilds { get; } = new();

        #region Core Functions

        internal async Task StartBotCoreAsync()
        {
            RegisterCoreActions();
            RegisterCoreVariables();
            RegisterCoreEventHandlers();
            RegisterCoreFilters();

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

        #region Actions

        public ConcurrentDictionary<string, IAction> RegisteredActions { get; } = new();

        public bool IsActionRegistered(string id)
        {
            return RegisteredActions.Keys.Any(k => k.Equals(id, StringComparison.InvariantCultureIgnoreCase));
        }

        public void RegisterAction(IAction action)
        {
            var id = action.GetActionId();

            if (IsActionRegistered(id))
            {
                Logger.LogDebug("Unable to register Action {id} as it has already been registered", id);
                return;
            }

            if (RegisteredActions.TryAdd(id, action))
            {
                Logger.LogDebug("Registered Action {id}", id);
            }
            else
            {
                Logger.LogDebug("Unable to register Action {id} (registration failed)", id);
            }
        }

        public void UnregisterAction(string id)
        {
            if (!IsActionRegistered(id))
            {
                Logger.LogDebug("Unable to unregister Action {id} since it is not currently registered", id);
                return;
            }

            if (RegisteredActions.TryRemove(id, out _))
            {
                Logger.LogDebug("Unregistered Action {id}", id);
            }
            else
            {
                Logger.LogDebug("Unable to unregister Action {id} (removal failed)", id);
            }
        }

        public IAction GetRegisteredAction(string id)
        {
            if (!IsActionRegistered(id))
            {
                throw new ArgumentException($"Action {id} is not registered");
            }

            return RegisteredActions[id];
        }

        #endregion

        #region Variables

        public ConcurrentDictionary<string, IVariable> RegisteredVariables { get; } = new();

        public bool IsVariableRegistered(string token)
        {
            return RegisteredVariables.Keys.Any(k => k.Equals(token, StringComparison.InvariantCultureIgnoreCase));
        }

        public void RegisterVariable(IVariable variable)
        {
            if (IsVariableRegistered(variable.Token))
            {
                Logger.LogDebug("Unable to register Variable {token} as it has already been registered", variable.Token);
                return;
            }

            if (RegisteredVariables.TryAdd(variable.Token, variable))
            {
                Logger.LogDebug("Registered Variable {token}", variable.Token);
            }
            else
            {
                Logger.LogDebug("Unable to register Variable {token} (registration failed)", variable.Token);
            }
        }

        public void UnregisterVariable(string token)
        {
            if (!IsVariableRegistered(token))
            {
                Logger.LogDebug("Unable to unregister Variable {token} since it is not currently registered", token);
                return;
            }

            if (RegisteredVariables.TryRemove(token, out _))
            {
                Logger.LogDebug("Unregistered Variable {token}", token);
            }
            else
            {
                Logger.LogDebug("Unable to unregister Variable {token} (removal failed)", token);
            }
        }

        #endregion

        #region Event Handlers

        public ConcurrentDictionary<string, IEventHandler> RegisteredEventHandlers { get; } = new();

        public bool IsEventHandlerRegistered(string id)
        {
            return RegisteredEventHandlers.Keys.Any(k => k.Equals(id, StringComparison.InvariantCultureIgnoreCase));
        }

        public void RegisterEventHandler(IEventHandler handler)
        {
            var id = handler.GetEventId();

            if (IsEventHandlerRegistered(id))
            {
                Logger.LogDebug("Unable to register Event Handler {id} as it has already been registered", id);
                return;
            }

            if (RegisteredEventHandlers.TryAdd(id, handler))
            {
                Logger.LogDebug("Registered Event Handler {id}", id);
            }
            else
            {
                Logger.LogDebug("Unable to register Event Handler {id} (registration failed)", id);
            }
        }

        public void UnregisterEventHandler(string id)
        {
            if (!IsEventHandlerRegistered(id))
            {
                Logger.LogDebug("Unable to unregister Event Handler {id} since it is not currently registered", id);
                return;
            }

            if (RegisteredEventHandlers.TryRemove(id, out _))
            {
                Logger.LogDebug("Unregistered Event Handler {id}", id);
            }
            else
            {
                Logger.LogDebug("Unable to unregister Event Handler {id} (removal failed)", id);
            }
        }

        #endregion

        #region Filters

        public ConcurrentDictionary<string, IFilter> RegisteredFilters { get; } = new();

        public bool IsFilterRegistered(string token)
        {
            return RegisteredFilters.Keys.Any(k => k.Equals(token, StringComparison.InvariantCultureIgnoreCase));
        }

        public void RegisterFilter(IFilter filter)
        {
            var id = filter.GetFilterId();

            if (IsFilterRegistered(id))
            {
                Logger.LogDebug("Unable to register Filter {id} as it has already been registered", id);
                return;
            }

            if (RegisteredFilters.TryAdd(id, filter))
            {
                Logger.LogDebug("Registered Filter {id}", id);
            }
            else
            {
                Logger.LogDebug("Unable to register Filter {id} (registration failed)", id);
            }
        }

        public void UnregisterFilter(string id)
        {
            if (!IsFilterRegistered(id))
            {
                Logger.LogDebug("Unable to unregister Filter {id} since it is not currently registered", id);
                return;
            }

            if (RegisteredFilters.TryRemove(id, out _))
            {
                Logger.LogDebug("Unregistered Filter {id}", id);
            }
            else
            {
                Logger.LogDebug("Unable to unregister Filter {id} (removal failed)", id);
            }
        }

        #endregion

        #region Register core items

        private void RegisterCoreActions()
        {
            foreach (var action in CoreHelpers.GetAllCoreActions())
            {
                RegisterAction(action);
            }
        }

        private void RegisterCoreVariables()
        {
            foreach (var variable in CoreHelpers.GetAllCoreVariables())
            {
                RegisterVariable(variable);
            }
        }

        private void RegisterCoreEventHandlers()
        {
            foreach (var handler in CoreHelpers.GetAllCoreEventHandlers())
            {
                RegisterEventHandler(handler);
            }
        }

        private void RegisterCoreFilters()
        {
            foreach (var filter in CoreHelpers.GetAllCoreFilters())
            {
                RegisterFilter(filter);
            }
        }

        #endregion
    }
}