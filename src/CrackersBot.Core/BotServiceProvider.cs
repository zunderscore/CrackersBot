using System.Diagnostics.CodeAnalysis;
using CrackersBot.Core.Actions;
using CrackersBot.Core.Commands;
using CrackersBot.Core.Events;
using CrackersBot.Core.Filters;
using CrackersBot.Core.Variables;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CrackersBot.Core;

public class BotServiceProvider
(
    IServiceProvider serviceProvider
)
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    internal const string BOT_SERVICE_KEY = "CrackersBot:Core:BotService";

    public TService GetBotService<TService>()
        where TService : class
    {
        return _serviceProvider.GetRequiredKeyedService<TService>(BOT_SERVICE_KEY);
    }

    public ILogger<T> GetLogger<T>()
        where T : class
    {
        return _serviceProvider.GetRequiredService<ILogger<T>>();
    }
}

public static class ServiceBuilderExtensions
{
    public static IServiceCollection AddBotSingleton<TService>(
        this IServiceCollection services,
        TService implementationInstance)
        where TService : class
        => services.AddKeyedSingleton(BotServiceProvider.BOT_SERVICE_KEY, implementationInstance);

    public static IServiceCollection AddBotSingleton<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TService>(this IServiceCollection services)
        where TService : class
        => services.AddKeyedSingleton<TService>(BotServiceProvider.BOT_SERVICE_KEY);

    public static IServiceCollection AddBotSingleton<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
        => services.AddKeyedSingleton<TService, TImplementation>(BotServiceProvider.BOT_SERVICE_KEY);

    public static IServiceCollection AddCrackersBotCore<TBot>(this IServiceCollection services)
        where TBot : class, IBotCore
    {
        ArgumentNullException.ThrowIfNull(services);

        // First, add the BotServiceProvider
        services.AddSingleton<BotServiceProvider>();

        // Add the basic services
        services.AddBotSingleton<IActionManager, ActionManager>();
        services.AddBotSingleton<IEventManager, EventManager>();
        services.AddBotSingleton<IFilterManager, FilterManager>();
        services.AddBotSingleton<IVariableManager, VariableManager>();
        services.AddBotSingleton<ICommandManager, CommandManager>();

        // Add the Discord socket client
        services.AddBotSingleton(new DiscordSocketClient(new DiscordSocketConfig()
        {
            MessageCacheSize = 50,
            GatewayIntents = GatewayIntents.All,
            AlwaysDownloadUsers = true
        }));

        // FInally, add the bot
        return services.AddSingleton<IBotCore, TBot>();
    }
}