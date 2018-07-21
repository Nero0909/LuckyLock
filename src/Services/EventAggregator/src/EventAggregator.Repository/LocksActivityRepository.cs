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
        private readonly IEventSerializer _serializer;

        public LocksActivityRepository(IConfiguration configuration, IEventSerializer serializer)
        {
            _config = configuration;
            _serializer = serializer;
        }

        private IDbConnection Connection => new SqliteConnection(_config.GetConnectionString("Events.db"));


        public async Task<SerializedEvent> CreateAsync(BaseLockMessage @event)
        {
            var serializedEvent = _serializer.Serialize(@event);
            using (var db = Connection)
            {
                await db.ExecuteAsync(
                    "INSERT INTO LocksActivity " +
                    $"({nameof(SerializedEvent.Id)}, {nameof(SerializedEvent.CreatedDate)}, {nameof(SerializedEvent.AggregateId)}, " +
                    $"{nameof(SerializedEvent.UserId)}, {nameof(SerializedEvent.EventType)}, {nameof(SerializedEvent.Data)}) " +
                    "VALUES " +
                    $"(@{nameof(SerializedEvent.Id)}, @{nameof(SerializedEvent.CreatedDate)}, @{nameof(SerializedEvent.AggregateId)}, " +
                    $"@{nameof(SerializedEvent.UserId)}, @{nameof(SerializedEvent.EventType)}, @{nameof(SerializedEvent.Data)}) ",
                    serializedEvent
                ).ConfigureAwait(false);
            }

            return serializedEvent;
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

                return events.Select(_serializer.Deserialize);
            }
        }
    }
}
