namespace CrackersBot.Web.Services.Automation.Variables
{
    public static class DefaultVariableProcessor
    {
        public static string GetValue(string token, Dictionary<string, object> context)
        {
            if (!context.ContainsKey(token) || context[token] is null)
            {
                return String.Empty;
            }

            return context[token].ToString() ?? String.Empty;
        }
    }
}