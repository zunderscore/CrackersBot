namespace CrackersBot.Core.Variables.Discord
{
    [VariableToken(CommonNames.IS_WEBHOOK)]
    [VariableDescription("Whether or not a user is a webhook user")]
    public class IsWebhookVariable(IBotCore bot) : VariableBase(bot) { }
}