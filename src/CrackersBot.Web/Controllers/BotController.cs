using CrackersBot.Core;
using CrackersBot.Core.Actions;
using CrackersBot.Core.Actions.Discord;
using Microsoft.AspNetCore.Mvc;

namespace CrackersBot.Web.Controllers;

[Route("api/botcontrol/[action]")]
[ApiController]
public class BotController(
    IBotCore bot,
    BotServiceProvider botServiceProvider
) : ControllerBase
{
    private readonly IBotCore _bot = bot;
    private readonly IActionManager _actionManager = botServiceProvider.GetBotService<IActionManager>();

    [HttpGet]
    public async Task<IActionResult> ReloadGuildConfigs()
    {
        await _bot.LoadGuildConfigsAsync();
        await _actionManager.RunActions([
            new(SendChannelMessageAction.ACTION_ID, new() {
                { CommonNames.DISCORD_CHANNEL_ID, "1008199559400398910" },
                { CommonNames.MESSAGE_TEXT, "Guilds reloaded via API" }
            })
        ], new());
        return Ok(_bot.Guilds.Count);
    }
}