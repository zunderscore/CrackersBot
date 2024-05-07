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
                Metadata.TryAdd(CommonNames.DISCORD_USER_AVATAR_URL, user.GetAvatarUrl());
                Metadata.TryAdd(CommonNames.IS_BOT, user.IsBot.ToString());
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
            }

            return this;
        }

        public RunContext WithDiscordMessage(IMessage? message)
        {
            if (message is not null)
            {
                Metadata.TryAdd(CommonNames.DISCORD_MESSAGE_ID, message.Id.ToString());
                Metadata.TryAdd(CommonNames.MESSAGE_TEXT, message.ToString() ?? String.Empty);
            }

            return this;
        }

        public RunContext WithPreviousMessageText(string? messageText)
        {
            Metadata.TryAdd(CommonNames.PREVIOUS_MESSAGE_TEXT, messageText ?? String.Empty);

            return this;
        }

        public RunContext WithDiscordPresense(IPresence? presence)
        {
            if (presence is not null)
            {
                var isStreaming = presence.Activities?.Any(a => a?.Type == ActivityType.Streaming) ?? false;

                Metadata.TryAdd(CommonNames.IS_STREAMING, isStreaming.ToString());

                if (isStreaming)
                {
                    var streamingActivity = presence.Activities!.First(a => a.Type == ActivityType.Streaming) as StreamingGame;

                    Metadata.TryAdd(CommonNames.STREAM_URL, streamingActivity!.Url.ToString());
                    Metadata.TryAdd(CommonNames.STREAM_TITLE, streamingActivity!.Details.ToString());
                    Metadata.TryAdd(CommonNames.GAME_NAME, streamingActivity!.Name.ToString());
                }
            }

            return this;
        }
    }
}