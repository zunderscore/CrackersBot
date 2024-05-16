using CrackersBot.Core.Actions.Discord;
using Discord;
using Microsoft.Extensions.Logging;

namespace CrackersBot.Core.Auditing
{
    public static class AuditHelpers
    {
        public static async Task SendAuditMessageAsync(
            IBotCore bot,
            ulong guildId,
            ulong? channelId,
            Embed? embed
        )
        {
            if (!channelId.HasValue || embed is null) return;

            try
            {
                var channel = bot.DiscordClient.GetGuild(guildId).GetTextChannel(channelId.Value);
                await channel.SendMessageAsync(embed: embed);
            }
            catch (Exception ex)
            {
                bot.Logger.LogError(ex, "Unable to send audit message");
            }
        }

        public static Embed GetUserJoinedMessage(IBotCore bot, IGuildUser user)
        {
            var timeSinceAccountCreated = DateTimeOffset.UtcNow - user.CreatedAt.UtcDateTime;
            var embed = new EmbedInstance()
            {
                Title = "User Has Joined",
                AuthorName = user.GlobalName,
                AuthorIconUrl = user.GetDisplayAvatarUrl(),
                Description = user.JoinedAt.HasValue
                    ? $"<@{user.Id}> joined the server at <t:{user.JoinedAt.Value.ToUnixTimeSeconds()}:F>. Account was created {timeSinceAccountCreated.ToHumanReadableString()} ago."
                    : $"<@{user.Id}> joined the server. Account was created {timeSinceAccountCreated.ToHumanReadableString()} ago."
            };

            return embed.BuildDiscordEmbed(bot, new());
        }

        public static Embed GetUserLeftMessage(IBotCore bot, IUser user)
        {
            var embed = new EmbedInstance()
            {
                Title = "User Has Left",
                AuthorName = user.GlobalName,
                AuthorIconUrl = user.GetDisplayAvatarUrl(),
                Description = user is IGuildUser guildUser && guildUser.JoinedAt.HasValue
                    ? $"<@{guildUser.Id}> left the server. They originally joined on <t:{guildUser.JoinedAt.Value.ToUnixTimeSeconds()}:F>."
                    : $"<@{user.Id}> left the server."
            };

            return embed.BuildDiscordEmbed(bot, new());
        }

        public static Embed? GetUserStartedStreamingMessage(IBotCore bot, IUser user)
        {
            if (user is IGuildUser guildUser)
            {
                var streamingActivity = user.Activities.First(a => a.Type == ActivityType.Streaming) as StreamingGame;
                var embed = new EmbedInstance()
                {
                    Title = "User Started Streaming",
                    AuthorName = user.GlobalName,
                    AuthorIconUrl = user.GetDisplayAvatarUrl(),
                    Fields = [
                        new("Stream URL", String.IsNullOrWhiteSpace(streamingActivity?.Url) ? "[unknown]" : streamingActivity.Url),
                        new("Stream Title", String.IsNullOrWhiteSpace(streamingActivity?.Details) ? "[none]" : streamingActivity.Details),
                    ]
                };

                return embed.BuildDiscordEmbed(bot, new());
            }

            return null;
        }

        public static Embed? GetUserStoppedStreamingMessage(IBotCore bot, IUser user)
        {
            var embed = new EmbedInstance()
            {
                Title = "User Stopped Streaming",
                AuthorName = user.GlobalName,
                AuthorIconUrl = user.GetDisplayAvatarUrl()
            };

            return embed.BuildDiscordEmbed(bot, new());
        }

        public static Embed? GetMessageUpdatedMessage(IBotCore bot, IMessage message, string originalMessage)
        {
            var updatedMessage = message.Content;
            var embed = new EmbedInstance()
            {
                Title = "Message Updated",
                AuthorName = message.Author.GlobalName,
                AuthorIconUrl = message.Author.GetDisplayAvatarUrl(),
                Fields = [
                    new("Channel", $"<#{message.Channel.Id}>"),
                    new("Original Message", String.IsNullOrWhiteSpace(originalMessage) ? "[empty]" : originalMessage),
                    new("Updated Message", String.IsNullOrWhiteSpace(updatedMessage) ? "[empty]" : updatedMessage)
                ]
            };

            return embed.BuildDiscordEmbed(bot, new());
        }

        public static Embed? GetMessageDeletedMessage(IBotCore bot, IMessage message)
        {
            var originalMessage = message.Content;
            var embed = new EmbedInstance()
            {
                Title = "Message Deleted",
                AuthorName = message.Author.GlobalName,
                AuthorIconUrl = message.Author.GetDisplayAvatarUrl(),
                Fields = [
                    new("Channel", $"<#{message.Channel.Id}>"),
                    new("Original Message", String.IsNullOrWhiteSpace(originalMessage) ? "[empty]" : originalMessage)
                ]
            };

            return embed.BuildDiscordEmbed(bot, new());
        }
    }
}