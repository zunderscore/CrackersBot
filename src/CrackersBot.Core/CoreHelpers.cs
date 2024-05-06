using CrackersBot.Core.Actions;
using CrackersBot.Core.Events;
using CrackersBot.Core.Filters;
using CrackersBot.Core.Variables;
using System.Reflection;

namespace CrackersBot.Core
{
    public static class CoreHelpers
    {
        public static IEnumerable<IAction> GetAllCoreActions()
        {
            var actions = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetCustomAttribute<ActionIdAttribute>() is not null)
                .Select(t => (IAction)Activator.CreateInstance(t)!);

            return actions;
        }

        public static IEnumerable<IVariable> GetAllCoreVariables()
        {
            var variables = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetCustomAttribute<VariableTokenAttribute>() is not null)
                .Select(t => (IVariable)Activator.CreateInstance(t)!); ;

            return variables;
        }

        public static IEnumerable<IEventHandler> GetAllCoreEventHandlers()
        {
            var eventHandlers = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetCustomAttribute<EventIdAttribute>() is not null)
                .Select(t => (IEventHandler)Activator.CreateInstance(t)!); ;

            return eventHandlers;
        }

        public static IEnumerable<IFilter> GetAllCoreFilters()
        {
            var filters = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetCustomAttribute<FilterIdAttribute>() is not null)
                .Select(t => (IFilter)Activator.CreateInstance(t)!); ;

            return filters;
        }
    }
}