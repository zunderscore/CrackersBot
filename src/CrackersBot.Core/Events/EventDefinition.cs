namespace CrackersBot.Core.Events;

public record EventDefinition(
    string Id,
    string Name,
    string Description
) : IRegisteredItem;