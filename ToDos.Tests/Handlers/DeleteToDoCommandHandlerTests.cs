using Microsoft.Extensions.Logging;
using Moq;
using ToDos.Api.Commands;
using ToDos.Api.Handlers;
using ToDos.Domain.Interfaces;
using ToDos.Infrastructure.Data.Entities;
using FluentAssertions;
using Xunit;

namespace ToDos.Tests.Handlers
{
    public class DeleteToDoCommandHandlerTests
    {
        private readonly Mock<IRepository<ToDo, Guid>> _mockRepository;
        private readonly Mock<ILogger<DeleteToDoCommandHandler>> _mockLogger;
        private readonly DeleteToDoCommandHandler _handler;

        public DeleteToDoCommandHandlerTests()
        {
            _mockRepository = new Mock<IRepository<ToDo, Guid>>();
            _mockLogger = new Mock<ILogger<DeleteToDoCommandHandler>>();
            _handler = new DeleteToDoCommandHandler(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ShouldDeleteToDo_WhenValidId()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new DeleteToDoCommand(id);
            
            var deletedToDo = new ToDo
            {
                Id = id,
                Name = "Test Todo",
                Email = "test@example.com",
                Date = DateTimeOffset.UtcNow,
                Done = false
            };

            _mockRepository.Setup(r => r.DeleteAsync(id)).ReturnsAsync(deletedToDo);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(deletedToDo);
            _mockRepository.Verify(r => r.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenToDoNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new DeleteToDoCommand(id);

            _mockRepository.Setup(r => r.DeleteAsync(id)).ReturnsAsync((ToDo?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeNull();
            _mockRepository.Verify(r => r.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenRepositoryFails()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new DeleteToDoCommand(id);

            _mockRepository.Setup(r => r.DeleteAsync(id))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            
            _mockRepository.Verify(r => r.DeleteAsync(id), Times.Once);
        }
    }
}
