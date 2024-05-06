using CrackersBot.Core;

namespace CrackersBot.Web.Services
{
    public class BotService(IBotCore bot) : IHostedService
    {
        private readonly BotCore _bot = (BotCore)bot;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _bot.StartBotCoreAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _bot.StopBotCoreAsync();
        }
    }
}