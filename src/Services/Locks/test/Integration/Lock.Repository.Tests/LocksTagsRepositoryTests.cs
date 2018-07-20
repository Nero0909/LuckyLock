using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using Dapper;
using FluentAssertions;
using Locks.Repository;
using Locks.Repository.Handlers;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Lock.Repository.Tests
{
    public class LocksTagsRepositoryTests
    {
        private IFixture _fixure;
        private IConfigurationRoot _configuration;
        private ILocksTagsRepository _repository;
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
            _repository = new LocksTagsRepository(_configuration);

            SqlMapper.AddTypeHandler(new DateTimeHandler());
            SqlMapper.AddTypeHandler(new GuidHandler());
        }

        [Test]
        public async Task CreateAsync_NotNullModel_Created()
        {
            // Arrage
            var @lock = _fixure.Create<Locks.Entities.LockTag>();
            @lock.CreatedBy = _userId;

            // Act
            var result = await _repository.TryCreateAsync(@lock, _userId);

            // Assert
            result.Should().BeEquivalentTo(@lock, opt => opt.WithStrictOrdering());
        }

        [Test]
        public async Task GetLinkedTagsByLockAsync_TagExists_TagReturned()
        {
            // Arrage
            var @lock = _fixure.Create<Locks.Entities.LockTag>();
            var created = await _repository.TryCreateAsync(@lock, _userId);

            // Act
            var result = (await _repository.GetLinkedTagsByLockAsync(created.LockId)).ToArray();

            // Assert
            result.Length.Should().Be(1);
            result.Should().BeEquivalentTo(created.TagId);
        }


        [Test]
        public async Task GetLinkedLocksByTagIdAsync_LockExists_LockReturned()
        {
            // Arrage
            var @lock = _fixure.Create<Locks.Entities.LockTag>();
            var created = await _repository.TryCreateAsync(@lock, _userId);

            // Act
            var result = (await _repository.GetLinkedLocksByTagIdAsync(created.TagId, _userId)).ToArray();

            // Assert
            result.Length.Should().Be(1);
            result.Should().BeEquivalentTo(created.LockId);
        }

        [Test]
        public async Task CheckLinkedLocksExistence_LockExists_ReturnedTrue()
        {
            // Arrage
            var @lock = _fixure.Create<Locks.Entities.LockTag>();
            var created = await _repository.TryCreateAsync(@lock, _userId);

            // Act
            var result = await _repository.CheckLinkedLocksExistence(created.TagId, _userId);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public async Task DeleteLinkAsync_LinkExists_Deleted()
        {
            // Arrage
            var @lock = _fixure.Create<Locks.Entities.LockTag>();
            var created = await _repository.TryCreateAsync(@lock, _userId);

            // Act
            var result = await _repository.DeleteLinkAsync(created, _userId);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public async Task DeleteLinkAsync_LinkDoesNotExist_NotDeleted()
        {
            // Arrage
            var @lock = _fixure.Create<Locks.Entities.LockTag>();

            // Act
            var result = await _repository.DeleteLinkAsync(@lock, _userId);

            // Assert
            result.Should().BeFalse();
        }


        [Test]
        public async Task CheckLinkedLocksExistence_LockDoesNotExist_ReturnedFalse()
        {
            // Arrage
            var id = Guid.NewGuid();

            // Act
            var result = await _repository.CheckLinkedLocksExistence(id, _userId);

            // Assert
            result.Should().BeFalse();
        }
    }
}
