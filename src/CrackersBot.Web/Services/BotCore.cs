using CrackersBot.Core;
using CrackersBot.Core.Actions;
using CrackersBot.Core.Events;
using CrackersBot.Core.Events.Discord;
using CrackersBot.Core.Filters;
using CrackersBot.Core.Variables;
using Discord;
using Discord.WebSocket;
using Microsoft.Azure.Cosmos;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace CrackersBot.Web.Services
{
    public class BotCore : IBotCore
    {
        private readonly IConfiguration _config;
        private readonly DiscordSocketClient _discordSocketClient;

        public BotCore(IConfiguration config)
        {
            _config = config;
            _discordSocketClient = new DiscordSocketClient(new DiscordSocketConfig()
            {
                MessageCacheSize = 50,
                GatewayIntents = GatewayIntents.All
            });
        }

        public IDiscordClient DiscordClient => _discordSocketClient;

        public ConcurrentDictionary<ulong, GuildConfig> Guilds { get; } = new();

        #region Core Functions

        internal async Task StartBotCoreAsync()
        {
            RegisterCoreActions();
            RegisterCoreVariables();
            RegisterCoreEventHandlers();
            RegisterCoreFilters();

            await LoadGuildConfigs();

            _discordSocketClient.Ready += ClientReady;
            _discordSocketClient.MessageReceived += OnMessageReceived;
            _discordSocketClient.MessageDeleted += OnMessageDeleted;
            _discordSocketClient.UserLeft += OnUserLeft;

            await _discordSocketClient.LoginAsync(TokenType.Bot, _config["Discord:BotToken"]);
            await _discordSocketClient.StartAsync();

            await _discordSocketClient.SetActivityAsync(new Game("with Loaf", ActivityType.Playing));

            await OnBotStarted();
        }

        internal async Task StopBotCoreAsync()
        {
            await _discordSocketClient.StopAsync();
            await _discordSocketClient.DisposeAsync();
        }

        internal async Task LoadGuildConfigs()
        {
            try
            {
                using var cosmosClient = new CosmosClient(_config["CosmosEndpoint"], _config["CosmosKey"]);

                var container = cosmosClient.GetContainer("CrackersBot", "CrackersBot");
                var iterator = container.GetItemQueryIterator<GuildConfig>();

                while (iterator.HasMoreResults)
                {
                    foreach (var item in await iterator.ReadNextAsync())
                    {
                        Guilds.TryAdd(item.GuildId, item);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
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
                Debug.WriteLine($"Unable to register Action {id} as it has already been registered");
                return;
            }

            if (RegisteredActions.TryAdd(id, action))
            {
                Debug.WriteLine($"Registered Action {id}");
            }
            else
            {
                Debug.WriteLine($"Unable to register Action {id} (registration failed)");
            }
        }

        public void UnregisterAction(string id)
        {
            if (!IsActionRegistered(id))
            {
                Debug.WriteLine($"Unable to unregister Action {id} since it is not currently registered");
                return;
            }

            if (RegisteredActions.TryRemove(id, out _))
            {
                Debug.WriteLine($"Unregistered Action {id}");
            }
            else
            {
                Debug.WriteLine($"Unable to unregister Action {id} (removal failed)");
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
                Debug.WriteLine($"Unable to register Variable {variable.Token} as it has already been registered");
                return;
            }

            if (RegisteredVariables.TryAdd(variable.Token, variable))
            {
                Debug.WriteLine($"Registered Variable {variable.Token}");
            }
            else
            {
                Debug.WriteLine($"Unable to register Variable {variable.Token} (registration failed)");
            }
        }

        public void UnregisterVariable(string token)
        {
            if (!IsVariableRegistered(token))
            {
                Debug.WriteLine($"Unable to unregister Variable {token} since it is not currently registered");
                return;
            }

            if (RegisteredVariables.TryRemove(token, out _))
            {
                Debug.WriteLine($"Unregistered Variable {token}");
            }
            else
            {
                Debug.WriteLine($"Unable to unregister Variable {token} (removal failed)");
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
                Debug.WriteLine($"Unable to register Event Handler {id} as it has already been registered");
                return;
            }

            if (RegisteredEventHandlers.TryAdd(id, handler))
            {
                Debug.WriteLine($"Registered Event Handler {id}");
            }
            else
            {
                Debug.WriteLine($"Unable to register Event Handler {id} (registration failed)");
            }
        }

        public void UnregisterEventHandler(string token)
        {
            if (!IsEventHandlerRegistered(token))
            {
                Debug.WriteLine($"Unable to unregister Event Handler {token} since it is not currently registered");
                return;
            }

            if (RegisteredEventHandlers.TryRemove(token, out _))
            {
                Debug.WriteLine($"Unregistered Event Handler {token}");
            }
            else
            {
                Debug.WriteLine($"Unable to unregister Event Handler {token} (removal failed)");
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
                Debug.WriteLine($"Unable to register Filter {id} as it has already been registered");
                return;
            }

            if (RegisteredFilters.TryAdd(id, filter))
            {
                Debug.WriteLine($"Registered Filter {id}");
            }
            else
            {
                Debug.WriteLine($"Unable to register Filter {id} (registration failed)");
            }
        }

        public void UnregisterFilter(string token)
        {
            if (!IsFilterRegistered(token))
            {
                Debug.WriteLine($"Unable to unregister Filter {token} since it is not currently registered");
                return;
            }

            if (RegisteredFilters.TryRemove(token, out _))
            {
                Debug.WriteLine($"Unregistered Filter {token}");
            }
            else
            {
                Debug.WriteLine($"Unable to unregister Filter {token} (removal failed)");
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

        // Bot events

        private async Task ClientReady()
        {
            await Task.Run(() => Debug.WriteLine("Client ready!"));
            return;
        }

        // Default event handlers

        private async Task OnBotStarted()
        {
            foreach (var (_, guild) in Guilds)
            {
                foreach (var eventHandlerDefinition in guild.EventHandlers.Where(h => h.EventId == BotStartedEventHandler.EVENT_ID))
                {
                    await RegisteredEventHandlers[BotStartedEventHandler.EVENT_ID]
                        .Handle(this, eventHandlerDefinition);
                }
            }
        }

        private async Task OnMessageReceived(SocketMessage message)
        {
            var context = new Dictionary<string, object>()
            {
                { CommonNames.DISCORD_AUTHOR_ID, message.Author.Id },
                { CommonNames.DISCORD_CHANNEL_ID, message.Channel.Id },
                { CommonNames.DISCORD_MESSAGE_ID, message.Id },
                { CommonNames.MESSAGE_TEXT, message.ToString() ?? String.Empty }
            };

            if (message.Channel is SocketTextChannel textChannel)
            {
                var guildId = textChannel.Guild.Id;

                if (Guilds.TryGetValue(guildId, out var guild))
                {
                    foreach (var eventHandlerDefinition in guild.EventHandlers.Where(h => h.EventId == MessageReceivedEventHandler.EVENT_ID))
                    {
                        await RegisteredEventHandlers[MessageReceivedEventHandler.EVENT_ID]
                            .Handle(this, eventHandlerDefinition, context);
                    }
                }
            }
        }

        private async Task OnMessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
        {
            if (message.HasValue)
            {
                var context = new Dictionary<string, object>()
                {
                    { CommonNames.DISCORD_AUTHOR_ID, message.Value.Author?.Id ?? 0 },
                    { CommonNames.DISCORD_CHANNEL_ID, channel.Id },
                    { CommonNames.DISCORD_MESSAGE_ID, message.Id },
                    { CommonNames.MESSAGE_TEXT, message.Value.ToString() ?? String.Empty }
                };

                if (message.Value.Channel is ITextChannel textChannel)
                {
                    var guildId = textChannel.Guild.Id;

                    if (Guilds.TryGetValue(guildId, out var guild))
                    {
                        foreach (var eventHandlerDefinition in guild.EventHandlers.Where(h => h.EventId == MessageDeletedEventHandler.EVENT_ID))
                        {
                            await RegisteredEventHandlers[MessageDeletedEventHandler.EVENT_ID]
                                .Handle(this, eventHandlerDefinition, context);
                        }
                    }
                }
            }
            else
            {
                Debug.WriteLine("Unable to retrieve deleted message from cache; skipping events");
            }
        }

        private async Task OnUserLeft(SocketGuild socketGuild, SocketUser user)
        {
            var context = new Dictionary<string, object>(){
                { CommonNames.DISCORD_USER_ID, user.Id }
            };

            if (Guilds.TryGetValue(socketGuild.Id, out var guild))
            {
                foreach (var eventHandlerDefinition in guild.EventHandlers.Where(h => h.EventId == UserLeftEventHandler.EVENT_ID))
                {
                    await RegisteredEventHandlers[UserLeftEventHandler.EVENT_ID]
                        .Handle(this, eventHandlerDefinition, context);
                }
            }
        }
    }
}