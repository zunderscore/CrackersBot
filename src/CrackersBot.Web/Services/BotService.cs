using CrackersBot.Core;

namespace CrackersBot.Web.Services
{
    public class BotService : IHostedService
    {
        private BotCore _bot;

        public BotService(IBotCore bot)
        {
            _bot = (BotCore)bot;
        }

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