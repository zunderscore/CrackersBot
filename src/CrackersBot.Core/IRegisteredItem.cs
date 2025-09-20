namespace CrackersBot.Core;

public interface IRegisteredItem
{
    string Id { get; }
    string Name { get; }
    string Description { get; }
}