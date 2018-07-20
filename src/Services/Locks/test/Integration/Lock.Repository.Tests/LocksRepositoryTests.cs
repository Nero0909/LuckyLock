using System;
using System.Threading.Tasks;
using AutoFixture;
using Dapper;
using FluentAssertions;
using Locks.Entities;
using Locks.Repository;
using Locks.Repository.Handlers;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Lock.Repository.Tests
{
    [TestFixture]
    public class LocksRepositoryTests
    {
        private IFixture _fixure;
        private IConfigurationRoot _configuration;
        private ILocksRepository _repository;
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
            _repository = new LocksRepository(_configuration);

            SqlMapper.AddTypeHandler(new DateTimeHandler());
            SqlMapper.AddTypeHandler(new GuidHandler());
        }

        [Test]
        public async Task CreateAsync_NotNullModel_LockCreated()
        {
            // Arrage
            var @lock = _fixure.Create<Locks.Entities.Lock>();

            // Act
            var result = await _repository.TryCreateAsync(@lock, _userId);

            // Assert
            result.Should().BeEquivalentTo(@lock, opt => opt
                .Excluding(x => x.CreatedBy));
        }

        [Test]
        public async Task UpdateAsync_CreateAndUpdate_LockUpdated()
        {
            // Arrage
            var @lock = _fixure.Create<Locks.Entities.Lock>();
            @lock.State = LockState.Created;
            var created = await _repository.TryCreateAsync(@lock, _userId);
            created.State = LockState.Locked;

            // Act
            var result = await _repository.UpdateAsync(created, _userId);

            // Assert
            result.Should().BeEquivalentTo(created, opt => opt.WithStrictOrdering());
        }

        [Test]
        public async Task GetByIdAsync_CreateAndGet_LockReturned()
        {
            // Arrage
            var @lock = _fixure.Create<Locks.Entities.Lock>();
            var created =  await _repository.TryCreateAsync(@lock, _userId);

            // Act
            var result = await _repository.GetByIdAsync(created.Id, _userId);

            // Assert
            result.Should().BeEquivalentTo(created, opt => opt.WithStrictOrdering());
        }

        [Test]
        public async Task DeleteAsync_CreateAndDelete_LockDeleted()
        {
            // Arrage
            var @lock = _fixure.Create<Locks.Entities.Lock>();
            var created = await _repository.TryCreateAsync(@lock, _userId);

            // Act
            var result = await _repository.DeleteAsync(created.Id, _userId);

            // Assert
            result.Should().BeTrue();
        }


        [Test]
        public async Task DeleteAsync_CreateAndDelete_LockWasNotDeleted()
        {
            // Arrage
            var id = Guid.NewGuid();

            // Act
            var result = await _repository.DeleteAsync(id, _userId);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public async Task GetByUserAsync_CreateAndGet_LocksReturned()
        {
            // Arrage
            var firstLock = _fixure.Create<Locks.Entities.Lock>();
            var secondLock = _fixure.Create<Locks.Entities.Lock>();

            var userId = Guid.NewGuid().ToString();
            var first = await _repository.TryCreateAsync(firstLock, userId);
            var second = await _repository.TryCreateAsync(secondLock, userId);

            // Act
            var result = await _repository.GetByUserAsync(userId);

            // Assert
            result.Should().BeEquivalentTo(first, second);
        }
    }
}
