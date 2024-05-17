using CrackersBot.Core.Actions;
using CrackersBot.Core.Actions.Discord;
using CrackersBot.Core.Events;
using CrackersBot.Core.Events.Discord;
using CrackersBot.Core.Filters;
using CrackersBot.Core.Filters.Discord;
using CrackersBot.Core.Variables;
using CrackersBot.Core.Variables.Bot;
using CrackersBot.Core.Variables.Discord;
using CrackersBot.Core.Variables.Shared;

namespace CrackersBot.Core
{
    public static class CoreHelpers
    {
        public static IEnumerable<IAction> GetAllCoreActions(IBotCore bot)
        {
            var actions = new List<IAction>(){
                // Basic actions
                new DelayAction(bot),

                // Discord actions
                new AddUserRoleAction(bot),
                new ClearChannelAction(bot),
                new ReactToMessageAction(bot),
                new RemoveUserRoleAction(bot),
                new SendChannelMessageAction(bot),
                new SendDirectMessageAction(bot),
            };

            return actions;
        }

        public static IEnumerable<IEventHandler> GetAllCoreEventHandlers(IBotCore bot)
        {
            var eventHandlers = new List<IEventHandler>() {
                // Base events
                new BotStartedEventHandler(bot),

                // Discord events
                new MessageDeletedEventHandler(bot),
                new MessageReceivedEventHandler(bot),
                new MessageUpdatedEventHandler(bot),
                new UserJoinedEventHandler(bot),
                new UserLeftEventHandler(bot),
                new UserPresenceUpdatedEventHandler(bot),
                new UserStartedStreamingEventHandler(bot),
                new UserStoppedStreamingEventHandler(bot),
            };

            return eventHandlers;
        }

        public static IEnumerable<IFilter> GetAllCoreFilters(IBotCore bot)
        {
            var filters = new List<IFilter>()
            {
                // Base filters
                new BotFilter(),
                new MessageTextFilter(),

                // Discord filters
                new ChannelFilter(),
                new UserFilter(),
            };

            return filters;
        }

        public static IEnumerable<IVariable> GetAllCoreVariables(IBotCore bot)
        {
            var variables = new List<IVariable>()
            {
                // Bot variables
                new RegisteredActionCountVariable(bot),
                new RegisteredEventHandlerCountVariable(bot),
                new RegisteredFilterCountVariable(bot),
                new RegisteredVariableCountVariable(bot),

                // Discord variables
                new ChannelIdVariable(bot),
                new ChannelNameVariable(bot),
                new ChannelTopicVariable(bot),
                new GuildIdVariable(bot),
                new GuildNameVariable(bot),
                new IsNsfwVariable(bot),
                new IsWebhookVariable(bot),
                new MessageIdVariable(bot),
                new TargetMessageIdVariable(bot),
                new TargetUserDisplayNameVariable(bot),
                new TargetUserGlobalDisplayNameVariable(bot),
                new TargetUserIdVariable(bot),
                new TargetUserNameVariable(bot),
                new UserAvatarUrlVariable(bot),
                new UserCustomStatusEmoteNameVariable(bot),
                new UserCustomStatusVariable(bot),
                new UserDisplayNameVariable(bot),
                new UserGlobalDisplayNameVariable(bot),
                new UserHasCustomStatusVariable(bot),
                new UserIdVariable(bot),
                new UserNameVariable(bot),
                new UserStatusVariable(bot),
                new VoiceChannelUserLimitVariable(bot),

                // Shared variables
                new GameNameVariable(bot),
                new IsBotVariable(bot),
                new IsStreamingVariable(bot),
                new MessageTextVariable(bot),
                new PreviousMessageTextVariable(bot),
                new StreamTitleVariable(bot),
                new StreamUrlVariable(bot),
                new TimeInMillisecondsVariable(bot),
            };

            return variables;
        }
    }
}