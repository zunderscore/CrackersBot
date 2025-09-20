using CrackersBot.Core;
using CrackersBot.Core.Auditing;
using CrackersBot.Core.Events.Common;
using CrackersBot.Core.Events.Discord;
using Discord;
using Discord.WebSocket;

namespace CrackersBot.Web.Services;

public partial class BotCore
{
    private bool _hasConnected = false;
    private DateTimeOffset _lastDisconnectTime = DateTimeOffset.MinValue;
    private Exception? _lastDisconnectException;

    private void SetupEventListeners()
    {
        _discordSocketClient.Ready += OnClientReady;
        _discordSocketClient.Disconnected += OnBotDisconnected;
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
        await TriggerEvent(BotStartedEvent.EVENT_ID);

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

    private async Task OnBotDisconnected(Exception disconnectEx)
    {
        try
        {
            Logger.LogWarning(disconnectEx, "Bot disconnected");
            _lastDisconnectTime = DateTimeOffset.Now;
            _lastDisconnectException = disconnectEx;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during OnBotDisconnected");
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
                await _commandManager.RunCommandActions(commandHandler, this, command);
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
                await _commandManager.RunCommandActions(commandHandler, this, command);
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
                await _commandManager.RunCommandActions(commandHandler, this, command);
            }
        }
    }

    private async Task OnPresenceUpdated(SocketUser user, SocketPresence oldPresence, SocketPresence newPresence)
    {
        LogEventTriggered("OnPresenceUpdated");
        var eventId = UserPresenceUpdatedEvent.EVENT_ID;

        var wasStreaming = oldPresence?.Activities?.Any(a => a?.Type == ActivityType.Streaming) ?? false;
        var isStreaming = newPresence?.Activities?.Any(a => a?.Type == ActivityType.Streaming) ?? false;

        var startedStreaming = !wasStreaming && isStreaming;
        var stoppedStreaming = wasStreaming && !isStreaming;

        async Task<RunContext> ContextBuilder(RunContext context, Core.Events.EventHandler instance)
        {
            var socketGuild = _discordSocketClient.GetGuild(instance.Guild!.GuildId);

            return (context ?? new RunContext())
                .WithDiscordGuild(socketGuild)
                .WithDiscordUser(user);
        }

        await TriggerEvent(eventId, ContextBuilder);

        foreach (var (guildId, guildConfig) in Guilds)
        {
            var socketGuild = _discordSocketClient.GetGuild(guildId);

            if (socketGuild.Users.Any(u => u.Id == user.Id))
            {
                var context = new RunContext()
                    .WithDiscordGuild(socketGuild)
                    .WithDiscordUser(user);

                if (startedStreaming)
                {
                    eventId = UserStartedStreamingEvent.EVENT_ID;

                    if (guildConfig.AuditSettings.UserStartedStreaming)
                    {
                        await AuditHelpers.SendAuditMessageAsync(
                            this,
                            guildConfig.GuildId,
                            guildConfig.AuditSettings.AuditChannelId,
                            AuditHelpers.GetUserStartedStreamingMessage(user)
                        );
                    }

                    foreach (var instance in guildConfig.EventHandlers.Where(e => e.EventId == eventId && e.Enabled))
                    {
                        await _eventManager.Handle(instance, context);
                    }
                }

                if (stoppedStreaming)
                {
                    eventId = UserStoppedStreamingEvent.EVENT_ID;

                    if (guildConfig.AuditSettings.UserStoppedStreaming)
                    {
                        await AuditHelpers.SendAuditMessageAsync(
                            this,
                            guildConfig.GuildId,
                            guildConfig.AuditSettings.AuditChannelId,
                            AuditHelpers.GetUserStoppedStreamingMessage(user)
                        );
                    }


                    foreach (var instance in guildConfig.EventHandlers.Where(e => e.EventId == eventId && e.Enabled))
                    {
                        await _eventManager.Handle(instance, context);
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
        var eventId = MessageReceivedEvent.EVENT_ID;

        if (message.Channel is SocketTextChannel textChannel)
        {
            var context = new RunContext()
                .WithDiscordMessage(message);

            var guildId = textChannel.Guild.Id;

            if (Guilds.TryGetValue(guildId, out var guild))
            {
                foreach (var instance in guild.EventHandlers.Where(e => e.EventId == eventId && e.Enabled))
                {
                    await _eventManager.Handle(instance, context);
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
        var eventId = MessageUpdatedEvent.EVENT_ID;

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
                            await _eventManager.Handle(instance, context);
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
        var eventId = MessageDeletedEvent.EVENT_ID;

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
                        await _eventManager.Handle(instance, context);
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

        var eventId = UserJoinedEvent.EVENT_ID;

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
                await _eventManager.Handle(instance, context);
            }
        }
    }

    private async Task OnUserLeft(SocketGuild socketGuild, SocketUser user)
    {
        LogEventTriggered("OnUserLeft");
        Logger.LogDebug($"User left Guild: {socketGuild.Name} ({socketGuild.Id}). User: {user.Username} ({user.Id})");

        var eventId = UserLeftEvent.EVENT_ID;

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
                await _eventManager.Handle(instance, context);
            }
        }
    }
}