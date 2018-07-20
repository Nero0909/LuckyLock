using System.Threading.Tasks;
using Contracts.Events;
using EventAggregator.Entities;
using EventAggregator.Repository;
using MassTransit;
using Newtonsoft.Json;

namespace EventAggregator.EventConsuming.Consumers
{
    public class LocksActivityConsumer :
        IConsumer<LockCreated>,
        IConsumer<LockDeleted>,
        IConsumer<LockStateChanged>,
        IConsumer<TagLinked>,
        IConsumer<TagUnlinked>
    {
        private readonly ILocksActivityRepository _repository;

        public LocksActivityConsumer(ILocksActivityRepository repository)
        {
            _repository = repository;
        }

        public Task Consume(ConsumeContext<LockCreated> context)
        {
            return Consume(context.Message);
        }

        public Task Consume(ConsumeContext<LockDeleted> context)
        {
            return Consume(context.Message);
        }

        public Task Consume(ConsumeContext<LockStateChanged> context)
        {
            return Consume(context.Message);
        }

        public Task Consume(ConsumeContext<TagLinked> context)
        {
            return Consume(context.Message);
        }

        public Task Consume(ConsumeContext<TagUnlinked> context)
        {
            return Consume(context.Message);
        }

        private Task Consume<T>(T @event) where T : BaseLockMessage
        {
            var serializedEvent = SerializeEvent(@event);
            return _repository.CreateAsync(serializedEvent);
        }

        private SerializedEvent SerializeEvent<T>(T @event) where T : BaseLockMessage
        {
            return new SerializedEvent
            {
                Id = @event.EventId,
                AggregateId = @event.LockId,
                CreatedDate = @event.CreatedDate,
                EventType = @event.GetType().Name,
                UserId = @event.UserId,
                Data = JsonConvert.SerializeObject(@event)
            };
        }
    }
}
