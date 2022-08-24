namespace CrackersBot.Core.Actions
{
    public interface IAction
    {
        Dictionary<string, object> ConvertRawParameters(Dictionary<string, string> rawParams);

        bool ValidateParameters(Dictionary<string, object> parameters);

        Task Run(IBotCore bot, Dictionary<string, object> parameters, Dictionary<string, object> context);
    }
}