using Microsoft.Extensions.Logging;
using Moq;
using ToDos.Api.Queries;
using ToDos.Api.Handlers;
using ToDos.Domain.Interfaces;
using ToDos.Infrastructure.Data.Entities;
using FluentAssertions;
using Xunit;

namespace ToDos.Tests.Handlers
{
    public class GetToDosQueryHandlerTests
    {
        private readonly Mock<IRepository<ToDo, Guid>> _mockRepository;
        private readonly Mock<ILogger<GetToDosQueryHandler>> _mockLogger;
        private readonly GetToDosQueryHandler _handler;

        public GetToDosQueryHandlerTests()
        {
            _mockRepository = new Mock<IRepository<ToDo, Guid>>();
            _mockLogger = new Mock<ILogger<GetToDosQueryHandler>>();
            _handler = new GetToDosQueryHandler(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnToDos_WhenFound()
        {
            // Arrange
            var email = "test@example.com";
            var query = new GetToDosQuery(email);
            
            var todos = new List<ToDo>
            {
                new ToDo
                {
                    Id = Guid.NewGuid(),
                    Name = "Todo 1",
                    Email = email,
                    Date = DateTimeOffset.UtcNow,
                    Done = false
                },
                new ToDo
                {
                    Id = Guid.NewGuid(),
                    Name = "Todo 2",
                    Email = email,
                    Date = DateTimeOffset.UtcNow,
                    Done = true
                }
            };

            _mockRepository.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ToDo, bool>>>()))
                .ReturnsAsync(todos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(todos);
            _mockRepository.Verify(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ToDo, bool>>>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoToDosFound()
        {
            // Arrange
            var email = "test@example.com";
            var query = new GetToDosQuery(email);

            _mockRepository.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ToDo, bool>>>()))
                .ReturnsAsync(new List<ToDo>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _mockRepository.Verify(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ToDo, bool>>>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenRepositoryFails()
        {
            // Arrange
            var email = "test@example.com";
            var query = new GetToDosQuery(email);

            _mockRepository.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ToDo, bool>>>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
            
            _mockRepository.Verify(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ToDo, bool>>>()), Times.Once);
        }
    }
}
