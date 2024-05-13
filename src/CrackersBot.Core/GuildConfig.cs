using CrackersBot.Core.Auditing;
using CrackersBot.Core.Commands;
using CrackersBot.Core.Events;
using Newtonsoft.Json;

namespace CrackersBot.Core
{
    public record GuildConfig(
        [property: JsonProperty("id")] string Id,
        ulong GuildId,
        AuditSettings AuditSettings,
        List<EventHandlerInstance> EventHandlers,
        List<CommandDefinition> Commands
    );
}