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
            return _repository.CreateAsync(context.Message);
        }

        public Task Consume(ConsumeContext<LockDeleted> context)
        {
            return _repository.CreateAsync(context.Message);
        }

        public Task Consume(ConsumeContext<LockStateChanged> context)
        {
            return _repository.CreateAsync(context.Message);
        }

        public Task Consume(ConsumeContext<TagLinked> context)
        {
            return _repository.CreateAsync(context.Message);
        }

        public Task Consume(ConsumeContext<TagUnlinked> context)
        {
            return _repository.CreateAsync(context.Message);
        }
    }
}
