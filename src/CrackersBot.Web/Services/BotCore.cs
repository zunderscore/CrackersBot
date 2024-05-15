using CrackersBot.Core;
using CrackersBot.Core.Actions;
using CrackersBot.Core.Auditing;
using CrackersBot.Core.Events;
using CrackersBot.Core.Events.Discord;
using CrackersBot.Core.Filters;
using CrackersBot.Core.Variables;
using Discord;
using Discord.WebSocket;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace CrackersBot.Web.Services
{
    public class BotCore : IBotCore
    {
        private const ulong ZUNDERSCORE_USER_ID = 209052262671187968;

        private readonly IConfiguration _config;
        private readonly DiscordSocketClient _discordSocketClient;

        public BotCore(IConfiguration config)
        {
            _config = config;
            _discordSocketClient = new DiscordSocketClient(new DiscordSocketConfig()
            {
                MessageCacheSize = 50,
                GatewayIntents = GatewayIntents.All,
                AlwaysDownloadUsers = true
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

            _discordSocketClient.Ready += OnClientReady;
            _discordSocketClient.SlashCommandExecuted += OnSlashCommandExecuted;
            _discordSocketClient.PresenceUpdated += OnPresenceUpdated;
            _discordSocketClient.MessageReceived += OnMessageReceived;
            _discordSocketClient.MessageUpdated += OnMessageUpdated;
            _discordSocketClient.MessageDeleted += OnMessageDeleted;
            _discordSocketClient.UserJoined += OnUserJoined;
            _discordSocketClient.UserLeft += OnUserLeft;

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
                Debug.WriteLine(ex.Message);
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
                        Debug.WriteLine($"No guild config found for guild ID {guildId}");
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
                    Debug.WriteLine($"Unable to setup guild commands for guild {guild.GuildId}. Error: {ex.Message}");
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

        private async Task OnClientReady()
        {
            await Task.Run(() => Debug.WriteLine("Client ready!"));
            await LoadGuildConfigsAsync();
            await OnBotStarted();
            return;
        }

        // Default event handlers

        private async Task OnBotStarted()
        {
            Debug.WriteLine("OnBotStarted triggered");
            var eventId = BotStartedEventHandler.EVENT_ID;

            foreach (var (_, guild) in Guilds)
            {
                foreach (var instance in guild.EventHandlers.Where(e => e.EventId == eventId && e.Enabled))
                {
                    await RegisteredEventHandlers[eventId].Handle(this, instance, new RunContext());
                }
            }

            try
            {
                await SendMessageToTheCaptainAsync(AdminCommandHandler.GetStartupMessageEmbed(this));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to send BotStarted DM. Error: {ex.Message}");
            }
        }

        private async Task OnSlashCommandExecuted(SocketSlashCommand slashCommand)
        {
            Debug.WriteLine("OnSlashCommandExecuted triggered");

            if (slashCommand.GuildId.HasValue
                && Guilds.TryGetValue(slashCommand.GuildId.Value, out var guild))
            {
                var commandHandler = guild.Commands.FirstOrDefault(c => c.Name == slashCommand.CommandName && c.Enabled);
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
                var socketGuild = _discordSocketClient.GetGuild(guildId);

                if (socketGuild.Users.Any(u => u.Id == user.Id))
                {
                    var context = new RunContext()
                        .WithDiscordGuild(socketGuild)
                        .WithDiscordUser(user);

                    foreach (var instance in guild.EventHandlers.Where(e => e.EventId == eventId && e.Enabled))
                    {
                        await RegisteredEventHandlers[eventId].Handle(this, instance, context);
                    }

                    if (startedStreaming)
                    {
                        eventId = UserStartedStreamingEventHandler.EVENT_ID;

                        if (guild.AuditSettings.UserStartedStreaming)
                        {
                            await AuditHelpers.SendAuditMessageAsync(
                                _discordSocketClient,
                                guild.GuildId,
                                guild.AuditSettings.AuditChannelId,
                                AuditHelpers.GetUserStartedStreamingMessage(this, user)
                            );
                        }

                        foreach (var instance in guild.EventHandlers.Where(e => e.EventId == eventId && e.Enabled))
                        {
                            await RegisteredEventHandlers[eventId].Handle(this, instance, context);
                        }
                    }

                    if (stoppedStreaming)
                    {
                        eventId = UserStoppedStreamingEventHandler.EVENT_ID;

                        if (guild.AuditSettings.UserStoppedStreaming)
                        {
                            await AuditHelpers.SendAuditMessageAsync(
                                _discordSocketClient,
                                guild.GuildId,
                                guild.AuditSettings.AuditChannelId,
                                AuditHelpers.GetUserStoppedStreamingMessage(this, user)
                            );
                        }


                        foreach (var instance in guild.EventHandlers.Where(e => e.EventId == eventId && e.Enabled))
                        {
                            await RegisteredEventHandlers[eventId].Handle(this, instance, context);
                        }
                    }
                }
            }
        }

        private async Task OnMessageReceived(SocketMessage message)
        {
            if (message.Author.Id == _discordSocketClient.CurrentUser.Id)
            {
                // Ignore messages from the bot itself
                return;
            }

            Debug.WriteLine("OnMessageReceived triggered");
            var eventId = MessageReceivedEventHandler.EVENT_ID;

            if (message.Channel is SocketTextChannel textChannel)
            {
                var context = new RunContext()
                    .WithDiscordMessage(message);

                var guildId = textChannel.Guild.Id;

                if (Guilds.TryGetValue(guildId, out var guild))
                {
                    foreach (var instance in guild.EventHandlers.Where(e => e.EventId == eventId && e.Enabled))
                    {
                        await RegisteredEventHandlers[eventId].Handle(this, instance, context);
                    }
                }
            }
            else if (message.Channel is SocketDMChannel dmChannel)
            {
                var context = new RunContext()
                    .WithDiscordMessage(message);

                // Honey... There's a bear at the door.
                if (message.Author.Id == ZUNDERSCORE_USER_ID)
                {
                    await AdminCommandHandler.HandleDMCommandAsync(this, message.ToString() ?? String.Empty);
                }
                else
                {
                    await dmChannel.SendMessageAsync("SQUAK! Sorry, I only talk to the cap'n.");
                }
            }
        }

        private async Task OnMessageUpdated(Cacheable<IMessage, ulong> oldMessage, SocketMessage message, ISocketMessageChannel channel)
        {
            Debug.WriteLine("OnMessageUpdated triggered");
            var eventId = MessageUpdatedEventHandler.EVENT_ID;

            if (oldMessage.HasValue)
            {
                if (oldMessage.ToString() != message.ToString())
                {
                    if (channel is ITextChannel textChannel)
                    {
                        var context = new RunContext()
                            .WithDiscordMessage(message)
                            .WithPreviousMessageText(oldMessage.Value.ToString());

                        var guildId = textChannel.Guild.Id;

                        if (Guilds.TryGetValue(guildId, out var guild))
                        {
                            if (guild.AuditSettings.MessageUpdated)
                            {
                                await AuditHelpers.SendAuditMessageAsync(
                                    _discordSocketClient,
                                    guild.GuildId,
                                    guild.AuditSettings.AuditChannelId,
                                    AuditHelpers.GetMessageUpdatedMessage(this, message, oldMessage.Value.ToString() ?? String.Empty)
                                );
                            }

                            foreach (var instance in guild.EventHandlers.Where(e => e.EventId == eventId && e.Enabled))
                            {
                                await RegisteredEventHandlers[eventId].Handle(this, instance, context);
                            }
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("Messages are identical; skipping events");
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
                        .WithDiscordMessage(message.Value);

                    var guildId = textChannel.Guild.Id;

                    if (Guilds.TryGetValue(guildId, out var guild))
                    {
                        if (guild.AuditSettings.MessageDeleted)
                        {
                            await AuditHelpers.SendAuditMessageAsync(
                                _discordSocketClient,
                                guild.GuildId,
                                guild.AuditSettings.AuditChannelId,
                                AuditHelpers.GetMessageDeletedMessage(this, message.Value)
                            );
                        }

                        foreach (var instance in guild.EventHandlers.Where(e => e.EventId == eventId && e.Enabled))
                        {
                            await RegisteredEventHandlers[eventId].Handle(this, instance, context);
                        }
                    }
                }
            }
            else
            {
                Debug.WriteLine("Unable to retrieve deleted message from cache; skipping events");
            }
        }

        private async Task OnUserJoined(SocketGuildUser user)
        {
            Debug.WriteLine($"OnUserJoined triggered. Guild: {user.Guild.Name} ({user.Guild.Id}). User: {user.Username} ({user.Id})");
            var eventId = UserJoinedEventHandler.EVENT_ID;

            var context = new RunContext()
                .WithDiscordGuild(user.Guild)
                .WithDiscordUser(user);

            if (Guilds.TryGetValue(user.Guild.Id, out var guild))
            {
                if (guild.AuditSettings.UserJoined)
                {
                    await AuditHelpers.SendAuditMessageAsync(
                        _discordSocketClient,
                        guild.GuildId,
                        guild.AuditSettings.AuditChannelId,
                        AuditHelpers.GetUserJoinedMessage(this, user)
                    );
                }

                foreach (var instance in guild.EventHandlers.Where(e => e.EventId == eventId && e.Enabled))
                {
                    await RegisteredEventHandlers[eventId].Handle(this, instance, context);
                }
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
                if (guild.AuditSettings.UserLeft)
                {
                    await AuditHelpers.SendAuditMessageAsync(
                        _discordSocketClient,
                        guild.GuildId,
                        guild.AuditSettings.AuditChannelId,
                        AuditHelpers.GetUserLeftMessage(this, user)
                    );
                }

                foreach (var instance in guild.EventHandlers.Where(e => e.EventId == eventId && e.Enabled))
                {
                    await RegisteredEventHandlers[eventId].Handle(this, instance, context);
                }
            }
        }
    }
}