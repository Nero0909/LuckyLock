using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Contracts.Events;
using Dapper;
using EventAggregator.Entities;
using EventAggregator.Repository;
using EventAggregator.Repository.Handlers;
using EventAggregator.Repository.Serializer;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Repository.Tests
{
    public class LocksActivityRepositoryTests
    {
        private IFixture _fixure;
        private IConfigurationRoot _configuration;
        private ILocksActivityRepository _repository;
        private string _userId = "userId";

        [OneTimeSetUp]
        public void Setup()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _fixure = new Fixture();
        }

        [SetUp]
        public void Initialization()
        {
            var serializer = new SerializerStub();
            _repository = new LocksActivityRepository(_configuration, serializer);

            SqlMapper.AddTypeHandler(new DateTimeHandler());
            SqlMapper.AddTypeHandler(new GuidHandler());
        }

        [Test]
        public async Task CreateAsync_NotNulModel_Created()
        {
            // Arrange
            BaseLockMessage entity = _fixure.Create<LockCreatedMessage>();

            // Act
            var created = await _repository.CreateAsync(entity);

            // Assert
            entity.LockId.Should().Be(created.AggregateId);
            entity.EventCreatedDate.Should().Be(created.CreatedDate);
            entity.EventId.Should().Be(created.Id);
            entity.UserId.Should().Be(created.UserId);
        }

        [Test]
        public async Task GetEventsAsync_CreateAndGet_TwoEntitiesReturned()
        {
            // Arrange
            var aggregateId = Guid.NewGuid();
            var first = _fixure.Create<LockCreatedMessage>();
            first.LockId = aggregateId;
            first.UserId = _userId;

            var second = _fixure.Create<LockCreatedMessage>();
            second.LockId = aggregateId;
            second.UserId = _userId;

            var createdFirst = await _repository.CreateAsync(first);
            var createdSecond = await _repository.CreateAsync(second);

            // Act
            var result = (await _repository.GetEventsAsync(aggregateId, _userId)).ToArray();

            // Assert
            result.Length.Should().Be(2);
            result[0].Should().BeEquivalentTo(createdFirst, opt => opt.Excluding(x => x.Data));
            result[1].Should().BeEquivalentTo(createdSecond, opt => opt.Excluding(x => x.Data));
        }
    }
}
