namespace CrackersBot.Core.Variables.Shared
{
    [VariableToken(CommonNames.MESSAGE_TEXT)]
    [VariableDescription("The message text contents")]
    public class MessageTextVariable(IBotCore bot) : VariableBase(bot) { }
}