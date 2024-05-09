using CrackersBot.Core;
using CrackersBot.Core.Actions;
using CrackersBot.Core.Actions.Discord;
using Microsoft.AspNetCore.Mvc;

namespace CrackersBot.Web.Controllers
{
    [Route("api/botcontrol/[action]")]
    [ApiController]
    public class BotController(IBotCore bot) : ControllerBase
    {
        private readonly IBotCore _bot = bot;

        [HttpGet]
        public async Task<IActionResult> ReloadGuildConfigs()
        {
            await _bot.LoadGuildConfigsAsync();
            await ActionRunner.RunActions(_bot, [
                new(SendChannelMessageAction.ACTION_ID, new() {
                    { CommonNames.DISCORD_CHANNEL_ID, "1008199559400398910" },
                    { CommonNames.MESSAGE_TEXT, "Guilds reloaded via API" }
                })
            ], new());
            return Ok(_bot.Guilds.Count);
        }
    }
}