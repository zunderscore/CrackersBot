namespace CrackersBot.Core.Actions
{
    public record ActionDefinition(
        string ActionId,
        Dictionary<string, string>? Parameters = null
    );
}