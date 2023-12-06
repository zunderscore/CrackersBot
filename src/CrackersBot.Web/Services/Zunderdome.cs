using CrackersBot.Core;
using CrackersBot.Core.Actions;
using CrackersBot.Core.Filters;
using CrackersBot.Web.Services.Automation.Actions;
using CrackersBot.Web.Services.Automation.Events;
using CrackersBot.Web.Services.Automation.Filters;

namespace CrackersBot.Web.Services
{
    public static class Zunderdome
    {
        // Guild IDs
        public const ulong ZUNDERDOME_DISCORD_GUILD_ID = 696408571327545397;

        // Channel IDs
        public const ulong BOT_AUDIT_DISCORD_CHANNEL_ID = 1008199559400398910;
        public const ulong MOD_CENTRAL_DISCORD_CHANNEL_ID = 801221297928667147;

        // User IDs
        public const ulong ZUNDERBOT_DISCORD_USER_ID = 792607555489890325;
        public const ulong ZUNDERSCORE_DISCORD_USER_ID = 209052262671187968;

        public const string BRAND_COLOR = "#0066FF";

        #region Bot Started Event Handlers

        public static List<BotStartedEventHandler> BotStartedEventHandlers =>
        [
            BotStarted
        ];

        private static BotStartedEventHandler BotStarted
        {
            get
            {
                var sendStartupMessageParameters = new Dictionary<string, object>()
                {
                    { CommonNames.DISCORD_CHANNEL_ID, BOT_AUDIT_DISCORD_CHANNEL_ID },
                    { CommonNames.DISCORD_EMBED, new Automation.Embed()
                        {
                            Title = "zunderbot is online!",
                            Color = BRAND_COLOR,
                            Fields =
                            [
                                new("Actions", $"Actions registered: ${CommonNames.REGISTERED_ACTION_COUNT}"),
                                new("Variables", $"Variables registered: ${CommonNames.REGISTERED_VARIABLE_COUNT}")
                            ]
                        }
                    }
                };

                var actions = new Dictionary<string, Dictionary<string, object>>()
                {
                    { BotCore.GetActionId(typeof(SendDiscordChannelMessageAction)), sendStartupMessageParameters }
                };

                return new BotStartedEventHandler(actions);
            }
        }

        #endregion

        #region Message Received Event Handlers

        public static List<MessageReceivedEventHandler> MessageReceivedEventHandlers =>
        [
            LoafReaction
        ];

        private static MessageReceivedEventHandler LoafReaction
        {
            get
            {
                var parameters = new Dictionary<string, object>()
                {
                    { CommonNames.DISCORD_GUILD_ID, ZUNDERDOME_DISCORD_GUILD_ID },
                    { CommonNames.DISCORD_EMOTE_NAME, "Loaf" }
                };

                var action = new Dictionary<string, Dictionary<string, object>>()
                {
                    { BotCore.GetActionId(typeof(ReactToDiscordMessageAction)), parameters }
                };
                var messageFilter = new DiscordMessageFilter("Loaf");

                return new MessageReceivedEventHandler(action, messageFilter, FilterMode.All);
            }
        }

        #endregion

        #region Message Deleted Event Handlers

        public static List<MessageDeletedEventHandler> MessageDeletedEventHandlers =>
        [
            DefaultMessageDeletedEventHandler
        ];

        private static MessageDeletedEventHandler DefaultMessageDeletedEventHandler
        {
            get
            {
                var parameters = new Dictionary<string, object>()
                {
                    { CommonNames.DISCORD_CHANNEL_ID, BOT_AUDIT_DISCORD_CHANNEL_ID },
                    { CommonNames.DISCORD_EMBED, new Automation.Embed()
                        {
                            Title = "Message Deleted",
                            Color = BRAND_COLOR,
                            Fields =
                            [
                                new("Channel", $"<#${CommonNames.DISCORD_CHANNEL_ID}>"),
                                new("Original Author", $"<@${CommonNames.DISCORD_AUTHOR_ID}>"),
                                new("Original Message", $"${CommonNames.MESSAGE_TEXT}", isInline: true)
                            ]
                        }
                    }
                };

                var action = new Dictionary<string, Dictionary<string, object>>()
                {
                    { BotCore.GetActionId(typeof(SendDiscordChannelMessageAction)), parameters }
                };

                var channelFilter = new DiscordChannelFilter(FilterInclusionType.Exclude, new List<ulong>() {
                    BOT_AUDIT_DISCORD_CHANNEL_ID,
                    MOD_CENTRAL_DISCORD_CHANNEL_ID
                });

                return new MessageDeletedEventHandler(action, channelFilter, FilterMode.All);
            }
        }

        #endregion

        #region User Leave Event Handlers

        public static List<UserLeaveEventHandler> UserLeaveEventHandlers =>
        [
            DefaultUserLeaveEventHandler
        ];

        private static UserLeaveEventHandler DefaultUserLeaveEventHandler
        {
            get
            {
                var parameters = new Dictionary<string, object>()
                {
                    { CommonNames.DISCORD_CHANNEL_ID, BOT_AUDIT_DISCORD_CHANNEL_ID },
                    { CommonNames.DISCORD_EMBED, new Automation.Embed()
                        {
                            Title = "User Has Left",
                            Color = BRAND_COLOR,
                            Fields =
                            [
                                new("User", $"<@${CommonNames.DISCORD_USER_ID}>")
                            ]
                        }
                    }
                };

                var action = new Dictionary<string, Dictionary<string, object>>()
                {
                    { BotCore.GetActionId(typeof(SendDiscordChannelMessageAction)), parameters }
                };

                return new UserLeaveEventHandler(action);
            }
        }

        #endregion
    }
}