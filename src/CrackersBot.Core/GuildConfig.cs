using CrackersBot.Core.Auditing;
using CrackersBot.Core.Commands;
using Newtonsoft.Json;

namespace CrackersBot.Core;

public record GuildConfig(
    [property: JsonProperty("id")] string Id,
    ulong GuildId,
    AuditSettings AuditSettings,
    List<Events.EventHandler> EventHandlers,
    List<CommandDefinition> Commands
);