using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Locks.API.Infrastructure.Services;
using Locks.Entities;
using Locks.EventPublishing;
using Locks.Repository;
using Moq;
using NUnit.Framework;

namespace Locks.API.Tests.Infrastructure.Services
{
    [TestFixture]
    public class LocksServiceTests
    {
        private Mock<ILocksRepository> _repositoryMock;
        private Mock<IEventPublisher> _eventPublisherMock;
        private readonly string _userId = "hackerman";

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<ILocksRepository>();
            _eventPublisherMock = new Mock<IEventPublisher>();
        }

        [Test]
        public async Task TryCreateAsync_LockDoesNotExist_Created()
        {
            // Arrange 
            var entity = Builder<Lock>.CreateNew().Build();

            _repositoryMock.Setup(x => x.TryCreateAsync(entity, _userId)).ReturnsAsync(entity);

            var instance = CreateInstance();

            // Act
            var result = await instance.TryCreateAsync(entity, _userId);

            // Asser
            result.Should().Be(entity);
        }

        [Test]
        public async Task TryCreateAsync_LockAlreadyExists_Returnednull()
        {
            // Arrange 
            var entity = Builder<Lock>.CreateNew().Build();

            var instance = CreateInstance();

            // Act
            var result = await instance.TryCreateAsync(entity, _userId);

            // Asser
            result.Should().Be(null);
        }

        [Test]
        public async Task TryCreateAsync_LockAlreadyExists_EventSent()
        {
            // Arrange 
            var entity = Builder<Lock>.CreateNew().Build();

            _repositoryMock.Setup(x => x.TryCreateAsync(entity, _userId)).ReturnsAsync(entity);

            var instance = CreateInstance();

            // Act
            await instance.TryCreateAsync(entity, _userId);

            // Asser
            _eventPublisherMock.Verify(x => x.SendLockCreatedMessageAsync(entity, _userId),
                Times.Once);
        }

        [Test]
        public async Task TryCreateAsync_LockAlreadyExists_EventWasNotSent()
        {
            // Arrange 
            var entity = Builder<Lock>.CreateNew().Build();
            var instance = CreateInstance();

            // Act
            await instance.TryCreateAsync(entity, _userId);

            // Asser
            _eventPublisherMock.Verify(x => x.SendLockCreatedMessageAsync(It.IsAny<Lock>(), It.IsAny<string>()),
                Times.Never);
        }


        [Theory]
        public async Task DeleteAsync_LockExistence_AppropriateResult(bool exists)
        {
            // Arrange 
            var entity = Builder<Lock>.CreateNew().Build();

            _repositoryMock.Setup(x => x.DeleteAsync(entity.Id, _userId)).ReturnsAsync(exists);

            var instance = CreateInstance();

            // Act
            var result = await instance.DeleteAsync(entity, _userId);

            // Asser
            result.Should().Be(exists);
        }

        [Test]
        public async Task ChangeStateAsync_LockDoesNotExist_ReturnedNull()
        {
            // Arrange
            var instance = CreateInstance();

            // Act
            var result = await instance.ChangeStateAsync(Guid.NewGuid(), _userId, LockState.Created);

            // Assert
            result.Should().BeNull();
        }

        [Theory]
        public async Task ChangeStateAsync_TheSameState_StateWasNotChanged(LockState state)
        {
            // Arrange
            var instance = CreateInstance();
            var entity = Builder<Lock>.CreateNew()
                .With(x => x.State == state)
                .Build();

            _repositoryMock.Setup(x => x.GetByIdAsync(entity.Id, _userId)).ReturnsAsync(entity);

            // Act
            await instance.ChangeStateAsync(entity.Id, _userId, state);

            // Assert
            _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Lock>(), It.IsAny<string>()), Times.Never);
        }

        [Theory]
        public async Task ChangeStateAsync_TheSameState_EventWasNotSent(LockState state)
        {
            // Arrange
            var instance = CreateInstance();
            var entity = Builder<Lock>.CreateNew()
                .With(x => x.State == state)
                .Build();

            _repositoryMock.Setup(x => x.GetByIdAsync(entity.Id, _userId)).ReturnsAsync(entity);

            // Act
            await instance.ChangeStateAsync(entity.Id, _userId, state);

            // Assert
            _eventPublisherMock.Verify(
                x => x.SendLockChangedMessageAsync(It.IsAny<Lock>(), 
                    It.IsAny<LockState>(), It.IsAny<LockState>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [Test]
        public async Task ChangeStateAsync_DifferentState_StateChanged()
        {
            // Arrange
            var instance = CreateInstance();
            var entity = Builder<Lock>.CreateNew()
                .With(x => x.State == LockState.Created)
                .Build();

            _repositoryMock.Setup(x => x.GetByIdAsync(entity.Id, _userId)).ReturnsAsync(entity);
            _repositoryMock.Setup(x => x.UpdateAsync(entity, _userId)).ReturnsAsync(entity);

            // Act
            var result = await instance.ChangeStateAsync(entity.Id, _userId, LockState.Locked);

            // Assert
            result.State.Should().Be(LockState.Locked);
        }

        [Test]
        public async Task ChangeStateAsync_DifferentState_LockChangedMessageSent()
        {
            // Arrange
            var instance = CreateInstance();
            var entity = Builder<Lock>.CreateNew()
                .With(x => x.State == LockState.Created)
                .Build();

            _repositoryMock.Setup(x => x.GetByIdAsync(entity.Id, _userId)).ReturnsAsync(entity);
            _repositoryMock.Setup(x => x.UpdateAsync(entity, _userId)).ReturnsAsync(entity);

            // Act
            await instance.ChangeStateAsync(entity.Id, _userId, LockState.Locked);

            // Assert
            _eventPublisherMock.Verify(
                x => x.SendLockChangedMessageAsync(entity,
                    LockState.Created, LockState.Locked,
                    _userId),
                Times.Once);
        }

        private ILocksService CreateInstance()
        {
            return new LocksService(_repositoryMock.Object, _eventPublisherMock.Object);
        }
    }
}
