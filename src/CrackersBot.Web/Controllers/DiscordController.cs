using CrackersBot.Core;
using Microsoft.AspNetCore.Mvc;

namespace CrackersBot.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DiscordController : ControllerBase
    {
        private IBotCore _bot;

        public DiscordController(IBotCore bot)
        {
            _bot = bot;
        }

        [HttpGet]
        public async Task<IActionResult> RegisterCallback()
        {
            await Task.FromResult(() => 4);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> UnregisterCallback()
        {
            await Task.FromResult(() => 4);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Callback([FromBody]dynamic request)
        {
            await Task.FromResult(() => 4);
            return Ok();
        }
    }
}
