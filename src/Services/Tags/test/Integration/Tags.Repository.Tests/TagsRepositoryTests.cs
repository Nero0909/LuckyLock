using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using Dapper;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Tags.Entities;
using Tags.Repositories;
using Tags.Repositories.Handlers;

namespace Tags.Repository.Tests
{
    [TestFixture]
    public class TagsRepositoryTests
    {
        private IFixture _fixure;
        private IConfigurationRoot _configuration;
        private ITagsRepository _repository;
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
            _repository = new TagsRepository(_configuration);

            SqlMapper.AddTypeHandler(new DateTimeHandler());
            SqlMapper.AddTypeHandler(new GuidHandler());
        }

        [Test]
        public async Task CreateAsync_NotNullModel_TagCreated()
        {
            // Arrage
            var tag = _fixure.Create<Tag>();

            // Act
            var result = await _repository.TryCreateAsync(tag, _userId);

            // Assert
            result.Should().BeEquivalentTo(tag, opt => opt
                .Excluding(x => x.CreatedBy));
        }

        [Test]
        public async Task GetByIdAsync_CreateAndGet_TagReturned()
        {
            // Arrage
            var tag = _fixure.Create<Tag>();
            var created =  await _repository.TryCreateAsync(tag, _userId);

            // Act
            var result = await _repository.GetByIdAsync(created.Id, _userId);

            // Assert
            result.Should().BeEquivalentTo(created, opt => opt.WithStrictOrdering());
        }

        [Test]
        public async Task GetByUserAsync_CreateAndGet_TagsReturned()
        {
            // Arrage
            var firsttTag = _fixure.Create<Tag>();
            var secondTag = _fixure.Create<Tag>();

            var userId = Guid.NewGuid().ToString();
            var first = await _repository.TryCreateAsync(firsttTag, userId);
            var second = await _repository.TryCreateAsync(secondTag, userId);

            // Act
            var result = await _repository.GetByUserAsync(userId);

            // Assert
            result.Should().BeEquivalentTo(first, second);
        }

        [Test]
        public async Task DeleteAsync_CreateAndDelete_TagsDeleted()
        {
            // Arrage
            var tag = _fixure.Create<Tag>();
            var created = await _repository.TryCreateAsync(tag, _userId);

            // Act
            var result = await _repository.DeleteAsync(created.Id, _userId);

            // Assert
            result.Should().BeTrue();
        }
    }
}
