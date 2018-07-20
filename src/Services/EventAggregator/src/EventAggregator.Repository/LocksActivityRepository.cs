using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Events;
using Dapper;
using EventAggregator.Entities;
using EventAggregator.Repository.Serializer;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace EventAggregator.Repository
{
    public class LocksActivityRepository : ILocksActivityRepository
    {
        private readonly IConfiguration _config;
        private readonly IEventDeserializer _deserializer;

        public LocksActivityRepository(IConfiguration configuration, IEventDeserializer deserializer)
        {
            _config = configuration;
            _deserializer = deserializer;
        }

        private IDbConnection Connection => new SqliteConnection(_config.GetConnectionString("Events.db"));


        public async Task<SerializedEvent> CreateAsync(SerializedEvent @event)
        {
            using (var db = Connection)
            {
                await db.ExecuteAsync(
                    "INSERT INTO LocksActivity " +
                    $"({nameof(SerializedEvent.Id)}, {nameof(SerializedEvent.CreatedDate)}, {nameof(SerializedEvent.AggregateId)}, " +
                    $"{nameof(SerializedEvent.UserId)}, {nameof(SerializedEvent.EventType)}, {nameof(SerializedEvent.Data)}) " +
                    "VALUES " +
                    $"(@{nameof(SerializedEvent.Id)}, @{nameof(SerializedEvent.CreatedDate)}, @{nameof(SerializedEvent.AggregateId)}, " +
                    $"@{nameof(SerializedEvent.UserId)}, @{nameof(SerializedEvent.EventType)}, @{nameof(SerializedEvent.Data)}) ",
                    @event
                ).ConfigureAwait(false);
            }

            return @event;
        }

        public async Task<IEnumerable<DeserializedLockEvent>> GetEventsAsync(Guid aggregateId, string userId)
        {
            using (var db = Connection)
            {
                var events = await db.QueryAsync<SerializedEvent>(
                    "SELECT * FROM LocksActivity " +
                    $"WHERE {nameof(SerializedEvent.AggregateId)}=@{nameof(SerializedEvent.AggregateId)} " +
                    $"AND {nameof(SerializedEvent.UserId)}=@{nameof(SerializedEvent.UserId)} ",
                    new {AggregateId = aggregateId, UserId = userId}
                ).ConfigureAwait(false);

                return events.Select(_deserializer.Deserialize);
            }
        }
    }
}
