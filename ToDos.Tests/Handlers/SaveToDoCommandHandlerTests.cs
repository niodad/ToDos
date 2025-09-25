using AutoMapper;
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
    public class SaveToDoCommandHandlerTests
    {
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IRepository<ToDo, Guid>> _mockRepository;
        private readonly Mock<ILogger<SaveToDoCommandHandler>> _mockLogger;
        private readonly SaveToDoCommandHandler _handler;

        public SaveToDoCommandHandlerTests()
        {
            _mockMapper = new Mock<IMapper>();
            _mockRepository = new Mock<IRepository<ToDo, Guid>>();
            _mockLogger = new Mock<ILogger<SaveToDoCommandHandler>>();
            _handler = new SaveToDoCommandHandler(_mockMapper.Object, _mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ShouldSaveToDo_WhenValidCommand()
        {
            // Arrange
            var command = new SaveToDoCommand
            {
                Name = "Test Todo",
                Email = "test@example.com",
                Date = DateTimeOffset.UtcNow,
                Done = false
            };

            var mappedToDo = new ToDo
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                Email = command.Email,
                Date = command.Date,
                Done = command.Done
            };

            var savedToDo = new ToDo
            {
                Id = mappedToDo.Id,
                Name = mappedToDo.Name,
                Email = mappedToDo.Email,
                Date = mappedToDo.Date,
                Done = mappedToDo.Done
            };

            _mockMapper.Setup(m => m.Map<ToDo>(command)).Returns(mappedToDo);
            _mockRepository.Setup(r => r.SaveAsync(mappedToDo)).ReturnsAsync(savedToDo);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(savedToDo);
            _mockMapper.Verify(m => m.Map<ToDo>(command), Times.Once);
            _mockRepository.Verify(r => r.SaveAsync(mappedToDo), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenRepositoryFails()
        {
            // Arrange
            var command = new SaveToDoCommand
            {
                Name = "Test Todo",
                Email = "test@example.com",
                Date = DateTimeOffset.UtcNow,
                Done = false
            };

            var mappedToDo = new ToDo
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                Email = command.Email,
                Date = command.Date,
                Done = command.Done
            };

            _mockMapper.Setup(m => m.Map<ToDo>(command)).Returns(mappedToDo);
            _mockRepository.Setup(r => r.SaveAsync(mappedToDo))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            
            _mockMapper.Verify(m => m.Map<ToDo>(command), Times.Once);
            _mockRepository.Verify(r => r.SaveAsync(mappedToDo), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldLogInformation_WhenSavingToDo()
        {
            // Arrange
            var command = new SaveToDoCommand
            {
                Name = "Test Todo",
                Email = "test@example.com",
                Date = DateTimeOffset.UtcNow,
                Done = false
            };

            var mappedToDo = new ToDo
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                Email = command.Email,
                Date = command.Date,
                Done = command.Done
            };

            _mockMapper.Setup(m => m.Map<ToDo>(command)).Returns(mappedToDo);
            _mockRepository.Setup(r => r.SaveAsync(mappedToDo)).ReturnsAsync(mappedToDo);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Saving ToDo")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Successfully saved ToDo")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
