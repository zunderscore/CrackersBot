using Discord;

namespace CrackersBot.Core
{
    public class RunContext
    {
        public Dictionary<string, string> Metadata { get; } = [];

        public RunContext WithDiscordUser(IUser? user)
        {
            if (user is not null)
            {
                Metadata.TryAdd(CommonNames.DISCORD_USER_ID, user.Id.ToString());
                Metadata.TryAdd(CommonNames.DISCORD_USER_NAME, user.Username);
                Metadata.TryAdd(CommonNames.DISCORD_TARGET_USER_GLOBAL_DISPLAY_NAME, user.GlobalName);
                Metadata.TryAdd(CommonNames.DISCORD_USER_STATUS, user.Status.ToString());
                Metadata.TryAdd(CommonNames.DISCORD_USER_AVATAR_URL, user.GetAvatarUrl());
                Metadata.TryAdd(CommonNames.IS_BOT, user.IsBot.ToString());
                Metadata.TryAdd(CommonNames.IS_WEBHOOK, user.IsWebhook.ToString());

                var isStreaming = user.Activities?.Any(a => a?.Type == ActivityType.Streaming) ?? false;

                Metadata.TryAdd(CommonNames.IS_STREAMING, isStreaming.ToString());

                if (isStreaming)
                {
                    var streamingActivity = user.Activities!.First(a => a.Type == ActivityType.Streaming) as StreamingGame;

                    Metadata.TryAdd(CommonNames.STREAM_URL, streamingActivity!.Url);
                    Metadata.TryAdd(CommonNames.STREAM_TITLE, streamingActivity!.Details);
                    Metadata.TryAdd(CommonNames.GAME_NAME, streamingActivity!.Name);
                }

                var hasCustomStatus = user.Activities?.Any(a => a?.Type == ActivityType.CustomStatus) ?? false;

                Metadata.TryAdd(CommonNames.DISCORD_USER_HAS_CUSTOM_STATUS, hasCustomStatus.ToString());

                if (hasCustomStatus)
                {
                    var customStatusActivity = user.Activities!.First(a => a.Type == ActivityType.CustomStatus) as CustomStatusGame;

                    Metadata.TryAdd(CommonNames.DISCORD_USER_CUSTOM_STATUS, customStatusActivity!.State?.ToString() ?? String.Empty);
                    Metadata.TryAdd(CommonNames.DISCORD_USER_CUSTOM_STATUS_EMOTE_NAME, customStatusActivity!.Emote?.Name ?? String.Empty);
                }

                if (user is IGuildUser guildUser)
                {
                    Metadata.TryAdd(CommonNames.DISCORD_USER_DISPLAY_NAME, guildUser.DisplayName);

                    WithDiscordGuild(guildUser.Guild);
                }
            }

            return this;
        }

        public RunContext WithDiscordGuild(IGuild? guild)
        {
            if (guild is not null)
            {
                Metadata.TryAdd(CommonNames.DISCORD_GUILD_ID, guild.Id.ToString());
                Metadata.TryAdd(CommonNames.DISCORD_GUILD_NAME, guild.Name);
            }

            return this;
        }

        public RunContext WithDiscordChannel(IChannel? channel)
        {
            if (channel is not null)
            {
                Metadata.TryAdd(CommonNames.DISCORD_CHANNEL_ID, channel.Id.ToString());
                Metadata.TryAdd(CommonNames.DISCORD_CHANNEL_NAME, channel.Name);

                if (channel is IGuildChannel guildChannel)
                {
                    WithDiscordGuild(guildChannel.Guild);
                }

                if (channel is ITextChannel textChannel)
                {
                    Metadata.TryAdd(CommonNames.DISCORD_CHANNEL_TOPIC, textChannel.Topic ?? String.Empty);
                    Metadata.TryAdd(CommonNames.IS_NSFW, textChannel.IsNsfw.ToString());
                }

                if (channel is IVoiceChannel voiceChannel)
                {
                    Metadata.TryAdd(CommonNames.DISCORD_VOICE_CHANNEL_USER_LIMIT, voiceChannel.UserLimit?.ToString() ?? String.Empty);
                }
            }

            return this;
        }

        public RunContext WithDiscordMessage(IMessage? message)
        {
            if (message is not null)
            {
                Metadata.TryAdd(CommonNames.DISCORD_MESSAGE_ID, message.Id.ToString());
                Metadata.TryAdd(CommonNames.MESSAGE_TEXT, message.Content ?? String.Empty);

                if (message.Channel is not null)
                {
                    WithDiscordChannel(message.Channel);
                }

                if (message.Author is not null)
                {
                    WithDiscordUser(message.Author);
                }
            }

            return this;
        }

        public RunContext WithPreviousMessageText(string? messageText)
        {
            Metadata.TryAdd(CommonNames.PREVIOUS_MESSAGE_TEXT, messageText ?? String.Empty);

            return this;
        }

        public RunContext WithDiscordTargetUser(IUser? user)
        {
            if (user is not null)
            {
                Metadata.TryAdd(CommonNames.DISCORD_TARGET_USER_ID, user.Id.ToString());
                Metadata.TryAdd(CommonNames.DISCORD_TARGET_USER_NAME, user.Username);
                Metadata.TryAdd(CommonNames.DISCORD_TARGET_USER_GLOBAL_DISPLAY_NAME, user.GlobalName);

                if (user is IGuildUser guildUser)
                {
                    Metadata.TryAdd(CommonNames.DISCORD_TARGET_USER_DISPLAY_NAME, guildUser.DisplayName);
                }
            }

            return this;
        }

        public RunContext WithDiscordTargetMessage(IMessage? message)
        {
            if (message is not null)
            {
                Metadata.TryAdd(CommonNames.DISCORD_TARGET_MESSAGE_ID, message.Id.ToString());
                Metadata.TryAdd(CommonNames.MESSAGE_TEXT, message.Content ?? String.Empty);

                if (message.Author is not null)
                {
                    WithDiscordTargetUser(message.Author);
                }
            }

            return this;
        }
    }
}