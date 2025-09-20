namespace CrackersBot.Core.Variables.Discord;

public class IsWebhookVariable(BotServiceProvider botServices)
    : VariableBase(
        CommonNames.IS_WEBHOOK,
        "Is Webhook?",
        "Whether or not a user is a webhook user",
        botServices
    );