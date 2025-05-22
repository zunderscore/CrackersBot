using CrackersBot.Core;
using CrackersBot.Core.Auditing;
using CrackersBot.Core.Events;
using CrackersBot.Core.Events.Discord;
using Discord;
using Discord.WebSocket;

namespace CrackersBot.Web.Services
{
    public partial class BotCore
    {
        private bool _hasConnected = false;

        private void SetupEventListeners()
        {
            _discordSocketClient.Ready += OnClientReady;
            _discordSocketClient.SlashCommandExecuted += OnSlashCommandExecuted;
            _discordSocketClient.UserCommandExecuted += OnUserCommandExecuted;
            _discordSocketClient.MessageCommandExecuted += OnMessageCommandExecuted;
            _discordSocketClient.PresenceUpdated += OnPresenceUpdated;
            _discordSocketClient.MessageReceived += OnMessageReceived;
            _discordSocketClient.MessageUpdated += OnMessageUpdated;
            _discordSocketClient.MessageDeleted += OnMessageDeleted;
            _discordSocketClient.UserJoined += OnUserJoined;
            _discordSocketClient.UserLeft += OnUserLeft;
        }

        [LoggerMessage(Level = LogLevel.Debug, Message = "{message}")]
        partial void LogDebugMessage(string message);

        [LoggerMessage(Level = LogLevel.Debug, Message = "{eventName} triggered")]
        partial void LogEventTriggered(string eventName);

        private async Task OnClientReady()
        {
            Logger.LogInformation("Client ready!");
            await LoadGuildConfigsAsync();
            await OnBotStarted();
            return;
        }

        // Default event handlers

        private async Task OnBotStarted()
        {
            LogEventTriggered("OnBotStarted");
            var eventId = BotStartedEventHandler.EVENT_ID;

            foreach (var (_, guild) in Guilds)
            {
                foreach (var instance in guild.EventHandlers.Where(e => e.EventId == eventId && e.Enabled))
                {
                    await RegisteredEventHandlers[eventId].Handle(instance, new RunContext());
                }
            }

            try
            {
                if (!_hasConnected)
                {
                    _hasConnected = true;
                    await SendMessageToTheCaptainAsync(GetStartupMessageEmbed());
                }
                else
                {
                    await SendMessageToTheCaptainAsync(GetReconnectEmbed());
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unable to send OnBotStarted message");
            }
        }

        private async Task OnSlashCommandExecuted(SocketSlashCommand command)
        {
            LogEventTriggered("OnSlashCommandExecuted");

            if (command.GuildId.HasValue
                && Guilds.TryGetValue(command.GuildId.Value, out var guild))
            {
                var commandHandler = guild.Commands.FirstOrDefault(c => c.Name == command.CommandName && c.Enabled);
                if (commandHandler is not null)
                {
                    await commandHandler.RunActions(this, command);
                }
            }
        }

        private async Task OnUserCommandExecuted(SocketUserCommand command)
        {
            LogEventTriggered("OnUserCommandExecuted");

            if (command.GuildId.HasValue
                && Guilds.TryGetValue(command.GuildId.Value, out var guild))
            {
                var commandHandler = guild.Commands.FirstOrDefault(c => c.Name == command.CommandName && c.Enabled);
                if (commandHandler is not null)
                {
                    await commandHandler.RunActions(this, command);
                }
            }
        }

        private async Task OnMessageCommandExecuted(SocketMessageCommand command)
        {
            LogEventTriggered("OnMessageCommandExecuted");

            if (command.GuildId.HasValue
                && Guilds.TryGetValue(command.GuildId.Value, out var guild))
            {
                var commandHandler = guild.Commands.FirstOrDefault(c => c.Name == command.CommandName && c.Enabled);
                if (commandHandler is not null)
                {
                    await commandHandler.RunActions(this, command);
                }
            }
        }

        private async Task OnPresenceUpdated(SocketUser user, SocketPresence oldPresence, SocketPresence newPresence)
        {
            LogEventTriggered("OnPresenceUpdated");
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
                        await RegisteredEventHandlers[eventId].Handle(instance, context);
                    }

                    if (startedStreaming)
                    {
                        eventId = UserStartedStreamingEventHandler.EVENT_ID;

                        if (guild.AuditSettings.UserStartedStreaming)
                        {
                            await AuditHelpers.SendAuditMessageAsync(
                                this,
                                guild.GuildId,
                                guild.AuditSettings.AuditChannelId,
                                AuditHelpers.GetUserStartedStreamingMessage(user)
                            );
                        }

                        foreach (var instance in guild.EventHandlers.Where(e => e.EventId == eventId && e.Enabled))
                        {
                            await RegisteredEventHandlers[eventId].Handle(instance, context);
                        }
                    }

                    if (stoppedStreaming)
                    {
                        eventId = UserStoppedStreamingEventHandler.EVENT_ID;

                        if (guild.AuditSettings.UserStoppedStreaming)
                        {
                            await AuditHelpers.SendAuditMessageAsync(
                                this,
                                guild.GuildId,
                                guild.AuditSettings.AuditChannelId,
                                AuditHelpers.GetUserStoppedStreamingMessage(user)
                            );
                        }


                        foreach (var instance in guild.EventHandlers.Where(e => e.EventId == eventId && e.Enabled))
                        {
                            await RegisteredEventHandlers[eventId].Handle(instance, context);
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

            LogEventTriggered("OnMessageReceived");
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
                        await RegisteredEventHandlers[eventId].Handle(instance, context);
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
                    await HandleDMCommandAsync(message.Content ?? String.Empty);
                }
                else
                {
                    await dmChannel.SendMessageAsync("SQUAK! Sorry, I only talk to the cap'n.");
                }
            }
        }

        private async Task OnMessageUpdated(Cacheable<IMessage, ulong> oldMessage, SocketMessage message, ISocketMessageChannel channel)
        {
            LogEventTriggered("OnMessageUpdated");
            var eventId = MessageUpdatedEventHandler.EVENT_ID;

            if (oldMessage.HasValue)
            {
                if (!message.IsEphemeral() && oldMessage.Value.Content != message.Content)
                {
                    if (channel is ITextChannel textChannel)
                    {
                        var context = new RunContext()
                            .WithDiscordMessage(message)
                            .WithPreviousMessageText(oldMessage.Value.Content);

                        var guildId = textChannel.Guild.Id;

                        if (Guilds.TryGetValue(guildId, out var guild))
                        {
                            if (guild.AuditSettings.MessageUpdated && guild.AuditSettings.ShouldSendAuditMessage(message.Channel.Id))
                            {
                                await AuditHelpers.SendAuditMessageAsync(
                                    this,
                                    guild.GuildId,
                                    guild.AuditSettings.AuditChannelId,
                                    AuditHelpers.GetMessageUpdatedMessage(message, oldMessage.Value.Content ?? String.Empty)
                                );
                            }

                            foreach (var instance in guild.EventHandlers.Where(e => e.EventId == eventId && e.Enabled))
                            {
                                await RegisteredEventHandlers[eventId].Handle(instance, context);
                            }
                        }
                    }
                }
                else
                {
                    Logger.LogDebug("Messages are identical, or message is ephemeral; skipping events");
                }
            }
            else
            {
                Logger.LogDebug("Unable to retrieve original contents of edited message from cache; skipping events");
            }
        }

        private async Task OnMessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
        {
            LogEventTriggered("OnMessageDeleted");
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
                        if (guild.AuditSettings.MessageDeleted && guild.AuditSettings.ShouldSendAuditMessage(channel.Id))
                        {
                            await AuditHelpers.SendAuditMessageAsync(
                                this,
                                guild.GuildId,
                                guild.AuditSettings.AuditChannelId,
                                AuditHelpers.GetMessageDeletedMessage(message.Value)
                            );
                        }

                        foreach (var instance in guild.EventHandlers.Where(e => e.EventId == eventId && e.Enabled))
                        {
                            await RegisteredEventHandlers[eventId].Handle(instance, context);
                        }
                    }
                }
            }
            else
            {
                Logger.LogDebug("Unable to retrieve deleted message from cache; skipping events");
            }
        }

        private async Task OnUserJoined(SocketGuildUser user)
        {
            LogEventTriggered("OnUserJoined");
            Logger.LogDebug(
                "User joined Guild: {guildName} ({guildId}). User: {username} ({userId})",
                user.Guild.Name,
                user.Guild.Id,
                user.Username,
                user.Id
            );

            var eventId = UserJoinedEventHandler.EVENT_ID;

            var context = new RunContext()
                .WithDiscordGuild(user.Guild)
                .WithDiscordUser(user);

            if (Guilds.TryGetValue(user.Guild.Id, out var guild))
            {
                if (guild.AuditSettings.UserJoined)
                {
                    await AuditHelpers.SendAuditMessageAsync(
                        this,
                        guild.GuildId,
                        guild.AuditSettings.AuditChannelId,
                        AuditHelpers.GetUserJoinedMessage(user)
                    );
                }

                foreach (var instance in guild.EventHandlers.Where(e => e.EventId == eventId && e.Enabled))
                {
                    await RegisteredEventHandlers[eventId].Handle(instance, context);
                }
            }
        }

        private async Task OnUserLeft(SocketGuild socketGuild, SocketUser user)
        {
            LogEventTriggered("OnUserLeft");
            Logger.LogDebug($"User left Guild: {socketGuild.Name} ({socketGuild.Id}). User: {user.Username} ({user.Id})");

            var eventId = UserLeftEventHandler.EVENT_ID;

            var context = new RunContext()
                .WithDiscordGuild(socketGuild)
                .WithDiscordUser(user);

            if (Guilds.TryGetValue(socketGuild.Id, out var guild))
            {
                if (guild.AuditSettings.UserLeft)
                {
                    await AuditHelpers.SendAuditMessageAsync(
                        this,
                        guild.GuildId,
                        guild.AuditSettings.AuditChannelId,
                        AuditHelpers.GetUserLeftMessage(user)
                    );
                }

                foreach (var instance in guild.EventHandlers.Where(e => e.EventId == eventId && e.Enabled))
                {
                    await RegisteredEventHandlers[eventId].Handle(instance, context);
                }
            }
        }
    }
}