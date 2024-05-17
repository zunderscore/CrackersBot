using CrackersBot.Core.Actions;
using CrackersBot.Core.Filters;

namespace CrackersBot.Core.Events
{
    public interface IEventHandler : IBotConsumer, IRegisteredItem
    {
        public virtual Task Handle(
            EventHandlerInstance instance,
            RunContext context
        ) => Handle(instance.Actions, context, instance.Filters, instance.FilterMode);

        Task Handle(
            IEnumerable<ActionInstance> actions,
            RunContext context,
            IEnumerable<FilterInstance>? filters = null,
            FilterMode filterMode = FilterMode.All
        );

        Task RunActions(
            IEnumerable<ActionInstance> actions,
            RunContext context
        );
    }
}