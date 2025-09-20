using CrackersBot.Core.Actions;
using CrackersBot.Core.Actions.Common;
using CrackersBot.Core.Actions.Discord;
using CrackersBot.Core.Events;
using CrackersBot.Core.Events.Common;
using CrackersBot.Core.Events.Discord;
using CrackersBot.Core.Filters;
using CrackersBot.Core.Filters.Common;
using CrackersBot.Core.Filters.Discord;
using CrackersBot.Core.Variables;
using CrackersBot.Core.Variables.Bot;
using CrackersBot.Core.Variables.Common;
using CrackersBot.Core.Variables.Discord;

namespace CrackersBot.Core;

public static class CoreHelpers
{
    private static object? CreateInstanceWithBotServices(this Type type, BotServiceProvider botServices)
    {
        return Activator.CreateInstance(type, botServices);
    }

    public static void RegisterCoreActions(BotServiceProvider botServices)
    {
        List<Type> coreActions =
        [
            // Base actions
            typeof(DelayAction),

            // Discord actions
            typeof(AddUserRoleAction),
            typeof(ClearChannelAction),
            typeof(ReactToMessageAction),
            typeof(RemoveUserRoleAction),
            typeof(SendChannelMessageAction),
            typeof(SendDirectMessageAction)
        ];

        var actionManager = botServices.GetBotService<IActionManager>();

        foreach (var actionType in coreActions)
        {
            actionManager.RegisterAction((IAction)actionType.CreateInstanceWithBotServices(botServices)!);
        }
    }

    public static void RegisterCoreEvents(BotServiceProvider botServices)
    {
        List<EventDefinition> coreEvents = [
            // Base events
            new BotStartedEvent(),

            // Discord events
            new MessageDeletedEvent(),
            new MessageDeletedEvent(),
            new MessageReceivedEvent(),
            new MessageUpdatedEvent(),
            new UserJoinedEvent(),
            new UserLeftEvent(),
            new UserPresenceUpdatedEvent(),
            new UserStartedStreamingEvent(),
            new UserStoppedStreamingEvent(),
        ];


        var eventManager = botServices.GetBotService<IEventManager>();

        foreach (var eventDef in coreEvents)
        {
            eventManager.RegisterEvent(eventDef);
        }
    }

    public static void RegisterCoreFilters(BotServiceProvider botServices)
    {
        List<Type> coreFilters =
        [
            // Base filters
            typeof(BotFilter),
            typeof(MessageTextFilter),

            // Discord filters
            typeof(ChannelFilter),
            typeof(UserFilter),
        ];

        var filterManager = botServices.GetBotService<IFilterManager>();

        foreach (var filterType in coreFilters)
        {
            filterManager.RegisterFilter((IFilter)filterType.CreateInstanceWithBotServices(botServices)!);
        }
    }

    public static void RegisterCoreVariables(BotServiceProvider botServices)
    {
        List<Type> coreVariables =
        [
            // Bot variables
            typeof(RegisteredActionCountVariable),
            typeof(RegisteredEventCountVariable),
            typeof(RegisteredFilterCountVariable),
            typeof(RegisteredVariableCountVariable),

            // Discord variables
            typeof(ChannelIdVariable),
            typeof(ChannelNameVariable),
            typeof(ChannelTopicVariable),
            typeof(GuildIdVariable),
            typeof(GuildNameVariable),
            typeof(IsNsfwVariable),
            typeof(IsWebhookVariable),
            typeof(MessageIdVariable),
            typeof(TargetMessageIdVariable),
            typeof(TargetUserDisplayNameVariable),
            typeof(TargetUserGlobalDisplayNameVariable),
            typeof(TargetUserIdVariable),
            typeof(TargetUserNameVariable),
            typeof(UserAvatarUrlVariable),
            typeof(UserCustomStatusEmoteNameVariable),
            typeof(UserCustomStatusVariable),
            typeof(UserDisplayNameVariable),
            typeof(UserGlobalDisplayNameVariable),
            typeof(UserHasCustomStatusVariable),
            typeof(UserIdVariable),
            typeof(UserNameVariable),
            typeof(UserStatusVariable),
            typeof(VoiceChannelUserLimitVariable),

            // Shared variables
            typeof(GameNameVariable),
            typeof(IsBotVariable),
            typeof(IsStreamingVariable),
            typeof(MessageTextVariable),
            typeof(PreviousMessageTextVariable),
            typeof(StreamTitleVariable),
            typeof(StreamUrlVariable),
            typeof(TimeInMillisecondsVariable),
        ];

        var variableManager = botServices.GetBotService<IVariableManager>();

        foreach (var variableType in coreVariables)
        {
            variableManager.RegisterVariable((IVariable)variableType.CreateInstanceWithBotServices(botServices)!);
        }
    }
}