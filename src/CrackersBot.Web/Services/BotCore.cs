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

        public DiscordSocketClient DiscordClient => _discordSocketClient;

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
            _discordSocketClient.SlashCommandExecuted += OnSlashCommandExecuted;
            _discordSocketClient.PresenceUpdated += OnPresenceUpdated;
            _discordSocketClient.MessageReceived += OnMessageReceived;
            _discordSocketClient.MessageUpdated += OnMessageUpdated;
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

        public async Task LoadGuildConfigs()
        {
            try
            {
                using var cosmosClient = new CosmosClient(_config["CosmosEndpoint"], _config["CosmosKey"]);

                var container = cosmosClient.GetContainer("CrackersBot", "CrackersBot");
                var iterator = container.GetItemQueryIterator<GuildConfig>();

                Guilds.Clear();

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

        internal async Task RegisterCommands()
        {
            foreach (var (guildId, guild) in Guilds)
            {
                foreach (var command in guild.Commands)
                {
                    var commandBuilder = new SlashCommandBuilder();
                    try
                    {
                        var socketGuild = _discordSocketClient.GetGuild(guildId);
                        if (socketGuild is not null)
                        {
                            await socketGuild.CreateApplicationCommandAsync(commandBuilder
                                .WithName(command.Name)
                                .WithDescription(command.Description)
                                .Build());
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Unable to create command {command.Name}: {ex.Message}");
                    }
                }
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
            await RegisterCommands();
            return;
        }

        // Default event handlers

        private async Task OnBotStarted()
        {
            Debug.WriteLine("OnBotStarted triggered");
            var eventId = BotStartedEventHandler.EVENT_ID;

            foreach (var (_, guild) in Guilds)
            {
                foreach (var eventHandlerDefinition in guild.EventHandlers.Where(h => h.EventId == eventId))
                {
                    await RegisteredEventHandlers[eventId]
                        .Handle(this, eventHandlerDefinition, new RunContext());
                }
            }
        }

        private async Task OnSlashCommandExecuted(SocketSlashCommand slashCommand)
        {
            Debug.WriteLine("OnSlashCommandExecuted triggered");

            if (slashCommand.GuildId.HasValue
                && Guilds.TryGetValue(slashCommand.GuildId.Value, out var guild))
            {
                var commandHandler = guild.Commands.FirstOrDefault(h => h.Name == slashCommand.CommandName);
                if (commandHandler is not null)
                {
                    await commandHandler.RunActions(this, slashCommand);
                }
            }
        }

        private async Task OnPresenceUpdated(SocketUser user, SocketPresence oldPresence, SocketPresence newPresence)
        {
            Debug.WriteLine("OnPresenceUpdated triggered");
            var eventId = UserPresenceUpdatedEventHandler.EVENT_ID;

            var wasStreaming = oldPresence?.Activities?.Any(a => a?.Type == ActivityType.Streaming) ?? false;
            var isStreaming = newPresence?.Activities?.Any(a => a?.Type == ActivityType.Streaming) ?? false;

            var startedStreaming = !wasStreaming && isStreaming;
            var stoppedStreaming = wasStreaming && !isStreaming;

            foreach (var (guildId, guild) in Guilds)
            {
                var context = new RunContext()
                    .WithDiscordGuild(_discordSocketClient.GetGuild(guildId))
                    .WithDiscordUser(user)
                    .WithDiscordPresense(newPresence);

                foreach (var eventDef in guild.EventHandlers.Where(e => e.EventId == eventId))
                {
                    await RegisteredEventHandlers[eventId]
                        .Handle(this, eventDef, context);
                }

                if (startedStreaming)
                {
                    eventId = UserStartedStreamingEventHandler.EVENT_ID;

                    foreach (var eventDef in guild.EventHandlers.Where(e => e.EventId == eventId))
                    {
                        await RegisteredEventHandlers[eventId]
                            .Handle(this, eventDef, context);
                    }
                }

                if (stoppedStreaming)
                {
                    eventId = UserStoppedStreamingEventHandler.EVENT_ID;

                    foreach (var eventDef in guild.EventHandlers.Where(e => e.EventId == eventId))
                    {
                        await RegisteredEventHandlers[eventId]
                            .Handle(this, eventDef, context);
                    }
                }
            }
        }

        private async Task OnMessageReceived(SocketMessage message)
        {
            Debug.WriteLine("OnMessageReceived triggered");
            var eventId = MessageReceivedEventHandler.EVENT_ID;

            if (message.Channel is SocketTextChannel textChannel)
            {
                var context = new RunContext()
                    .WithDiscordGuild(textChannel.Guild)
                    .WithDiscordUser(message.Author)
                    .WithDiscordChannel(message.Channel)
                    .WithDiscordMessage(message);

                var guildId = textChannel.Guild.Id;

                if (Guilds.TryGetValue(guildId, out var guild))
                {
                    foreach (var eventHandlerDefinition in guild.EventHandlers.Where(h => h.EventId == eventId))
                    {
                        await RegisteredEventHandlers[eventId]
                            .Handle(this, eventHandlerDefinition, context);
                    }
                }
            }
        }

        private async Task OnMessageUpdated(Cacheable<IMessage, ulong> oldMessage, SocketMessage newMessage, ISocketMessageChannel channel)
        {
            Debug.WriteLine("OnMessageUpdated triggered");
            var eventId = MessageUpdatedEventHandler.EVENT_ID;

            if (oldMessage.HasValue)
            {
                if (channel is ITextChannel textChannel)
                {
                    var context = new RunContext()
                        .WithDiscordGuild(textChannel.Guild)
                        .WithDiscordUser(newMessage.Author)
                        .WithDiscordChannel(channel)
                        .WithDiscordMessage(newMessage)
                        .WithPreviousMessageText(oldMessage.Value.ToString());

                    var guildId = textChannel.Guild.Id;

                    if (Guilds.TryGetValue(guildId, out var guild))
                    {
                        foreach (var eventHandlerDefinition in guild.EventHandlers.Where(h => h.EventId == eventId))
                        {
                            await RegisteredEventHandlers[eventId]
                                .Handle(this, eventHandlerDefinition, context);
                        }
                    }
                }
            }
            else
            {
                Debug.WriteLine("Unable to retrieve original contents of edited message from cache; skipping events");
            }
        }

        private async Task OnMessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
        {
            Debug.WriteLine("OnMessageDeleted triggered");
            var eventId = MessageDeletedEventHandler.EVENT_ID;

            if (message.HasValue && channel.HasValue)
            {
                if (channel.Value is ITextChannel textChannel)
                {
                    var context = new RunContext()
                        .WithDiscordGuild(textChannel.Guild)
                        .WithDiscordUser(message.Value.Author)
                        .WithDiscordChannel(channel.Value)
                        .WithDiscordMessage(message.Value);

                    var guildId = textChannel.Guild.Id;

                    if (Guilds.TryGetValue(guildId, out var guild))
                    {
                        foreach (var eventHandlerDefinition in guild.EventHandlers.Where(h => h.EventId == eventId))
                        {
                            await RegisteredEventHandlers[eventId]
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
            Debug.WriteLine($"OnUserLeft triggered. Guild: {socketGuild.Name} ({socketGuild.Id}). User: {user.Username} ({user.Id})");
            var eventId = UserLeftEventHandler.EVENT_ID;

            var context = new RunContext()
                .WithDiscordGuild(socketGuild)
                .WithDiscordUser(user);

            if (Guilds.TryGetValue(socketGuild.Id, out var guild))
            {
                foreach (var eventHandlerDefinition in guild.EventHandlers.Where(h => h.EventId == eventId))
                {
                    await RegisteredEventHandlers[eventId]
                        .Handle(this, eventHandlerDefinition, context);
                }
            }
        }
    }
}