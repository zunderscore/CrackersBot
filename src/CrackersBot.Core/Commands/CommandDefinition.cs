using CrackersBot.Core.Actions;
using Discord;

namespace CrackersBot.Core.Commands;

public class CommandDefinition(
    string name,
    CommandType type,
    IEnumerable<ActionInstance> actions,
    CommandOutput output,
    string? description = null,
    bool enabled = true,
    bool isNsfw = false
)
{
    public string Name { get; set; } = name;
    public string? Description { get; set; } = description;
    public CommandType Type { get; set; } = type;
    public bool Enabled { get; set; } = enabled;
    public bool IsNsfw { get; set; } = isNsfw;
    public IEnumerable<ActionInstance> Actions { get; set; } = actions;
    public CommandOutput Output { get; set; } = output;

    public ApplicationCommandProperties BuildCommand() => Type switch
    {
        CommandType.UserCommand => new UserCommandBuilder()
            .WithName(Name)
            .Build(),

        CommandType.MessageCommand => new MessageCommandBuilder()
            .WithName(Name)
            .Build(),

        _ => new SlashCommandBuilder()
            .WithName(Name)
            .WithDescription(Description ?? String.Empty)
            .Build(),
    };
}