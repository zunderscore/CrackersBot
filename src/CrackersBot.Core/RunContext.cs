using Discord;

namespace CrackersBot.Core;

public class RunContext
{
    public Dictionary<string, string> Metadata { get; } = [];
}

public static class RunContextExtensions
{
    public static RunContext WithDiscordUser(this RunContext context, IUser? user)
    {
        if (user is not null)
        {
            context.Metadata.TryAdd(CommonNames.DISCORD_USER_ID, user.Id.ToString());
            context.Metadata.TryAdd(CommonNames.DISCORD_USER_NAME, user.Username);
            context.Metadata.TryAdd(CommonNames.DISCORD_TARGET_USER_GLOBAL_DISPLAY_NAME, user.GlobalName);
            context.Metadata.TryAdd(CommonNames.DISCORD_USER_STATUS, user.Status.ToString());
            context.Metadata.TryAdd(CommonNames.DISCORD_USER_AVATAR_URL, user.GetAvatarUrl());
            context.Metadata.TryAdd(CommonNames.IS_BOT, user.IsBot.ToString());
            context.Metadata.TryAdd(CommonNames.IS_WEBHOOK, user.IsWebhook.ToString());

            var isStreaming = user.Activities?.Any(a => a?.Type == ActivityType.Streaming) ?? false;

            context.Metadata.TryAdd(CommonNames.IS_STREAMING, isStreaming.ToString());

            if (isStreaming)
            {
                var streamingActivity = user.Activities!.First(a => a.Type == ActivityType.Streaming) as StreamingGame;

                context.Metadata.TryAdd(CommonNames.STREAM_URL, streamingActivity!.Url);
                context.Metadata.TryAdd(CommonNames.STREAM_TITLE, streamingActivity!.Details);
                context.Metadata.TryAdd(CommonNames.GAME_NAME, streamingActivity!.Name);
            }

            var hasCustomStatus = user.Activities?.Any(a => a?.Type == ActivityType.CustomStatus) ?? false;

            context.Metadata.TryAdd(CommonNames.DISCORD_USER_HAS_CUSTOM_STATUS, hasCustomStatus.ToString());

            if (hasCustomStatus)
            {
                var customStatusActivity = user.Activities!.First(a => a.Type == ActivityType.CustomStatus) as CustomStatusGame;

                context.Metadata.TryAdd(CommonNames.DISCORD_USER_CUSTOM_STATUS, customStatusActivity!.State?.ToString() ?? String.Empty);
                context.Metadata.TryAdd(CommonNames.DISCORD_USER_CUSTOM_STATUS_EMOTE_NAME, customStatusActivity!.Emote?.Name ?? String.Empty);
            }

            if (user is IGuildUser guildUser)
            {
                context.Metadata.TryAdd(CommonNames.DISCORD_USER_DISPLAY_NAME, guildUser.DisplayName);

                context.WithDiscordGuild(guildUser.Guild);
            }
        }

        return context;
    }

    public static RunContext WithDiscordGuild(this RunContext context, IGuild? guild)
    {
        if (guild is not null)
        {
            context.Metadata.TryAdd(CommonNames.DISCORD_GUILD_ID, guild.Id.ToString());
            context.Metadata.TryAdd(CommonNames.DISCORD_GUILD_NAME, guild.Name);
        }

        return context;
    }

    public static RunContext WithDiscordChannel(this RunContext context, IChannel? channel)
    {
        if (channel is not null)
        {
            context.Metadata.TryAdd(CommonNames.DISCORD_CHANNEL_ID, channel.Id.ToString());
            context.Metadata.TryAdd(CommonNames.DISCORD_CHANNEL_NAME, channel.Name);

            if (channel is IGuildChannel guildChannel)
            {
                context.WithDiscordGuild(guildChannel.Guild);
            }

            if (channel is ITextChannel textChannel)
            {
                context.Metadata.TryAdd(CommonNames.DISCORD_CHANNEL_TOPIC, textChannel.Topic ?? String.Empty);
                context.Metadata.TryAdd(CommonNames.IS_NSFW, textChannel.IsNsfw.ToString());
            }

            if (channel is IVoiceChannel voiceChannel)
            {
                context.Metadata.TryAdd(CommonNames.DISCORD_VOICE_CHANNEL_USER_LIMIT, voiceChannel.UserLimit?.ToString() ?? String.Empty);
            }
        }

        return context;
    }

    public static RunContext WithDiscordMessage(this RunContext context, IMessage? message)
    {
        if (message is not null)
        {
            context.Metadata.TryAdd(CommonNames.DISCORD_MESSAGE_ID, message.Id.ToString());
            context.Metadata.TryAdd(CommonNames.MESSAGE_TEXT, message.Content ?? String.Empty);

            if (message.Channel is not null)
            {
                context.WithDiscordChannel(message.Channel);
            }

            if (message.Author is not null)
            {
                context.WithDiscordUser(message.Author);
            }
        }

        return context;
    }

    public static RunContext WithPreviousMessageText(this RunContext context, string? messageText)
    {
        context.Metadata.TryAdd(CommonNames.PREVIOUS_MESSAGE_TEXT, messageText ?? String.Empty);

        return context;
    }

    public static RunContext WithDiscordTargetUser(this RunContext context, IUser? user)
    {
        if (user is not null)
        {
            context.Metadata.TryAdd(CommonNames.DISCORD_TARGET_USER_ID, user.Id.ToString());
            context.Metadata.TryAdd(CommonNames.DISCORD_TARGET_USER_NAME, user.Username);
            context.Metadata.TryAdd(CommonNames.DISCORD_TARGET_USER_GLOBAL_DISPLAY_NAME, user.GlobalName);

            if (user is IGuildUser guildUser)
            {
                context.Metadata.TryAdd(CommonNames.DISCORD_TARGET_USER_DISPLAY_NAME, guildUser.DisplayName);
            }
        }

        return context;
    }

    public static RunContext WithDiscordTargetMessage(this RunContext context, IMessage? message)
    {
        if (message is not null)
        {
            context.Metadata.TryAdd(CommonNames.DISCORD_TARGET_MESSAGE_ID, message.Id.ToString());
            context.Metadata.TryAdd(CommonNames.MESSAGE_TEXT, message.Content ?? String.Empty);

            if (message.Author is not null)
            {
                context.WithDiscordTargetUser(message.Author);
            }
        }

        return context;
    }
}