using Microsoft.Extensions.Logging;
using Moq;
using ToDos.Api.Queries;
using ToDos.Api.Handlers;
using ToDos.Domain.Interfaces;
using ToDos.Domain.Exceptions;
using ToDos.Infrastructure.Data.Entities;
using FluentAssertions;
using Xunit;

namespace ToDos.Tests.Handlers
{
    public class GetToDoByIdQueryHandlerTests
    {
        private readonly Mock<IRepository<ToDo, Guid>> _mockRepository;
        private readonly Mock<ILogger<GetToDoByIdQueryHandler>> _mockLogger;
        private readonly GetToDoByIdQueryHandler _handler;

        public GetToDoByIdQueryHandlerTests()
        {
            _mockRepository = new Mock<IRepository<ToDo, Guid>>();
            _mockLogger = new Mock<ILogger<GetToDoByIdQueryHandler>>();
            _handler = new GetToDoByIdQueryHandler(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnToDo_WhenFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var query = new GetToDoByIdQuery(id);
            
            var todo = new ToDo
            {
                Id = id,
                Name = "Test Todo",
                Email = "test@example.com",
                Date = DateTimeOffset.UtcNow,
                Done = false
            };

            var todos = new List<ToDo> { todo };
            _mockRepository.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ToDo, bool>>>()))
                .ReturnsAsync(todos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(todo);
            _mockRepository.Verify(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ToDo, bool>>>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenToDoNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var query = new GetToDoByIdQuery(id);

            _mockRepository.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ToDo, bool>>>()))
                .ReturnsAsync(new List<ToDo>());

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None));
            
            _mockRepository.Verify(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ToDo, bool>>>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenRepositoryFails()
        {
            // Arrange
            var id = Guid.NewGuid();
            var query = new GetToDoByIdQuery(id);

            _mockRepository.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ToDo, bool>>>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
            
            _mockRepository.Verify(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ToDo, bool>>>()), Times.Once);
        }
    }
}
