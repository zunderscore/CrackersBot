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
    }
}