using System.Reflection;
using CrackersBot.Core.Parameters;
using Discord.WebSocket;

namespace CrackersBot.Core.Actions;

public abstract class ActionBase(
    string id,
    string name,
    string description,
    BotServiceProvider botServices
) : IAction
{
    public BotServiceProvider BotServices { get; } = botServices;

    public abstract Dictionary<string, IParameterType> ActionParameters { get; }

    public string Id { get; } = id ?? String.Empty;
    public string Name { get; } = name ?? String.Empty;
    public string Description { get; } = description ?? String.Empty;

    public bool DoPreRunCheck(Dictionary<string, string> rawParams)
        => BotServices.GetBotService<DiscordSocketClient>() is not null && ValidateParameters(rawParams);

    public abstract Task Run(Dictionary<string, object> parameters, RunContext context);

    public virtual bool ValidateParameters(Dictionary<string, string> rawParams)
        => ParameterHelpers.ValidateParameters(ActionParameters, rawParams);
}